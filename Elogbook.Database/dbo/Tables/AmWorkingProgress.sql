CREATE TABLE [dbo].[AmWorkingProgress] (
    [ID]                      INT            IDENTITY (1, 1) NOT NULL,
    [AutonomousMaintenanceID] INT            NOT NULL,
    [FactoryID]               INT            NOT NULL,
    [AmWorkDetailID]          INT            NULL,
    [AmWorkTypeID]            INT            NULL,
    [AmWorkID]                INT            NULL,
    [ImplementerID]           INT            NULL,
    [ActualDate]              DATETIME       NULL,
    [ReasonID]                INT            NULL,
    [ReasonType]              INT            NULL,
    [Content]                 NVARCHAR (MAX) NULL,
    [Remark]                  NVARCHAR (MAX) NULL,
    [SyncDataKey]             NVARCHAR (100) NULL,
    [AppID]                   NVARCHAR (100) NULL,
    [SyncedDate]              DATETIME       NULL,
    [IsDeleted]               BIT            NOT NULL,
    [CreatedUserID]           INT            NULL,
    [InsertedAt]              DATETIME       NOT NULL,
    [LastUpdatedUserID]       INT            NULL,
    [UpdatedAt]               DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.AmWorkingProgress] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.AmWorkingProgress_dbo.AmWork_AmWorkID] FOREIGN KEY ([AmWorkID]) REFERENCES [dbo].[AmWork] ([ID]),
    CONSTRAINT [FK_dbo.AmWorkingProgress_dbo.AmWorkDetail_AmWorkDetailID] FOREIGN KEY ([AmWorkDetailID]) REFERENCES [dbo].[AmWorkDetail] ([ID]),
    CONSTRAINT [FK_dbo.AmWorkingProgress_dbo.AmWorkType_AmWorkTypeID] FOREIGN KEY ([AmWorkTypeID]) REFERENCES [dbo].[AmWorkType] ([ID]),
    CONSTRAINT [FK_dbo.AmWorkingProgress_dbo.AutonomousMaintenance_AutonomousMaintenanceID] FOREIGN KEY ([AutonomousMaintenanceID]) REFERENCES [dbo].[AutonomousMaintenance] ([ID]),
    CONSTRAINT [FK_dbo.AmWorkingProgress_dbo.Factory_FactoryID] FOREIGN KEY ([FactoryID]) REFERENCES [dbo].[Factory] ([ID]),
    CONSTRAINT [FK_dbo.AmWorkingProgress_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.AmWorkingProgress_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);





GO
CREATE NONCLUSTERED INDEX [IX_FactoryID]
    ON [dbo].[AmWorkingProgress]([FactoryID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_AmWorkDetailID]
    ON [dbo].[AmWorkingProgress]([AmWorkDetailID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_AmWorkTypeID]
    ON [dbo].[AmWorkingProgress]([AmWorkTypeID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_AutonomousMaintenanceID]
    ON [dbo].[AmWorkingProgress]([AutonomousMaintenanceID] ASC);

GO


GO


GO


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[AmWorkingProgress]([LastUpdatedUserID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[AmWorkingProgress]([CreatedUserID] ASC);
