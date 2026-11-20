USE Elogbook

--Release 20231122
IF OBJECT_ID (N'SigningMethod', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[SigningMethod] (
		[ID]                INT            IDENTITY (1, 1) NOT NULL,
		[SigningMethodName] NVARCHAR (255) NOT NULL,
		[Active]            INT            NOT NULL,
		[IsDeleted]         BIT            NOT NULL,
		[CreatedUserID]     INT            NULL,
		[InsertedAt]        DATETIME       NOT NULL,
		[LastUpdatedUserID] INT            NULL,
		[UpdatedAt]         DATETIME       NOT NULL,
		CONSTRAINT [PK_dbo.SigningMethod] PRIMARY KEY CLUSTERED ([ID] ASC),
		CONSTRAINT [FK_dbo.SigningMethod_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
		CONSTRAINT [FK_dbo.SigningMethod_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
	);
END
GO

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'Users'
                 AND COLUMN_NAME = 'AvatarAttachmentID')
BEGIN
	ALTER TABLE dbo.Users ADD [AvatarAttachmentID] [int] NULL
END
GO

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'Users'
                 AND COLUMN_NAME = 'SigningMethodID')
BEGIN
	ALTER TABLE dbo.Users ADD [SigningMethodID] [int] NOT NULL DEFAULT(2)
END
GO

IF NOT EXISTS (SELECT TOP 1 SigningMethodName FROM dbo.SigningMethod WHERE SigningMethodName = N'Tài khoản người dùng')
BEGIN
	INSERT INTO [dbo].[SigningMethod]([SigningMethodName], [Active], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt])
     VALUES(N'Tài khoản người dùng', 1, 0, NULL, GETDATE(), NULL, GETDATE())
END

IF NOT EXISTS (SELECT TOP 1 SigningMethodName FROM dbo.SigningMethod WHERE SigningMethodName = N'VIETTEL - CA')
BEGIN
	INSERT INTO [dbo].[SigningMethod]([SigningMethodName], [Active], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt])
     VALUES(N'VIETTEL - CA', 1, 0, NULL, GETDATE(), NULL, GETDATE())
END

IF NOT EXISTS (SELECT TOP 1 SigningMethodName FROM dbo.SigningMethod WHERE SigningMethodName = N'MOBILE PHONE - CA')
BEGIN
	INSERT INTO [dbo].[SigningMethod]([SigningMethodName], [Active], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt])
     VALUES(N'MOBILE PHONE - CA', 0, 0, NULL, GETDATE(), NULL, GETDATE())
END

IF NOT EXISTS (SELECT TOP 1 SigningMethodName FROM dbo.SigningMethod WHERE SigningMethodName = N'VINA PHONE - CA')
BEGIN
	INSERT INTO [dbo].[SigningMethod]([SigningMethodName], [Active], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt])
     VALUES(N'VINA PHONE - CA', 0, 0, NULL, GETDATE(), NULL, GETDATE())
END
--Release 20231122
GO

