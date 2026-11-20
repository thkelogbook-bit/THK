CREATE TABLE [dbo].[StepContentSequenceOperation] (
    [ID]                  INT            IDENTITY (1, 1) NOT NULL,
    [SequenceOperationId] INT            NOT NULL,
    [Step]                INT            NOT NULL,
    [Content]             NVARCHAR (MAX) NULL,
    [BeginTime]           DATETIME       NULL,
    [EndTime]             DATETIME       NULL,
    [LeaderID]            INT            NULL,
    [WorkerID]            INT            NULL,
    [IsDone]              BIT            NOT NULL,
    [IsDeleted]           BIT            NOT NULL,
    [CreatedUserID]       INT            NULL,
    [InsertedAt]          DATETIME       NOT NULL,
    [LastUpdatedUserID]   INT            NULL,
    [UpdatedAt]           DATETIME       NOT NULL,
    [SyncedDate]          DATETIME       NULL,
    CONSTRAINT [PK_dbo.StepContentSequenceOperation] PRIMARY KEY CLUSTERED ([ID] ASC)
);

