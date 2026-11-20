CREATE TABLE [dbo].[AmMaintenanceSchedule] (
    [ID]                      INT            IDENTITY (1, 1) NOT NULL,
    [MaintenanceScheduleName] NVARCHAR (255) COLLATE SQL_Latin1_General_CP1_CI_AI NOT NULL,
    [FactoryID]               INT            NOT NULL,
    [AmLocationID]            INT            NOT NULL,
    [AmDeviceID]              INT            NOT NULL,
    [OperationPositionID]     INT            NOT NULL,
    [OperationShiftID]        INT            NOT NULL,
    [Active]                  INT            NULL,
    [CronJobFormat]           NVARCHAR (50)  NULL,
    [CronJobDesc]             NVARCHAR (255) NULL,
    [NotificationsTime]       INT            NULL,
    [StartDate]               DATETIME       NOT NULL,
    [EndDate]                 DATETIME       NOT NULL,
    [SyncDataKey]             NVARCHAR (100) NULL,
    [AppID]                   NVARCHAR (100) NULL,
    [SyncedDate]              DATETIME       NULL,
    [IsDeleted]               BIT            NOT NULL,
    [CreatedUserID]           INT            NULL,
    [InsertedAt]              DATETIME       NOT NULL,
    [LastUpdatedUserID]       INT            NULL,
    [UpdatedAt]               DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.AmMaintenanceSchedule] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.AmMaintenanceSchedule_dbo.AmDevice_AmDeviceID] FOREIGN KEY ([AmDeviceID]) REFERENCES [dbo].[AmDevice] ([ID]),
    CONSTRAINT [FK_dbo.AmMaintenanceSchedule_dbo.AmLocation_AmLocationID] FOREIGN KEY ([AmLocationID]) REFERENCES [dbo].[AmLocation] ([ID]),
    CONSTRAINT [FK_dbo.AmMaintenanceSchedule_dbo.Factory_FactoryID] FOREIGN KEY ([FactoryID]) REFERENCES [dbo].[Factory] ([ID]),
    CONSTRAINT [FK_dbo.AmMaintenanceSchedule_dbo.OperationPosition_OperationPositionID] FOREIGN KEY ([OperationPositionID]) REFERENCES [dbo].[OperationPosition] ([ID]),
    CONSTRAINT [FK_dbo.AmMaintenanceSchedule_dbo.OperationShift_OperationShiftID] FOREIGN KEY ([OperationShiftID]) REFERENCES [dbo].[OperationShift] ([ID]),
    CONSTRAINT [FK_dbo.AmMaintenanceSchedule_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.AmMaintenanceSchedule_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);



GO
CREATE NONCLUSTERED INDEX [IX_FactoryID]
    ON [dbo].[AmMaintenanceSchedule]([FactoryID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_AmLocationID]
    ON [dbo].[AmMaintenanceSchedule]([AmLocationID] ASC);

GO


GO
CREATE NONCLUSTERED INDEX [IX_AmDeviceID]
    ON [dbo].[AmMaintenanceSchedule]([AmDeviceID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_OperationPositionID]
    ON [dbo].[AmMaintenanceSchedule]([OperationPositionID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_OperationShiftID]
    ON [dbo].[AmMaintenanceSchedule]([OperationShiftID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[AmMaintenanceSchedule]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[AmMaintenanceSchedule]([CreatedUserID] ASC);
