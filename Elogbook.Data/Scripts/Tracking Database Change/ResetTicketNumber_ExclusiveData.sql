USE [msdb]
GO

/****** Object:  Job [ResetTicketNumber_ExclusiveData]    Script Date: 8/4/2022 1:09:08 AM ******/
BEGIN TRANSACTION
DECLARE @ReturnCode INT
SELECT @ReturnCode = 0
/****** Object:  JobCategory [[Uncategorized (Local)]]    Script Date: 8/4/2022 1:09:09 AM ******/
IF NOT EXISTS (SELECT name FROM msdb.dbo.syscategories WHERE name=N'[Uncategorized (Local)]' AND category_class=1)
BEGIN
EXEC @ReturnCode = msdb.dbo.sp_add_category @class=N'JOB', @type=N'LOCAL', @name=N'[Uncategorized (Local)]'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

END

DECLARE @jobId BINARY(16)
EXEC @ReturnCode =  msdb.dbo.sp_add_job @job_name=N'ResetTicketNumber_ExclusiveData', 
		@enabled=1, 
		@notify_level_eventlog=0, 
		@notify_level_email=0, 
		@notify_level_netsend=0, 
		@notify_level_page=0, 
		@delete_level=0, 
		@description=N'Cập nhật số phiếu được tạo tự động nhưng không được sử dụng trong phiếu thực tế. Khi người dùng kích hoạt để tạo phiếu sẽ tạo ra số mới, nếu người dùng không lưu phiếu thì qua ngày mới số phiếu đó sẽ được xóa.', 
		@category_name=N'[Uncategorized (Local)]', 
		@owner_login_name=N'sa', @job_id = @jobId OUTPUT
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [ResetTicketNumber]    Script Date: 8/4/2022 1:09:09 AM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'ResetTicketNumber', 
		@step_id=1, 
		@cmdexec_success_code=0, 
		@on_success_action=1, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'DECLARE @TicketNumberTemp TABLE (TicketNumber INT)

-- Đăng ký phiếu điện
INSERT INTO @TicketNumberTemp
SELECT SUBSTRING(TicketNumber,0, CHARINDEX(''/'', TicketNumber)) FROM dbo.RegistrationTicket WITH(NOLOCK) WHERE RegistrationTicketType = 1
DELETE FROM dbo.ExclusiveData WHERE TicketNumber NOT IN(SELECT TicketNumber FROM @TicketNumberTemp) AND TicketType = ''DKPCTD''
AND InsertedAt <= DATEADD(HOUR,-1,GETDATE())
DELETE @TicketNumberTemp

-- Đăng ký phiếu thủy cơ nhiệt hóa
INSERT INTO @TicketNumberTemp
SELECT SUBSTRING(TicketNumber,0, CHARINDEX(''/'', TicketNumber)) FROM dbo.RegistrationTicket WITH(NOLOCK) WHERE RegistrationTicketType = 2
DELETE FROM dbo.ExclusiveData WHERE TicketNumber NOT IN(SELECT TicketNumber FROM @TicketNumberTemp) AND TicketType = ''DKPTCNH''
AND InsertedAt <= DATEADD(HOUR,-1,GETDATE())
DELETE @TicketNumberTemp

-- Quản lý phiếu điện
INSERT INTO @TicketNumberTemp
SELECT SUBSTRING(WorkTicketNumber,0, CHARINDEX(''/'', WorkTicketNumber)) FROM dbo.WorkTicket WITH(NOLOCK) WHERE ScreenType = 1
DELETE FROM dbo.ExclusiveData WHERE TicketNumber NOT IN(SELECT TicketNumber FROM @TicketNumberTemp) AND TicketType = ''QLPCTD''
AND InsertedAt <= DATEADD(HOUR,-1,GETDATE())
DELETE @TicketNumberTemp

-- Quản lý phiếu thủy cơ nhiệt hóa
INSERT INTO @TicketNumberTemp
SELECT SUBSTRING(WorkTicketNumber,0, CHARINDEX(''/'', WorkTicketNumber)) FROM dbo.WorkTicket WITH(NOLOCK) WHERE ScreenType = 2
DELETE FROM dbo.ExclusiveData WHERE TicketNumber NOT IN(SELECT TicketNumber FROM @TicketNumberTemp) AND TicketType = ''QLPTCNH''
AND InsertedAt <= DATEADD(HOUR,-1,GETDATE())
DELETE @TicketNumberTemp', 
		@database_name=N'elogbook', 
		@flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_update_job @job_id = @jobId, @start_step_id = 1
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_add_jobschedule @job_id=@jobId, @name=N'ResetTicketNumber_Time', 
		@enabled=1, 
		@freq_type=4, 
		@freq_interval=1, 
		@freq_subday_type=1, 
		@freq_subday_interval=0, 
		@freq_relative_interval=0, 
		@freq_recurrence_factor=0, 
		@active_start_date=20220531, 
		@active_end_date=99991231, 
		@active_start_time=10000, 
		@active_end_time=235959, 
		@schedule_uid=N'35508afa-6ab5-47d6-895e-66c18f99c0bc'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_add_jobserver @job_id = @jobId, @server_name = N'(local)'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
COMMIT TRANSACTION
GOTO EndSave
QuitWithRollback:
    IF (@@TRANCOUNT > 0) ROLLBACK TRANSACTION
EndSave:
GO


