CREATE TABLE [dbo].[ExistInShift] (
    [ID]                     INT            IDENTITY (1, 1) NOT NULL,
    [OperationDiaryID]       INT            NOT NULL,
    [RegistrationTicketID]   INT            NULL,
    [RegistrationTicketName] NVARCHAR (255) NULL,
    [TeamLeader]             NVARCHAR (255) NULL,
    [Area]                   NVARCHAR (MAX) NULL,
    [Content]                NVARCHAR (MAX) NOT NULL,
    [Attachments]            NVARCHAR (500) NULL,
    [StatusID]               INT            NOT NULL,
    [IsDeleteIssueOfShift]   BIT            NOT NULL,
    [CompletedDateTime]      DATETIME       NULL,
    [IsDeleted]              BIT            NOT NULL,
    [CreatedUserID]          INT            NULL,
    [InsertedAt]             DATETIME       NOT NULL,
    [LastUpdatedUserID]      INT            NULL,
    [UpdatedAt]              DATETIME       NOT NULL,
    [SyncDataKey]            NVARCHAR (100) NULL,
    [AppID]                  NVARCHAR (100) NULL,
    [SyncedDate]             DATETIME       NULL,
    [FactoryID]              INT            NULL,
    CONSTRAINT [PK_dbo.ExistInShift] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.ExistInShift_dbo.OperationDiary_OperationDiaryID] FOREIGN KEY ([OperationDiaryID]) REFERENCES [dbo].[OperationDiary] ([ID]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.ExistInShift_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.ExistInShift_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.ExistInShift_dbo.WorkTicket_RegistrationTicketID] FOREIGN KEY ([RegistrationTicketID]) REFERENCES [dbo].[WorkTicket] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_RegistrationTicketID]
    ON [dbo].[ExistInShift]([RegistrationTicketID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Performance_Search]
    ON [dbo].[ExistInShift]([IsDeleted] ASC, [InsertedAt] ASC, [FactoryID] ASC)
    INCLUDE([OperationDiaryID], [Content]);


GO
CREATE NONCLUSTERED INDEX [IX_OperationDiaryID]
    ON [dbo].[ExistInShift]([OperationDiaryID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[ExistInShift]([CreatedUserID] ASC);

