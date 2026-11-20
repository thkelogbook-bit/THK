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

CREATE TABLE [dbo].[WorkTicketFeatureLink] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [FeatureID]       INT NOT NULL,
	[WorkTicketID]       INT NOT NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.WorkTicketFeatureLink] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.WorkTicketFeatureLink_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.WorkTicketFeatureLink_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);

GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[WorkTicketFeatureLink]([CreatedUserID] ASC)

GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[WorkTicketFeatureLink]([LastUpdatedUserID] ASC)
	
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