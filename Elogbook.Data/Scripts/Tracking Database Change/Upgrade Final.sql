USE Elogbook

--AbnormalIncident
IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'AbnormalIncident'
                 AND COLUMN_NAME = 'AppID')
BEGIN
	ALTER TABLE dbo.AbnormalIncident ADD AppID NVARCHAR (100) NULL 
END

--DeliveryReceiveShift
IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'DeliveryReceiveShift'
                 AND COLUMN_NAME = 'AppID')
BEGIN
	ALTER TABLE dbo.DeliveryReceiveShift ADD AppID NVARCHAR (100) NULL 
END

--DeliveryShiftMethod
IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'DeliveryShiftMethod'
                 AND COLUMN_NAME = 'AppID')
BEGIN
	ALTER TABLE dbo.DeliveryShiftMethod ADD AppID NVARCHAR (100) NULL 
END

--ExistInShift
IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'ExistInShift'
                 AND COLUMN_NAME = 'AppID')
BEGIN
	ALTER TABLE dbo.ExistInShift ADD AppID NVARCHAR (100) NULL 
END

--OperationDiary
IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'OperationDiary'
                 AND COLUMN_NAME = 'AppID')
BEGIN
	ALTER TABLE dbo.OperationDiary ADD AppID NVARCHAR (100) NULL 
END

--OperationDiaryPositionShift
IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'OperationDiaryPositionShift'
                 AND COLUMN_NAME = 'AppID')
BEGIN
	ALTER TABLE dbo.OperationDiaryPositionShift ADD AppID NVARCHAR (100) NULL 
END

--OperationSituation
IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'OperationSituation'
                 AND COLUMN_NAME = 'AppID')
BEGIN
	ALTER TABLE dbo.OperationSituation ADD AppID NVARCHAR (100) NULL 
END

--OperationParameters
IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'OperationParameters'
                 AND COLUMN_NAME = 'AppID')
BEGIN
	ALTER TABLE dbo.OperationParameters ADD AppID NVARCHAR (100) NULL 
END

--OperationTimeLine
IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'OperationTimeLine'
                 AND COLUMN_NAME = 'AppID')
BEGIN
	ALTER TABLE dbo.OperationTimeLine ADD AppID NVARCHAR (100) NULL 
END

--RegistrationTicket
IF EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'RegistrationTicket'
                 AND COLUMN_NAME = 'RegulationNumber')
BEGIN
	ALTER TABLE dbo.RegistrationTicket ALTER COLUMN RegulationNumber NVARCHAR (255) NULL 
END

--07052022
--ExistInShift
IF EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'ExistInShift'
                 AND COLUMN_NAME = 'CompletedDateTime')
BEGIN
	ALTER TABLE dbo.ExistInShift ALTER COLUMN CompletedDateTime DATETIME NULL 
END

--06052022
IF NOT EXISTS (SELECT 1
            FROM sys.indexes I              
            WHERE I.Name = 'IDX_Get_Parameters_Performance'
             AND I.object_id = OBJECT_ID('dbo.OperationParameters'))
