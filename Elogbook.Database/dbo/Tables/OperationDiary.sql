CREATE TABLE [dbo].[OperationDiary] (
    [ID]                  INT            IDENTITY (1, 1) NOT NULL,
    [FactoryID]           INT            NULL,
    [CreatedDate]         DATETIME       NOT NULL,
    [OperationPositionID] INT            NULL,
    [OperationShiftID]    INT            NULL,
    [OperationCrewID]     INT            NULL,
    [Comments]            NVARCHAR (MAX) NULL,
    [IsExistsShift]       BIT            NULL,
    [IsReceived]          BIT            NOT NULL,
    [IsDelivered]         BIT            NOT NULL,
    [IsCanEdit]           BIT            NOT NULL,
    [TimeDelivery]        DATETIME       NULL,
    [IsDeleted]           BIT            NOT NULL,
    [CreatedUserID]       INT            NULL,
    [InsertedAt]          DATETIME       NOT NULL,
    [LastUpdatedUserID]   INT            NULL,
    [UpdatedAt]           DATETIME       NOT NULL,
    [SyncDataKey]         NVARCHAR (100) NULL,
    [AppID]               NVARCHAR (100) NULL,
    [SyncedDate]          DATETIME       NULL,
    CONSTRAINT [PK_dbo.OperationDiary] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.OperationDiary_dbo.Factory_FactoryID] FOREIGN KEY ([FactoryID]) REFERENCES [dbo].[Factory] ([ID]),
    CONSTRAINT [FK_dbo.OperationDiary_dbo.OperationCrew_OperationCrewID] FOREIGN KEY ([OperationCrewID]) REFERENCES [dbo].[OperationCrew] ([ID]),
    CONSTRAINT [FK_dbo.OperationDiary_dbo.OperationPosition_OperationPositionID] FOREIGN KEY ([OperationPositionID]) REFERENCES [dbo].[OperationPosition] ([ID]),
    CONSTRAINT [FK_dbo.OperationDiary_dbo.OperationShift_OperationShiftID] FOREIGN KEY ([OperationShiftID]) REFERENCES [dbo].[OperationShift] ([ID]),
    CONSTRAINT [FK_dbo.OperationDiary_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.OperationDiary_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);










GO
CREATE NONCLUSTERED INDEX [IX_Performance_Search]
    ON [dbo].[OperationDiary]([CreatedDate] ASC, [FactoryID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_OperationShiftID]
    ON [dbo].[OperationDiary]([OperationShiftID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_OperationPositionID]
    ON [dbo].[OperationDiary]([OperationPositionID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_OperationCrewID]
    ON [dbo].[OperationDiary]([OperationCrewID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[OperationDiary]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FactoryID]
    ON [dbo].[OperationDiary]([FactoryID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[OperationDiary]([CreatedUserID] ASC);

