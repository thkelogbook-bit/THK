CREATE TABLE [dbo].[AutonomousMaintenance] (
    [ID]                        INT            IDENTITY (1, 1) NOT NULL,
    [FactoryID]                 INT            NOT NULL,
    [AmLocationID]              INT            NOT NULL,
    [AmDeviceID]                INT            NOT NULL,
    [OperationPositionID]       INT            NOT NULL,
    [OperationShiftID]          INT            NOT NULL,
    [AmMaintenanceScheduleID]   INT            NOT NULL,
    [ActualDate]                DATETIME       NULL,
    [AmStatusID]                INT            NOT NULL,
    [IsAbnormal]                BIT            NULL,
    [AmDeviceAbnormalStatusID]  INT            NULL,
    [BeforeImages]              NVARCHAR (255) NULL,
    [AfterImages]               NVARCHAR (255) NULL,
    [DeviceAbnormalDescription] NVARCHAR (MAX) NULL,
    [DeviceAbnormalAttachments] VARCHAR (255)  NULL,
    [AmRequestTicketID]         INT            NULL,
    [CompleterUserID]           INT            NULL,
    [CompleterSignatureContent] VARCHAR (MAX)  NULL,
    [CompleteDate]              DATETIME       NULL,
    [IsSendNotify]              BIT            NULL,
    [IsAutoComplete]            BIT            NULL,
    [SyncDataKey]               NVARCHAR (100) NULL,
    [AppID]                     NVARCHAR (100) NULL,
    [SyncedDate]                DATETIME       NULL,
    [IsDeleted]                 BIT            NOT NULL,
    [CreatedUserID]             INT            NULL,
    [InsertedAt]                DATETIME       NOT NULL,
    [LastUpdatedUserID]         INT            NULL,
    [UpdatedAt]                 DATETIME       NOT NULL,
    [IsSyncToOperationDiary]    BIT            CONSTRAINT [DF_AutonomousMaintenance_IsSyncToOperationDiary] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_dbo.AutonomousMaintenance] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.AutonomousMaintenance_dbo.AmDevice_AmDeviceID] FOREIGN KEY ([AmDeviceID]) REFERENCES [dbo].[AmDevice] ([ID]),
    CONSTRAINT [FK_dbo.AutonomousMaintenance_dbo.AmDeviceAbnormalStatus_AmDeviceAbnormalStatusID] FOREIGN KEY ([AmDeviceAbnormalStatusID]) REFERENCES [dbo].[AmDeviceAbnormalStatus] ([ID]),
    CONSTRAINT [FK_dbo.AutonomousMaintenance_dbo.AmLocation_AmLocationID] FOREIGN KEY ([AmLocationID]) REFERENCES [dbo].[AmLocation] ([ID]),
    CONSTRAINT [FK_dbo.AutonomousMaintenance_dbo.AmMaintenanceSchedule_AmMaintenanceScheduleID] FOREIGN KEY ([AmMaintenanceScheduleID]) REFERENCES [dbo].[AmMaintenanceSchedule] ([ID]),
    CONSTRAINT [FK_dbo.AutonomousMaintenance_dbo.AmStatus_AmStatusID] FOREIGN KEY ([AmStatusID]) REFERENCES [dbo].[AmStatus] ([ID]),
    CONSTRAINT [FK_dbo.AutonomousMaintenance_dbo.Factory_FactoryID] FOREIGN KEY ([FactoryID]) REFERENCES [dbo].[Factory] ([ID]),
    CONSTRAINT [FK_dbo.AutonomousMaintenance_dbo.OperationPosition_OperationPositionID] FOREIGN KEY ([OperationPositionID]) REFERENCES [dbo].[OperationPosition] ([ID]),
    CONSTRAINT [FK_dbo.AutonomousMaintenance_dbo.OperationShift_OperationShiftID] FOREIGN KEY ([OperationShiftID]) REFERENCES [dbo].[OperationShift] ([ID]),
    CONSTRAINT [FK_dbo.AutonomousMaintenance_dbo.Users_CompleterUserID] FOREIGN KEY ([CompleterUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.AutonomousMaintenance_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.AutonomousMaintenance_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);





GO
CREATE NONCLUSTERED INDEX [IX_FactoryID]
    ON [dbo].[AutonomousMaintenance]([FactoryID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_AmLocationID]
    ON [dbo].[AutonomousMaintenance]([AmLocationID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_AmStatusID]
    ON [dbo].[AutonomousMaintenance]([AmStatusID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_AmDeviceID]
    ON [dbo].[AutonomousMaintenance]([AmDeviceID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_OperationPositionID]
    ON [dbo].[AutonomousMaintenance]([OperationPositionID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_OperationShiftID]
    ON [dbo].[AutonomousMaintenance]([OperationShiftID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_AmMaintenanceScheduleID]
    ON [dbo].[AutonomousMaintenance]([AmMaintenanceScheduleID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_AmDeviceAbnormalStatusID]
    ON [dbo].[AutonomousMaintenance]([AmDeviceAbnormalStatusID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[AutonomousMaintenance]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[AutonomousMaintenance]([CreatedUserID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_CompleterUserID]
    ON [dbo].[AutonomousMaintenance]([CompleterUserID] ASC);

