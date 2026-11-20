CREATE TABLE [dbo].[AmRequestTicketHistory] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [RequestTicketId]   INT            NOT NULL,
    [Action]            NVARCHAR (100) NOT NULL,
    [Comment]           NVARCHAR (500) NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    [SyncDataKey]       NVARCHAR (100) NULL,
    [AppID]             NVARCHAR (100) NULL,
    [SyncedDate]        DATETIME       NULL,
    CONSTRAINT [PK_dbo.AmRequestTicketHistory] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.AmRequestTicketHistory_dbo.AmRequestTicket_RequestTicketId] FOREIGN KEY ([RequestTicketId]) REFERENCES [dbo].[AmRequestTicket] ([ID]),
    CONSTRAINT [FK_dbo.AmRequestTicketHistory_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.AmRequestTicketHistory_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);



GO
CREATE NONCLUSTERED INDEX [IX_RequestTicketId]
    ON [dbo].[AmRequestTicketHistory]([RequestTicketId] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[AmRequestTicketHistory]([LastUpdatedUserID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[AmRequestTicketHistory]([CreatedUserID] ASC);