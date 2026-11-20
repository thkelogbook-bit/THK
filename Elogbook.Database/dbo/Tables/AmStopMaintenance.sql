CREATE TABLE [dbo].[AmStopMaintenance] (
    [ID]                          INT            IDENTITY (1, 1) NOT NULL,
    [AutonomousMaintenanceID]     INT            NOT NULL,
    [FactoryID]                   INT            NOT NULL,
    [AmLocationID]                INT            NOT NULL,
    [AmReasonForStopID]           INT            NULL,
    [ImplementerID]               INT            NULL,
    [ImplementerSignatureContent] VARCHAR (MAX)  NULL,
    [Description]                 NVARCHAR (MAX) NULL,
    [StopTime]                    DATETIME       NOT NULL,
    [SyncDataKey]                 NVARCHAR (100) NULL,
    [AppID]                       NVARCHAR (100) NULL,
    [SyncedDate]                  DATETIME       NULL,
    [IsDeleted]                   BIT            NOT NULL,
    [CreatedUserID]               INT            NULL,
    [InsertedAt]                  DATETIME       NOT NULL,
    [LastUpdatedUserID]           INT            NULL,
    [UpdatedAt]                   DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.AmStopMaintenance] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.AmStopMaintenance_dbo.AmLocation_AmLocationID] FOREIGN KEY ([AmLocationID]) REFERENCES [dbo].[AmLocation] ([ID]),
    CONSTRAINT [FK_dbo.AmStopMaintenance_dbo.AmReasonForStop_AmReasonForStopID] FOREIGN KEY ([AmReasonForStopID]) REFERENCES [dbo].[AmReasonForStop] ([ID]),
    CONSTRAINT [FK_dbo.AmStopMaintenance_dbo.AutonomousMaintenance_AutonomousMaintenanceID] FOREIGN KEY ([AutonomousMaintenanceID]) REFERENCES [dbo].[AutonomousMaintenance] ([ID]),
    CONSTRAINT [FK_dbo.AmStopMaintenance_dbo.Factory_FactoryID] FOREIGN KEY ([FactoryID]) REFERENCES [dbo].[Factory] ([ID]),
    CONSTRAINT [FK_dbo.AmStopMaintenance_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.AmStopMaintenance_dbo.Users_ImplementerID] FOREIGN KEY ([ImplementerID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.AmStopMaintenance_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);



GO
CREATE NONCLUSTERED INDEX [IX_FactoryID]
    ON [dbo].[AmStopMaintenance]([FactoryID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_AmLocationID]
    ON [dbo].[AmStopMaintenance]([AmLocationID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_AutonomousMaintenanceID]
    ON [dbo].[AmStopMaintenance]([AutonomousMaintenanceID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_AmReasonForStopID]
    ON [dbo].[AmStopMaintenance]([AmReasonForStopID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_ImplementerID]
    ON [dbo].[AmStopMaintenance]([ImplementerID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[AmStopMaintenance]([LastUpdatedUserID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[AmStopMaintenance]([CreatedUserID] ASC);
