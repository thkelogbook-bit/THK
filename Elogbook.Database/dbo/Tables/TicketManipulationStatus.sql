CREATE TABLE [dbo].[TicketManipulationStatus] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [StatusName]        NVARCHAR (255) NOT NULL,
    [Active]            INT            NOT NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.TicketManipulationStatus] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.TicketManipulationStatus_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.TicketManipulationStatus_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[TicketManipulationStatus]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[TicketManipulationStatus]([CreatedUserID] ASC);

