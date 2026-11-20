CREATE TABLE [dbo].[TimeLineCycle] (
    [ID]                INT      IDENTITY (1, 1) NOT NULL,
    [Hour]              INT      NOT NULL,
    [CycleID]           INT      NOT NULL,
    [Active]            INT      NOT NULL,
    [IsDeleted]         BIT      NOT NULL,
    [CreatedUserID]     INT      NULL,
    [InsertedAt]        DATETIME NOT NULL,
    [LastUpdatedUserID] INT      NULL,
    [UpdatedAt]         DATETIME NOT NULL,
    CONSTRAINT [PK_dbo.TimeLineCycle] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.TimeLineCycle_dbo.CycleManagement_CycleID] FOREIGN KEY ([CycleID]) REFERENCES [dbo].[CycleManagement] ([ID]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.TimeLineCycle_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.TimeLineCycle_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[TimeLineCycle]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CycleID]
    ON [dbo].[TimeLineCycle]([CycleID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[TimeLineCycle]([CreatedUserID] ASC);

