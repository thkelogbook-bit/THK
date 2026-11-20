-- =============================================
-- Get parameters group
-- =============================================
IF OBJECT_ID('TSVH_GetParametersGroup', 'P') IS NOT NULL
    DROP PROCEDURE TSVH_GetParametersGroup
GO
CREATE PROCEDURE TSVH_GetParametersGroup
    @CycleID INT,
    @FactoryID INT,
    @OperationPositionID INT,
    @Status INT,
    @OperationTimeLineID INT
AS
BEGIN
    SELECT
        pg.ID AS ParameterGroupID,
        pg.ParameterGroupName,
        COUNT(op.ID) AS TotalParameters,
        COUNT(CASE WHEN op.IsChecked = 1 THEN 1 ELSE NULL END) AS TotalParametersChecked
    FROM ParameterGroup pg
    LEFT JOIN OperationParameters op ON pg.ID = op.ParameterGroupID
    WHERE pg.CycleID = @CycleID
        AND pg.FactoryID = @FactoryID
        AND pg.OperationPositionID = @OperationPositionID
        AND pg.Status = @Status
        AND pg.IsDeleted = 0
        AND op.CycleID = @CycleID
        AND op.FactoryID = @FactoryID
        AND op.OperationPositionID = @OperationPositionID
        AND op.OperationTimeLineID = @OperationTimeLineID
        AND op.IsDeleted = 0		
    GROUP BY pg.ID, pg.ParameterGroupName		
    UNION
    SELECT
        NULL AS ParameterGroupID,
        'Khác' AS ParameterGroupName,
        COUNT(op.ID) AS TotalParameters,
        COUNT(CASE WHEN op.IsChecked = 1 THEN 1 ELSE NULL END) AS TotalParametersChecked
    FROM OperationParameters op
    WHERE op.CycleID = @CycleID
        AND op.FactoryID = @FactoryID
        AND op.OperationPositionID = @OperationPositionID
        AND (op.ParameterGroupID IS NULL OR op.ParameterGroupID = 0)
        AND op.OperationTimeLineID = @OperationTimeLineID
        AND op.IsDeleted = 0
		HAVING COUNT(op.ID) > 0
END

