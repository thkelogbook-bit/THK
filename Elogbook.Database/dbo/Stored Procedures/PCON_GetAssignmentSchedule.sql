-- =============================================
-- Lấy bảng phân công ca theo bộ lọc
-- Trả về flat rows, grouping thực hiện ở tầng C#
--
-- Params:
--   @FactoryID           : ID nhà máy (bắt buộc)
--   @FromDate            : Từ ngày (dd/MM/yyyy hoặc yyyy-MM-dd)
--   @ToDate              : Đến ngày (dd/MM/yyyy hoặc yyyy-MM-dd)
--   @OperationPositionID : ID cương vị (0 = tất cả)
--   @OperationShiftID    : ID ca vận hành (0 = tất cả)
--
-- Output columns:
--   AssignmentSlotID, OperationPositionID, OperationPositionName,
--   OperationShiftID, OperationShiftName, ShiftPosition, StartTime, EndTime,
--   WorkDate, DayOfWeekCode, HasOverride,
--   AssignmentEmployeeID, UserID, UserName, FullName, AccountType
-- =============================================
CREATE PROCEDURE [dbo].[PCON_GetAssignmentSchedule]
    @FactoryID           INT,
    @FromDate            DATE,
    @ToDate              DATE,
    @OperationPositionID INT = 0,
    @OperationShiftID    INT = 0
AS
BEGIN
    SET NOCOUNT ON;

    -- Query chính: flat rows, mỗi row = 1 nhân viên trong 1 slot
    SELECT
        s.ID                                        AS AssignmentSlotID,

        -- Cương vị
        op.ID                                       AS OperationPositionID,
        ISNULL(op.OperationPositionName, N'')       AS OperationPositionName,

        -- Ca vận hành
        os.ID                                       AS OperationShiftID,
        ISNULL(os.OperationShiftName, N'')          AS OperationShiftName,
        ISNULL(os.[Position], 0)                    AS ShiftPosition,
        ISNULL(os.StartTime, 0)                     AS StartTime,
        ISNULL(os.EndTime,   0)                     AS EndTime,

        -- Ngày làm việc
        CAST(s.WorkDate AS DATE)                    AS WorkDate,
        -- DayOfWeekCode: 0=CN, 1=T2, ..., 6=T7 (khớp với C# DayOfWeek enum)
        (DATEPART(WEEKDAY, s.WorkDate) + 5) % 7     AS DayOfWeekCode,

        -- Đánh dấu ô đã từng bị thay đổi nhân sự (FE dùng để render badge amber)
        -- Chỉ tính là HasOverride khi OldUserID IS NOT NULL (tức là có đổi ca thực sự,
        -- không tính lần ghi nhận ban đầu lúc tạo phân công khi OldUserID = NULL)
        CASE WHEN EXISTS (
                SELECT 1 FROM dbo.AssignmentEmployeeHistory h WITH(NOLOCK)
                WHERE  h.AssignmentSlotID = s.ID
                  AND  h.IsDeleted        = 0
                  AND  h.OldUserID        IS NOT NULL
             )
             THEN CAST(1 AS BIT)
             ELSE CAST(0 AS BIT)
        END                                         AS HasOverride,

        -- Nhân viên được phân công
        ae.ID                                       AS AssignmentEmployeeID,
        u.ID                                        AS UserID,
        ISNULL(u.UserName, N'')                     AS UserName,
        ISNULL(u.FullName, N'')                     AS FullName,
        ISNULL(u.AccountType, 0)                    AS AccountType

    FROM       dbo.AssignmentSlot               s   WITH(NOLOCK)
    INNER JOIN dbo.OperationPosition            op  WITH(NOLOCK) ON op.ID  = s.OperationPositionID  AND op.IsDeleted = 0 AND op.Active = 1 
    INNER JOIN dbo.OperationShift               os  WITH(NOLOCK) ON os.ID  = s.OperationShiftID     AND os.IsDeleted = 0 AND os.Active = 1 
    INNER JOIN dbo.AssignmentEmployee           ae  WITH(NOLOCK) ON ae.AssignmentSlotID = s.ID      AND ae.IsDeleted = 0
    INNER JOIN dbo.Users                        u   WITH(NOLOCK) ON u.ID   = ae.UserID

    WHERE
        s.IsDeleted   = 0
        AND s.FactoryID = @FactoryID
        AND s.WorkDate >= @FromDate
        AND s.WorkDate <= @ToDate
        AND (@OperationPositionID = 0 OR s.OperationPositionID = @OperationPositionID)
        AND (@OperationShiftID    = 0 OR s.OperationShiftID    = @OperationShiftID)

    ORDER BY
        op.OperationPositionName,
        os.[Position],          -- Ca sáng → chiều → đêm
        s.WorkDate,
        u.FullName;

END;