BEGIN
	/****** Object:  Index [IDX_Get_Parameters_Performance]    Script Date: 5/6/2022 11:00:59 AM ******/
	CREATE NONCLUSTERED INDEX [IDX_Get_Parameters_Performance] ON [dbo].[OperationParameters]
	(
		[OperationTimeLineID] ASC,
		[FactoryID] ASC,
		[OperationPositionID] ASC,
		[CycleID] ASC,
		[IsDeleted] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END

IF NOT EXISTS (SELECT 1
            FROM sys.indexes I              
            WHERE I.Name = 'IDX_Update_Parameters_Performance'
             AND I.object_id = OBJECT_ID('dbo.OperationParameters'))
BEGIN
	CREATE NONCLUSTERED INDEX [IDX_Update_Parameters_Performance] ON [dbo].[OperationParameters]
	(
		[FactoryID] ASC,
		[OperationPositionID] ASC,
		[CycleID] ASC,
		[OperationTimeLineID] ASC,
		[ParameterID] ASC,
		[IsDeleted] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END
--06052022

--10052022
IF NOT EXISTS (SELECT 1
            FROM sys.indexes I              
            WHERE I.Name = 'IX_Mapping_TimeLine'
             AND I.object_id = OBJECT_ID('dbo.OperationTimeLine'))
BEGIN
	PRINT N'Create IX_Mapping_TimeLine success'
    /****** Object:  Index [IX_Mapping_TimeLine]    Script Date: 5/10/2022 1:08:02 AM ******/
	CREATE NONCLUSTERED INDEX [IX_Mapping_TimeLine] ON [dbo].[OperationTimeLine]
	(
		[TimeLineSyncDataKey] ASC,
		[IsDeleted] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END

IF NOT EXISTS (SELECT 1
            FROM sys.indexes I              
            WHERE I.Name = 'ID_Mapping_Parameter'
             AND I.object_id = OBJECT_ID('dbo.OperationParameters'))
BEGIN
	PRINT N'Create ID_Mapping_Parameter success'
	/****** Object:  Index [ID_Mapping_Parameter]    Script Date: 5/10/2022 1:44:30 AM ******/
	CREATE NONCLUSTERED INDEX [ID_Mapping_Parameter] ON [dbo].[OperationParameters]
	(
		[SyncDataKey] ASC,
		[ParameterID] ASC,
		[IsDeleted] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END
--10052022

--12052022
IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'TrackingSyncMobileData'
                 AND COLUMN_NAME = 'SyncDataModule')
BEGIN
	ALTER TABLE TrackingSyncMobileData ADD SyncDataModule DATETIME NOT NULL DEFAULT(GETDATE())
END

IF EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'WorkTicket'
                 AND COLUMN_NAME = 'ApprovedDate')
BEGIN
	ALTER TABLE WorkTicket ALTER COLUMN ApprovedDate DATETIME NULL
END

IF EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'WorkPlacement'
                 AND COLUMN_NAME = 'StartTime')
BEGIN
	ALTER TABLE WorkPlacement ALTER COLUMN StartTime DATETIME NULL
END

IF EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'WorkPlacement'
                 AND COLUMN_NAME = 'EndTime')
BEGIN
	ALTER TABLE WorkPlacement ALTER COLUMN EndTime DATETIME NULL
END

--23052022
-- UPDATE par
-- SET
	-- par.FactoryID = Grp.FactoryID,
	-- par.CycleID = Grp.CycleID,
	-- par.OperationPositionID = Grp.OperationPositionID
-- FROM dbo.Parameters par WITH(NOLOCK)
-- INNER JOIN dbo.ParameterGroup Grp WITH(NOLOCK) ON par.ParameterGroupID = Grp.ID
-- WHERE ParameterGroupID IS NOT NULL

--08062022
IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'IssueOfShiftDetail'
                 AND COLUMN_NAME = 'AppID')
BEGIN
	ALTER TABLE IssueOfShiftDetail ADD AppID NVARCHAR (100) NULL 
END

--10062022
UPDATE dbo.users SET PhoneNumber = concat('0', substring(PhoneNumber, 4, LEN(PhoneNumber)))
where substring(PhoneNumber, 1, 3) = '+84';

UPDATE dbo.users SET PhoneNumber = concat('0', substring(PhoneNumber, 3, LEN(PhoneNumber)))
where substring(PhoneNumber, 1, 2) = '84';
--10062022

--12062022
ALTER TABLE dbo.OperationDiaryPositionShift ALTER COLUMN AssignedUserID INT NULL
--12062022

--18062022
IF EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'Parameters'
                 AND COLUMN_NAME = 'UnitMeasure')
BEGIN
	ALTER TABLE dbo.[Parameters] ALTER COLUMN UnitMeasure NVARCHAR (255) NULL 
END

IF EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'OperationParameters'
                 AND COLUMN_NAME = 'UnitMeasure')
BEGIN
	ALTER TABLE dbo.[OperationParameters] ALTER COLUMN UnitMeasure NVARCHAR (255) NULL 
END

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'OperationSituation'
                 AND COLUMN_NAME = 'AssignedToName')
BEGIN
	ALTER TABLE [dbo].[OperationSituation] ADD [AssignedToName] NVARCHAR (255) NULL
END

