CREATE TABLE [dbo].[OperationDiaryPositionShift] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [OperationDiaryID]  INT            NOT NULL,
    [PositionInShiftID] INT            NULL,
    [Label]             NVARCHAR (255) NULL,
    [AssignedUserID]    INT            NULL,
    [IsDefault]         BIT            NOT NULL,
    [Position]          INT            NOT NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    [SyncDataKey]       NVARCHAR (100) NULL,
    [AppID]             NVARCHAR (100) NULL,
    [SyncedDate]        DATETIME       NULL,
    CONSTRAINT [PK_dbo.OperationDiaryPositionShift] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.OperationDiaryPositionShift_dbo.OperationDiary_OperationDiaryID] FOREIGN KEY ([OperationDiaryID]) REFERENCES [dbo].[OperationDiary] ([ID]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.OperationDiaryPositionShift_dbo.PositionInShift_PositionInShiftID] FOREIGN KEY ([PositionInShiftID]) REFERENCES [dbo].[PositionInShift] ([ID]),
    CONSTRAINT [FK_dbo.OperationDiaryPositionShift_dbo.Users_AssignedUserID] FOREIGN KEY ([AssignedUserID]) REFERENCES [dbo].[Users] ([ID]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.OperationDiaryPositionShift_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.OperationDiaryPositionShift_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_PositionInShiftID]
    ON [dbo].[OperationDiaryPositionShift]([PositionInShiftID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_OperationDiaryID]
    ON [dbo].[OperationDiaryPositionShift]([OperationDiaryID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[OperationDiaryPositionShift]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[OperationDiaryPositionShift]([CreatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AssignedUserID]
    ON [dbo].[OperationDiaryPositionShift]([AssignedUserID] ASC);

