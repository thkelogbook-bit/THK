-- =============================================
-- Lấy dữ liệu nhân sự ca vận hành để xuất Excel
-- Trả về flat data: WorkDate | FactoryID | FactoryName | ShiftLabel | ShiftOrder | PositionName | PositionOrder | FullName
-- Pivot thực hiện ở tầng C#
-- Params:
--   @FactoryID : ID nhà máy (0 = tất cả)
--   @FromDate  : Từ ngày (yyyy-MM-dd)
--   @ToDate    : Đến ngày (yyyy-MM-dd)
-- =============================================
CREATE PROCEDURE [dbo].[NKVH_GetPersonnelExport]
    @FactoryID INT  = 0,
    @FromDate  DATE = NULL,
    @ToDate    DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT ShiftLabel, PositionOrder, PositionName, FullName,
           WorkDate, FactoryID, FactoryName, ShiftOrder
    FROM (
        SELECT
            CAST(od.CreatedDate AS DATE)                    AS WorkDate,
            f.ID                                            AS FactoryID,
            ISNULL(f.FactoryName, N'')                      AS FactoryName,
            os.OperationShiftName                           AS ShiftLabel,
            os.[Position]                                   AS ShiftOrder,
            CASE
                WHEN odps.PositionInShiftID IS NOT NULL AND odps.PositionInShiftID > 0
                THEN ISNULL(op_pos.OperationPositionName, odps.[Label])
                ELSE odps.[Label]
            END                                             AS PositionName,
            odps.[Position]                                 AS PositionOrder,
            ISNULL(u.FullName, N'')                         AS FullName,
            ROW_NUMBER() OVER (
                PARTITION BY
                    CAST(od.CreatedDate AS DATE),
                    od.FactoryID,
                    od.OperationShiftID,
                    odps.[Position]
                ORDER BY od.ID DESC
            ) AS rn
        FROM dbo.OperationDiary              od     WITH(NOLOCK)
        INNER JOIN dbo.Factory               f      WITH(NOLOCK) ON f.ID      = od.FactoryID            AND f.IsDeleted    = 0
        INNER JOIN dbo.OperationShift        os     WITH(NOLOCK) ON os.ID     = od.OperationShiftID     AND os.IsDeleted   = 0
        INNER JOIN dbo.OperationDiaryPositionShift odps WITH(NOLOCK)
                                                         ON odps.OperationDiaryID = od.ID               AND odps.IsDeleted = 0
        LEFT  JOIN dbo.PositionInShift       pis    WITH(NOLOCK) ON pis.ID    = odps.PositionInShiftID  AND pis.IsDeleted  = 0
        LEFT  JOIN dbo.OperationPosition     op_pos WITH(NOLOCK) ON op_pos.ID = pis.OperationPositionID
        LEFT  JOIN dbo.Users                 u      WITH(NOLOCK) ON u.ID      = odps.AssignedUserID
        WHERE
            od.IsDeleted = 0
            AND (@FactoryID = 0 OR od.FactoryID = @FactoryID)
            AND (@FromDate IS NULL OR od.CreatedDate >= @FromDate)
            AND (@ToDate   IS NULL OR od.CreatedDate <  DATEADD(DAY, 1, @ToDate))
    ) src
    WHERE src.rn = 1
    ORDER BY WorkDate, FactoryName, ShiftOrder, PositionOrder;
END;
GO