--CREATE TABLE [dbo].[WorkTicketFeatureLink] (
--    [ID]                INT            IDENTITY (1, 1) NOT NULL,
--    [FeatureID]       INT NOT NULL,
--	[WorkTicketID]       INT NOT NULL,
--    [IsDeleted]         BIT            NOT NULL,
--    [CreatedUserID]     INT            NULL,
--    [InsertedAt]        DATETIME       NOT NULL,
--    [LastUpdatedUserID] INT            NULL,
--    [UpdatedAt]         DATETIME       NOT NULL,
--    CONSTRAINT [PK_dbo.WorkTicketFeatureLink] PRIMARY KEY CLUSTERED ([ID] ASC),
--    CONSTRAINT [FK_dbo.WorkTicketFeatureLink_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
--    CONSTRAINT [FK_dbo.WorkTicketFeatureLink_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
--);

--GO
--CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
--    ON [dbo].[WorkTicketFeatureLink]([CreatedUserID] ASC)

--GO
--CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
--    ON [dbo].[WorkTicketFeatureLink]([LastUpdatedUserID] ASC)
	
IF NOT EXISTS(SELECT FeatureName FROM [dbo].[WorkTicketFeatures] WHERE FeatureName = N'Xây dựng' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[WorkTicketFeatures] (FeatureName, IsDeleted, InsertedAt, UpdatedAt, CreatedUserID, LastUpdatedUserID) 
	VALUES (N'Xây dựng', 0,  GETDATE(), GETDATE(), NULL, NULL)
END

IF NOT EXISTS(SELECT ModuleValue FROM [dbo].[Modules] WHERE ModuleValue = N'APP_VER' AND ModuleName = N'Cấu hình phiên bản' AND SystemType = N'Admin Portal' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[Modules] ([ModuleValue], [ModuleName], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'APP_VER', N'Cấu hình phiên bản', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END

IF NOT EXISTS(SELECT PermissionKey FROM [dbo].[UserPermission] WHERE PermissionKey = N'VIEW_CHHT' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[UserPermission] ( [PermissionKey], [PermissionName], [Active], [ModuleValue], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'VIEW_CHHT', N'Xem phiên bản', 1, N'APP_VER', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END

IF NOT EXISTS(SELECT PermissionKey FROM [dbo].[UserPermission] WHERE PermissionKey = N'EDIT_CHHT' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[UserPermission] ( [PermissionKey], [PermissionName], [Active], [ModuleValue], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'EDIT_CHHT', N'Cập nhật phiên bản', 1, N'APP_VER', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END
--18062022

--04072022
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='FK_dbo.OperationDiary_dbo.AbnormalIncident_AbnormalIncidentID')
BEGIN
	ALTER TABLE [dbo].[OperationDiary] DROP CONSTRAINT [FK_dbo.OperationDiary_dbo.AbnormalIncident_AbnormalIncidentID]
END

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='FK_dbo.OperationDiary_dbo.DeliveryShiftMethod_DeliveryShiftMethodID')
BEGIN
	ALTER TABLE [dbo].[OperationDiary] DROP CONSTRAINT [FK_dbo.OperationDiary_dbo.DeliveryShiftMethod_DeliveryShiftMethodID]
END

IF EXISTS (SELECT * FROM sys.indexes  WHERE name='IX_AbnormalIncidentID' AND object_id = OBJECT_ID('dbo.OperationDiary'))
BEGIN
	DROP INDEX [IX_AbnormalIncidentID] ON [dbo].[OperationDiary]
END

IF EXISTS (SELECT * FROM sys.indexes  WHERE name='IX_DeliveryShiftMethodID' AND object_id = OBJECT_ID('dbo.OperationDiary'))
BEGIN
	DROP INDEX [IX_DeliveryShiftMethodID] ON [dbo].[OperationDiary]
END

IF EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'OperationDiary'
                 AND COLUMN_NAME = 'AbnormalIncidentID')
BEGIN
	ALTER TABLE dbo.OperationDiary DROP COLUMN AbnormalIncidentID
END

IF EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'OperationDiary'
                 AND COLUMN_NAME = 'DeliveryShiftMethodID')
BEGIN
	ALTER TABLE dbo.OperationDiary DROP COLUMN DeliveryShiftMethodID
END

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'CancelTicketInfo'
                 AND COLUMN_NAME = 'AppID')
BEGIN
	ALTER TABLE dbo.CancelTicketInfo ADD AppID NVARCHAR (100) NULL 
END

IF EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'WorkPlacement'
                 AND COLUMN_NAME = 'Place')
BEGIN
	ALTER TABLE WorkPlacement ALTER COLUMN Place NVARCHAR(MAX) NULL
END
--04072022

--18072022
IF NOT EXISTS(SELECT ModuleValue FROM [dbo].[Modules] WHERE ModuleValue = N'PMIS' AND ModuleName = N'Liên kết Mã QLKT' AND SystemType = N'Admin Portal' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[Modules] ([ModuleValue], [ModuleName], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'PMIS', N'Liên kết Mã QLKT', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END

IF NOT EXISTS(SELECT PermissionKey FROM [dbo].[UserPermission] WHERE PermissionKey = N'VIEW_PMIS' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[UserPermission] ( [PermissionKey], [PermissionName], [Active], [ModuleValue], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'VIEW_PMIS', N'Xem Mã QLKT', 1, N'PMIS', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END

IF NOT EXISTS(SELECT PermissionKey FROM [dbo].[UserPermission] WHERE PermissionKey = N'EDIT_PMIS' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[UserPermission] ( [PermissionKey], [PermissionName], [Active], [ModuleValue], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'EDIT_PMIS', N'Cập nhật Mã QLKT', 1, N'PMIS', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END
--18072022

--22072022
IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'OperationParameters'
                 AND COLUMN_NAME = 'CheckedDate')
BEGIN
	ALTER TABLE dbo.OperationParameters ADD CheckedDate DATETIME NULL
END

DeLETE T
FROM
(
SELECT DupRank = ROW_NUMBER() OVER (
              PARTITION BY DeviceID
              ORDER BY (SELECT NULL)
            ), *
FROM TrackingSyncMobileData
) AS T
WHERE DupRank > 1
--22072022

--25072022
IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'Attachment'
                 AND COLUMN_NAME = 'SyncedDate')
BEGIN
	ALTER TABLE dbo.Attachment ADD SyncedDate DATETIME NULL
END

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'CancelTicketInfo'
                 AND COLUMN_NAME = 'SyncedDate')
BEGIN
	ALTER TABLE dbo.CancelTicketInfo ADD SyncedDate DATETIME NULL
END

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'DeliveryAccepDevice'
                 AND COLUMN_NAME = 'SyncedDate')
BEGIN
	ALTER TABLE dbo.DeliveryAccepDevice ADD SyncedDate DATETIME NULL
END

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'ManipulationTicket'
                 AND COLUMN_NAME = 'SyncedDate')
BEGIN
	ALTER TABLE dbo.ManipulationTicket ADD SyncedDate DATETIME NULL
END

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'OtherRequest'
                 AND COLUMN_NAME = 'SyncedDate')
BEGIN
	ALTER TABLE dbo.OtherRequest ADD SyncedDate DATETIME NULL
END

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'Participant'
                 AND COLUMN_NAME = 'SyncedDate')
BEGIN
	ALTER TABLE dbo.Participant ADD SyncedDate DATETIME NULL
END

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'SequenceOperation'
                 AND COLUMN_NAME = 'SyncedDate')
BEGIN
	ALTER TABLE dbo.SequenceOperation ADD SyncedDate DATETIME NULL
END

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'AbnormalIncident'
                 AND COLUMN_NAME = 'SyncedDate')
