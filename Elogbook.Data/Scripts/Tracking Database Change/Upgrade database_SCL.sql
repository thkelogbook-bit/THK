--USE Elogbook
--GO

--1. MajorRepairStatus
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MajorRepairStatus]') AND type in (N'U'))
DROP TABLE [dbo].[MajorRepairStatus]
GO

CREATE TABLE [dbo].[MajorRepairStatus](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Status] [nvarchar](255) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedUserID] [int] NULL,
	[InsertedAt] [datetime] NOT NULL,
	[LastUpdatedUserID] [int] NULL,
	[UpdatedAt] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.MajorRepairStatus] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[MajorRepairStatus] ON 

INSERT [dbo].[MajorRepairStatus] ([ID], [Status], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) VALUES (1, N'Chưa thực hiện cấp phiếu', 0, NULL, CAST(N'2023-12-29T20:58:53.397' AS DateTime), NULL, CAST(N'2023-12-29T20:58:53.397' AS DateTime))
INSERT [dbo].[MajorRepairStatus] ([ID], [Status], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) VALUES (2, N'Đang cấp phiếu hàng loạt', 0, NULL, CAST(N'2023-12-29T20:58:53.397' AS DateTime), NULL, CAST(N'2023-12-29T20:58:53.397' AS DateTime))
INSERT [dbo].[MajorRepairStatus] ([ID], [Status], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) VALUES (3, N'Hoàn thành cấp phiếu', 0, NULL, CAST(N'2023-12-29T20:58:53.397' AS DateTime), NULL, CAST(N'2023-12-29T20:58:53.397' AS DateTime))
INSERT [dbo].[MajorRepairStatus] ([ID], [Status], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) VALUES (4, N'Có lỗi trong quá trình xử lý', 0, NULL, CAST(N'2023-12-29T20:58:53.397' AS DateTime), NULL, CAST(N'2023-12-29T20:58:53.397' AS DateTime))
SET IDENTITY_INSERT [dbo].[MajorRepairStatus] OFF
GO
ALTER TABLE [dbo].[MajorRepairStatus]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MajorRepairStatus_dbo.Users_CreatedUserID] FOREIGN KEY([CreatedUserID])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[MajorRepairStatus] CHECK CONSTRAINT [FK_dbo.MajorRepairStatus_dbo.Users_CreatedUserID]
GO
ALTER TABLE [dbo].[MajorRepairStatus]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MajorRepairStatus_dbo.Users_LastUpdatedUserID] FOREIGN KEY([LastUpdatedUserID])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[MajorRepairStatus] CHECK CONSTRAINT [FK_dbo.MajorRepairStatus_dbo.Users_LastUpdatedUserID]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MajorRepairExcelDataTemps]') AND type in (N'U'))
DROP TABLE [dbo].[MajorRepairExcelDataTemps]
GO
--MajorRepairStatus

--2. MajorRepair
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MajorRepair]') AND type in (N'U'))
DROP TABLE [dbo].[MajorRepair]
GO

CREATE TABLE [dbo].[MajorRepair](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[FactoryID] [int] NOT NULL,
	[Title] [nvarchar](250) NOT NULL,
	[MajorRepairStatusID] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[PlanCreateTicketTime] [datetime] NOT NULL,
	[AttachmentID] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedUserID] [int] NULL,
	[InsertedAt] [datetime] NOT NULL,
	[LastUpdatedUserID] [int] NULL,
	[UpdatedAt] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.MajorRepair] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[MajorRepair]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MajorRepair_dbo.Attachment_AttachmentID] FOREIGN KEY([AttachmentID])
REFERENCES [dbo].[Attachment] ([ID])
GO

ALTER TABLE [dbo].[MajorRepair] CHECK CONSTRAINT [FK_dbo.MajorRepair_dbo.Attachment_AttachmentID]
GO

ALTER TABLE [dbo].[MajorRepair]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MajorRepair_dbo.Factory_FactoryID] FOREIGN KEY([FactoryID])
REFERENCES [dbo].[Factory] ([ID])
GO

ALTER TABLE [dbo].[MajorRepair] CHECK CONSTRAINT [FK_dbo.MajorRepair_dbo.Factory_FactoryID]
GO

ALTER TABLE [dbo].[MajorRepair]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MajorRepair_dbo.MajorRepairStatus_MajorRepairStatusID] FOREIGN KEY([MajorRepairStatusID])
REFERENCES [dbo].[MajorRepairStatus] ([ID])
GO

ALTER TABLE [dbo].[MajorRepair] CHECK CONSTRAINT [FK_dbo.MajorRepair_dbo.MajorRepairStatus_MajorRepairStatusID]
GO

