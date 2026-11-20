CREATE TABLE [dbo].[AmRelatedInformation] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [RequestTicketID]   INT            NOT NULL,
    [Type]              NVARCHAR (100) NOT NULL,
    [Info]              NVARCHAR (200) NULL,
    [Status]            INT            NOT NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.AmRelatedInformation] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.AmRelatedInformation_dbo.AmRequestTicket_RequestTicketID] FOREIGN KEY ([RequestTicketID]) REFERENCES [dbo].[AmRequestTicket] ([ID]),
    CONSTRAINT [FK_dbo.AmRelatedInformation_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.AmRelatedInformation_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_RequestTicketID]
    ON [dbo].[AmRelatedInformation]([RequestTicketID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[AmRelatedInformation]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[AmRelatedInformation]([CreatedUserID] ASC);

