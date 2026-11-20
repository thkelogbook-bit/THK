CREATE TABLE [dbo].[ScheduleHistories] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [ScheduleName]      NVARCHAR (500) NULL,
    [Status]            INT            NULL,
    [ScheduleTime]      DATETIME       NULL,
    [ErrorMessage]      NVARCHAR (MAX) NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    CONSTRAINT [PK__Schedule__3214EC27C7EED5C1] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.ScheduleHistories_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.ScheduleHistories_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);

