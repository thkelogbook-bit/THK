USE Elogbook

--20032023
IF NOT EXISTS(SELECT ModuleValue FROM [dbo].[Modules] WHERE ModuleValue = N'CHHTEMAIL' AND ModuleName = N'Cấu hình hộp thư' AND SystemType = N'Admin Portal' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[Modules] ([ModuleValue], [ModuleName], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'CHHTEMAIL', N'Cấu hình hộp thư', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END

IF NOT EXISTS(SELECT PermissionKey FROM [dbo].[UserPermission] WHERE PermissionKey = N'VIEW_CHHTEMAIL' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[UserPermission] ( [PermissionKey], [PermissionName], [Active], [ModuleValue], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'VIEW_CHHTEMAIL', N'Xem cấu hình hộp thư', 1, N'CHHTEMAIL', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END

IF NOT EXISTS(SELECT PermissionKey FROM [dbo].[UserPermission] WHERE PermissionKey = N'EDIT_CHHTEMAIL' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[UserPermission] ( [PermissionKey], [PermissionName], [Active], [ModuleValue], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'EDIT_CHHTEMAIL', N'Cập nhật cấu hình hộp thư', 1, N'CHHTEMAIL', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END
--20032023

--21032023
DECLARE @CurrentDate DATETIME = GETDATE()
INSERT INTO [dbo].[SystemData]([DataType], [Description], [Position], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt])
     VALUES(1, N'Tạo phiếu đăng ký công tác/Phiếu thao tác', 1, 0, NULL, @CurrentDate, NULL, @CurrentDate)
		,(1, N'Gửi phiếu đăng ký công tác/Phiếu thao tác', 2, 0, NULL, @CurrentDate, NULL, @CurrentDate)
		,(1, N'Thu hồi phiếu đăng ký công tác/Phiếu thao tác', 3, 0, NULL, @CurrentDate, NULL, @CurrentDate)
		,(1, N'Từ chối phiếu đăng ký công tác/Phiếu thao tác', 4, 0, NULL, @CurrentDate, NULL, @CurrentDate)
		,(1, N'Cấp phiếu đăng ký công tác/Phiếu thao tác', 5, 0, NULL, @CurrentDate, NULL, @CurrentDate)
		,(1, N'Cho phép thực hiện phiếu công tác/Phiếu thao tác', 6, 0, NULL, @CurrentDate, NULL, @CurrentDate)
		,(1, N'Thực hiện hoàn thành phiếu công tác/Phiếu thao tác', 7, 0, NULL, @CurrentDate, NULL, @CurrentDate)
		,(1, N'Hủy phiếu thao tác', 8, 0, NULL, @CurrentDate, NULL, @CurrentDate)
		,(2, N'Phiếu công tác điện', 1, 0, NULL, @CurrentDate, NULL, @CurrentDate)
		,(2, N'Phiếu công tác thủy-cơ-nhiệt-hóa', 1, 0, NULL, @CurrentDate, NULL, @CurrentDate)
		,(2, N'Phiếu thao tác', 1, 0, NULL, @CurrentDate, NULL, @CurrentDate)
		,(2, N'Các chức năng khác', 1, 0, NULL, @CurrentDate, NULL, @CurrentDate)

IF NOT EXISTS(SELECT ModuleValue FROM [dbo].[Modules] WHERE ModuleValue = N'EMAILT' AND ModuleName = N'Cấu hình thư mẫu' AND SystemType = N'Admin Portal' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[Modules] ([ModuleValue], [ModuleName], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'EMAILT', N'Cấu hình thư mẫu', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END

IF NOT EXISTS(SELECT PermissionKey FROM [dbo].[UserPermission] WHERE PermissionKey = N'VIEW_EMAILT' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[UserPermission] ( [PermissionKey], [PermissionName], [Active], [ModuleValue], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'VIEW_EMAILT', N'Xem thư mẫu', 1, N'EMAILT', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END

IF NOT EXISTS(SELECT PermissionKey FROM [dbo].[UserPermission] WHERE PermissionKey = N'ADD_EMAILT' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[UserPermission] ( [PermissionKey], [PermissionName], [Active], [ModuleValue], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'ADD_EMAILT', N'Thêm mới thư mẫu', 1, N'EMAILT', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END

IF NOT EXISTS(SELECT PermissionKey FROM [dbo].[UserPermission] WHERE PermissionKey = N'EDIT_EMAILT' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[UserPermission] ( [PermissionKey], [PermissionName], [Active], [ModuleValue], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'EDIT_EMAILT', N'Cập nhật thư mẫu', 1, N'EMAILT', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END

IF NOT EXISTS(SELECT PermissionKey FROM [dbo].[UserPermission] WHERE PermissionKey = N'DELETE_EMAILT' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[UserPermission] ( [PermissionKey], [PermissionName], [Active], [ModuleValue], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'DELETE_EMAILT', N'Xóa thư mẫu', 1, N'EMAILT', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END
--21032023

--25032023
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
--25032023

--26032023
IF NOT EXISTS(SELECT ModuleValue FROM [dbo].[Modules] WHERE ModuleValue = N'AD' AND ModuleName = N'Cấu hình Active Directory' AND SystemType = N'Admin Portal' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[Modules] ([ModuleValue], [ModuleName], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'AD', N'Cấu hình Active Directory', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END

IF NOT EXISTS(SELECT PermissionKey FROM [dbo].[UserPermission] WHERE PermissionKey = N'VIEW_AD' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[UserPermission] ( [PermissionKey], [PermissionName], [Active], [ModuleValue], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'VIEW_AD', N'Xem cấu hình Active Directory', 1, N'AD', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END

IF NOT EXISTS(SELECT PermissionKey FROM [dbo].[UserPermission] WHERE PermissionKey = N'EDIT_AD' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[UserPermission] ( [PermissionKey], [PermissionName], [Active], [ModuleValue], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'EDIT_AD', N'Cập nhật cấu hình Active Directory', 1, N'AD', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END
--26032023

--14042023
IF NOT EXISTS(SELECT ModuleValue FROM [dbo].[Modules] WHERE ModuleValue = N'CH_PMIS' AND ModuleName = N'Cấu hình PMIS' AND SystemType = N'Admin Portal' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[Modules] ([ModuleValue], [ModuleName], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'CH_PMIS', N'Cấu hình PMIS', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END

IF NOT EXISTS(SELECT PermissionKey FROM [dbo].[UserPermission] WHERE PermissionKey = N'VIEW_CH_PMIS' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[UserPermission] ( [PermissionKey], [PermissionName], [Active], [ModuleValue], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'VIEW_CH_PMIS', N'Xem cấu hình PMIS', 1, N'CH_PMIS', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END

IF NOT EXISTS(SELECT PermissionKey FROM [dbo].[UserPermission] WHERE PermissionKey = N'EDIT_CH_PMIS' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[UserPermission] ( [PermissionKey], [PermissionName], [Active], [ModuleValue], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'EDIT_CH_PMIS', N'Cập nhật cấu hình PMIS', 1, N'CH_PMIS', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END
--14042023

--17052023 (#11)
IF NOT EXISTS(SELECT PermissionKey FROM [dbo].[UserPermission] WHERE PermissionKey = N'EDIT_DELETE_NKVH' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[UserPermission] ( [PermissionKey], [PermissionName], [Active], [ModuleValue], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'EDIT_DELETE_NKVH', N'Cho phép Trưởng ca cập nhật/xóa nhật ký vận hành đã giao/nhận ca', 1, N'NKVH', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END
--17052023

--05072023
DECLARE @CurrentDate DATETIME = GETDATE()
INSERT INTO [dbo].[SystemData]([DataType], [Description], [Position], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt])
     VALUES(3, N'Trạng thái Bất thường sự cố', 1, 0, NULL, @CurrentDate, NULL, @CurrentDate)

IF NOT EXISTS(SELECT ModuleValue FROM [dbo].[Modules] WHERE ModuleValue = N'DM_TT' AND ModuleName = N'Danh mục trạng thái' AND SystemType = N'Admin Portal' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[Modules] ([ModuleValue], [ModuleName], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'DM_TT', N'Danh mục trạng thái', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END

IF NOT EXISTS(SELECT PermissionKey FROM [dbo].[UserPermission] WHERE PermissionKey = N'VIEW_DM_TT' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[UserPermission] ( [PermissionKey], [PermissionName], [Active], [ModuleValue], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'VIEW_DM_TT', N'Xem trạng thái', 1, N'DM_TT', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END

IF NOT EXISTS(SELECT PermissionKey FROM [dbo].[UserPermission] WHERE PermissionKey = N'ADD_DM_TT' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[UserPermission] ( [PermissionKey], [PermissionName], [Active], [ModuleValue], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'ADD_DM_TT', N'Thêm mới trạng thái', 1, N'DM_TT', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END

IF NOT EXISTS(SELECT PermissionKey FROM [dbo].[UserPermission] WHERE PermissionKey = N'EDIT_DM_TT' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[UserPermission] ( [PermissionKey], [PermissionName], [Active], [ModuleValue], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'EDIT_DM_TT', N'Cập nhật trạng thái', 1, N'DM_TT', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END

IF NOT EXISTS(SELECT PermissionKey FROM [dbo].[UserPermission] WHERE PermissionKey = N'DELETE_DM_TT' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[UserPermission] ( [PermissionKey], [PermissionName], [Active], [ModuleValue], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'DELETE_DM_TT', N'Xóa trạng thái', 1, N'DM_TT', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END

IF NOT EXISTS (SELECT * FROM dbo.StatusCategory WHERE StatusName = N'Mới phát hiện' AND DataTypeID = 13 AND IsDeleted = 0)
BEGIN
	INSERT INTO dbo.StatusCategory([StatusName], [DataTypeID], [Active], [Position], [IsDeleted], [InsertedAt], [UpdatedAt])
	VALUES(N'Mới phát hiện', 13, 1, 1, 0, GETDATE(), GETDATE())
END

IF NOT EXISTS (SELECT * FROM dbo.StatusCategory WHERE StatusName = N'Chờ dừng máy' AND DataTypeID = 13 AND IsDeleted = 0)
BEGIN
	INSERT INTO dbo.StatusCategory([StatusName], [DataTypeID], [Active], [Position], [IsDeleted], [InsertedAt], [UpdatedAt])
	VALUES(N'Chờ dừng máy', 13, 1, 2, 0, GETDATE(), GETDATE())
END

IF NOT EXISTS (SELECT * FROM dbo.StatusCategory WHERE StatusName = N'Chờ vật tư' AND DataTypeID = 13 AND IsDeleted = 0)
BEGIN
	INSERT INTO dbo.StatusCategory([StatusName], [DataTypeID], [Active], [Position], [IsDeleted], [InsertedAt], [UpdatedAt])
	VALUES(N'Chờ vật tư', 13, 1, 3, 0, GETDATE(), GETDATE())
END

IF NOT EXISTS (SELECT * FROM dbo.StatusCategory WHERE StatusName = N'Đang xử lý' AND DataTypeID = 13 AND IsDeleted = 0)
BEGIN
	INSERT INTO dbo.StatusCategory([StatusName], [DataTypeID], [Active], [Position], [IsDeleted], [InsertedAt], [UpdatedAt])
	VALUES(N'Đang xử lý', 13, 1, 4, 0, GETDATE(), GETDATE())
END

IF NOT EXISTS (SELECT * FROM dbo.StatusCategory WHERE StatusName = N'Kết thúc' AND DataTypeID = 13 AND IsDeleted = 0)
BEGIN
	INSERT INTO dbo.StatusCategory([StatusName], [DataTypeID], [Active], [Position], [IsDeleted], [InsertedAt], [UpdatedAt])
	VALUES(N'Kết thúc', 13, 1, 5, 0, GETDATE(), GETDATE())
END
--05072023

--10072023
ALTER TABLE [dbo].[AbnormalIncident]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AbnormalIncident_dbo.OperationDiary_OperationDiaryID] FOREIGN KEY([OperationDiaryID])
REFERENCES [dbo].[OperationDiary] ([ID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[AbnormalIncident] CHECK CONSTRAINT [FK_dbo.AbnormalIncident_dbo.OperationDiary_OperationDiaryID]
GO

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'AbnormalIncident'
                 AND COLUMN_NAME = 'RegistrationTicketID')
BEGIN
	ALTER TABLE dbo.AbnormalIncident ADD [RegistrationTicketID] [int] NULL
END

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'AbnormalIncident'
                 AND COLUMN_NAME = 'RegistrationTicketName')
BEGIN
	ALTER TABLE dbo.AbnormalIncident ADD [RegistrationTicketName] [nvarchar](255) NULL
END

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'AbnormalIncident'
                 AND COLUMN_NAME = 'TeamLeader')
BEGIN
	ALTER TABLE dbo.AbnormalIncident ADD [TeamLeader] [nvarchar](255) NULL
END

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'AbnormalIncident'
                 AND COLUMN_NAME = 'Area')
BEGIN
	ALTER TABLE dbo.AbnormalIncident ADD [Area] [nvarchar](max) NULL
END

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'AbnormalIncident'
                 AND COLUMN_NAME = 'StatusID')
BEGIN
	ALTER TABLE dbo.AbnormalIncident ADD [StatusID] [int] NULL
END

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'AbnormalIncident'
                 AND COLUMN_NAME = 'IsDeleteIssueOfShift')
BEGIN
	ALTER TABLE dbo.AbnormalIncident ADD [IsDeleteIssueOfShift] [bit] NOT NULL DEFAULT(0)
END

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'AbnormalIncident'
                 AND COLUMN_NAME = 'CompletedDateTime')
BEGIN
	ALTER TABLE dbo.AbnormalIncident ADD [CompletedDateTime] [datetime] NULL
END
--10072023
--#31, #32 update for Vĩnh Tân
UPDATE dbo.Factory SET IsFilterFactory = 0