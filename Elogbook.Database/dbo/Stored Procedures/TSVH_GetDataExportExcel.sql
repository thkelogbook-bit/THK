
CREATE PROCEDURE [dbo].[TSVH_GetDataExportExcel]
	@FactoryID INT,
	@OperationPositionID INT,
	@CycleID INT,
	@ParameterGroupIDs NVARCHAR(500),
	@FromDate DATETIME = NULL,
	@ToDate DATETIME = NULL
AS
BEGIN
	SET NOCOUNT ON

	SELECT
		op.FactoryID,
		op.OperationPositionID,
		op.CycleID,
		op.ParameterName,
		op.ExcelAIColIndex,
		op.ParameterID, 
		CASE WHEN op.IsNumberType = 1 AND ISNUMERIC(CurrentValue) = 1 THEN (ISNULL(CAST(CurrentValue AS FLOAT), CAST(0.00 AS FLOAT))) ELSE CAST(0.00 AS FLOAT) END AS CurrentValue,
		otl.TimeLineName,
		otl.InsertedAt
	FROM dbo.OperationParameters op WITH(NOLOCK)
	INNER JOIN dbo.OperationTimeLine otl WITH(NOLOCK) ON op.OperationTimeLineID = otl.ID
	WHERE otl.FactoryID = @FactoryID
		AND otl.OperationPositionID = @OperationPositionID
		AND otl.CycleID = @CycleID
		AND (CAST(otl.InsertedAt AS DATE) >= CAST(@FromDate AS DATE)) AND (CAST(otl.InsertedAt AS DATE) <= CAST(@ToDate AS DATE))
		AND op.IsDeleted = 0
		AND otl.IsDeleted = 0
		AND op.ParameterGroupID IN (SELECT value from string_split(@ParameterGroupIDs, ';'))
	ORDER BY otl.TimeLineName
END