IF OBJECT_ID (N'SystemData', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[SystemData] (
		[ID]                INT            IDENTITY (1, 1) NOT NULL,
		[DataType]          INT            NOT NULL,
		[Description]       NVARCHAR (500) NOT NULL,
		[Position]          INT            NOT NULL,
		[IsDeleted]         BIT            NOT NULL,
		[CreatedUserID]     INT            NULL,
		[InsertedAt]        DATETIME       NOT NULL,
		[LastUpdatedUserID] INT            NULL,
		[UpdatedAt]         DATETIME       NOT NULL,
		[IsSyncDataMobile]  BIT            DEFAULT ((0)) NOT NULL,
		CONSTRAINT [PK_dbo.SystemData] PRIMARY KEY CLUSTERED ([ID] ASC),
		CONSTRAINT [FK_dbo.SystemData_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
		CONSTRAINT [FK_dbo.SystemData_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
	);
END
GO

IF OBJECT_ID (N'ProbationaryStaff', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[ProbationaryStaff] (
		[ID]                  INT            IDENTITY (1, 1) NOT NULL,
		[OperationDiaryID]    INT            NOT NULL,
		[OperationPositionID] INT            NOT NULL,
		[SyncDataKey]         NVARCHAR (100) NULL,
		[SyncedDate]          DATETIME       NULL,
		[AppID]               NVARCHAR (100) NULL,
		[AssignedUserID]      INT            NULL,
		[AssignedUserName]    NVARCHAR (255) NULL,
		[PositionName]        NVARCHAR (255) NULL,
		[Description]         NVARCHAR (MAX) NULL,
		[IsDeleted]           BIT            NOT NULL,
		[CreatedUserID]       INT            NULL,
		[InsertedAt]          DATETIME       NOT NULL,
		[LastUpdatedUserID]   INT            NULL,
		[UpdatedAt]           DATETIME       NOT NULL,
		CONSTRAINT [PK_dbo.ProbationaryStaff] PRIMARY KEY CLUSTERED ([ID] ASC),
		CONSTRAINT [FK_dbo.ProbationaryStaff_dbo.OperationDiary_OperationDiaryID] FOREIGN KEY ([OperationDiaryID]) REFERENCES [dbo].[OperationDiary] ([ID]) ON DELETE CASCADE,
		CONSTRAINT [FK_dbo.ProbationaryStaff_dbo.OperationPosition_OperationPositionID] FOREIGN KEY ([OperationPositionID]) REFERENCES [dbo].[OperationPosition] ([ID]),
		CONSTRAINT [FK_dbo.ProbationaryStaff_dbo.Users_AssignedUserID] FOREIGN KEY ([AssignedUserID]) REFERENCES [dbo].[Users] ([ID]),
		CONSTRAINT [FK_dbo.ProbationaryStaff_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
		CONSTRAINT [FK_dbo.ProbationaryStaff_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
	);
END
GO

IF OBJECT_ID (N'EmailConfiguration', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[EmailConfiguration] (
		[ID]                INT            IDENTITY (1, 1) NOT NULL,
		[EmailAddress]      NVARCHAR (255) NOT NULL,
		[EmailServer]       NVARCHAR (255) NOT NULL,
		[Port]              INT            NOT NULL,
		[EmailType]         INT            NOT NULL,
		[UserName]          NVARCHAR (255) NOT NULL,
		[PassWord]          NVARCHAR (255) NOT NULL,
		[IsDeleted]         BIT            NOT NULL,
		[CreatedUserID]     INT            NULL,
		[InsertedAt]        DATETIME       NOT NULL,
		[LastUpdatedUserID] INT            NULL,
		[UpdatedAt]         DATETIME       NOT NULL,
		CONSTRAINT [PK_dbo.EmailConfiguration] PRIMARY KEY CLUSTERED ([ID] ASC),
		CONSTRAINT [FK_dbo.EmailConfiguration_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
		CONSTRAINT [FK_dbo.EmailConfiguration_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
	);
END
GO

IF OBJECT_ID (N'EmailTemplate', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[EmailTemplate] (
		[ID]                 INT            IDENTITY (1, 1) NOT NULL,
		[NotificationTypeID] INT            NOT NULL,
		[ModuleID]           INT            NOT NULL,
		[EmailSubject]       NVARCHAR (500) NOT NULL,
		[EmailContent]       NTEXT          NOT NULL,
		[Active]             INT            NOT NULL,
		[FactoryID]          INT            NOT NULL,
		[IsDeleted]          BIT            NOT NULL,
		[CreatedUserID]      INT            NULL,
		[InsertedAt]         DATETIME       NOT NULL,
		[LastUpdatedUserID]  INT            NULL,
		[UpdatedAt]          DATETIME       NOT NULL,
		CONSTRAINT [PK_dbo.EmailTemplate] PRIMARY KEY CLUSTERED ([ID] ASC),
		CONSTRAINT [FK_dbo.EmailTemplate_dbo.Factory_FactoryID] FOREIGN KEY ([FactoryID]) REFERENCES [dbo].[Factory] ([ID]),
		CONSTRAINT [FK_dbo.EmailTemplate_dbo.SystemData_ModuleID] FOREIGN KEY ([ModuleID]) REFERENCES [dbo].[SystemData] ([ID]),
		CONSTRAINT [FK_dbo.EmailTemplate_dbo.SystemData_NotificationTypeID] FOREIGN KEY ([NotificationTypeID]) REFERENCES [dbo].[SystemData] ([ID]),
		CONSTRAINT [FK_dbo.EmailTemplate_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
		CONSTRAINT [FK_dbo.EmailTemplate_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
	);
END
GO

IF OBJECT_ID (N'Schedule', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[Schedule] (
		[ID]                  INT             IDENTITY (1, 1) NOT NULL,
		[ScheduleName]        NVARCHAR (500)  NOT NULL,
		[ScheduleContent]     NVARCHAR (500)  NOT NULL,
		[ScheduleValue]       NVARCHAR (500)  NOT NULL,
		[Active]              INT             NOT NULL,
		[IsSystem]            BIT             NOT NULL,
		[IsSendEmail]         BIT             NOT NULL,
		[EmailTo]             NVARCHAR (500)  NOT NULL,
		[EmailCC]             NVARCHAR (500)  NOT NULL,
		[EmailSubject]        NVARCHAR (500)  NOT NULL,
		[EmailContent]        NTEXT           NOT NULL,
		[IsDeleted]           BIT             NOT NULL,
		[CreatedUserID]       INT             NULL,
		[InsertedAt]          DATETIME        NOT NULL,
		[LastUpdatedUserID]   INT             NULL,
		[UpdatedAt]           DATETIME        NOT NULL,
		[ScheduleDescription] NVARCHAR (1000) NULL,
		CONSTRAINT [PK_dbo.Schedule] PRIMARY KEY CLUSTERED ([ID] ASC),
		CONSTRAINT [FK_dbo.Schedule_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
		CONSTRAINT [FK_dbo.Schedule_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
	);
END
GO

IF OBJECT_ID (N'ActiveDirectory', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[ActiveDirectory] (
		[ID]                      INT            IDENTITY (1, 1) NOT NULL,
		[IsEnableActiveDirectory] BIT            NOT NULL,
		[IPAddress]               NVARCHAR (255) NOT NULL,
		[DomainController]        NVARCHAR (255) NOT NULL,
		[OrganizationalUnit]      NVARCHAR (255) NOT NULL,
		[IsDeleted]               BIT            NOT NULL,
		[CreatedUserID]           INT            NULL,
		[InsertedAt]              DATETIME       NOT NULL,
		[LastUpdatedUserID]       INT            NULL,
		[UpdatedAt]               DATETIME       NOT NULL,
		CONSTRAINT [PK_dbo.ActiveDirectory] PRIMARY KEY CLUSTERED ([ID] ASC),
		CONSTRAINT [FK_dbo.ActiveDirectory_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
		CONSTRAINT [FK_dbo.ActiveDirectory_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
	);
END
GO

IF OBJECT_ID (N'StatusCategory', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[StatusCategory] (
		[ID]                INT            IDENTITY (1, 1) NOT NULL,
		[StatusName]        NVARCHAR (255) NOT NULL,
		[DataTypeID]        INT            NOT NULL,
		[Active]            INT            NOT NULL,
		[Position]          INT            NULL,
		[IsDeleted]         BIT            NOT NULL,
		[CreatedUserID]     INT            NULL,
		[InsertedAt]        DATETIME       NOT NULL,
		[LastUpdatedUserID] INT            NULL,
		[UpdatedAt]         DATETIME       NOT NULL,
		CONSTRAINT [PK_dbo.StatusCategory] PRIMARY KEY CLUSTERED ([ID] ASC),
		CONSTRAINT [FK_dbo.StatusCategory_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
		CONSTRAINT [FK_dbo.StatusCategory_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
	);
END
GO

IF OBJECT_ID (N'EmailTransmissions', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[EmailTransmissions] (
		[ID]                       INT            IDENTITY (1, 1) NOT NULL,
		[EmailReplyToAddr]         NVARCHAR (255) NOT NULL,
		[Subject]                  NVARCHAR (300) NOT NULL,
		[MessageBody]              TEXT           NOT NULL,
		[IsBodyHtml]               BIT            NULL,
		[Retry]                    INT            NULL,
		[DetailFailure]            TEXT           NULL,
		[IsSent]                   BIT            NULL,
		[AttackmentID]             VARCHAR (300)  NULL,
		[TransmissionCompleteFlag] BIT            NULL,
		[InsertedAt]               DATETIME       NOT NULL,
		[UpdatedAt]                DATETIME       NOT NULL,
		[LastUpdatedUserID]        INT            NULL,
		[CreatedUserID]            INT            NULL,
		[IsDeleted]                BIT            NOT NULL
	);
END
GO

IF OBJECT_ID (N'EmailTransmissionRecipients', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[EmailTransmissionRecipients] (
		[ID]                  INT      IDENTITY (1, 1) NOT NULL,
		[EmailTransmissionID] INT      NOT NULL,
		[RecipientEmailAddr]  TEXT     NOT NULL,
		[RecipientEmailName]  TEXT     NOT NULL,
		[RecipientType]       INT      NOT NULL,
		[InsertedAt]          DATETIME NOT NULL,
		[UpdatedAt]           DATETIME NOT NULL,
		[LastUpdatedUserID]   INT      NULL,
		[CreatedUserID]       INT      NULL,
		[IsDeleted]           BIT      NOT NULL
	);
END
GO

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'AppVersion'
                 AND COLUMN_NAME = 'LeftMenuLogoIcon')
BEGIN
	ALTER TABLE dbo.AppVersion ADD [LeftMenuLogoIcon] [int] NULL
END
GO

DECLARE @CurrentDate DATETIME = GETDATE()
IF NOT EXISTS (SELECT TOP 1 DataType FROM dbo.SystemData WHERE DataType = 3 AND Description = N'Vận hành viên')
BEGIN
	INSERT INTO [dbo].[SystemData]([DataType], [Description], [Position], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt], IsSyncDataMobile)
     VALUES(3, N'Vận hành viên', 1, 0, NULL, @CurrentDate, NULL, @CurrentDate, 1)
END

IF NOT EXISTS (SELECT TOP 1 DataType FROM dbo.SystemData WHERE DataType = 3 AND Description = N'EPS - Sửa chữa')
BEGIN
	INSERT INTO [dbo].[SystemData]([DataType], [Description], [Position], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt], IsSyncDataMobile)
     VALUES(3, N'EPS - Sửa chữa', 2, 0, NULL, @CurrentDate, NULL, @CurrentDate, 0)
END

--CR: #64
--Init Schedule module
IF NOT EXISTS(SELECT ModuleValue FROM [dbo].[Modules] WHERE ModuleValue = N'SCHEDULE' AND ModuleName = N'Thiết lập lịch' AND SystemType = N'Admin Portal' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[Modules] ([ModuleValue], [ModuleName], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'SCHEDULE', N'Thiết lập lịch', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END

IF NOT EXISTS(SELECT PermissionKey FROM [dbo].[UserPermission] WHERE PermissionKey = N'VIEW_SCHEDULE' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[UserPermission] ( [PermissionKey], [PermissionName], [Active], [ModuleValue], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'VIEW_SCHEDULE', N'Xem lập lịch', 1, N'SCHEDULE', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END

IF NOT EXISTS(SELECT PermissionKey FROM [dbo].[UserPermission] WHERE PermissionKey = N'ADD_SCHEDULE' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[UserPermission] ( [PermissionKey], [PermissionName], [Active], [ModuleValue], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'ADD_SCHEDULE', N'Thêm mới lập lịch', 1, N'SCHEDULE', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END

IF NOT EXISTS(SELECT PermissionKey FROM [dbo].[UserPermission] WHERE PermissionKey = N'EDIT_SCHEDULE' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[UserPermission] ( [PermissionKey], [PermissionName], [Active], [ModuleValue], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'EDIT_SCHEDULE', N'Cập nhật lập lịch', 1, N'SCHEDULE', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END

IF NOT EXISTS(SELECT PermissionKey FROM [dbo].[UserPermission] WHERE PermissionKey = N'DELETE_SCHEDULE' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[UserPermission] ( [PermissionKey], [PermissionName], [Active], [ModuleValue], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'DELETE_SCHEDULE', N'Xóa lập lịch', 1, N'SCHEDULE', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END

--CR: #11
IF NOT EXISTS(SELECT PermissionKey FROM [dbo].[UserPermission] WHERE PermissionKey = N'EDIT_DELETE_NKVH' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[UserPermission] ( [PermissionKey], [PermissionName], [Active], [ModuleValue], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'EDIT_DELETE_NKVH', N'Cho phép Trưởng ca cập nhật/xóa nhật ký vận hành đã giao/nhận ca', 1, N'NKVH', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END
--CR: #11

IF NOT EXISTS (SELECT Permissions FROM dbo.UserRoles WHERE IsAdmin = 1 AND Permissions LIKE '%VIEW_SCHEDULE%')
BEGIN
	UPDATE dbo.UserRoles SET Permissions = Permissions + ';EDIT_DELETE_NKVH;VIEW_SCHEDULE;ADD_SCHEDULE;EDIT_SCHEDULE;DELETE_SCHEDULE' WHERE IsAdmin = 1
END

IF NOT EXISTS (SELECT TOP 1 ScheduleName FROM dbo.Schedule WHERE ScheduleName = N'DeleteLog')
BEGIN
	INSERT [dbo].[Schedule] ([ScheduleName], [ScheduleContent], [ScheduleValue], [Active], [IsSystem], [IsSendEmail], 
	[EmailTo], [EmailCC], [EmailSubject], [EmailContent], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt], [ScheduleDescription]) 
	VALUES (N'DeleteLog', N'Hiện tại file log trên server khá nhiều, qua thời gian sử dụng số file sẽ tăng lên làm ảnh hưởng đến bộ nhớ của server. Job sẽ tự động xóa log file của nhừng ngày cũ để không làm ảnh hưởng đến bộ nhớ của server.', N'0 0 3 ? * SAT *', 1, 1, 0, N'', N'', N'', N'', 0, NULL, GETDATE(), NULL, GETDATE(), N'Tại 03:00 AM, chỉ trong Thứ Bảy')
END

IF NOT EXISTS (SELECT TOP 1 ScheduleName FROM dbo.Schedule WHERE ScheduleName = N'ResetTicket')
BEGIN
	INSERT [dbo].[Schedule] ([ScheduleName], [ScheduleContent], [ScheduleValue], [Active], [IsSystem], [IsSendEmail], 
	[EmailTo], [EmailCC], [EmailSubject], [EmailContent], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt], [ScheduleDescription]) 
	VALUES (N'ResetTicket', N'Mỗi khi người dùng tạo PĐK hoặc cấp phiếu thì sẽ tự động tăng số phiếu lên một đơn vị. Tuy nhiên nếu người dùng hủy thao tác tạo hoặc cấp thì số phiếu đó sẽ không được sử dụng trong hệ thống. Điều này dẫn đến việc có nhiều số phiếu không được sử dụng, để tối ưu hóa các số phiếu theo số thứ tự được sử dụng. Mỗi ngày sẽ có job chạy để reset tất cả những số phiếu không dược sử dụng đó, để cho phép lần sau khi người dùng tạo hoặc cấp thì số phiếu đó sẽ được sử dụng', N'0 0 1 1/1 * ? *', 1, 1, 0, N'', N'', N'', N'', 0, NULL, GETDATE(), NULL, GETDATE(), N'Tại 01:00 AM')
END
GO
--CR: #64

--CR: #3, #4
IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'Factory'
                 AND COLUMN_NAME = 'ShortName')
BEGIN
	ALTER TABLE dbo.Factory ADD [ShortName] NVARCHAR (100) CONSTRAINT [DF_Factory_ShortName] DEFAULT ('') NOT NULL
END
GO

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'Factory'
                 AND COLUMN_NAME = 'IsUseShortNameToTicket')
BEGIN
	ALTER TABLE dbo.Factory ADD [IsUseShortNameToTicket] BIT CONSTRAINT [DF_Factory_IsUseShortNameToTicket] DEFAULT ((0)) NOT NULL
END
GO

UPDATE dbo.Organization SET ShortName = N'NĐPM' WHERE ISNULL(CompanyType, '') = '' AND Level = N'Công ty' AND Name = N'Công ty nhiệt điện Phú Mỹ'
UPDATE dbo.Organization SET ShortName = N'VH1' WHERE ShortName = 'PXVH1' AND Name = N'Phân xưởng vận hành 1'
UPDATE dbo.Organization SET ShortName = N'VH2' WHERE ShortName = 'PXVH2' AND Name = N'Phân xưởng vận hành 2'
UPDATE dbo.Factory SET ShortName = N'PM1' WHERE FactoryName = N'Phú Mỹ 1'
UPDATE dbo.Factory SET ShortName = N'PM4' WHERE FactoryName = N'Phú Mỹ 4'
UPDATE dbo.Factory SET ShortName = N'PM2.1' WHERE FactoryName = N'Phú Mỹ 2.1'
--CR: #3, #4

--CR: #10
IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'OperationPosition'
                 AND COLUMN_NAME = 'IsConcurrentlyInShift')
BEGIN
	ALTER TABLE dbo.OperationPosition ADD [IsConcurrentlyInShift] BIT DEFAULT ((0)) NOT NULL
END
--CR: #10

--CR: #48, #52
IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'Factory'
                 AND COLUMN_NAME = 'TicketDescription')
BEGIN
	ALTER TABLE dbo.Factory ADD [TicketDescription] NVARCHAR (255) NULL
END
GO

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'Organization'
                 AND COLUMN_NAME = 'TicketDescription')
BEGIN
	ALTER TABLE dbo.Organization ADD [TicketDescription] NVARCHAR (255) NULL
END
GO

UPDATE dbo.Factory SET TicketDescription = N'PHÂN XƯỞNG VHBK' WHERE FactoryName = N'Buôn Kuốp'
UPDATE dbo.Factory SET TicketDescription = N'PHÂN XƯỞNG VHBTS' WHERE FactoryName = N'Buôn Tua Srah'
UPDATE dbo.Factory SET TicketDescription = N'PHÂN XƯỞNG VHSP3' WHERE FactoryName = N'Srêpốk 3'
UPDATE dbo.Organization SET TicketDescription = N'CÔNG TY TĐ BUÔN KUỐP' WHERE Name = N'CÔNG TY THỦY ĐIỆN BUÔN KUỐP'
--CR: #48, #52
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('PCT_ResetTicketNumber'))
	DROP PROC [dbo].[PCT_ResetTicketNumber]
GO

--Clear ticket number of PCT not use every day
CREATE PROCEDURE [dbo].[PCT_ResetTicketNumber]
AS
BEGIN
	SET NOCOUNT ON

    DECLARE @TicketNumberTemp TABLE (TicketNumber INT)

	-- Đăng ký phiếu điện
	INSERT INTO @TicketNumberTemp
	SELECT SUBSTRING(TicketNumber,0, CHARINDEX('/', TicketNumber)) FROM dbo.RegistrationTicket WITH(NOLOCK) WHERE RegistrationTicketType = 1
	DELETE FROM dbo.ExclusiveData WHERE TicketNumber NOT IN(SELECT TicketNumber FROM @TicketNumberTemp) AND TicketType = 'DKPCTD'
	AND InsertedAt <= DATEADD(HOUR,-1,GETDATE())
	DELETE @TicketNumberTemp

	-- Đăng ký phiếu thủy cơ nhiệt hóa
	INSERT INTO @TicketNumberTemp
	SELECT SUBSTRING(TicketNumber,0, CHARINDEX('/', TicketNumber)) FROM dbo.RegistrationTicket WITH(NOLOCK) WHERE RegistrationTicketType = 2
	DELETE FROM dbo.ExclusiveData WHERE TicketNumber NOT IN(SELECT TicketNumber FROM @TicketNumberTemp) AND TicketType = 'DKPTCNH'
	AND InsertedAt <= DATEADD(HOUR,-1,GETDATE())
	DELETE @TicketNumberTemp

	-- Quản lý phiếu điện
	INSERT INTO @TicketNumberTemp
	SELECT SUBSTRING(WorkTicketNumber,0, CHARINDEX('/', WorkTicketNumber)) FROM dbo.WorkTicket WITH(NOLOCK) WHERE ScreenType = 1
	DELETE FROM dbo.ExclusiveData WHERE TicketNumber NOT IN(SELECT TicketNumber FROM @TicketNumberTemp) AND TicketType = 'QLPCTD'
	AND InsertedAt <= DATEADD(HOUR,-1,GETDATE())
	DELETE @TicketNumberTemp

	-- Quản lý phiếu thủy cơ nhiệt hóa
	INSERT INTO @TicketNumberTemp
	SELECT SUBSTRING(WorkTicketNumber,0, CHARINDEX('/', WorkTicketNumber)) FROM dbo.WorkTicket WITH(NOLOCK) WHERE ScreenType = 2
	DELETE FROM dbo.ExclusiveData WHERE TicketNumber NOT IN(SELECT TicketNumber FROM @TicketNumberTemp) AND TicketType = 'QLPTCNH'
	AND InsertedAt <= DATEADD(HOUR,-1,GETDATE())
	DELETE @TicketNumberTemp

	SELECT 'SUCCESS' AS Message
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('TSVH_GetParameters'))
	DROP PROC [dbo].[TSVH_GetParameters]
GO
-- =============================================
-- Lấy danh sách ghi thông số theo điều kiện
-- Fix issue performance
-- =============================================
--TEST: EXECUTE [dbo].[TSVH_GetParameters] 1, 2, 3, 2, 45672, 1, 10
CREATE PROCEDURE [dbo].[TSVH_GetParameters]
	@FactoryID INT = 0,
	@CycleID INT = 0,
	@ParameterGroupID INT = 0,
	@OperationPositionID INT = 0,
	@OperationTimeLineID INT = 0,
	@PageNumber INT = 1,
	@PageSize INT = 10
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @IsUseOperationParameterGroup BIT = (SELECT TOP 1 IsUseOperationParameterGroup FROM dbo.Factory WITH(NOLOCK) WHERE ID = @FactoryID)
	IF(@ParameterGroupID IS NULL) SET @ParameterGroupID = 0
	IF(@IsUseOperationParameterGroup IS NULL) SET @IsUseOperationParameterGroup = 0

	DECLARE @TempData TABLE(
		[RowIndex] INT IDENTITY(1,1),
		[ID] INT,
		[IsGroup] BIT,
		[ParameterID] INT,
		[ParameterName] NVARCHAR(255),
		[KKSCode] NVARCHAR(255),
		[UnitMeasure] NVARCHAR(255),
		[PreviousValue] NVARCHAR(MAX),
		[CurrentValue] NVARCHAR(MAX),
		[ErrorCode] INT,
		[FromRange] REAL,
		[ToRange] REAL,
		[IsNumberType] BIT,
		[ParameterGroupID] INT,
		[OprerationTimeLineID] INT,
		[IsChecked] BIT,
		[AlertDetails] NVARCHAR(MAX),
		[CheckedDate] DATETIME,
		[Setting] NVARCHAR(MAX)
	)

	INSERT INTO @TempData
	SELECT
		p.ID,
		0,
		p.ParameterID,
		p.ParameterName,
		p.KKSCode,
		p.UnitMeasure,
		p.PreviousValue,
		p.CurrentValue,
		p.ErrorCode,
		p.FromRange,
		p.ToRange,
		p.IsNumberType,
		p.ParameterGroupID,
		p.OperationTimeLineID,
		p.IsChecked,
		p.AlertDetails,
		p.CheckedDate,
		NULL
	FROM dbo.OperationParameters p WITH(NOLOCK)
	WHERE p.FactoryID = @FactoryID
	AND p.OperationPositionID = @OperationPositionID
	AND p.OperationTimeLineID = @OperationTimeLineID
	AND p.CycleID = @CycleID
	AND p.IsDeleted = 0
	AND ((@IsUseOperationParameterGroup = 1 AND (@ParameterGroupID > 0 AND p.ParameterGroupID = @ParameterGroupID)
		OR (@ParameterGroupID <= 0 AND p.ParameterGroupID IS NULL)) OR @IsUseOperationParameterGroup = 0)

	DECLARE @TotalRecords INT = (SELECT COUNT(ID) FROM @TempData)

	SELECT temp.ID,
		CAST(CASE WHEN @ParameterGroupID > 0 THEN 1 ELSE 0 END AS BIT) AS IsGroup,
		temp.ParameterID,
		temp.ParameterName,
		temp.KKSCode,
		temp.UnitMeasure,
		REVERSE(REPLACE(STUFF((
					SELECT ':' + (CASE WHEN ISNULL(pd.ErrorCode, 0) = 1 THEN 'F' --Thiết bị bị hỏng (F)
										WHEN ISNULL(pd.ErrorCode, 0) = 2 THEN '-' --Dừng thiết bị (-)
										WHEN ISNULL(pd.ErrorCode, 0) = 3 THEN 'S' --Sim thiết bị (S)
										WHEN ISNULL(pd.ErrorCode, 0) = 4 THEN 'J' --Jump thiết bị (J)
										WHEN ISNULL(pd.ErrorCode, 0) = 5 THEN 'x' --Bận công tác
									ELSE ISNULL(REVERSE(pd.CurrentValue), '') END)
					FROM dbo.OperationParameters pd WITH(NOLOCK)
					WHERE pd.FactoryID = @FactoryID
						AND pd.OperationPositionID = @OperationPositionID
						AND pd.OperationTimeLineID < temp.OprerationTimeLineID
						AND pd.CycleID = @CycleID
						AND pd.ParameterID = temp.ParameterID
						AND pd.IsDeleted = 0
					ORDER BY pd.OperationTimeLineID DESC
					OFFSET (1-1) * 5 ROWS
					FETCH NEXT 5 ROWS ONLY
					FOR XML PATH(''), TYPE).value('.', 'VARCHAR(MAX)'), 1, 1, ''), 
			':', ' : ')) AS PreviousValue,
		temp.CurrentValue,
		CASE WHEN temp.ErrorCode = 5 THEN NULL ELSE temp.ErrorCode END AS ErrorCode,
		temp.FromRange,
		temp.ToRange,
		temp.IsNumberType,
		temp.ParameterGroupID,
		temp.OprerationTimeLineID,
		temp.IsChecked,
		temp.AlertDetails,
		temp.CheckedDate,
		REPLACE(STUFF((
					SELECT ',' + ISNULL(Alert,'') + ' ' + ISNULL(Operator,'') + ' ' + ISNULL(CAST(Value AS NVARCHAR(100)),'') + ' : ' + ISNULL(Color,'') + ';'
					FROM dbo.AlertDetails WITH(NOLOCK)
					WHERE ParameterID = temp.ParameterID
					FOR XML PATH(''), TYPE).value('.', 'VARCHAR(MAX)'), 1, 1, ''), 
			';,', ';') AS Setting,
		@TotalRecords AS TotalRecords
	FROM @TempData temp
	ORDER BY temp.ID
	OFFSET (@PageNumber-1) * @PageSize ROWS
	FETCH NEXT @PageSize ROWS ONLY
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('TSVH_GetDataForPMIS'))
	DROP PROC [dbo].[TSVH_GetDataForPMIS]
GO
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
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('TSVH_AddOperationParameterByTime'))
	DROP PROC [dbo].[TSVH_AddOperationParameterByTime]
GO
-- =============================================
-- Thêm mới danh sách ghi thông số theo chu kỳ
-- Fix issue performance: giảm từ 1 phút còn 3 giây
-- =============================================
CREATE PROCEDURE [dbo].[TSVH_AddOperationParameterByTime]
	@FactoryID INT,
	@OperationPositionID INT,
	@CycleID INT,
	@DateCreate DATE,
	@CurrentUserID INT
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @TotalParamOther INT = 0,
		@CurrentDate DATETIME = GETDATE(),
		@Result NVARCHAR(20) = 'ERROR'

	--Danh sách tất cả thông số theo Nhà máy, Cương vị, Chu kỳ đang sử dụng
	DECLARE @OperationParametersTemp TABLE
	(
		[RowIndex] INT IDENTITY(1,1) NOT NULL,
		[ParameterName] [nvarchar](255) NOT NULL,
		[ParameterID] [int] NOT NULL,
		[ParameterGroupID] [int] NULL,
		[KKSCode] [nvarchar](255) NULL,
		[UnitMeasure] [nvarchar](255) NULL,
		[Setting] [nvarchar](max) NULL,
		[FromRange] [real] NULL,
		[ToRange] [real] NULL,
		[IsNumberType] [bit] NOT NULL,
		[AlertDetails] [nvarchar](max) NULL
	)
	INSERT INTO @OperationParametersTemp
	SELECT
		p.ParameterName,
		p.ID,
		p.ParameterGroupID,
		p.KKSCode,
		p.UnitMeasure,
		p.Setting,
		p.FromRange,
		p.ToRange,
		p.IsNumberType,
		REPLACE(STUFF((
				SELECT ',' + ISNULL(Alert,'') + ' ' + ISNULL(Operator,'') + ' ' + ISNULL(CAST(Value AS NVARCHAR(100)),'') + ' : ' + ISNULL(Color,'') + ';'
				FROM dbo.AlertDetails WITH(NOLOCK)
				WHERE ParameterID = p.ID
				FOR XML PATH(''), TYPE).value('.', 'VARCHAR(MAX)'), 1, 1, ''), 
		';,', ';') AS AlertDetails
	FROM dbo.Parameters p WITH(NOLOCK)
	WHERE p.FactoryID = @FactoryID
		AND p.OperationPositionID = @OperationPositionID
		AND p.CycleID = @CycleID
		AND p.Status = 1
		AND p.IsDeleted = 0
	ORDER BY p.Position ASC, p.ID ASC

	DECLARE @OperationTimeLineTemp TABLE
	(
		RowIndex INT IDENTITY(1,1), 
		OperationTimeLineID INT, 
		TimeLineSyncDataKey NVARCHAR(100)
	)
	SET @TotalParamOther = (SELECT COUNT(ParameterID) FROM @OperationParametersTemp)
	--Tạo các khung giờ để tạo dữ liệu ghi thông số vận hành
	IF(ISNULL(@TotalParamOther, 0) > 0 AND EXISTS(SELECT TOP 1 ID FROM dbo.TimeLineCycle WITH(NOLOCK) WHERE CycleID = @CycleID AND Active = 1 AND IsDeleted = 0))
	BEGIN
		INSERT INTO dbo.OperationTimeLine
		(
			[TimeLineSyncDataKey], 
			[TimeLineName], 
			[FactoryID], 
			[OperationPositionID], 
			[CycleID], 
			[TotalParametersDefault], 
			[TotalParametersChecked], 
			[TotalParametersSynchronized], 
			[IsDeleted], 
			[CreatedUserID], 
			[InsertedAt], 
			[LastUpdatedUserID], 
			[UpdatedAt], 
			[AppID], 
			[SyncedDate]
		)
		OUTPUT INSERTED.[ID], INSERTED.[TimeLineSyncDataKey]
		INTO @OperationTimeLineTemp
		(
			OperationTimeLineID,
			TimeLineSyncDataKey
		)
		SELECT 
			CAST(Hour AS VARCHAR(20)) + '_' + Replace(CONVERT(VARCHAR(20), @DateCreate, 103),'/','') + '_' + 
				CAST(@FactoryID AS VARCHAR(20)) + '_' + CAST(@OperationPositionID AS VARCHAR(20)) + '_' + CAST(@CycleID AS VARCHAR(20)), --[TimeLineSyncDataKey]
			Hour, --[TimeLineName]
			@FactoryID, --[FactoryID]
			@OperationPositionID, --[OperationPositionID]
			@CycleID, --[CycleID]
			@TotalParamOther, --[TotalParametersDefault]
			0, --[TotalParametersChecked]
			0, --[TotalParametersSynchronized]
			0, --[IsDeleted]
			@CurrentUserID, --[CreatedUserID]
			@DateCreate, --[InsertedAt]
			@CurrentUserID, --[LastUpdatedUserID]
			@CurrentDate, --[UpdatedAt]
			NULL, --[AppID]
			@CurrentDate --[SyncedDate]
		FROM dbo.TimeLineCycle WITH(NOLOCK)
		WHERE CycleID = @CycleID AND Active = 1 AND IsDeleted = 0
		ORDER BY Hour ASC

		DECLARE @TotalTimeLine INT = (SELECT COUNT(OperationTimeLineID) OperationTimeLineID FROM @OperationTimeLineTemp),
			@TimeLineIndex INT = 1,
			@OperationTimeLineID INT,
			@TimeLineSyncDataKey NVARCHAR(100)

		IF(ISNULL(@TotalTimeLine, 0) > 0)
		BEGIN
			--Duyệt qua các khung giờ để tạo danh sách thông số
			WHILE(@TimeLineIndex <= @TotalTimeLine)
			BEGIN
				SELECT @OperationTimeLineID = OperationTimeLineID, @TimeLineSyncDataKey = TimeLineSyncDataKey FROM @OperationTimeLineTemp WHERE RowIndex = @TimeLineIndex
				IF(ISNULL(@OperationTimeLineID, 0) > 0 AND ISNULL(@TimeLineSyncDataKey, '') != '')
				BEGIN
					INSERT INTO dbo.OperationParameters
					(
						[ParameterName], 
						[ParameterID], 
						[ParameterGroupID], 
						[OperationTimeLineID], 
						[FactoryID], 
						[OperationPositionID], 
						[CycleID], 
						[CurrentValue], 
						[KKSCode], 
						[UnitMeasure], 
						[Setting], 
						[FromRange], 
						[ToRange], 
						[IsNumberType], 
						[PreviousValue], 
						[ErrorCode], 
						[IsChecked], 
						[AlertDetails], 
						[IsDeleted], 
						[CreatedUserID], 
						[InsertedAt], 
						[LastUpdatedUserID], 
						[UpdatedAt], 
						[SyncDataKey], 
						[AppID], 
						[CheckedDate], 
						[SyncedDate]
					)
					SELECT
						[ParameterName], 
						[ParameterID], 
						[ParameterGroupID], 
						@OperationTimeLineID, 
						@FactoryID, 
						@OperationPositionID, 
						@CycleID, 
						NULL, 
						[KKSCode], 
						[UnitMeasure], 
						[Setting], 
						[FromRange], 
						[ToRange], 
						[IsNumberType], 
						'', 
						NULL, 
						0, 
						[AlertDetails], 
						0, --[IsDeleted] 
						@CurrentUserID, --[CreatedUserID] 
						@CurrentDate, --[InsertedAt] 
						@CurrentUserID, --[LastUpdatedUserID] 
						@CurrentDate, --[UpdatedAt] 
						@TimeLineSyncDataKey, --[SyncDataKey] 
						NULL, --[AppID] 
						NULL, --[CheckedDate] 
						@CurrentDate --[SyncedDate]
					FROM @OperationParametersTemp
					ORDER BY RowIndex
				END

				SET @OperationTimeLineID = NULL
				SET @TimeLineSyncDataKey = NULL
				SET @TimeLineIndex = @TimeLineIndex + 1
			END

			SET @Result = 'SUCCESS'
		END
	END

	--Kết quả tạo danh sách thông số
	SELECT @Result AS Message
END