CREATE TABLE [dbo].[Participant] (
    [ID]                         INT            IDENTITY (1, 1) NOT NULL,
    [TicketID]                   INT            NULL,
    [RegistrationManipulationID] INT            NULL,
    [IsCreated]                  BIT            NOT NULL,
    [UserName]                   NVARCHAR (100) NULL,
    [FullName]                   NVARCHAR (100) NULL,
    [PositionName]               NVARCHAR (255) NULL,
    [Roles]                      INT            NOT NULL,
    [ElectricRank]               NVARCHAR (10)  NULL,
    [ElectricRankCard]           NVARCHAR (255) NULL,
    [SignatureID]                INT            NULL,
    [IsExternal]                 BIT            NULL,
    [Order]                      INT            NULL,
    [IsDeleted]                  BIT            NOT NULL,
    [CreatedUserID]              INT            NULL,
    [InsertedAt]                 DATETIME       NOT NULL,
    [LastUpdatedUserID]          INT            NULL,
    [UpdatedAt]                  DATETIME       NOT NULL,
    [SyncedDate]                 DATETIME       NULL,
    CONSTRAINT [PK_dbo.Participant] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.Participant_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.Participant_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);




GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[Participant]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[Participant]([CreatedUserID] ASC);

