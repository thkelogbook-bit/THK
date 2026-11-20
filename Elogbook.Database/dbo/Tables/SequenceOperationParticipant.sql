CREATE TABLE [dbo].[SequenceOperationParticipant] (
    [ID]                  INT            IDENTITY (1, 1) NOT NULL,
    [TicketID]            INT            NOT NULL,
    [SequenceOperationID] INT            NOT NULL,
    [UserName]            NVARCHAR (100) NULL,
    [FullName]            NVARCHAR (100) NULL,
    [PositionName]        NVARCHAR (255) NULL,
    [Roles]               INT            NOT NULL,
    [ElectricRank]        NVARCHAR (10)  NULL,
    [ElectricRankCard]    NVARCHAR (255) NULL,
    [Signature]           NVARCHAR (MAX) NULL,
    [IsExternal]          BIT            NULL,
    [IsDeleted]           BIT            NOT NULL,
    [CreatedUserID]       INT            NULL,
    [InsertedAt]          DATETIME       NOT NULL,
    [LastUpdatedUserID]   INT            NULL,
    [UpdatedAt]           DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.SequenceOperationParticipant] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.SequenceOperationParticipant_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.SequenceOperationParticipant_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);




GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[SequenceOperationParticipant]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[SequenceOperationParticipant]([CreatedUserID] ASC);

