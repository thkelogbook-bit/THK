USE Elogbook

--CR: #63
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
IF NOT EXISTS (SELECT TOP 1 ScheduleName FROM dbo.Schedule WHERE ScheduleName = N'PMIS')
BEGIN
	INSERT [dbo].[Schedule] ([ScheduleName], [ScheduleContent], [ScheduleValue], [Active], [IsSystem], [IsSendEmail], 
	[EmailTo], [EmailCC], [EmailSubject], [EmailContent], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt], [ScheduleDescription]) 
	VALUES (N'PMIS', N'Đồng bộ PMIS', N'0 0/30 * 1/1 * ? *', 1, 1, 0, N'', N'', N'', N'', 0, NULL, GETDATE(), NULL, GETDATE(), N'Mỗi 30 phút, mỗi giờ')
END

GO
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

GO
IF NOT EXISTS (SELECT Permissions FROM dbo.UserRoles WHERE IsAdmin = 1 AND Permissions LIKE '%VIEW_CH_PMIS%')
BEGIN
	UPDATE dbo.UserRoles SET Permissions = Permissions + ';VIEW_CH_PMIS;EDIT_CH_PMIS' WHERE IsAdmin = 1
END
--CR: #63

GO
--CR: #17, #20
IF OBJECT_ID (N'TicketEvent', N'U') IS NOT NULL
BEGIN
	DROP TABLE dbo.[TicketEvent]
END

CREATE TABLE [dbo].[TicketEvent](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[FactoryID] [int] NOT NULL,
	[DataType] [int] NOT NULL,
	[EventTypeID] [int] NOT NULL,
	[EventContent] [nvarchar](500) NULL,
	[Active] [int] NOT NULL,
	[IsSystem] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedUserID] [int] NULL,
	[InsertedAt] [datetime] NOT NULL,
	[LastUpdatedUserID] [int] NULL,
	[UpdatedAt] [datetime] NOT NULL,
CONSTRAINT [PK_dbo.TicketEvent] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TicketEvent]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TicketEvent_dbo.Factory_FactoryID] FOREIGN KEY([FactoryID])
REFERENCES [dbo].[Factory] ([ID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[TicketEvent] CHECK CONSTRAINT [FK_dbo.TicketEvent_dbo.Factory_FactoryID]
GO

ALTER TABLE [dbo].[TicketEvent]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TicketEvent_dbo.SystemData_EventTypeID] FOREIGN KEY([EventTypeID])
REFERENCES [dbo].[SystemData] ([ID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[TicketEvent] CHECK CONSTRAINT [FK_dbo.TicketEvent_dbo.SystemData_EventTypeID]
GO

ALTER TABLE [dbo].[TicketEvent]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TicketEvent_dbo.Users_CreatedUserID] FOREIGN KEY([CreatedUserID])
REFERENCES [dbo].[Users] ([ID])
GO

ALTER TABLE [dbo].[TicketEvent] CHECK CONSTRAINT [FK_dbo.TicketEvent_dbo.Users_CreatedUserID]
GO

ALTER TABLE [dbo].[TicketEvent]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TicketEvent_dbo.Users_LastUpdatedUserID] FOREIGN KEY([LastUpdatedUserID])
REFERENCES [dbo].[Users] ([ID])
GO

ALTER TABLE [dbo].[TicketEvent] CHECK CONSTRAINT [FK_dbo.TicketEvent_dbo.Users_LastUpdatedUserID]
GO

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'AppVersion'
                 AND COLUMN_NAME = 'IsSyncData_PCT_TDTTC')
BEGIN
	ALTER TABLE dbo.AppVersion Add IsSyncData_PCT_TDTTC BIT NOT NULL DEFAULT(0)
END

GO
DECLARE @CurrentDate DATETIME = GETDATE(), @DataType INT = NULL
--Phiếu công tác điện
SET @DataType = 5
IF NOT EXISTS (SELECT TOP 1 DataType FROM dbo.SystemData WHERE DataType = @DataType AND Description = N'Cấp phiếu công tác')
BEGIN
	INSERT INTO [dbo].[SystemData]([DataType], [Description], [Position], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt])
     VALUES(@DataType, N'Cấp phiếu công tác', 1, 0, NULL, @CurrentDate, NULL, @CurrentDate)
