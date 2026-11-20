CREATE TABLE [dbo].[AmWorkDetail] (
    [ID]                      INT            IDENTITY (1, 1) NOT NULL,
    [AutonomousMaintenanceID] INT            NOT NULL,
    [FactoryID]               INT            NOT NULL,
    [AmWorkTypeID]            INT            NOT NULL,
    [AmWorkID]                INT            NOT NULL,
    [AmStatusID]              INT            NOT NULL,
    [ImplementerID]           INT            NULL,
    [Order]                   INT            NOT NULL,
    [SLA]                     INT            NULL,
    [Description]             NVARCHAR (MAX) NULL,
    [StickTime]               VARCHAR (20)   NULL,
    [SyncDataKey]             NVARCHAR (100) NULL,
    [AppID]                   NVARCHAR (100) NULL,
    [SyncedDate]              DATETIME       NULL,
    [IsDeleted]               BIT            NOT NULL,
    [CreatedUserID]           INT            NULL,
    [InsertedAt]              DATETIME       NOT NULL,
    [LastUpdatedUserID]       INT            NULL,
    [UpdatedAt]               DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.AmWorkDetail] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.AmWorkDetail_dbo.AmStatus_AmStatusID] FOREIGN KEY ([AmStatusID]) REFERENCES [dbo].[AmStatus] ([ID]),
    CONSTRAINT [FK_dbo.AmWorkDetail_dbo.AmWork_AmWorkID] FOREIGN KEY ([AmWorkID]) REFERENCES [dbo].[AmWork] ([ID]),
    CONSTRAINT [FK_dbo.AmWorkDetail_dbo.AmWorkType_AmWorkTypeID] FOREIGN KEY ([AmWorkTypeID]) REFERENCES [dbo].[AmWorkType] ([ID]),
    CONSTRAINT [FK_dbo.AmWorkDetail_dbo.AutonomousMaintenance_AutonomousMaintenanceID] FOREIGN KEY ([AutonomousMaintenanceID]) REFERENCES [dbo].[AutonomousMaintenance] ([ID]),
    CONSTRAINT [FK_dbo.AmWorkDetail_dbo.Factory_FactoryID] FOREIGN KEY ([FactoryID]) REFERENCES [dbo].[Factory] ([ID]),
    CONSTRAINT [FK_dbo.AmWorkDetail_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.AmWorkDetail_dbo.Users_ImplementerID] FOREIGN KEY ([ImplementerID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.AmWorkDetail_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);





GO
CREATE NONCLUSTERED INDEX [IX_AutonomousMaintenanceID]
    ON [dbo].[AmWorkDetail]([AutonomousMaintenanceID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_FactoryID]
    ON [dbo].[AmWorkDetail]([FactoryID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_AmWorkTypeID]
    ON [dbo].[AmWorkDetail]([AmWorkTypeID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_AmWorkID]
    ON [dbo].[AmWorkDetail]([AmWorkID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_AmStatusID]
    ON [dbo].[AmWorkDetail]([AmStatusID] ASC);

GO


GO
CREATE NONCLUSTERED INDEX [IX_ImplementerID]
    ON [dbo].[AmWorkDetail]([ImplementerID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[AmWorkDetail]([LastUpdatedUserID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[AmWorkDetail]([CreatedUserID] ASC);
