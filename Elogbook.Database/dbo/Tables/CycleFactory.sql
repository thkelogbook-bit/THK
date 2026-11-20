CREATE TABLE [dbo].[CycleFactory] (
    [ID]                INT      IDENTITY (1, 1) NOT NULL,
    [CycleID]           INT      NOT NULL,
    [FactoryID]         INT      NOT NULL,
    [IsDeleted]         BIT      NOT NULL,
    [CreatedUserID]     INT      NULL,
    [InsertedAt]        DATETIME NOT NULL,
    [LastUpdatedUserID] INT      NULL,
    [UpdatedAt]         DATETIME NOT NULL,
    CONSTRAINT [PK_dbo.CycleFactory] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.CycleFactory_dbo.CycleManagement_CycleID] FOREIGN KEY ([CycleID]) REFERENCES [dbo].[CycleManagement] ([ID]),
    CONSTRAINT [FK_dbo.CycleFactory_dbo.Factory_FactoryID] FOREIGN KEY ([FactoryID]) REFERENCES [dbo].[Factory] ([ID]),
    CONSTRAINT [FK_dbo.CycleFactory_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.CycleFactory_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[CycleFactory]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FactoryID]
    ON [dbo].[CycleFactory]([FactoryID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CycleID]
    ON [dbo].[CycleFactory]([CycleID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[CycleFactory]([CreatedUserID] ASC);