BEGIN
	ALTER TABLE dbo.AbnormalIncident ADD SyncedDate DATETIME NULL
END

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'DeliveryReceiveShift'
                 AND COLUMN_NAME = 'SyncedDate')
BEGIN
	ALTER TABLE dbo.DeliveryReceiveShift ADD SyncedDate DATETIME NULL
END

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'DeliveryShiftMethod'
                 AND COLUMN_NAME = 'SyncedDate')
BEGIN
	ALTER TABLE dbo.DeliveryShiftMethod ADD SyncedDate DATETIME NULL
END

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'ExistInShift'
                 AND COLUMN_NAME = 'SyncedDate')
BEGIN
	ALTER TABLE dbo.ExistInShift ADD SyncedDate DATETIME NULL
END

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'OperationDiary'
                 AND COLUMN_NAME = 'SyncedDate')
BEGIN
	ALTER TABLE dbo.OperationDiary ADD SyncedDate DATETIME NULL
END

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'OperationDiaryPositionShift'
                 AND COLUMN_NAME = 'SyncedDate')
BEGIN
	ALTER TABLE dbo.OperationDiaryPositionShift ADD SyncedDate DATETIME NULL
END

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'OperationSituation'
                 AND COLUMN_NAME = 'SyncedDate')
BEGIN
	ALTER TABLE dbo.OperationSituation ADD SyncedDate DATETIME NULL
END

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'OperationParameters'
                 AND COLUMN_NAME = 'SyncedDate')
BEGIN
	ALTER TABLE dbo.OperationParameters ADD SyncedDate DATETIME NULL
END

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'OperationTimeLine'
                 AND COLUMN_NAME = 'SyncedDate')
BEGIN
	ALTER TABLE dbo.OperationTimeLine ADD SyncedDate DATETIME NULL
