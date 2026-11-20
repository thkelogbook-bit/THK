CREATE TABLE [dbo].[AmMaintenanceWork] (
    [ID]                      INT            IDENTITY (1, 1) NOT NULL,
    [AmMaintenanceScheduleID] INT            NOT NULL,
    [AmWorkTypeID]            INT            NOT NULL,
    [AmWorkID]                INT            NOT NULL,
    [Order]                   INT            NOT NULL,
    [SLA]                     INT            NULL,
    [SyncDataKey]             NVARCHAR (100) NULL,
    [AppID]                   NVARCHAR (100) NULL,
    [SyncedDate]              DATETIME       NULL,
    [IsDeleted]               BIT            NOT NULL,
    [CreatedUserID]           INT            NULL,
    [InsertedAt]              DATETIME       NOT NULL,
    [LastUpdatedUserID]       INT            NULL,
    [UpdatedAt]               DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.AmMaintenanceWork] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.AmMaintenanceWork_dbo.AmMaintenanceSchedule_AmMaintenanceScheduleID] FOREIGN KEY ([AmMaintenanceScheduleID]) REFERENCES [dbo].[AmMaintenanceSchedule] ([ID]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.AmMaintenanceWork_dbo.AmWork_AmWorkID] FOREIGN KEY ([AmWorkID]) REFERENCES [dbo].[AmWork] ([ID]),
    CONSTRAINT [FK_dbo.AmMaintenanceWork_dbo.AmWorkType_AmWorkTypeID] FOREIGN KEY ([AmWorkTypeID]) REFERENCES [dbo].[AmWorkType] ([ID]),
    CONSTRAINT [FK_dbo.AmMaintenanceWork_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.AmMaintenanceWork_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);



GO
CREATE NONCLUSTERED INDEX [IX_AmMaintenanceScheduleID]
    ON [dbo].[AmMaintenanceWork]([AmMaintenanceScheduleID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_AmWorkTypeID]
    ON [dbo].[AmMaintenanceWork]([AmWorkTypeID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_AmWorkID]
    ON [dbo].[AmMaintenanceWork]([AmWorkID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[AmMaintenanceWork]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[AmMaintenanceWork]([CreatedUserID] ASC);
