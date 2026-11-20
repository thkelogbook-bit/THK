CREATE TABLE [dbo].[WorkTicketTracking] (
    [ID]                   INT            IDENTITY (1, 1) NOT NULL,
    [WorkTicketID]         INT            NOT NULL,
    [RequesterUserID]      INT            NOT NULL,
    [CurrentAllowerUserID] INT            NOT NULL,
    [NewAllowerUserID]     INT            NULL,
    [Content]              NVARCHAR (4000) NULL,
    [Reason]               NVARCHAR (4000) NULL,
    [TrackingStatusID]     INT            NULL,
    [IsDeleted]            BIT            NOT NULL,
    [CreatedUserID]        INT            NULL,
    [InsertedAt]           DATETIME       NOT NULL,
    [LastUpdatedUserID]    INT            NULL,
    [UpdatedAt]            DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.WorkTicketTracking] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.WorkTicketTracking_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.WorkTicketTracking_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.WorkTicketTracking_dbo.Users_NewAllowerUserID] FOREIGN KEY ([NewAllowerUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.WorkTicketTracking_dbo.Users_RequesterUserID] FOREIGN KEY ([RequesterUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.WorkTicketTracking_dbo.WorkTicket_WorkTicketID] FOREIGN KEY ([WorkTicketID]) REFERENCES [dbo].[WorkTicket] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[WorkTicketTracking]([CreatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[WorkTicketTracking]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_NewAllowerUserID]
    ON [dbo].[WorkTicketTracking]([NewAllowerUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_RequesterUserID]
    ON [dbo].[WorkTicketTracking]([RequesterUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_WorkTicketID]
    ON [dbo].[WorkTicketTracking]([WorkTicketID] ASC);

