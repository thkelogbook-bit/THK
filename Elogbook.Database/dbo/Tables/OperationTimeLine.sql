CREATE TABLE [dbo].[OperationTimeLine] (
    [ID]                          INT            IDENTITY (1, 1) NOT NULL,
    [TimeLineSyncDataKey]         NVARCHAR (100) NULL,
    [TimeLineName]                INT            NOT NULL,
    [FactoryID]                   INT            NULL,
    [OperationPositionID]         INT            NULL,
    [CycleID]                     INT            NULL,
    [TotalParametersDefault]      INT            NOT NULL,
    [TotalParametersChecked]      INT            NOT NULL,
    [TotalParametersSynchronized] INT            NOT NULL,
    [IsDeleted]                   BIT            NOT NULL,
    [CreatedUserID]               INT            NULL,
    [InsertedAt]                  DATETIME       NOT NULL,
    [LastUpdatedUserID]           INT            NULL,
    [UpdatedAt]                   DATETIME       NOT NULL,
    [AppID]                       NVARCHAR (100) NULL,
    [SyncedDate]                  DATETIME       NULL,
    CONSTRAINT [PK_dbo.OperationTimeLine] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.OperationTimeLine_dbo.CycleManagement_CycleID] FOREIGN KEY ([CycleID]) REFERENCES [dbo].[CycleManagement] ([ID]),
    CONSTRAINT [FK_dbo.OperationTimeLine_dbo.Factory_FactoryID] FOREIGN KEY ([FactoryID]) REFERENCES [dbo].[Factory] ([ID]),
    CONSTRAINT [FK_dbo.OperationTimeLine_dbo.OperationPosition_OperationPositionID] FOREIGN KEY ([OperationPositionID]) REFERENCES [dbo].[OperationPosition] ([ID]),
    CONSTRAINT [FK_dbo.OperationTimeLine_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.OperationTimeLine_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_OperationTimeLine_partitioncol]
    ON [dbo].[OperationTimeLine]([InsertedAt] ASC)
    ON [PS_MonthWise] ([InsertedAt]);


GO
CREATE NONCLUSTERED INDEX [IX_OperationPositionID]
    ON [dbo].[OperationTimeLine]([OperationPositionID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Mapping_TimeLine]
    ON [dbo].[OperationTimeLine]([TimeLineSyncDataKey] ASC, [IsDeleted] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[OperationTimeLine]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FactoryID]
    ON [dbo].[OperationTimeLine]([FactoryID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CycleID]
    ON [dbo].[OperationTimeLine]([CycleID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[OperationTimeLine]([CreatedUserID] ASC);

