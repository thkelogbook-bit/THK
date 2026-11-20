CREATE TABLE [dbo].[OperationParameters] (
    [ID]                  BIGINT         IDENTITY (1, 1) NOT NULL,
    [ParameterName]       NVARCHAR (255) NOT NULL,
    [ParameterID]         INT            NOT NULL,
    [ParameterGroupID]    INT            NULL,
    [OperationTimeLineID] INT            NULL,
    [FactoryID]           INT            NOT NULL,
    [OperationPositionID] INT            NULL,
    [CycleID]             INT            NULL,
    [CurrentValue]        NVARCHAR (MAX) NULL,
    [KKSCode]             NVARCHAR (255) NULL,
    [UnitMeasure]         NVARCHAR (255) NULL,
    [Setting]             NVARCHAR (MAX) NULL,
    [FromRange]           REAL           NULL,
    [ToRange]             REAL           NULL,
    [IsNumberType]        BIT            NOT NULL,
    [PreviousValue]       NVARCHAR (MAX) NULL,
    [ErrorCode]           INT            NULL,
    [IsChecked]           BIT            NOT NULL,
    [AlertDetails]        NVARCHAR (MAX) NULL,
    [IsDeleted]           BIT            NOT NULL,
    [CreatedUserID]       INT            NULL,
    [InsertedAt]          DATETIME       NOT NULL,
    [LastUpdatedUserID]   INT            NULL,
    [UpdatedAt]           DATETIME       NOT NULL,
    [SyncDataKey]         NVARCHAR (100) NULL,
    [AppID]               NVARCHAR (100) NULL,
    [CheckedDate]         DATETIME       NULL,
    [SyncedDate]          DATETIME       NULL,
    CONSTRAINT [PK_dbo.OperationParameters] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.OperationParameters_dbo.CycleManagement_CycleID] FOREIGN KEY ([CycleID]) REFERENCES [dbo].[CycleManagement] ([ID]),
    CONSTRAINT [FK_dbo.OperationParameters_dbo.Factory_FactoryID] FOREIGN KEY ([FactoryID]) REFERENCES [dbo].[Factory] ([ID]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.OperationParameters_dbo.OperationPosition_OperationPositionID] FOREIGN KEY ([OperationPositionID]) REFERENCES [dbo].[OperationPosition] ([ID]),
    CONSTRAINT [FK_dbo.OperationParameters_dbo.OperationTimeLine_OperationTimeLineID] FOREIGN KEY ([OperationTimeLineID]) REFERENCES [dbo].[OperationTimeLine] ([ID]),
    CONSTRAINT [FK_dbo.OperationParameters_dbo.ParameterGroup_ParameterGroupID] FOREIGN KEY ([ParameterGroupID]) REFERENCES [dbo].[ParameterGroup] ([ID]),
    CONSTRAINT [FK_dbo.OperationParameters_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.OperationParameters_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_OperationParameters_partitioncol]
    ON [dbo].[OperationParameters]([InsertedAt] ASC)
    ON [PS_MonthWise] ([InsertedAt]);


GO
CREATE NONCLUSTERED INDEX [IX_ParameterGroupID]
    ON [dbo].[OperationParameters]([ParameterGroupID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_OperationTimeLineID]
    ON [dbo].[OperationParameters]([OperationTimeLineID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_OperationPositionID]
    ON [dbo].[OperationParameters]([OperationPositionID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[OperationParameters]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FactoryID]
    ON [dbo].[OperationParameters]([FactoryID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CycleID]
    ON [dbo].[OperationParameters]([CycleID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[OperationParameters]([CreatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IDX_Update_Parameters_Performance]
    ON [dbo].[OperationParameters]([FactoryID] ASC, [OperationPositionID] ASC, [CycleID] ASC, [OperationTimeLineID] ASC, [ParameterID] ASC, [IsDeleted] ASC);


GO
CREATE NONCLUSTERED INDEX [IDX_Get_Parameters_Performance]
    ON [dbo].[OperationParameters]([OperationTimeLineID] ASC, [FactoryID] ASC, [OperationPositionID] ASC, [CycleID] ASC, [IsDeleted] ASC);


GO
CREATE NONCLUSTERED INDEX [ID_Mapping_Parameter]
    ON [dbo].[OperationParameters]([SyncDataKey] ASC, [ParameterID] ASC, [IsDeleted] ASC);