END

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'AlertDetails'
                 AND COLUMN_NAME = 'SyncedDate')
BEGIN
	ALTER TABLE dbo.AlertDetails ADD SyncedDate DATETIME NULL
END

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'Parameters'
                 AND COLUMN_NAME = 'SyncedDate')
BEGIN
	ALTER TABLE dbo.Parameters ADD SyncedDate DATETIME NULL
END
--25072022

--31072022
IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'LeadershipUser'
                 AND COLUMN_NAME = 'LaborCard')
BEGIN
	ALTER TABLE LeadershipUser ADD LaborCard NVARCHAR (255) NULL
END

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'LeadershipUser'
                 AND COLUMN_NAME = 'LaborRank')
BEGIN
	ALTER TABLE LeadershipUser ADD LaborRank INT NULL
END

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'Stakeholders'
                 AND COLUMN_NAME = 'LaborCard')
BEGIN
	ALTER TABLE Stakeholders ADD LaborCard NVARCHAR (255) NULL
END

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'Stakeholders'
                 AND COLUMN_NAME = 'LaborRank')
BEGIN
	ALTER TABLE Stakeholders ADD LaborRank INT NULL
END

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'Signature'
                 AND COLUMN_NAME = 'SignatureContentByte')
BEGIN
	ALTER TABLE Signature ADD SignatureContentByte VARBINARY (MAX) NULL
END
--31072022

--08082022
--Định nghĩa user đồng bộ dữ liệu sang PMIS
IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'Factory'
                 AND COLUMN_NAME = 'PMIS_USERID')
BEGIN
	ALTER table dbo.Factory ADD PMIS_USERID NVARCHAR(25) NULL
END

GO
--Với từng nhà máy thì cần xác định user cụ thể
UPDATE dbo.Factory SET PMIS_USERID = N'admin'

UPDATE dbo.OperationTimeLine SET InsertedAt = (DATEADD(hh, 0, DATEDIFF(day, 0, InsertedAt)))
WHERE DATEPART(SECOND,InsertedAt) > 0 OR DATEPART(HOUR,InsertedAt) > 0 OR DATEPART(MINUTE,InsertedAt) > 0
--08082022

--13082022
IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'Factory'
                 AND COLUMN_NAME = 'IsFilterFactory')
BEGIN
	ALTER TABLE dbo.Factory ADD IsFilterFactory BIT NOT NULL DEFAULT (1)
END
--13082022

--Chưa deploy khách hàng
--19082022
IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'TrackingSyncMobileData'
                 AND COLUMN_NAME = 'SyncUsers')
BEGIN
	ALTER TABLE dbo.TrackingSyncMobileData ADD SyncUsers DATETIME NOT NULL DEFAULT(GETDATE())
END
--19082022

--29082022
IF NOT EXISTS(SELECT *
	FROM   INFORMATION_SCHEMA.COLUMNS
	WHERE  TABLE_NAME = 'Factory'
		 AND COLUMN_NAME = 'IsUseOperationParameterGroup')
BEGIN
	ALTER TABLE dbo.Factory ADD IsUseOperationParameterGroup BIT NOT NULL DEFAULT (0)
END
GO
UPDATE dbo.Factory SET IsUseOperationParameterGroup = 1 
--29082022

--04092022
IF EXISTS(SELECT * FROM sys.views WHERE name = 'view_Quick_Search_NKVH')
BEGIN
	DROP VIEW dbo.view_Quick_Search_NKVH
END
GO
CREATE VIEW [dbo].[view_Quick_Search_NKVH] 
AS
SELECT OperationDiaryID AS 'ID', Description AS 'Description', 1 AS 'DiaryType' FROM dbo.OperationSituation WITH(NOLOCK)
WHERE IsDeleted = 0 AND LEN(LTRIM(RTRIM(ISNULL(Description, '')))) > 0
UNION ALL
SELECT OperationDiaryID AS 'ID', Description AS 'Description', 2 AS 'DiaryType' FROM dbo.DeliveryShiftMethod WITH(NOLOCK)
WHERE IsDeleted = 0 AND LEN(LTRIM(RTRIM(ISNULL(Description, '')))) > 0
UNION ALL
SELECT OperationDiaryID AS 'ID', Description AS 'Description', 3 AS 'DiaryType' FROM dbo.AbnormalIncident WITH(NOLOCK)
WHERE IsDeleted = 0 AND LEN(LTRIM(RTRIM(ISNULL(Description, '')))) > 0
UNION ALL
SELECT OperationDiaryID AS 'ID', Content AS 'Description', 4 AS 'DiaryType' FROM dbo.ExistInShift WITH(NOLOCK)
WHERE IsDeleted = 0 AND LEN(LTRIM(RTRIM(ISNULL(Content, '')))) > 0
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'Quick_Search_NKVH')
BEGIN
	DROP PROCEDURE dbo.Quick_Search_NKVH
