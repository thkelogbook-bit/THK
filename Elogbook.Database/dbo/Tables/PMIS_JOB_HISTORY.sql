CREATE TABLE [dbo].[PMIS_JOB_HISTORY] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [LAST_RUN_DATE]     DATETIME       NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    [JobId]             INT            NULL,
    [status]            INT            NULL,
    [SyncType]          INT            NULL,
    [UserID]            INT            NULL,
    [Object]            NVARCHAR (150) NULL,
    [StartTime]         DATETIME       NULL,
    [EndTime]           DATETIME       NULL,
    [Duration]          INT            NULL,
    [NumRecord]         INT            NULL,
    [Logs]              TEXT           NULL,
    CONSTRAINT [PMIS_JOB_HISTORY_PK] PRIMARY KEY CLUSTERED ([ID] ASC)
);

