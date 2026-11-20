CREATE TABLE [dbo].[IssueOfShiftDetail] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [Content]           NVARCHAR (MAX) NULL,
    [Attachments]       NVARCHAR (500) NULL,
    [IsComplete]        INT            NOT NULL,
    [ExistInShiftID]    INT            NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    [AppID]             NVARCHAR (100) NULL,
    CONSTRAINT [PK_dbo.IssueOfShiftDetail] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.IssueOfShiftDetail_dbo.ExistInShift_ExistInShiftID] FOREIGN KEY ([ExistInShiftID]) REFERENCES [dbo].[ExistInShift] ([ID]),
    CONSTRAINT [FK_dbo.IssueOfShiftDetail_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.IssueOfShiftDetail_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[IssueOfShiftDetail]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExistInShiftID]
    ON [dbo].[IssueOfShiftDetail]([ExistInShiftID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[IssueOfShiftDetail]([CreatedUserID] ASC);