END

IF NOT EXISTS (SELECT TOP 1 DataType FROM dbo.SystemData WHERE DataType = @DataType AND Description = N'Cho phép thực hiện phiếu công tác')
BEGIN
	INSERT INTO [dbo].[SystemData]([DataType], [Description], [Position], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt])
     VALUES(@DataType, N'Cho phép thực hiện phiếu công tác', 1, 0, NULL, @CurrentDate, NULL, @CurrentDate)
END

IF NOT EXISTS (SELECT TOP 1 DataType FROM dbo.SystemData WHERE DataType = @DataType AND Description = N'Bắt đầu công tác hàng ngày')
BEGIN
	INSERT INTO [dbo].[SystemData]([DataType], [Description], [Position], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt])
     VALUES(@DataType, N'Bắt đầu công tác hàng ngày', 1, 0, NULL, @CurrentDate, NULL, @CurrentDate)
END

IF NOT EXISTS (SELECT TOP 1 DataType FROM dbo.SystemData WHERE DataType = @DataType AND Description = N'Kết thúc công tác hàng ngày')
BEGIN
	INSERT INTO [dbo].[SystemData]([DataType], [Description], [Position], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt])
     VALUES(@DataType, N'Kết thúc công tác hàng ngày', 1, 0, NULL, @CurrentDate, NULL, @CurrentDate)
END

IF NOT EXISTS (SELECT TOP 1 DataType FROM dbo.SystemData WHERE DataType = @DataType AND Description = N'Khóa phiếu')
BEGIN
	INSERT INTO [dbo].[SystemData]([DataType], [Description], [Position], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt])
     VALUES(@DataType, N'Khóa phiếu', 1, 0, NULL, @CurrentDate, NULL, @CurrentDate)
END

IF NOT EXISTS (SELECT TOP 1 DataType FROM dbo.SystemData WHERE DataType = @DataType AND Description = N'Hoàn thành phiếu công tác')
BEGIN
	INSERT INTO [dbo].[SystemData]([DataType], [Description], [Position], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt])
     VALUES(@DataType, N'Hoàn thành phiếu công tác', 1, 0, NULL, @CurrentDate, NULL, @CurrentDate)
END

--Phiếu công tác thủy-cơ-nhiệt-hóa
SET @DataType = 6
IF NOT EXISTS (SELECT TOP 1 DataType FROM dbo.SystemData WHERE DataType = @DataType AND Description = N'Cấp phiếu công tác')
BEGIN
	INSERT INTO [dbo].[SystemData]([DataType], [Description], [Position], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt])
     VALUES(@DataType, N'Cấp phiếu công tác', 1, 0, NULL, @CurrentDate, NULL, @CurrentDate)
END

IF NOT EXISTS (SELECT TOP 1 DataType FROM dbo.SystemData WHERE DataType = @DataType AND Description = N'Cho phép thực hiện phiếu công tác')
BEGIN
	INSERT INTO [dbo].[SystemData]([DataType], [Description], [Position], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt])
     VALUES(@DataType, N'Cho phép thực hiện phiếu công tác', 1, 0, NULL, @CurrentDate, NULL, @CurrentDate)
END

IF NOT EXISTS (SELECT TOP 1 DataType FROM dbo.SystemData WHERE DataType = @DataType AND Description = N'Kết thúc công tác')
BEGIN
	INSERT INTO [dbo].[SystemData]([DataType], [Description], [Position], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt])
     VALUES(@DataType, N'Kết thúc công tác', 1, 0, NULL, @CurrentDate, NULL, @CurrentDate)
END

GO
TRUNCATE TABLE dbo.TicketEvent
DECLARE @CurrentDate DATETIME = GETDATE(),
	@TotalFactory INT = 0,
	@IndexNumber INT = 1,
	@FactoryID INT = NULL,
	@DataType INT = NULL

DECLARE @FactoryTemp TABLE (IndexNumber INT IDENTITY(1,1), FactoryID INT)
INSERT INTO @FactoryTemp
SELECT ID FROM dbo.Factory WITH(NOLOCK) WHERE Status = 1 AND IsDeleted = 0