ALTER TABLE [dbo].[MajorRepair]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MajorRepair_dbo.Users_CreatedUserID] FOREIGN KEY([CreatedUserID])
REFERENCES [dbo].[Users] ([ID])
GO

ALTER TABLE [dbo].[MajorRepair] CHECK CONSTRAINT [FK_dbo.MajorRepair_dbo.Users_CreatedUserID]
GO

ALTER TABLE [dbo].[MajorRepair]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MajorRepair_dbo.Users_LastUpdatedUserID] FOREIGN KEY([LastUpdatedUserID])
REFERENCES [dbo].[Users] ([ID])
GO

ALTER TABLE [dbo].[MajorRepair] CHECK CONSTRAINT [FK_dbo.MajorRepair_dbo.Users_LastUpdatedUserID]
GO
--MajorRepair

--3. MajorRepairExcelDataTemps
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MajorRepairExcelDataTemps]') AND type in (N'U'))
DROP TABLE [dbo].[MajorRepairExcelDataTemps]
GO

CREATE TABLE [dbo].[MajorRepairExcelDataTemps](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[MajorRepairID] [int] NOT NULL,
	[RowIndex] [int] NOT NULL,
	[WorkContent] [nvarchar](4000) NOT NULL,
	[WorkTicketLocation] [nvarchar](500) NOT NULL,
	[ScreenType] [int] NOT NULL,
	[WorkTicketUnitArea] [nvarchar](500) NOT NULL,
	[Errors] [nvarchar](4000) NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedUserID] [int] NULL,
	[InsertedAt] [datetime] NOT NULL,
	[LastUpdatedUserID] [int] NULL,
	[UpdatedAt] [datetime] NOT NULL,
 CONSTRAINT [PK_ExcelDataTemps] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[MajorRepairExcelDataTemps]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MajorRepairExcelDataTemps_dbo.MajorRepair_MajorRepairStatusID] FOREIGN KEY([MajorRepairID])
REFERENCES [dbo].[MajorRepair] ([ID])
GO

ALTER TABLE [dbo].[MajorRepairExcelDataTemps] CHECK CONSTRAINT [FK_dbo.MajorRepairExcelDataTemps_dbo.MajorRepair_MajorRepairStatusID]
GO

ALTER TABLE [dbo].[MajorRepairExcelDataTemps]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MajorRepairExcelDataTemps_dbo.Users_CreatedUserID] FOREIGN KEY([CreatedUserID])
REFERENCES [dbo].[Users] ([ID])
GO

ALTER TABLE [dbo].[MajorRepairExcelDataTemps] CHECK CONSTRAINT [FK_dbo.MajorRepairExcelDataTemps_dbo.Users_CreatedUserID]
GO

ALTER TABLE [dbo].[MajorRepairExcelDataTemps]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MajorRepairExcelDataTemps_dbo.Users_LastUpdatedUserID] FOREIGN KEY([LastUpdatedUserID])
REFERENCES [dbo].[Users] ([ID])
GO

ALTER TABLE [dbo].[MajorRepairExcelDataTemps] CHECK CONSTRAINT [FK_dbo.MajorRepairExcelDataTemps_dbo.Users_LastUpdatedUserID]
GO
--MajorRepairExcelDataTemps

--4. ScheduleHistories
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ScheduleHistories]') AND type in (N'U'))
DROP TABLE [dbo].[ScheduleHistories]
GO

CREATE TABLE [dbo].[ScheduleHistories](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ScheduleName] [nvarchar](500) NULL,
	[Status] [int] NULL,
	[ScheduleTime] [datetime] NULL,
	[ErrorMessage] [nvarchar](max) NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedUserID] [int] NULL,
	[InsertedAt] [datetime] NOT NULL,
	[LastUpdatedUserID] [int] NULL,
	[UpdatedAt] [datetime] NOT NULL,
 CONSTRAINT [PK__Schedule__3214EC27C7EED5C1] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[ScheduleHistories]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ScheduleHistories_dbo.Users_CreatedUserID] FOREIGN KEY([CreatedUserID])
REFERENCES [dbo].[Users] ([ID])
GO

ALTER TABLE [dbo].[ScheduleHistories] CHECK CONSTRAINT [FK_dbo.ScheduleHistories_dbo.Users_CreatedUserID]
GO

ALTER TABLE [dbo].[ScheduleHistories]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ScheduleHistories_dbo.Users_LastUpdatedUserID] FOREIGN KEY([LastUpdatedUserID])
REFERENCES [dbo].[Users] ([ID])
GO

