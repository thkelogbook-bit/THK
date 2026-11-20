CREATE TABLE [dbo].[TicketStatusHistory] (
    [ID]                         INT            IDENTITY (1, 1) NOT NULL,
    [RegistrationTicketID]       INT            NULL,
    [RegistrationManipulationID] INT            NULL,
    [Description]                NVARCHAR (MAX) NOT NULL,
    [Reason]                     NVARCHAR (MAX) NOT NULL,
    [Attachments]                NVARCHAR (500) NULL,
    [StatusValue]                INT            NOT NULL,
    [IsDeleted]                  BIT            NOT NULL,
    [CreatedUserID]              INT            NULL,
    [InsertedAt]                 DATETIME       NOT NULL,
    [LastUpdatedUserID]          INT            NULL,
    [UpdatedAt]                  DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.TicketStatusHistory] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.TicketStatusHistory_dbo.RegistrationManipulationTicket_RegistrationManipulationID] FOREIGN KEY ([RegistrationManipulationID]) REFERENCES [dbo].[RegistrationManipulationTicket] ([ID]),
    CONSTRAINT [FK_dbo.TicketStatusHistory_dbo.RegistrationTicket_RegistrationTicketID] FOREIGN KEY ([RegistrationTicketID]) REFERENCES [dbo].[RegistrationTicket] ([ID]),
    CONSTRAINT [FK_dbo.TicketStatusHistory_dbo.TicketStatus_StatusValue] FOREIGN KEY ([StatusValue]) REFERENCES [dbo].[TicketStatus] ([ID]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.TicketStatusHistory_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.TicketStatusHistory_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_StatusValue]
    ON [dbo].[TicketStatusHistory]([StatusValue] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_RegistrationTicketID]
    ON [dbo].[TicketStatusHistory]([RegistrationTicketID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_RegistrationManipulationID]
    ON [dbo].[TicketStatusHistory]([RegistrationManipulationID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[TicketStatusHistory]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[TicketStatusHistory]([CreatedUserID] ASC);

