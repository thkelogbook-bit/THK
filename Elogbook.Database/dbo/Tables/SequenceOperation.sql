CREATE TABLE [dbo].[SequenceOperation] (
    [ID]                           INT            IDENTITY (1, 1) NOT NULL,
    [TicketID]                     INT            NULL,
    [RegistrationManipulationID]   INT            NULL,
    [ManipulationTemplateTicketID] INT            NULL,
    [IsCreated]                    BIT            NOT NULL,
    [TableOfContents]              NVARCHAR (255) NULL,
    [Location]                     NVARCHAR (MAX) NULL,
    [IsDeleted]                    BIT            NOT NULL,
    [CreatedUserID]                INT            NULL,
    [InsertedAt]                   DATETIME       NOT NULL,
    [LastUpdatedUserID]            INT            NULL,
    [UpdatedAt]                    DATETIME       NOT NULL,
    [SyncedDate]                   DATETIME       NULL,
    [Content]                      NVARCHAR (MAX) NULL,
    [BeginTime]                    DATETIME       NULL,
    [EndTime]                      DATETIME       NULL,
    [LeaderID]                     INT            NULL,
    [WorkerID]                     INT            NULL,
    [IsDone]                       BIT            NOT NULL,
    [Step]                         INT            NOT NULL,
    [ParentStep]                   INT            NULL,
    CONSTRAINT [PK_dbo.SequenceOperation] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.SequenceOperation_dbo.SequenceOperationParticipant_LeaderID] FOREIGN KEY ([LeaderID]) REFERENCES [dbo].[SequenceOperationParticipant] ([ID]),
    CONSTRAINT [FK_dbo.SequenceOperation_dbo.SequenceOperationParticipant_WorkerID] FOREIGN KEY ([WorkerID]) REFERENCES [dbo].[SequenceOperationParticipant] ([ID]),
    CONSTRAINT [FK_dbo.SequenceOperation_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.SequenceOperation_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);








GO
CREATE NONCLUSTERED INDEX [IX_WorkerID]
    ON [dbo].[SequenceOperation]([WorkerID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LeaderID]
    ON [dbo].[SequenceOperation]([LeaderID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[SequenceOperation]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[SequenceOperation]([CreatedUserID] ASC);

