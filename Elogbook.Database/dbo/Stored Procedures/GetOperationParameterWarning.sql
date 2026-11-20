CREATE PROCEDURE [dbo].[GetOperationParameterWarning]
(
    @WarningLevel INT = -1,
    @PageNumber INT = 1,
    @PageSize INT = 10
)
AS
BEGIN
    SET NOCOUNT ON;
	DECLARE @Days INT = 7,
		@IsActive BIT,
		@IsActiverRule BIT,
		@ThresholdPercent1 DECIMAL(5, 2) = 5,
		@ConsecutiveExceedCount1 INT = 3,
		@ThresholdPercent2 DECIMAL(5, 2) = 10,
		@ConsecutiveExceedCount2 INT = 3,
		@ThresholdPercent3 DECIMAL(5, 2) = 15,
		@ConsecutiveExceedCount3 INT = 3

	SELECT TOP 1 @Days = [Days], @IsActive = IsActive FROM dbo.ConfigurationWarnings WITH(NOLOCK);
	SET @Days = ISNULL(@Days, 0);
	SET @IsActive = ISNULL(@IsActive, 0);
	SELECT TOP 1
		@ThresholdPercent1 = ThresholdPercent1,
		@ConsecutiveExceedCount1 = ConsecutiveExceedCount1,
		@ThresholdPercent2 = ThresholdPercent2,
		@ConsecutiveExceedCount2 = ConsecutiveExceedCount2,
		@ThresholdPercent3 = ThresholdPercent3,
		@ConsecutiveExceedCount3 = ConsecutiveExceedCount3,
		@IsActiverRule = IsActive
	FROM dbo.ConfigurationWarningRules WITH(NOLOCK);

    IF ISNULL(@IsActive, 0) = 0 OR ISNULL(@IsActiverRule, 0) = 0
    BEGIN
        SELECT
            CAST(0 AS INT) AS ParameterID,
            CAST(0 AS INT) AS TotalWarningLevel1,
            CAST(0 AS INT) AS TotalWarningLevel2,
            CAST(0 AS INT) AS TotalWarningLevel3,
            CAST(NULL AS NVARCHAR(100)) AS KKSCode,
            CAST(NULL AS NVARCHAR(200)) AS ParameterName,
            CAST(NULL AS NVARCHAR(200)) AS ParameterGroupName,
            CAST(NULL AS NVARCHAR(500)) AS WarningMessage,
            CAST(0 AS INT) AS WarningLevel,
            CAST(0 AS INT) AS TotalRecords
        WHERE 1 = 0;
        RETURN;
    END

    ;WITH DataSource AS
    (
        SELECT
            oph.*,
            COALESCE(
                TRY_CONVERT(DECIMAL(18,4), oph.CurrentValue),
                TRY_CONVERT(DECIMAL(18,4), REPLACE(oph.CurrentValue, ',', '.'))
            ) AS CurrentValueNumber
        FROM dbo.OperationParameterHistory oph WITH(NOLOCK)
        WHERE oph.InsertedAt >= DATEADD(DAY, -@Days, CAST(GETDATE() AS DATE))
    ),
    Flagged AS
    (
        SELECT *,
            CASE WHEN CurrentValueNumber < FromRange * (1 - @ThresholdPercent3/100.0)
                   OR CurrentValueNumber >= ToRange * (1 + @ThresholdPercent3/100.0)
                 THEN 1 ELSE 0 END AS F3,
            CASE WHEN CurrentValueNumber < FromRange * (1 - @ThresholdPercent2/100.0)
                   OR CurrentValueNumber >= ToRange * (1 + @ThresholdPercent2/100.0)
                 THEN 1 ELSE 0 END AS F2,
            CASE WHEN CurrentValueNumber < FromRange * (1 - @ThresholdPercent1/100.0)
                   OR CurrentValueNumber >= ToRange * (1 + @ThresholdPercent1/100.0)
                 THEN 1 ELSE 0 END AS F1
        FROM DataSource
        WHERE CurrentValueNumber IS NOT NULL
    ),
    Grouped AS
    (
        SELECT *,
            ROW_NUMBER() OVER (PARTITION BY ParameterID, FactoryID, OperationPositionID, CycleID ORDER BY InsertedAt, TimeLine)
          - ROW_NUMBER() OVER (PARTITION BY ParameterID, FactoryID, OperationPositionID, CycleID, F3 ORDER BY InsertedAt, TimeLine) AS Grp3,
            ROW_NUMBER() OVER (PARTITION BY ParameterID, FactoryID, OperationPositionID, CycleID ORDER BY InsertedAt, TimeLine)
          - ROW_NUMBER() OVER (PARTITION BY ParameterID, FactoryID, OperationPositionID, CycleID, F2 ORDER BY InsertedAt, TimeLine) AS Grp2,
            ROW_NUMBER() OVER (PARTITION BY ParameterID, FactoryID, OperationPositionID, CycleID ORDER BY InsertedAt, TimeLine)
          - ROW_NUMBER() OVER (PARTITION BY ParameterID, FactoryID, OperationPositionID, CycleID, F1 ORDER BY InsertedAt, TimeLine) AS Grp1
        FROM Flagged
    ),
    Streaks AS
    (
        SELECT *,
            CASE WHEN F3 = 1 THEN
                COUNT(*) OVER (PARTITION BY ParameterID, FactoryID, OperationPositionID, CycleID, F3, Grp3 ORDER BY InsertedAt, TimeLine)
            ELSE 0 END AS Streak3,
            CASE WHEN F2 = 1 THEN
                COUNT(*) OVER (PARTITION BY ParameterID, FactoryID, OperationPositionID, CycleID, F2, Grp2 ORDER BY InsertedAt, TimeLine)
            ELSE 0 END AS Streak2,
            CASE WHEN F1 = 1 THEN
                COUNT(*) OVER (PARTITION BY ParameterID, FactoryID, OperationPositionID, CycleID, F1, Grp1 ORDER BY InsertedAt, TimeLine)
            ELSE 0 END AS Streak1
        FROM Grouped
    ),
    WarningData AS
    (
        SELECT *,
            CASE
                WHEN F3 = 1 AND Streak3 >= @ConsecutiveExceedCount3 THEN 3
                WHEN F2 = 1 AND Streak2 >= @ConsecutiveExceedCount2 THEN 2
                WHEN F1 = 1 AND Streak1 >= @ConsecutiveExceedCount1 THEN 1
                ELSE 0
            END AS RowWarningLevel,
            CASE
                WHEN F3 = 1 AND Streak3 >= @ConsecutiveExceedCount3 THEN Streak3
                WHEN F2 = 1 AND Streak2 >= @ConsecutiveExceedCount2 THEN Streak2
                WHEN F1 = 1 AND Streak1 >= @ConsecutiveExceedCount1 THEN Streak1
                ELSE 0
            END AS ConsecutiveCount,
            CASE
                WHEN F3 = 1 AND Streak3 >= @ConsecutiveExceedCount3 THEN @ThresholdPercent3
                WHEN F2 = 1 AND Streak2 >= @ConsecutiveExceedCount2 THEN @ThresholdPercent2
                WHEN F1 = 1 AND Streak1 >= @ConsecutiveExceedCount1 THEN @ThresholdPercent1
                ELSE NULL
            END AS ThresholdPercentApplied
        FROM Streaks
    ),
    -- Chỉ lấy bản ghi mới nhất (trạng thái hiện tại) của mỗi thông số
    LatestPerParameter AS
    (
        SELECT *,
            ROW_NUMBER() OVER (
                PARTITION BY ParameterID, FactoryID, OperationPositionID, CycleID
                ORDER BY InsertedAt DESC, TimeLine DESC
            ) AS rn
        FROM WarningData
    ),
    -- Chỉ giữ các thông số ĐANG có cảnh báo (không phân trang, không lọc theo @WarningLevel ở bước này)
    AlertedParameters AS
    (
        SELECT *
        FROM LatestPerParameter
        WHERE rn = 1 AND RowWarningLevel > 0
    ),
    -- Tính tổng số thông số theo từng mức, GIỐNG NHAU trên mọi dòng (window không partition)
    WithGlobalTotals AS
    (
        SELECT *,
            SUM(CASE WHEN RowWarningLevel = 1 THEN 1 ELSE 0 END) OVER () AS TotalWarningLevel1,
            SUM(CASE WHEN RowWarningLevel = 2 THEN 1 ELSE 0 END) OVER () AS TotalWarningLevel2,
            SUM(CASE WHEN RowWarningLevel = 3 THEN 1 ELSE 0 END) OVER () AS TotalWarningLevel3
        FROM AlertedParameters
    ),
    FinalResult AS
    (
        SELECT
            wt.ParameterID,
            wt.TotalWarningLevel1,
            wt.TotalWarningLevel2,
            wt.TotalWarningLevel3,
            wt.KKSCode,
            wt.ParameterName,
            pg.ParameterGroupName AS ParameterGroupName,
            N'Thông số vượt ngưỡng '
            + CAST(CAST(wt.ThresholdPercentApplied AS INT) AS NVARCHAR(10))
            + N'% trong '
            + CAST(wt.ConsecutiveCount AS NVARCHAR(10))
            + N' lần ghi nhận liên tiếp.'
            AS WarningMessage,
            wt.RowWarningLevel AS WarningLevel
        FROM WithGlobalTotals wt
        LEFT JOIN ParameterGroup pg ON pg.ID = wt.ParameterGroupID
    )
    SELECT
        ParameterID,
        TotalWarningLevel1,
        TotalWarningLevel2,
        TotalWarningLevel3,
        KKSCode,
        ParameterName,
        ParameterGroupName,
        WarningMessage,
        WarningLevel,
        COUNT(*) OVER() AS TotalRecords
    FROM FinalResult
    WHERE
        (@WarningLevel = -1 OR WarningLevel = @WarningLevel)
    ORDER BY
        WarningLevel DESC
    OFFSET (@PageNumber-1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;