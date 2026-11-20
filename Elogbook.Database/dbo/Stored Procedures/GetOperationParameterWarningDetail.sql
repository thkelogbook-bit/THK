-- =============================================
-- Lấy thông tin chi tiết cảnh báo AI của 1 thông số vận hành
-- Result set 1 : thông tin thiết bị + nội dung cảnh báo
-- Result set 2 : danh sách các bản ghi vượt ngưỡng liên tiếp hiện tại
--                (số record = số lần vượt ngưỡng liên tiếp của streak đang kích hoạt)
-- =============================================
-- TEST: EXEC [dbo].[GetOperationParameterWarningDetail] @ParameterID = 10
CREATE PROCEDURE [dbo].[GetOperationParameterWarningDetail]
    @ParameterID INT
AS
BEGIN
    SET NOCOUNT ON;

    -- -------------------------------------------------------
    -- Đọc cấu hình AI
    -- -------------------------------------------------------
    DECLARE
        @Days                    INT          = 7,
        @IsActive                BIT          = 0,
        @IsActiveRule            BIT          = 0,
        @ThresholdPercent1       DECIMAL(5,2) = 5,
        @ConsecutiveExceedCount1 INT          = 3,
        @ThresholdPercent2       DECIMAL(5,2) = 10,
        @ConsecutiveExceedCount2 INT          = 3,
        @ThresholdPercent3       DECIMAL(5,2) = 15,
        @ConsecutiveExceedCount3 INT          = 3

    SELECT TOP 1 @Days = [Days], @IsActive = IsActive
    FROM dbo.ConfigurationWarnings WITH(NOLOCK)

    SET @Days     = ISNULL(@Days, 0)
    SET @IsActive = ISNULL(@IsActive, 0)

    SELECT TOP 1
        @ThresholdPercent1       = ThresholdPercent1,
        @ConsecutiveExceedCount1 = ConsecutiveExceedCount1,
        @ThresholdPercent2       = ThresholdPercent2,
        @ConsecutiveExceedCount2 = ConsecutiveExceedCount2,
        @ThresholdPercent3       = ThresholdPercent3,
        @ConsecutiveExceedCount3 = ConsecutiveExceedCount3,
        @IsActiveRule            = IsActive
    FROM dbo.ConfigurationWarningRules WITH(NOLOCK)

    -- -------------------------------------------------------
    -- Biến lưu kết quả tính cảnh báo
    -- -------------------------------------------------------
    DECLARE
        @WarningLevel     INT           = 0,
        @WarningMessage   NVARCHAR(500) = NULL,
        @ConsecutiveCount INT           = 0,
        @ThresholdApplied DECIMAL(5,2)  = NULL,
        @ActiveGrp        INT           = NULL,
        @ActiveFlag       INT           = NULL

    IF ISNULL(@IsActive, 0) = 1 AND ISNULL(@IsActiveRule, 0) = 1
    BEGIN
        ;WITH DataSource AS (
            SELECT *,
                -- Thử convert trực tiếp trước (số thập phân dùng dấu chấm: 50.20)
                -- Nếu fail thì thử replace dấu phẩy thập phân kiểu Việt (50,20 → 50.20)
                COALESCE(
                    TRY_CONVERT(DECIMAL(18,4), CurrentValue),
                    TRY_CONVERT(DECIMAL(18,4), REPLACE(CurrentValue, ',', '.'))
                ) AS cvn
            FROM dbo.OperationParameterHistory WITH(NOLOCK)
            WHERE ParameterID = @ParameterID
              AND InsertedAt >= DATEADD(DAY, -@Days, CAST(GETDATE() AS DATE))
        ),
        Flagged AS (
            SELECT *,
                CASE WHEN cvn < FromRange*(1-@ThresholdPercent3/100.0) OR cvn >= ToRange*(1+@ThresholdPercent3/100.0) THEN 1 ELSE 0 END AS F3,
                CASE WHEN cvn < FromRange*(1-@ThresholdPercent2/100.0) OR cvn >= ToRange*(1+@ThresholdPercent2/100.0) THEN 1 ELSE 0 END AS F2,
                CASE WHEN cvn < FromRange*(1-@ThresholdPercent1/100.0) OR cvn >= ToRange*(1+@ThresholdPercent1/100.0) THEN 1 ELSE 0 END AS F1
            FROM DataSource
            WHERE cvn IS NOT NULL
        ),
        Grouped AS (
            SELECT *,
                ROW_NUMBER() OVER (PARTITION BY ParameterID,FactoryID,OperationPositionID,CycleID ORDER BY InsertedAt,TimeLine)
                - ROW_NUMBER() OVER (PARTITION BY ParameterID,FactoryID,OperationPositionID,CycleID,F3 ORDER BY InsertedAt,TimeLine) AS Grp3,
                ROW_NUMBER() OVER (PARTITION BY ParameterID,FactoryID,OperationPositionID,CycleID ORDER BY InsertedAt,TimeLine)
                - ROW_NUMBER() OVER (PARTITION BY ParameterID,FactoryID,OperationPositionID,CycleID,F2 ORDER BY InsertedAt,TimeLine) AS Grp2,
                ROW_NUMBER() OVER (PARTITION BY ParameterID,FactoryID,OperationPositionID,CycleID ORDER BY InsertedAt,TimeLine)
                - ROW_NUMBER() OVER (PARTITION BY ParameterID,FactoryID,OperationPositionID,CycleID,F1 ORDER BY InsertedAt,TimeLine) AS Grp1
            FROM Flagged
        ),
        Streaks AS (
            SELECT *,
                CASE WHEN F3=1 THEN COUNT(*) OVER (PARTITION BY ParameterID,FactoryID,OperationPositionID,CycleID,F3,Grp3 ORDER BY InsertedAt,TimeLine) ELSE 0 END AS Streak3,
                CASE WHEN F2=1 THEN COUNT(*) OVER (PARTITION BY ParameterID,FactoryID,OperationPositionID,CycleID,F2,Grp2 ORDER BY InsertedAt,TimeLine) ELSE 0 END AS Streak2,
                CASE WHEN F1=1 THEN COUNT(*) OVER (PARTITION BY ParameterID,FactoryID,OperationPositionID,CycleID,F1,Grp1 ORDER BY InsertedAt,TimeLine) ELSE 0 END AS Streak1
            FROM Grouped
        ),
        Latest AS (
            SELECT TOP 1
                CASE WHEN F3=1 AND Streak3>=@ConsecutiveExceedCount3 THEN 3
                     WHEN F2=1 AND Streak2>=@ConsecutiveExceedCount2 THEN 2
                     WHEN F1=1 AND Streak1>=@ConsecutiveExceedCount1 THEN 1
                     ELSE 0 END AS lvl,
                CASE WHEN F3=1 AND Streak3>=@ConsecutiveExceedCount3 THEN Streak3
                     WHEN F2=1 AND Streak2>=@ConsecutiveExceedCount2 THEN Streak2
                     WHEN F1=1 AND Streak1>=@ConsecutiveExceedCount1 THEN Streak1
                     ELSE 0 END AS cnt,
                CASE WHEN F3=1 AND Streak3>=@ConsecutiveExceedCount3 THEN @ThresholdPercent3
                     WHEN F2=1 AND Streak2>=@ConsecutiveExceedCount2 THEN @ThresholdPercent2
                     WHEN F1=1 AND Streak1>=@ConsecutiveExceedCount1 THEN @ThresholdPercent1
                     ELSE NULL END AS pct,
                CASE WHEN F3=1 AND Streak3>=@ConsecutiveExceedCount3 THEN Grp3
                     WHEN F2=1 AND Streak2>=@ConsecutiveExceedCount2 THEN Grp2
                     WHEN F1=1 AND Streak1>=@ConsecutiveExceedCount1 THEN Grp1
                     ELSE NULL END AS activeGrp,
                CASE WHEN F3=1 AND Streak3>=@ConsecutiveExceedCount3 THEN 3
                     WHEN F2=1 AND Streak2>=@ConsecutiveExceedCount2 THEN 2
                     WHEN F1=1 AND Streak1>=@ConsecutiveExceedCount1 THEN 1
                     ELSE NULL END AS activeFlag
            FROM Streaks
            ORDER BY InsertedAt DESC, TimeLine DESC
        )
        SELECT
            @WarningLevel     = ISNULL(lvl, 0),
            @ConsecutiveCount = ISNULL(cnt, 0),
            @ThresholdApplied = pct,
            @ActiveGrp        = activeGrp,
            @ActiveFlag       = activeFlag
        FROM Latest
        WHERE lvl > 0

        -- Message: làm tròn nguyên, bỏ .00
        IF @WarningLevel > 0
            SET @WarningMessage = N'Thông số vượt ngưỡng '
                + CONVERT(NVARCHAR(10), CONVERT(INT, @ThresholdApplied))
                + N'% trong '
                + CAST(@ConsecutiveCount AS NVARCHAR(10))
                + N' lần ghi nhận liên tiếp.'
    END

    -- -------------------------------------------------------
    -- Result set 1: Thông tin thiết bị + cảnh báo
    -- -------------------------------------------------------
    SELECT
        p.ID                        AS ParameterID,
        p.ParameterName,
        p.KKSCode,
        p.UnitMeasure,
        p.FromRange,
        p.ToRange,
        pg.ParameterGroupName,
        f.FactoryName,
        op.OperationPositionName    AS CuongVi,
        @WarningLevel               AS WarningLevel,
        @WarningMessage             AS WarningMessage
    FROM dbo.Parameters p WITH(NOLOCK)
    LEFT JOIN dbo.ParameterGroup    pg  WITH(NOLOCK) ON pg.ID = p.ParameterGroupID    AND pg.IsDeleted = 0
    LEFT JOIN dbo.Factory           f   WITH(NOLOCK) ON f.ID  = p.FactoryID           AND f.IsDeleted  = 0
    LEFT JOIN dbo.OperationPosition op  WITH(NOLOCK) ON op.ID = p.OperationPositionID AND op.IsDeleted = 0
    WHERE p.ID = @ParameterID
      AND p.IsDeleted = 0

    -- -------------------------------------------------------
    -- Result set 2: Các bản ghi thuộc streak vượt ngưỡng đang kích hoạt
    -- -------------------------------------------------------
    IF @WarningLevel > 0 AND @ActiveGrp IS NOT NULL AND @ActiveFlag IS NOT NULL
    BEGIN
        ;WITH DataSource AS (
            SELECT *,
                COALESCE(
                    TRY_CONVERT(DECIMAL(18,4), CurrentValue),
                    TRY_CONVERT(DECIMAL(18,4), REPLACE(CurrentValue, ',', '.'))
                ) AS cvn
            FROM dbo.OperationParameterHistory WITH(NOLOCK)
            WHERE ParameterID = @ParameterID
              AND InsertedAt >= DATEADD(DAY, -@Days, CAST(GETDATE() AS DATE))
        ),
        Flagged AS (
            SELECT *,
                CASE WHEN cvn < FromRange*(1-@ThresholdPercent3/100.0) OR cvn >= ToRange*(1+@ThresholdPercent3/100.0) THEN 1 ELSE 0 END AS F3,
                CASE WHEN cvn < FromRange*(1-@ThresholdPercent2/100.0) OR cvn >= ToRange*(1+@ThresholdPercent2/100.0) THEN 1 ELSE 0 END AS F2,
                CASE WHEN cvn < FromRange*(1-@ThresholdPercent1/100.0) OR cvn >= ToRange*(1+@ThresholdPercent1/100.0) THEN 1 ELSE 0 END AS F1
            FROM DataSource
            WHERE cvn IS NOT NULL
        ),
        Grouped AS (
            SELECT *,
                ROW_NUMBER() OVER (PARTITION BY ParameterID,FactoryID,OperationPositionID,CycleID ORDER BY InsertedAt,TimeLine)
                - ROW_NUMBER() OVER (PARTITION BY ParameterID,FactoryID,OperationPositionID,CycleID,F3 ORDER BY InsertedAt,TimeLine) AS Grp3,
                ROW_NUMBER() OVER (PARTITION BY ParameterID,FactoryID,OperationPositionID,CycleID ORDER BY InsertedAt,TimeLine)
                - ROW_NUMBER() OVER (PARTITION BY ParameterID,FactoryID,OperationPositionID,CycleID,F2 ORDER BY InsertedAt,TimeLine) AS Grp2,
                ROW_NUMBER() OVER (PARTITION BY ParameterID,FactoryID,OperationPositionID,CycleID ORDER BY InsertedAt,TimeLine)
                - ROW_NUMBER() OVER (PARTITION BY ParameterID,FactoryID,OperationPositionID,CycleID,F1 ORDER BY InsertedAt,TimeLine) AS Grp1
            FROM Flagged
        ),
        ActiveStreak AS (
            SELECT
                ID,
                CAST(InsertedAt AS DATE) AS RecordDate,
                TimeLine                 AS TimeSlot,
                FromRange                AS RangeFrom,
                ToRange                  AS RangeTo,
                CurrentValue,
                1                        AS IsOutOfRange
            FROM Grouped
            WHERE (
                    (@ActiveFlag = 3 AND F3 = 1 AND Grp3 = @ActiveGrp)
                 OR (@ActiveFlag = 2 AND F2 = 1 AND Grp2 = @ActiveGrp)
                 OR (@ActiveFlag = 1 AND F1 = 1 AND Grp1 = @ActiveGrp)
                )
        )
        SELECT * FROM ActiveStreak
        ORDER BY RecordDate DESC, TimeSlot DESC
    END
    ELSE
    BEGIN
        SELECT
            CAST(NULL AS BIGINT)        AS ID,
            CAST(NULL AS DATE)          AS RecordDate,
            CAST(NULL AS INT)           AS TimeSlot,
            CAST(NULL AS REAL)          AS RangeFrom,
            CAST(NULL AS REAL)          AS RangeTo,
            CAST(NULL AS NVARCHAR(200)) AS CurrentValue,
            CAST(NULL AS BIT)           AS IsOutOfRange
        WHERE 1 = 0
    END
END
