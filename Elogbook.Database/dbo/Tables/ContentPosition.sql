CREATE TABLE [dbo].[ContentPosition] (
    [ID]                   INT            IDENTITY (1, 1) NOT NULL,
    [RegistrationTicketID] INT            NOT NULL,
    [Position]             NVARCHAR (MAX) NULL,
    [IsDeleted]            BIT            NOT NULL,
    [CreatedUserID]        INT            NULL,
    [InsertedAt]           DATETIME       NOT NULL,
    [LastUpdatedUserID]    INT            NULL,
    [UpdatedAt]            DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.ContentPosition] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.ContentPosition_dbo.RegistrationTicket_RegistrationTicketID] FOREIGN KEY ([RegistrationTicketID]) REFERENCES [dbo].[RegistrationTicket] ([ID]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.ContentPosition_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.ContentPosition_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_RegistrationTicketID]
    ON [dbo].[ContentPosition]([RegistrationTicketID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[ContentPosition]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[ContentPosition]([CreatedUserID] ASC);

