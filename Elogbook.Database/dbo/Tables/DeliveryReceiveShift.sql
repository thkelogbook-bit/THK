CREATE TABLE [dbo].[DeliveryReceiveShift] (
    [ID]                            INT            IDENTITY (1, 1) NOT NULL,
    [OperationDiaryID]              INT            NOT NULL,
    [OperationDiaryPositionShiftID] INT            NOT NULL,
    [SupervisorID]                  INT            NOT NULL,
    [IsDeliveryShift]               BIT            NOT NULL,
    [IsDeleted]                     BIT            NOT NULL,
    [CreatedUserID]                 INT            NULL,
    [InsertedAt]                    DATETIME       NOT NULL,
    [LastUpdatedUserID]             INT            NULL,
    [UpdatedAt]                     DATETIME       NOT NULL,
    [SyncDataKey]                   NVARCHAR (100) NULL,
    [AppID]                         NVARCHAR (100) NULL,
    [SyncedDate]                    DATETIME       NULL,
    CONSTRAINT [PK_dbo.DeliveryReceiveShift] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.DeliveryReceiveShift_dbo.OperationDiary_OperationDiaryID] FOREIGN KEY ([OperationDiaryID]) REFERENCES [dbo].[OperationDiary] ([ID]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.DeliveryReceiveShift_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.DeliveryReceiveShift_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.DeliveryReceiveShift_dbo.Users_SupervisorID] FOREIGN KEY ([SupervisorID]) REFERENCES [dbo].[Users] ([ID]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_SupervisorID]
    ON [dbo].[DeliveryReceiveShift]([SupervisorID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_OperationDiaryID]
    ON [dbo].[DeliveryReceiveShift]([OperationDiaryID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[DeliveryReceiveShift]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[DeliveryReceiveShift]([CreatedUserID] ASC);

