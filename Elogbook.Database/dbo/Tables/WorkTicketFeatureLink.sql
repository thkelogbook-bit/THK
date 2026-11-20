CREATE TABLE [dbo].[WorkTicketFeatureLink] (
    [ID]                INT      IDENTITY (1, 1) NOT NULL,
    [FeatureID]         INT      NOT NULL,
    [WorkTicketID]      INT      NOT NULL,
    [IsDeleted]         BIT      NOT NULL,
    [CreatedUserID]     INT      NULL,
    [InsertedAt]        DATETIME NOT NULL,
    [LastUpdatedUserID] INT      NULL,
    [UpdatedAt]         DATETIME NOT NULL,
    CONSTRAINT [PK_dbo.WorkTicketFeatureLink] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.WorkTicketFeatureLink_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.WorkTicketFeatureLink_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[WorkTicketFeatureLink]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[WorkTicketFeatureLink]([CreatedUserID] ASC);