END
GO

-- =============================================
-- Tìm kiếm nhanh thông tin NKVH
-- =============================================
CREATE PROCEDURE [dbo].[Quick_Search_NKVH]
	@PageNumber INT = 1,
	@PageSize INT = 10,
	@Year INT,
	@FactoryID INT = -1,
	@SearchText NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @TempData TABLE(OperationShiftID INT,
							OperationShiftName NVARCHAR(255), 
							FactoryID INT,
							FactoryName NVARCHAR(255),
							[Description] NVARCHAR(MAX), 
							CreatedDate DATETIME, 
							OperationPositionID INT,
							OperationPositionName NVARCHAR(255),
							DiaryType INT)
	INSERT INTO @TempData
	SELECT 
		os.ID,
		os.OperationShiftName, 
		f.ID,
		f.FactoryName, 
		temp.[Description], 
		od.CreatedDate,
		op.ID,
		op.OperationPositionName,
		temp.DiaryType
	FROM dbo.[view_Quick_Search_NKVH] temp WITH(NOLOCK)
	INNER JOIN dbo.[OperationDiary] od WITH(NOLOCK) ON temp.ID = od.ID
	INNER JOIN dbo.[OperationShift] os WITH(NOLOCK) ON od.OperationShiftID = os.ID
	INNER JOIN dbo.[OperationPosition] op WITH(NOLOCK) ON od.OperationPositionID = op.ID
	INNER JOIN dbo.[Factory] f WITH(NOLOCK) ON od.FactoryID = f.ID
	WHERE YEAR(od.CreatedDate) = @Year
		AND (od.FactoryID = @FactoryID OR @FactoryID = -1)
		AND temp.[Description] LIKE N'%'+@SearchText+'%'
	ORDER BY od.CreatedDate DESC, f.ID ASC, op.ID ASC, os.ID ASC

	DECLARE @TotalRecords INT = (SELECT COUNT(1) FROM @TempData) 

	SELECT 
		OperationShiftName + ' - ' + FORMAT(CreatedDate, 'd', 'en-gb') AS ShiftDescription,
		FactoryName AS FactoryName,
		(CASE WHEN DiaryType = 1 THEN N'<span class="headerContent">' + N'Tình hình vận hành' + N' - ' + OperationPositionName + N'</span>'
			WHEN DiaryType = 2 THEN N'<span class="headerContent">' + N'Phương thức giao ca' + N' - ' + OperationPositionName + N'</span>'
			WHEN DiaryType = 3 THEN N'<span class="headerContent">' + N'Bất thường sự cố' + N' - ' + OperationPositionName + N'</span>'
			ELSE N'<span class="headerContent">' + N'Tồn tại trong ca' + N' - ' + OperationPositionName + N'</span>' END) AS HeaderContent, 
		[Description] AS [Description],
		ISNULL(@TotalRecords, 0) AS TotalRecords,
		CreatedDate,
		DiaryType
	FROM @TempData
	ORDER BY CreatedDate DESC, FactoryID ASC, OperationPositionID ASC, OperationShiftID ASC, DiaryType ASC
	OFFSET (@PageNumber-1) * @PageSize ROWS
	FETCH NEXT @PageSize ROWS ONLY
END
GO