SELECT @TotalFactory = COUNT(ID) FROM dbo.Factory WITH(NOLOCK) WHERE Status = 1 AND IsDeleted = 0

WHILE (@IndexNumber <= @TotalFactory)
BEGIN
	SET @FactoryID = (SELECT TOP 1 FactoryID FROM @FactoryTemp WHERE IndexNumber = @IndexNumber)
	--Phiếu công tác điện
	SET @DataType = 5
	INSERT INTO [dbo].[TicketEvent](FactoryID, DataType, EventTypeID, EventContent, Active, IsSystem, IsDeleted, CreatedUserID, InsertedAt, LastUpdatedUserID, UpdatedAt)
		 VALUES(@FactoryID, 
		 @DataType, 
		 (SELECT TOP 1 ID FROM dbo.SystemData WHERE DataType = @DataType AND Description = N'Cấp phiếu công tác'), 
		 N'Cấp PCT {NUMBER_TICKET} lúc {DATE}.', 1, 1, 0, NULL, @CurrentDate, NULL, @CurrentDate)

	INSERT INTO [dbo].[TicketEvent](FactoryID, DataType, EventTypeID, EventContent, Active, IsSystem, IsDeleted, CreatedUserID, InsertedAt, LastUpdatedUserID, UpdatedAt)
		 VALUES(@FactoryID, 
		 @DataType, 
		 (SELECT TOP 1 ID FROM dbo.SystemData WHERE DataType = @DataType AND Description = N'Cho phép thực hiện phiếu công tác'), 
		 N'Cho phép PCT {NUMBER_TICKET} bắt đầu làm việc lúc {DATE}.', 1, 1, 0, NULL, @CurrentDate, NULL, @CurrentDate)

	INSERT INTO [dbo].[TicketEvent](FactoryID, DataType, EventTypeID, EventContent, Active, IsSystem, IsDeleted, CreatedUserID, InsertedAt, LastUpdatedUserID, UpdatedAt)
		 VALUES(@FactoryID, 
		 @DataType, 
		 (SELECT TOP 1 ID FROM dbo.SystemData WHERE DataType = @DataType AND Description = N'Bắt đầu công tác hàng ngày'), 
		 N'PCT {NUMBER_TICKET} bắt đầu công tác hàng ngày lúc {DATE}. Số nhân viên đơn vị công tác: {NUMBER}.', 1, 1, 0, NULL, @CurrentDate, NULL, @CurrentDate)

	INSERT INTO [dbo].[TicketEvent](FactoryID, DataType, EventTypeID, EventContent, Active, IsSystem, IsDeleted, CreatedUserID, InsertedAt, LastUpdatedUserID, UpdatedAt)
		 VALUES(@FactoryID, 
		 @DataType, 
		 (SELECT TOP 1 ID FROM dbo.SystemData WHERE DataType = @DataType AND Description = N'Kết thúc công tác hàng ngày'), 
		 N'PCT {NUMBER_TICKET} kết thúc công tác hàng ngày lúc {DATE}. Số nhân viên đơn vị công tác: {NUMBER}.', 1, 1, 0, NULL, @CurrentDate, NULL, @CurrentDate)

	INSERT INTO [dbo].[TicketEvent](FactoryID, DataType, EventTypeID, EventContent, Active, IsSystem, IsDeleted, CreatedUserID, InsertedAt, LastUpdatedUserID, UpdatedAt)
		 VALUES(@FactoryID, 
		 @DataType, 
		 (SELECT TOP 1 ID FROM dbo.SystemData WHERE DataType = @DataType AND Description = N'Khóa phiếu'), 
		 N'Khóa kết thúc công tác PCT {NUMBER_TICKET} lúc {DATE} (lưu sổ VHV).', 1, 1, 0, NULL, @CurrentDate, NULL, @CurrentDate)

	INSERT INTO [dbo].[TicketEvent](FactoryID, DataType, EventTypeID, EventContent, Active, IsSystem, IsDeleted, CreatedUserID, InsertedAt, LastUpdatedUserID, UpdatedAt)
		 VALUES(@FactoryID, 
		 @DataType, 
		 (SELECT TOP 1 ID FROM dbo.SystemData WHERE DataType = @DataType AND Description = N'Hoàn thành phiếu công tác'), 
		 N'Khóa kết thúc công tác PCT {NUMBER_TICKET} lúc {DATE} (sổ Trưởng ca).', 1, 1, 0, NULL, @CurrentDate, NULL, @CurrentDate)

	--Phiếu công tác thủy-cơ-nhiệt-hóa
	SET @DataType = 6
	INSERT INTO [dbo].[TicketEvent](FactoryID, DataType, EventTypeID, EventContent, Active, IsSystem, IsDeleted, CreatedUserID, InsertedAt, LastUpdatedUserID, UpdatedAt)
		 VALUES(@FactoryID, 
		 @DataType, 
		 (SELECT TOP 1 ID FROM dbo.SystemData WHERE DataType = @DataType AND Description = N'Cấp phiếu công tác'), 
		 N'Cấp PCT {NUMBER_TICKET} lúc {DATE} (lưu sổ NKVH của trưởng ca).', 1, 1, 0, NULL, @CurrentDate, NULL, @CurrentDate)

	INSERT INTO [dbo].[TicketEvent](FactoryID, DataType, EventTypeID, EventContent, Active, IsSystem, IsDeleted, CreatedUserID, InsertedAt, LastUpdatedUserID, UpdatedAt)
		 VALUES(@FactoryID, 
		 @DataType, 
		 (SELECT TOP 1 ID FROM dbo.SystemData WHERE DataType = @DataType AND Description = N'Cho phép thực hiện phiếu công tác'), 
		 N'Cho phép PCT {NUMBER_TICKET} bắt đầu làm việc lúc {DATE} (Lưu sổ VHV).', 1, 1, 0, NULL, @CurrentDate, NULL, @CurrentDate)

	INSERT INTO [dbo].[TicketEvent](FactoryID, DataType, EventTypeID, EventContent, Active, IsSystem, IsDeleted, CreatedUserID, InsertedAt, LastUpdatedUserID, UpdatedAt)
		 VALUES(@FactoryID, 
		 @DataType, 
		 (SELECT TOP 1 ID FROM dbo.SystemData WHERE DataType = @DataType AND Description = N'Kết thúc công tác'), 
		 N'Khóa kết thúc công tác PCT {NUMBER_TICKET} lúc {DATE} (lưu sổ VHV).', 1, 1, 0, NULL, @CurrentDate, NULL, @CurrentDate)

	SET @IndexNumber = @IndexNumber + 1
	SET @FactoryID = NULL
	SET @DataType = NULL
