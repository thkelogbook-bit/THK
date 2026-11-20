CREATE TABLE [dbo].[OtherRequest] (
    [ID]                         INT            IDENTITY (1, 1) NOT NULL,
    [TicketID]                   INT            NULL,
    [RegistrationManipulationID] INT            NULL,
    [IsCreated]                  BIT            NOT NULL,
    [Description]                NVARCHAR (MAX) NOT NULL,
    [Type]                       INT            NOT NULL,
    [IsDeleted]                  BIT            NOT NULL,
    [CreatedUserID]              INT            NULL,
    [InsertedAt]                 DATETIME       NOT NULL,
    [LastUpdatedUserID]          INT            NULL,
    [UpdatedAt]                  DATETIME       NOT NULL,
    [SyncedDate]                 DATETIME       NULL,
    CONSTRAINT [PK_dbo.OtherRequest] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.OtherRequest_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.OtherRequest_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[OtherRequest]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[OtherRequest]([CreatedUserID] ASC);

