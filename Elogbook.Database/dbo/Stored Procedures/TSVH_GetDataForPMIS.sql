-- =============================================
-- Lấy dữ liệu để đồng bộ sang PMIS
-- =============================================
CREATE PROCEDURE [dbo].[TSVH_GetDataForPMIS]
	@Hour INT = -2
AS
BEGIN
	SET NOCOUNT ON

	SELECT
		p.QLKTCode AS AMETERID,
		CASE WHEN otl.TimeLineName = 24 
			THEN (DATEADD(hh, otl.TimeLineName, DATEDIFF(day, 1, otl.InsertedAt)))
			ELSE (DATEADD(hh, otl.TimeLineName, DATEDIFF(day, 0, otl.InsertedAt))) END AS DTIME,
		(CASE WHEN ISNULL(op.ErrorCode, 0) != 0 
			THEN (CASE WHEN ISNULL(op.ErrorCode, 0) = 1 THEN 'F' --Thiết bị bị hỏng (F)
							WHEN ISNULL(op.ErrorCode,0) = 2 THEN '-' --Dừng thiết bị (-)
							WHEN ISNULL(op.ErrorCode,0) = 3 THEN 'S' --Sim thiết bị (S)
							WHEN ISNULL(op.ErrorCode,0) = 4 THEN 'J' --Jump thiết bị (J)
							WHEN ISNULL(op.ErrorCode,0) = 5 THEN 'X' --Bận công tác
							ELSE '' END)
			ELSE op.CurrentValue END) AS VAL_S,
		N'MANUAL' AS READMETHODID,
		N'PM_VH1QD' AS USERID
	FROM dbo.OperationParameters op WITH(NOLOCK)
	INNER JOIN dbo.Parameters p WITH(NOLOCK) ON op.ParameterID = p.ID
	INNER JOIN dbo.OperationTimeLine otl WITH(NOLOCK) on op.OperationTimeLineID = otl.ID AND op.FactoryID = otl.FactoryID 
		AND op.OperationPositionID = otl.OperationPositionID AND op.CycleID = otl.CycleID
	WHERE LEN(ISNULL(p.QLKTCode, '')) > 0 
	AND op.CheckedDate >= DATEADD(HOUR, @Hour, GETDATE())
	ORDER BY op.CheckedDate DESC
END