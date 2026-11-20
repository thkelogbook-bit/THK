CREATE TABLE [dbo].[WorkDaily] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [WorkTicketID]      INT            NULL,
    [WorkLocationID]    INT            NULL,
    [WorkDay]           DATETIME       NOT NULL,
    [WorkLocationText]  NVARCHAR (MAX) NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.WorkDaily] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.WorkDaily_dbo.ContentPosition_WorkLocationID] FOREIGN KEY ([WorkLocationID]) REFERENCES [dbo].[ContentPosition] ([ID]),
    CONSTRAINT [FK_dbo.WorkDaily_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.WorkDaily_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.WorkDaily_dbo.WorkTicket_WorkTicketID] FOREIGN KEY ([WorkTicketID]) REFERENCES [dbo].[WorkTicket] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_WorkTicketID]
    ON [dbo].[WorkDaily]([WorkTicketID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_WorkLocationID]
    ON [dbo].[WorkDaily]([WorkLocationID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[WorkDaily]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[WorkDaily]([CreatedUserID] ASC);

