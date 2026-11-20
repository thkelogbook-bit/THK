-- ========================================================
-- Lấy giá trị ghi thông số của chu kỳ
-- ========================================================
CREATE PROCEDURE [dbo].[TSVH_GetParameterValuesPrint]
	@FactoryID INT,
	@OperationPositionID INT,
	@InsertedAt DATE
AS
BEGIN
	SET NOCOUNT ON

	SELECT 
		op.CycleID,
		op.ParameterID, 
		CAST(otl.TimeLineName AS VARCHAR(5)) AS OpeartionTimeLineName,
		CASE WHEN ISNULL(ErrorCode, 0) > 0 THEN 
			(CASE WHEN ErrorCode = 1 THEN 'F'
				WHEN ErrorCode = 2 THEN '-'
				WHEN ErrorCode = 3 THEN 'S'
				WHEN ErrorCode = 4 THEN 'J'
				WHEN ErrorCode = 5 THEN 'x' END)
			ELSE CAST(CurrentValue AS NVARCHAR(MAX)) END AS ParameterValue
	FROM dbo.OperationParameters op WITH(NOLOCK)
	INNER JOIN dbo.OperationTimeLine otl WITH(NOLOCK) ON op.OperationTimeLineID = otl.ID
	WHERE otl.FactoryID = @FactoryID 
	AND otl.OperationPositionID = @OperationPositionID
	AND CAST(otl.InsertedAt AS DATE) = @InsertedAt
	AND op.IsDeleted = 0
	AND otl.IsDeleted = 0
END