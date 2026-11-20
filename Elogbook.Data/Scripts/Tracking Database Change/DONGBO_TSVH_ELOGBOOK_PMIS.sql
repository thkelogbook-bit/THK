USE [msdb]
GO

/****** Object:  Job [DONGBO_TSVH_ELOGBOOK_PMIS]    Script Date: 8/4/2022 12:54:51 AM ******/
BEGIN TRANSACTION
DECLARE @ReturnCode INT
SELECT @ReturnCode = 0
/****** Object:  JobCategory [[Uncategorized (Local)]]    Script Date: 8/4/2022 12:54:51 AM ******/
IF NOT EXISTS (SELECT name FROM msdb.dbo.syscategories WHERE name=N'[Uncategorized (Local)]' AND category_class=1)
BEGIN
EXEC @ReturnCode = msdb.dbo.sp_add_category @class=N'JOB', @type=N'LOCAL', @name=N'[Uncategorized (Local)]'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

END

DECLARE @jobId BINARY(16)
EXEC @ReturnCode =  msdb.dbo.sp_add_job @job_name=N'DONGBO_TSVH_ELOGBOOK_PMIS', 
		@enabled=1, 
		@notify_level_eventlog=0, 
		@notify_level_email=0, 
		@notify_level_netsend=0, 
		@notify_level_page=0, 
		@delete_level=0, 
		@description=N'JOB có chức năng đồng bộ dữ liệu TSVH từ phần mềm Elogbook sang phần mềm PMIS', 
		@category_name=N'[Uncategorized (Local)]', 
		@owner_login_name=N'sa', @job_id = @jobId OUTPUT
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [DONGBO_TSVH_ELOGBOOK_PMIS]    Script Date: 8/4/2022 12:54:51 AM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'DONGBO_TSVH_ELOGBOOK_PMIS', 
		@step_id=1, 
		@cmdexec_success_code=0, 
		@on_success_action=1, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'--Tạo table chưa dữ liệu
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
		THEN (CASE WHEN op.ErrorCode = 1 THEN ''F'' --Thiết bị bị hỏng (F)
						WHEN op.ErrorCode = 2 THEN ''-'' --Dừng thiết bị (-)
						WHEN op.ErrorCode = 3 THEN ''S'' --Sim thiết bị (S)
						WHEN op.ErrorCode = 4 THEN ''J'' --Jump thiết bị (J)
						WHEN op.ErrorCode = 5 THEN ''X'' --Bận công tác
						ELSE '''' END)
		ELSE ISNULL(op.CurrentValue, '''') END) AS VAL_S,
	N''MANUAL'' AS READMETHODID,
	(SELECT TOP 1 ISNULL(PMIS_USERID,'''') FROM elogbook.dbo.Factory WHERE ID = op.FactoryID) AS USERID
FROM elogbook.dbo.OperationParameters op WITH(NOLOCK)
INNER JOIN elogbook.dbo.Parameters p WITH(NOLOCK) ON op.ParameterID = p.ID
INNER JOIN elogbook.dbo.OperationTimeLine otl WITH(NOLOCK) on op.OperationTimeLineID = otl.ID AND op.FactoryID = otl.FactoryID 
	AND op.OperationPositionID = otl.OperationPositionID AND op.CycleID = otl.CycleID
WHERE LEN(ISNULL(p.QLKTCode, '''')) > 0 
AND op.CheckedDate >= DATEADD(HOUR,-2, GETDATE())
ORDER BY op.CheckedDate DESC

DECLARE @TotalParams INT = (SELECT COUNT(RowIndex) FROM @OperationParametersTemp),
	@CurrentRowIndex INT = 1,
	@AMETERID_Temp NVARCHAR(50),
	@DTIME_Temp DATETIME,
	@VAL_S_Temp NVARCHAR(255),
	@USERID NVARCHAR(25)

WHILE(@CurrentRowIndex <= @TotalParams)
BEGIN
	SELECT @AMETERID_Temp = temp.AMETERID, @DTIME_Temp = temp.DTIME, @VAL_S_Temp = temp.VAL_S, @USERID = temp.USERID FROM @OperationParametersTemp temp WHERE temp.RowIndex = @CurrentRowIndex

	--Nếu dữ liệu đã tồn tại thì cập nhật
	IF EXISTS(SELECT pmis.AMETERID FROM [BIZCENTRAL].QLKT_OP.dbo.ZOP_METER2022 pmis WITH(NOLOCK) WHERE pmis.AMETERID = @AMETERID_Temp AND pmis.DTIME = @DTIME_Temp)
	BEGIN
		UPDATE [BIZCENTRAL].QLKT_OP.dbo.ZOP_METER2022 SET VAL_S = @VAL_S_Temp WHERE AMETERID = @AMETERID_Temp AND DTIME = @DTIME_Temp
	END
	--Nếu dữ liệu chưa tồn tại thì thêm mới
	ELSE
	BEGIN
		INSERT INTO [BIZCENTRAL].QLKT_OP.dbo.ZOP_METER2022(AMETERID, DTIME, VAL_S, READMETHODID, USERID, USERDTIME, DELTA_VAL, FAILURE)
		VALUES(@AMETERID_Temp, @DTIME_Temp, @VAL_S_Temp,''MANUAL'', @USERID, GETDATE(), NULL, 0)
	END

	SET @CurrentRowIndex = @CurrentRowIndex + 1
	SET @AMETERID_Temp = NULL
	SET @DTIME_Temp = NULL
	SET @VAL_S_Temp = NULL
	SET @USERID = NULL
END', 
		@database_name=N'elogbook', 
		@flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_update_job @job_id = @jobId, @start_step_id = 1
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_add_jobschedule @job_id=@jobId, @name=N'DONGBO', 
		@enabled=1, 
		@freq_type=4, 
		@freq_interval=1, 
		@freq_subday_type=4, 
		@freq_subday_interval=30, 
		@freq_relative_interval=0, 
		@freq_recurrence_factor=0, 
		@active_start_date=20200316, 
		@active_end_date=99991231, 
		@active_start_time=0, 
		@active_end_time=235959, 
		@schedule_uid=N'76243f8e-24e6-44e9-9bbc-adbcc46c6c9f'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_add_jobserver @job_id = @jobId, @server_name = N'(local)'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
COMMIT TRANSACTION
GOTO EndSave
QuitWithRollback:
    IF (@@TRANCOUNT > 0) ROLLBACK TRANSACTION
EndSave:
GO
