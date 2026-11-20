CREATE TABLE [dbo].[PositionInShift] (
    [ID]                  INT      IDENTITY (1, 1) NOT NULL,
    [ParentID]            INT      NOT NULL,
    [OperationPositionID] INT      NOT NULL,
    [IsSystem]            BIT      NOT NULL,
    [Active]              INT      NOT NULL,
    [Quantity]            INT      NOT NULL,
    [Position]            INT      NOT NULL,
    [IsDeleted]           BIT      NOT NULL,
    [CreatedUserID]       INT      NULL,
    [InsertedAt]          DATETIME NOT NULL,
    [LastUpdatedUserID]   INT      NULL,
    [UpdatedAt]           DATETIME NOT NULL,
    CONSTRAINT [PK_dbo.PositionInShift] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.PositionInShift_dbo.OperationPosition_OperationPositionID] FOREIGN KEY ([OperationPositionID]) REFERENCES [dbo].[OperationPosition] ([ID]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.PositionInShift_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.PositionInShift_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_OperationPositionID]
    ON [dbo].[PositionInShift]([OperationPositionID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[PositionInShift]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[PositionInShift]([CreatedUserID] ASC);

