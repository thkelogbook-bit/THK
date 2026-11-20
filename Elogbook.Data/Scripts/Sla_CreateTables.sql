----------SlaCategories----------
CREATE TABLE [dbo].[SlaCategories]
(
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [Name]              NVARCHAR (100) NOT NULL,
    [Description]       NVARCHAR (255) NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.SlaCategories] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.SlaCategories_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.SlaCategories_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);

GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[SlaCategories]([LastUpdatedUserID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[SlaCategories]([CreatedUserID] ASC);

GO

----------SlaCategories Data----------
INSERT INTO [dbo].[SlaCategories] ([Name], [Description], [IsDeleted], [InsertedAt], [UpdatedAt])
VALUES
    (N'Phiếu công tác', N'PCT', 0, GETDATE(), GETDATE()),
    (N'Phiếu thao tác', N'PTT', 0, GETDATE(), GETDATE()),
    (N'Phiếu yêu cầu', N'PYC', 0, GETDATE(), GETDATE());

GO

----------SlaConditions----------
CREATE TABLE [dbo].[SlaConditions]
(
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [Name]              NVARCHAR (100) NOT NULL,
    [Description]       NVARCHAR (500) NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.SlaConditions] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.SlaConditions_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.SlaConditions_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);

GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[SlaConditions]([LastUpdatedUserID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[SlaConditions]([CreatedUserID] ASC);

GO

----------SlaRoles----------
CREATE TABLE [dbo].[SlaRoles]
(
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [Name]              NVARCHAR (100) NOT NULL,
    [Description]       NVARCHAR (255) NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.SlaRoles] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.SlaRoles_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.SlaRoles_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);

GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[SlaRoles]([LastUpdatedUserID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[SlaRoles]([CreatedUserID] ASC);

GO

----------SlaRoles Data----------
INSERT INTO [dbo].[SlaRoles] ([Name], [Description], [IsDeleted], [InsertedAt], [UpdatedAt])
VALUES
    (N'CHTT', N'Người chỉ huy trực tiếp', 0, GETDATE(), GETDATE()),
    (N'NCPhep', N'Người cho phép', 0, GETDATE(), GETDATE()),
    (N'NCPhieu', N'Người cấp phiếu', 0, GETDATE(), GETDATE()),
    (N'TT', N'Tổ trưởng', 0, GETDATE(), GETDATE()),
    (N'NVAT', N'Nhân viên an toàn', 0, GETDATE(), GETDATE());

GO

----------SlaConfigs----------
CREATE TABLE [dbo].[SlaConfigs]
(
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [FactoryID]         INT            NOT NULL,
    [SlaCategoryID]     INT            NOT NULL,
    [Name]              NVARCHAR (255) NOT NULL,
    [Description]       NVARCHAR (500) NULL,
    [IsActive]          BIT            NOT NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.SlaConfigs] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.SlaConfigs_dbo.Factory_FactoryID] FOREIGN KEY ([FactoryID]) REFERENCES [dbo].[Factory] ([ID]),
    CONSTRAINT [FK_dbo.SlaConfigs_dbo.SlaCategories_SlaCategoryID] FOREIGN KEY ([SlaCategoryID]) REFERENCES [dbo].[SlaCategories] ([ID]),
    CONSTRAINT [FK_dbo.SlaConfigs_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.SlaConfigs_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);

GO
CREATE NONCLUSTERED INDEX [IX_FactoryID]
    ON [dbo].[SlaConfigs]([FactoryID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_SlaCategoryID]
    ON [dbo].[SlaConfigs]([SlaCategoryID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[SlaConfigs]([LastUpdatedUserID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[SlaConfigs]([CreatedUserID] ASC);

GO

----------SlaLevels----------
CREATE TABLE [dbo].[SlaLevels]
(
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [SlaConfigID]       INT            NOT NULL,
    [LevelOrder]        INT            NOT NULL,
    [SlaConditionID]    INT            NOT NULL,
    [OffsetDays]        INT            NOT NULL,
    [OffsetHours]       INT            NOT NULL,
    [OffsetMinutes]     INT            NOT NULL,
    [IncludeHolidays]   BIT            NOT NULL,
    [IncludeWeekends]   BIT            NOT NULL,
    [WorkingHoursOnly]  BIT            NOT NULL,
    [SendEmail]         BIT            NOT NULL,
    [EmailTemplateID]   INT            NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.SlaLevels] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.SlaLevels_dbo.SlaConfigs_SlaConfigID] FOREIGN KEY ([SlaConfigID]) REFERENCES [dbo].[SlaConfigs] ([ID]),
    CONSTRAINT [FK_dbo.SlaLevels_dbo.SlaConditions_SlaConditionID] FOREIGN KEY ([SlaConditionID]) REFERENCES [dbo].[SlaConditions] ([ID]),
    CONSTRAINT [FK_dbo.SlaLevels_dbo.EmailTemplates_EmailTemplateID] FOREIGN KEY ([EmailTemplateID]) REFERENCES [dbo].[EmailTemplates] ([ID]),
    CONSTRAINT [FK_dbo.SlaLevels_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.SlaLevels_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);

GO
CREATE NONCLUSTERED INDEX [IX_SlaConfigID]
    ON [dbo].[SlaLevels]([SlaConfigID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_SlaConditionID]
    ON [dbo].[SlaLevels]([SlaConditionID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_EmailTemplateID]
    ON [dbo].[SlaLevels]([EmailTemplateID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_NotifiTemplateID]
    ON [dbo].[SlaLevels]([NotifiTemplateID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[SlaLevels]([LastUpdatedUserID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[SlaLevels]([CreatedUserID] ASC);

GO

----------SlaLevelRoles----------
CREATE TABLE [dbo].[SlaLevelRoles]
(
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [SlaLevelID]        INT            NOT NULL,
    [SlaRoleID]         INT            NOT NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.SlaLevelRoles] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.SlaLevelRoles_dbo.SlaLevels_SlaLevelID] FOREIGN KEY ([SlaLevelID]) REFERENCES [dbo].[SlaLevels] ([ID]),
    CONSTRAINT [FK_dbo.SlaLevelRoles_dbo.SlaRoles_SlaRoleID] FOREIGN KEY ([SlaRoleID]) REFERENCES [dbo].[SlaRoles] ([ID]),
    CONSTRAINT [FK_dbo.SlaLevelRoles_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.SlaLevelRoles_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);

GO
CREATE NONCLUSTERED INDEX [IX_SlaLevelID]
    ON [dbo].[SlaLevelRoles]([SlaLevelID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_SlaRoleID]
    ON [dbo].[SlaLevelRoles]([SlaRoleID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[SlaLevelRoles]([LastUpdatedUserID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[SlaLevelRoles]([CreatedUserID] ASC);

GO

----------SlaConditions Data----------
INSERT INTO [dbo].[SlaConditions] ([Name], [Description], [IsDeleted], [InsertedAt], [UpdatedAt])
VALUES
    (N'Trước hạn', N'Điều kiện SLA trước hạn', 0, GETDATE(), GETDATE()),
    (N'Đến hạn', N'Điều kiện SLA đến hạn', 0, GETDATE(), GETDATE()),
    (N'Quá hạn', N'Điều kiện SLA quá hạn', 0, GETDATE(), GETDATE());
