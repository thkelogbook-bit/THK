CREATE TABLE [dbo].[AmWorkPending] (
    [ID]                      INT            IDENTITY (1, 1) NOT NULL,
    [AutonomousMaintenanceID] INT            NOT NULL,
    [AmWorkDetailID]          INT            NOT NULL,
    [FactoryID]               INT            NOT NULL,
    [AmLocationID]            INT            NOT NULL,
    [AmReasonForPendingID]    INT            NULL,
    [AmReasonForStopID]       INT            NULL,
    [ImplementerID]           INT            NULL,
    [Description]             NVARCHAR (MAX) NULL,
    [PendingTime]             DATETIME       NOT NULL,
    [SyncDataKey]             NVARCHAR (100) NULL,
    [AppID]                   NVARCHAR (100) NULL,
    [SyncedDate]              DATETIME       NULL,
    [IsDeleted]               BIT            NOT NULL,
    [CreatedUserID]           INT            NULL,
    [InsertedAt]              DATETIME       NOT NULL,
    [LastUpdatedUserID]       INT            NULL,
    [UpdatedAt]               DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.AmWorkPending] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.AmWorkPending_dbo.AmLocation_AmLocationID] FOREIGN KEY ([AmLocationID]) REFERENCES [dbo].[AmLocation] ([ID]),
    CONSTRAINT [FK_dbo.AmWorkPending_dbo.AmReasonForPending_AmReasonForPendingID] FOREIGN KEY ([AmReasonForPendingID]) REFERENCES [dbo].[AmReasonForPending] ([ID]),
    CONSTRAINT [FK_dbo.AmWorkPending_dbo.AmReasonForStop_AmReasonForStopID] FOREIGN KEY ([AmReasonForStopID]) REFERENCES [dbo].[AmReasonForStop] ([ID]),
    CONSTRAINT [FK_dbo.AmWorkPending_dbo.AmWorkDetail_AmWorkDetailID] FOREIGN KEY ([AmWorkDetailID]) REFERENCES [dbo].[AmWorkDetail] ([ID]),
    CONSTRAINT [FK_dbo.AmWorkPending_dbo.AutonomousMaintenance_AutonomousMaintenanceID] FOREIGN KEY ([AutonomousMaintenanceID]) REFERENCES [dbo].[AutonomousMaintenance] ([ID]),
    CONSTRAINT [FK_dbo.AmWorkPending_dbo.Factory_FactoryID] FOREIGN KEY ([FactoryID]) REFERENCES [dbo].[Factory] ([ID]),
    CONSTRAINT [FK_dbo.AmWorkPending_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.AmWorkPending_dbo.Users_ImplementerID] FOREIGN KEY ([ImplementerID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.AmWorkPending_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);




GO
CREATE NONCLUSTERED INDEX [IX_FactoryID]
    ON [dbo].[AmWorkPending]([FactoryID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_AmWorkDetailID]
    ON [dbo].[AmWorkPending]([AmWorkDetailID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_AmLocationID]
    ON [dbo].[AmWorkPending]([AmLocationID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_AutonomousMaintenanceID]
    ON [dbo].[AmWorkPending]([AutonomousMaintenanceID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_AmReasonForPendingID]
    ON [dbo].[AmWorkPending]([AmReasonForPendingID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_AmReasonForStopID]
    ON [dbo].[AmWorkPending]([AmReasonForStopID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_ImplementerID]
    ON [dbo].[AmWorkPending]([ImplementerID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[AmWorkPending]([LastUpdatedUserID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[AmWorkPending]([CreatedUserID] ASC);
