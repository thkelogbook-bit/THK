CREATE TABLE [dbo].[AllowedInfo] (
    [ID]                  INT      IDENTITY (1, 1) NOT NULL,
    [WorkTicketID]        INT      NULL,
    [AllowedTime]         DATETIME NOT NULL,
    [CommanderSignature]  INT      NULL,
    [TimeCommanderSigned] DATETIME NOT NULL,
    [IsDeleted]           BIT      NOT NULL,
    [CreatedUserID]       INT      NULL,
    [InsertedAt]          DATETIME NOT NULL,
    [LastUpdatedUserID]   INT      NULL,
    [UpdatedAt]           DATETIME NOT NULL,
    CONSTRAINT [PK_dbo.AllowedInfo] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.AllowedInfo_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.AllowedInfo_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.AllowedInfo_dbo.WorkTicket_WorkTicketID] FOREIGN KEY ([WorkTicketID]) REFERENCES [dbo].[WorkTicket] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_WorkTicketID]
    ON [dbo].[AllowedInfo]([WorkTicketID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[AllowedInfo]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[AllowedInfo]([CreatedUserID] ASC);

