CREATE TABLE [dbo].[CancelTicketInfo] (
    [ID]                 INT            IDENTITY (1, 1) NOT NULL,
    [TicketID]           INT            NOT NULL,
    [Reason]             NVARCHAR (MAX) NULL,
    [ReferAttachmentIDs] NVARCHAR (500) NULL,
    [IsDeleted]          BIT            NOT NULL,
    [CreatedUserID]      INT            NULL,
    [InsertedAt]         DATETIME       NOT NULL,
    [LastUpdatedUserID]  INT            NULL,
    [UpdatedAt]          DATETIME       NOT NULL,
    [AppID]              NVARCHAR (100) NULL,
    [SyncedDate]         DATETIME       NULL,
    CONSTRAINT [PK_dbo.CancelTicketInfo] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.CancelTicketInfo_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.CancelTicketInfo_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);




GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[CancelTicketInfo]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[CancelTicketInfo]([CreatedUserID] ASC);