ALTER TABLE [dbo].[ScheduleHistories] CHECK CONSTRAINT [FK_dbo.ScheduleHistories_dbo.Users_LastUpdatedUserID]
GO
--ScheduleHistories

--5. RegistrationTicket
IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'RegistrationTicket'
                 AND COLUMN_NAME = 'WorkTicketID')
BEGIN
	ALTER TABLE dbo.RegistrationTicket Add WorkTicketID INT NULL
END

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'RegistrationTicket'
                 AND COLUMN_NAME = 'MajorRepairID')
BEGIN
	ALTER TABLE dbo.RegistrationTicket Add MajorRepairID INT NULL
END
--RegistrationTicket

--6. WorkTicket
IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'WorkTicket'
                 AND COLUMN_NAME = 'MajorRepairID')
BEGIN
	ALTER TABLE dbo.WorkTicket Add MajorRepairID INT NULL
END
--WorkTicket

--7. TicketStatus
IF NOT EXISTS (SELECT TOP 1 StatusName FROM dbo.TicketStatus WHERE StatusName = N'Chờ đăng ký')
BEGIN
	SET IDENTITY_INSERT TicketStatus ON
	INSERT INTO dbo.TicketStatus (ID, StatusName, Active, IsDeleted, CreatedUserID, InsertedAt, LastUpdatedUserID, UpdatedAt)
	VALUES(11, N'Chờ đăng ký', 1, 0, NULL, GETDATE(), NULL, GETDATE())
	SET IDENTITY_INSERT TicketStatus OFF
END
--TicketStatus

--8. Schedule
IF NOT EXISTS (SELECT TOP 1 ScheduleName FROM dbo.Schedule WHERE ScheduleName = N'workticket')
BEGIN
	INSERT [dbo].[Schedule] ([ScheduleName], [ScheduleContent], [ScheduleValue], [Active], [IsSystem], [IsSendEmail], 
	[EmailTo], [EmailCC], [EmailSubject], [EmailContent], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt], [ScheduleDescription]) 
	VALUES (N'workticket', N'Tạo đợt công tác SCL', N'0 0/1 * 1/1 * ? *', 1, 1, 0, N'', N'', N'', N'', 0, NULL, GETDATE(), NULL, GETDATE(), N'Mỗi phút, mỗi giờ')
END
--Schedule

--Thêm quyền cho Đợt công tác SCL
IF NOT EXISTS(SELECT ModuleValue FROM [dbo].[Modules] WHERE ModuleValue = N'QLSCL' AND ModuleName = N'Quản lý Đợt công tác SCL' AND SystemType = N'Admin Portal' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[Modules] ([ModuleValue], [ModuleName], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'QLSCL', N'Quản lý Đợt công tác SCL', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END

GO
IF NOT EXISTS(SELECT PermissionKey FROM [dbo].[UserPermission] WHERE PermissionKey = N'VIEW_QLSCL' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[UserPermission] ( [PermissionKey], [PermissionName], [Active], [ModuleValue], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'VIEW_QLSCL', N'Xem thông tin Đợt công tác SCL', 1, N'QLSCL', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END

GO
IF NOT EXISTS(SELECT PermissionKey FROM [dbo].[UserPermission] WHERE PermissionKey = N'ADD_QLSCL' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[UserPermission] ( [PermissionKey], [PermissionName], [Active], [ModuleValue], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'ADD_QLSCL', N'Thêm mới Đợt công tác SCL', 1, N'QLSCL', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END

GO
IF NOT EXISTS(SELECT PermissionKey FROM [dbo].[UserPermission] WHERE PermissionKey = N'EDIT_QLSCL' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[UserPermission] ( [PermissionKey], [PermissionName], [Active], [ModuleValue], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'EDIT_QLSCL', N'Cập nhật Đợt công tác SCL', 1, N'QLSCL', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END

GO
IF NOT EXISTS(SELECT PermissionKey FROM [dbo].[UserPermission] WHERE PermissionKey = N'DELETE_QLSCL' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[UserPermission] ( [PermissionKey], [PermissionName], [Active], [ModuleValue], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'DELETE_QLSCL', N'Xóa Đợt công tác SCL', 1, N'QLSCL', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END

GO
IF NOT EXISTS (SELECT Permissions FROM dbo.UserRoles WHERE IsAdmin = 1 AND Permissions LIKE '%VIEW_QLSCL%')
BEGIN
	UPDATE dbo.UserRoles SET Permissions = Permissions + ';VIEW_QLSCL;ADD_QLSCL;EDIT_QLSCL;DELETE_QLSCL' WHERE IsAdmin = 1
END
--Thêm quyền cho Đợt công tác SCL