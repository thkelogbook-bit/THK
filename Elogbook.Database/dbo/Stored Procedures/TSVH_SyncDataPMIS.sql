-- =======================================================================================
-- Đồng bộ dữ liệu từ Elogbook sang PMIS
-- Với từng nhà máy thì thông tin chuyển qua PMIS khác nhau nên cần custom đúng thông tin
-- =======================================================================================
CREATE PROCEDURE [dbo].[TSVH_SyncDataPMIS]
AS
BEGIN
	SET NOCOUNT ON

	--Tạo table chưa dữ liệu
	DECLARE @OperationParametersTemp TABLE
	(
		RowIndex INT IDENTITY(1,1),
		AMETERID NVARCHAR(50) NOT NULL,
		DTIME DATETIME NOT NULL,
		VAL_S NVARCHAR(255) NOT NULL,
		READMETHODID NVARCHAR(25) NOT NULL,
		USERID NVARCHAR(25) NULL
	)

	--Lấy dữ liệu từ Elogbook
	INSERT INTO @OperationParametersTemp
	SELECT
		p.QLKTCode AS AMETERID,
		(DATEADD(hh, otl.TimeLineName, DATEDIFF(day, 0, otl.InsertedAt))) AS DTIME,
		(CASE WHEN ISNULL(op.ErrorCode, 0) != 0 
			THEN (CASE WHEN op.ErrorCode = 1 THEN 'F' --Thiết bị bị hỏng (F)
							WHEN op.ErrorCode = 2 THEN '-' --Dừng thiết bị (-)
							WHEN op.ErrorCode = 3 THEN 'S' --Sim thiết bị (S)
							WHEN op.ErrorCode = 4 THEN 'J' --Jump thiết bị (J)
							WHEN op.ErrorCode = 5 THEN 'X' --Bận công tác
							ELSE '' END)
			ELSE op.CurrentValue END) AS VAL_S,
		N'MANUAL' AS READMETHODID,
		N'PM_VH1QD' AS USERID
	FROM Elogbook.dbo.OperationParameters op WITH(NOLOCK)
	INNER JOIN Elogbook.dbo.Parameters p WITH(NOLOCK) ON op.ParameterID = p.ID
	INNER JOIN Elogbook.dbo.OperationTimeLine otl WITH(NOLOCK) on op.OperationTimeLineID = otl.ID AND op.FactoryID = otl.FactoryID 
		AND op.OperationPositionID = otl.OperationPositionID AND op.CycleID = otl.CycleID
	WHERE LEN(ISNULL(p.QLKTCode, '')) > 0 
	AND op.CheckedDate >= DATEADD(HOUR,-200, GETDATE())
	ORDER BY op.CheckedDate DESC

	DECLARE @TotalParams INT = (SELECT COUNT(RowIndex) FROM @OperationParametersTemp),
		@CurrentRowIndex INT = 1,
		@AMETERID_Temp NVARCHAR(50),
		@DTIME_Temp DATETIME,
		@VAL_S_Temp NVARCHAR(255),
		@USERID NVARCHAR(25)

	--WHILE(@CurrentRowIndex <= @TotalParams)
	--BEGIN
	--	SELECT @AMETERID_Temp = temp.AMETERID, @DTIME_Temp = temp.DTIME, @VAL_S_Temp = temp.VAL_S, @USERID = temp.USERID FROM @OperationParametersTemp temp WHERE temp.RowIndex = @CurrentRowIndex

	--	--Nếu dữ liệu đã tồn tại thì cập nhật
	--	IF EXISTS(SELECT pmis.AMETERID FROM [BIZCENTRAL].QLKT_OP.dbo.ZOP_METER2022 pmis WITH(NOLOCK) WHERE pmis.AMETERID = @AMETERID_Temp AND pmis.DTIME = @DTIME_Temp)
	--	BEGIN
	--		UPDATE [BIZCENTRAL].QLKT_OP.dbo.ZOP_METER2022 SET VAL_S = @VAL_S_Temp WHERE AMETERID = @AMETERID_Temp AND DTIME = @DTIME_Temp
	--	END
	--	--Nếu dữ liệu chưa tồn tại thì thêm mới
	--	ELSE
	--	BEGIN
	--		INSERT INTO [BIZCENTRAL].QLKT_OP.dbo.ZOP_METER2022(AMETERID, DTIME, VAL_S, READMETHODID, USERID, USERDTIME, DELTA_VAL, FAILURE)
	--		VALUES(@AMETERID_Temp, @DTIME_Temp, @VAL_S_Temp,'MANUAL', @USERID, GETDATE(), NULL, 0)
	--	END

	--	SET @CurrentRowIndex = @CurrentRowIndex + 1
	--	SET @AMETERID_Temp = NULL
	--	SET @DTIME_Temp = NULL
	--	SET @VAL_S_Temp = NULL
	--	SET @USERID = NULL
	--END
END