END

GO
IF NOT EXISTS(SELECT ModuleValue FROM [dbo].[Modules] WHERE ModuleValue = N'QLSK' AND ModuleName = N'Quản lý sự kiện' AND SystemType = N'Admin Portal' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[Modules] ([ModuleValue], [ModuleName], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'QLSK', N'Quản lý sự kiện', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END

GO
IF NOT EXISTS(SELECT PermissionKey FROM [dbo].[UserPermission] WHERE PermissionKey = N'VIEW_QLSK' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[UserPermission] ( [PermissionKey], [PermissionName], [Active], [ModuleValue], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'VIEW_QLSK', N'Xem sự kiện', 1, N'QLSK', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END

GO
IF NOT EXISTS(SELECT PermissionKey FROM [dbo].[UserPermission] WHERE PermissionKey = N'ADD_QLSK' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[UserPermission] ( [PermissionKey], [PermissionName], [Active], [ModuleValue], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'ADD_QLSK', N'Thêm mới sự kiện', 1, N'QLSK', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END

GO
IF NOT EXISTS(SELECT PermissionKey FROM [dbo].[UserPermission] WHERE PermissionKey = N'EDIT_QLSK' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[UserPermission] ( [PermissionKey], [PermissionName], [Active], [ModuleValue], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'EDIT_QLSK', N'Cập nhật sự kiện', 1, N'QLSK', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END

GO
IF NOT EXISTS(SELECT PermissionKey FROM [dbo].[UserPermission] WHERE PermissionKey = N'DELETE_QLSK' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[UserPermission] ( [PermissionKey], [PermissionName], [Active], [ModuleValue], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'DELETE_QLSK', N'Xóa sự kiện', 1, N'QLSK', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END

GO
IF NOT EXISTS (SELECT Permissions FROM dbo.UserRoles WHERE IsAdmin = 1 AND Permissions LIKE '%VIEW_QLSK%')
BEGIN
	UPDATE dbo.UserRoles SET Permissions = Permissions + ';VIEW_QLSK;ADD_QLSK;EDIT_QLSK;DELETE_QLSK' WHERE IsAdmin = 1
END
--CR: #17, #20

--CR: #7
GO
IF NOT EXISTS(SELECT PermissionKey FROM [dbo].[UserPermission] WHERE PermissionKey = N'DELETE_TPTT_ADMIN' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[UserPermission] ( [PermissionKey], [PermissionName], [Active], [ModuleValue], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'DELETE_TPTT_ADMIN', N'Cho phép admin xóa phiếu thao tác', 1, N'PTT', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END

GO
IF NOT EXISTS (SELECT Permissions FROM dbo.UserRoles WHERE IsAdmin = 1 AND Permissions LIKE '%DELETE_TPTT_ADMIN%')
BEGIN
	UPDATE dbo.UserRoles SET Permissions = Permissions + ';DELETE_TPTT_ADMIN' WHERE IsAdmin = 1
END
--CR: #7

GO
--CR: #2, #3
IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'Factory'
                 AND COLUMN_NAME = 'PCTDFormat')
BEGIN
	ALTER TABLE dbo.Factory Add PCTDFormat NVARCHAR(50) NULL
END

GO
IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'Factory'
                 AND COLUMN_NAME = 'PCTTCNHFormat')
BEGIN
	ALTER TABLE dbo.Factory Add PCTTCNHFormat NVARCHAR(50) NULL
END

GO
IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'Factory'
                 AND COLUMN_NAME = 'PTTFormat')
BEGIN
	ALTER TABLE dbo.Factory Add PTTFormat NVARCHAR(50) NULL
END

GO
--PTTFormat (Phiếu thao tác): 1: Số phiếu, 2: Năm, 3: Loại phiếu, KH: kế hoạch or ĐX: đột xuất, 4: Tên viết tắt nhà máy, 5: Tên viết tắt phân xưởng đơn vị cấp phiếu, 6: /, 7: -
--Dữ liệu mặc định: $"{(max + 1).ToString().PadLeft(3, '0')}/{year}/{ticketType}/{manipulationTicket.ApproveUnit.ShortName}"
UPDATE dbo.Factory SET PTTFormat = '1,6,2,6,3,6,5';
UPDATE dbo.Factory SET PTTFormat = '1,6,2,6,3,6,4,7,5' WHERE FactoryName IN (N'Phú Mỹ 1', N'Phú Mỹ 4', N'Phú Mỹ 2.1')

GO
--PCTDFormat (Phiếu công tác điện): 1: Số phiếu, 2: Năm, 3: Tên viết tắt nhà máy, 4: Tên viết tắt phân xưởng đơn vị cấp phiếu, 5: Tên viết tắt công ty, 6: /, 7: -
--Dữ liệu mặc định: $"{availableMin}/{DateTime.Now.Year}/{user.ProductionPlant?.ShortName}-{user.Company?.ShortName}"
UPDATE dbo.Factory SET PCTDFormat = '1,6,2,6,4,7,5';

GO
--PCTTCNHFormat (Phiếu công tác Thủy cơ nhiệt hóa):  1: Số phiếu, 2: Năm, 3: Tên viết tắt nhà máy, 4: Tên viết tắt phân xưởng đơn vị cấp phiếu, 5: Tên viết tắt công ty, 6: /, 7: -
--Dữ liệu mặc định: $"{availableMin}/{DateTime.Now.Year}/{user.Company?.ShortName}-{user.ProductionPlant?.ShortName}"
UPDATE dbo.Factory SET PCTTCNHFormat = '1,6,2,6,5,7,4';
--CR: #2, #3

GO
IF EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'Factory'
                 AND COLUMN_NAME = 'ID_NM')
BEGIN
	ALTER TABLE dbo.Factory ALTER COLUMN ID_NM NVARCHAR(20) NULL
END