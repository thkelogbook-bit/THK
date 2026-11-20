CREATE TABLE [dbo].[ParameterGroup] (
    [ID]                  INT            IDENTITY (1, 1) NOT NULL,
    [ParameterGroupName]  NVARCHAR (255) NOT NULL,
    [FactoryID]           INT            NULL,
    [OperationPositionID] INT            NULL,
    [CycleID]             INT            NULL,
    [Position]            INT            NOT NULL,
    [Status]              INT            NOT NULL,
    [IsDeleted]           BIT            NOT NULL,
    [CreatedUserID]       INT            NULL,
    [InsertedAt]          DATETIME       NOT NULL,
    [LastUpdatedUserID]   INT            NULL,
    [UpdatedAt]           DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.ParameterGroup] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.ParameterGroup_dbo.CycleManagement_CycleID] FOREIGN KEY ([CycleID]) REFERENCES [dbo].[CycleManagement] ([ID]),
    CONSTRAINT [FK_dbo.ParameterGroup_dbo.Factory_FactoryID] FOREIGN KEY ([FactoryID]) REFERENCES [dbo].[Factory] ([ID]),
    CONSTRAINT [FK_dbo.ParameterGroup_dbo.OperationPosition_OperationPositionID] FOREIGN KEY ([OperationPositionID]) REFERENCES [dbo].[OperationPosition] ([ID]),
    CONSTRAINT [FK_dbo.ParameterGroup_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.ParameterGroup_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_OperationPositionID]
    ON [dbo].[ParameterGroup]([OperationPositionID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[ParameterGroup]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FactoryID]
    ON [dbo].[ParameterGroup]([FactoryID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CycleID]
    ON [dbo].[ParameterGroup]([CycleID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[ParameterGroup]([CreatedUserID] ASC);

