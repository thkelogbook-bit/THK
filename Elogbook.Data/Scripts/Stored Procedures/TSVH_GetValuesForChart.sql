-- =============================================
-- Lấy dữ liệu để vẽ chart cho TSVH
-- =============================================
CREATE PROCEDURE [dbo].[TSVH_GetValuesForChart]
	@ParameterID INT,
	@FromDate DATETIME,
	@ToDate DATETIME
AS
BEGIN
	SET NOCOUNT ON

	SELECT
		op.ID,
		op.ParameterID,
		FORMAT(otl.InsertedAt, 'd', 'en-gb') AS N'Ngay',
		otl.TimeLineName AS N'Gio',
		CAST(otl.TimeLineName AS VARCHAR(10)) + 'h - ' + FORMAT(otl.InsertedAt, 'd', 'en-gb') 'XValue',
		ISNULL(op.CurrentValue, 0) AS N'YValue',
		(DATEADD(hh, otl.TimeLineName, DATEDIFF(day, 0, otl.InsertedAt))) AS CycleDate
	FROM dbo.OperationParameters op WITH(NOLOCK)
	INNER JOIN dbo.OperationTimeLine otl WITH(NOLOCK) on op.OperationTimeLineID = otl.ID AND op.FactoryID = otl.FactoryID 
		AND op.OperationPositionID = otl.OperationPositionID AND op.CycleID = otl.CycleID
	WHERE CAST(otl.InsertedAt AS DATE) >= CAST(@FromDate AS DATE) AND CAST(otl.InsertedAt AS DATE) <= CAST(@ToDate AS DATE)
	AND ISNULL(op.ErrorCode, 0) = 0
	AND ISNUMERIC(op.CurrentValue) = 1
	AND CAST(op.CurrentValue AS FLOAT) > 0
	AND op.ParameterID = @ParameterID
	ORDER BY (DATEADD(hh, otl.TimeLineName, DATEDIFF(day, 0, otl.InsertedAt))) ASC
END