IF NOT EXISTS(SELECT ModuleValue FROM [dbo].[Modules] WHERE ModuleValue = N'QUICK_SEARCH' AND ModuleName = N'Tìm kiếm' AND SystemType = N'Admin Portal' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[Modules] ([ModuleValue], [ModuleName], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'QUICK_SEARCH', N'Tìm kiếm', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END

IF NOT EXISTS(SELECT PermissionKey FROM [dbo].[UserPermission] WHERE PermissionKey = N'QUICK_SEARCH_NKVH' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[UserPermission] ( [PermissionKey], [PermissionName], [Active], [ModuleValue], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'QUICK_SEARCH_NKVH', N'Nhật ký vận hành', 1, N'QUICK_SEARCH', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END

IF NOT EXISTS(SELECT PermissionKey FROM [dbo].[UserPermission] WHERE PermissionKey = N'QUICK_SEARCH_TSVH' AND IsDeleted = 0)
BEGIN
	INSERT INTO [dbo].[UserPermission] ( [PermissionKey], [PermissionName], [Active], [ModuleValue], [SystemType], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) 
	VALUES ( N'QUICK_SEARCH_TSVH', N'Thông số vận hành', 1, N'QUICK_SEARCH', N'Admin Portal', 0, NULL, GETDATE(), NULL, GETDATE())
END

UPDATE dbo.UserRoles SET [Permissions] = CONCAT(Permissions, ';QUICK_SEARCH_NKVH;QUICK_SEARCH_TSVH')
--04092022

--26092022
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'TSVH_GetValuesForChart')
BEGIN
	DROP PROCEDURE dbo.TSVH_GetValuesForChart
END
GO

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
GO

IF NOT EXISTS(SELECT *
	FROM   INFORMATION_SCHEMA.COLUMNS
	WHERE  TABLE_NAME = 'OperationPosition'
		 AND COLUMN_NAME = 'Position')
BEGIN
	ALTER TABLE dbo.OperationPosition ADD Position INT NOT NULL DEFAULT (0)
END
GO
UPDATE dbo.OperationPosition SET Position = ID
GO
--Phú Mỹ 1
UPDATE dbo.OperationPosition SET Position = 1 WHERE FactoryID = (SELECT ID FROM Factory WHERE FactoryName LIKE N'Phú Mỹ 1') AND OperationPositionName LIKE N'Trưởng ca'
UPDATE dbo.OperationPosition SET Position = 2 WHERE FactoryID = (SELECT ID FROM Factory WHERE FactoryName LIKE N'Phú Mỹ 1') AND OperationPositionName LIKE N'Trưởng kíp'
UPDATE dbo.OperationPosition SET Position = 3 WHERE FactoryID = (SELECT ID FROM Factory WHERE FactoryName LIKE N'Phú Mỹ 1') AND OperationPositionName LIKE N'Bảng điện'
UPDATE dbo.OperationPosition SET Position = 4 WHERE FactoryID = (SELECT ID FROM Factory WHERE FactoryName LIKE N'Phú Mỹ 1') AND OperationPositionName LIKE N'Máy tính điều khiển GT'
UPDATE dbo.OperationPosition SET Position = 5 WHERE FactoryID = (SELECT ID FROM Factory WHERE FactoryName LIKE N'Phú Mỹ 1') AND OperationPositionName LIKE N'Máy tính điều khiển ST'
UPDATE dbo.OperationPosition SET Position = 6 WHERE FactoryID = (SELECT ID FROM Factory WHERE FactoryName LIKE N'Phú Mỹ 1') AND OperationPositionName LIKE N'Gian máy GT'
UPDATE dbo.OperationPosition SET Position = 7 WHERE FactoryID = (SELECT ID FROM Factory WHERE FactoryName LIKE N'Phú Mỹ 1') AND OperationPositionName LIKE N'Gian máy ST - xử lý nước'
UPDATE dbo.OperationPosition SET Position = 8 WHERE FactoryID = (SELECT ID FROM Factory WHERE FactoryName LIKE N'Phú Mỹ 1') AND OperationPositionName LIKE N'Gian máy Lò'
UPDATE dbo.OperationPosition SET Position = 9 WHERE FactoryID = (SELECT ID FROM Factory WHERE FactoryName LIKE N'Phú Mỹ 1') AND OperationPositionName LIKE N'Trạm bơm tuần hoàn'
--Phú Mỹ 4
UPDATE dbo.OperationPosition SET Position = 1 WHERE FactoryID = (SELECT ID FROM Factory WHERE FactoryName LIKE N'Phú Mỹ 4') AND OperationPositionName LIKE N'Trưởng ca'
UPDATE dbo.OperationPosition SET Position = 2 WHERE FactoryID = (SELECT ID FROM Factory WHERE FactoryName LIKE N'Phú Mỹ 4') AND OperationPositionName LIKE N'Trưởng kíp'
UPDATE dbo.OperationPosition SET Position = 3 WHERE FactoryID = (SELECT ID FROM Factory WHERE FactoryName LIKE N'Phú Mỹ 4') AND OperationPositionName LIKE N'MTĐK Tuabin khí'
UPDATE dbo.OperationPosition SET Position = 4 WHERE FactoryID = (SELECT ID FROM Factory WHERE FactoryName LIKE N'Phú Mỹ 4') AND OperationPositionName LIKE N'MTĐK Tuabin hơi'
UPDATE dbo.OperationPosition SET Position = 5 WHERE FactoryID = (SELECT ID FROM Factory WHERE FactoryName LIKE N'Phú Mỹ 4') AND OperationPositionName LIKE N'GM Tuabin khí và Lò thu hồi nhiệt'
UPDATE dbo.OperationPosition SET Position = 6 WHERE FactoryID = (SELECT ID FROM Factory WHERE FactoryName LIKE N'Phú Mỹ 4') AND OperationPositionName LIKE N'GM Tuabin hơi và Hệ thống XLN'
--Phú Mỹ 2.1
UPDATE dbo.OperationPosition SET Position = 1 WHERE FactoryID = (SELECT ID FROM Factory WHERE FactoryName LIKE N'Phú Mỹ 2.1') AND OperationPositionName LIKE N'Trưởng ca'
UPDATE dbo.OperationPosition SET Position = 2 WHERE FactoryID = (SELECT ID FROM Factory WHERE FactoryName LIKE N'Phú Mỹ 2.1') AND OperationPositionName LIKE N'Trưởng kíp PM2.1'
UPDATE dbo.OperationPosition SET Position = 3 WHERE FactoryID = (SELECT ID FROM Factory WHERE FactoryName LIKE N'Phú Mỹ 2.1') AND OperationPositionName LIKE N'Trưởng kíp PM2.1 MR'
UPDATE dbo.OperationPosition SET Position = 4 WHERE FactoryID = (SELECT ID FROM Factory WHERE FactoryName LIKE N'Phú Mỹ 2.1') AND OperationPositionName LIKE N'MTĐK GT21/22'
UPDATE dbo.OperationPosition SET Position = 5 WHERE FactoryID = (SELECT ID FROM Factory WHERE FactoryName LIKE N'Phú Mỹ 2.1') AND OperationPositionName LIKE N'MTĐK ST23'
UPDATE dbo.OperationPosition SET Position = 6 WHERE FactoryID = (SELECT ID FROM Factory WHERE FactoryName LIKE N'Phú Mỹ 2.1') AND OperationPositionName LIKE N'MTĐK GT24/25'
UPDATE dbo.OperationPosition SET Position = 7 WHERE FactoryID = (SELECT ID FROM Factory WHERE FactoryName LIKE N'Phú Mỹ 2.1') AND OperationPositionName LIKE N'MTĐK ST26'
UPDATE dbo.OperationPosition SET Position = 8 WHERE FactoryID = (SELECT ID FROM Factory WHERE FactoryName LIKE N'Phú Mỹ 2.1') AND OperationPositionName LIKE N'GM GT & LÒ 21/22'
UPDATE dbo.OperationPosition SET Position = 9 WHERE FactoryID = (SELECT ID FROM Factory WHERE FactoryName LIKE N'Phú Mỹ 2.1') AND OperationPositionName LIKE N'GM ST23'
UPDATE dbo.OperationPosition SET Position = 10 WHERE FactoryID = (SELECT ID FROM Factory WHERE FactoryName LIKE N'Phú Mỹ 2.1') AND OperationPositionName LIKE N'GM GT & LÒ 24/25'
UPDATE dbo.OperationPosition SET Position = 11 WHERE FactoryID = (SELECT ID FROM Factory WHERE FactoryName LIKE N'Phú Mỹ 2.1') AND OperationPositionName LIKE N'GM ST26'
UPDATE dbo.OperationPosition SET Position = 12 WHERE FactoryID = (SELECT ID FROM Factory WHERE FactoryName LIKE N'Phú Mỹ 2.1') AND OperationPositionName LIKE N'NGM ST & GT'
--Phân xưởng Hóa
UPDATE dbo.OperationPosition SET Position = 1 WHERE FactoryID = (SELECT ID FROM Factory WHERE FactoryName LIKE N'Phân xưởng Hóa') AND OperationPositionName LIKE N'VHV Hóa PM1'
UPDATE dbo.OperationPosition SET Position = 2 WHERE FactoryID = (SELECT ID FROM Factory WHERE FactoryName LIKE N'Phân xưởng Hóa') AND OperationPositionName LIKE N'VHV Hóa PM21'

--Cập nhật thứ tự hiển thị danh sách cương vị trong ca
UPDATE odps SET odps.Position = pis.Position
FROM dbo.OperationDiaryPositionShift odps
INNER JOIN dbo.PositionInShift pis ON odps.PositionInShiftID = pis.ID
--26092022