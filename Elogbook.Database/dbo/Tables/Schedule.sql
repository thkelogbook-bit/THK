CREATE TABLE [dbo].[Schedule] (
    [ID]                  INT             IDENTITY (1, 1) NOT NULL,
    [ScheduleName]        NVARCHAR (500)  NOT NULL,
    [ScheduleContent]     NVARCHAR (500)  NOT NULL,
    [ScheduleValue]       NVARCHAR (500)  NOT NULL,
    [Active]              INT             NOT NULL,
    [IsSystem]            BIT             NOT NULL,
    [IsSendEmail]         BIT             NOT NULL,
    [EmailTo]             NVARCHAR (500)  NOT NULL,
    [EmailCC]             NVARCHAR (500)  NOT NULL,
    [EmailSubject]        NVARCHAR (500)  NOT NULL,
    [EmailContent]        NTEXT           NOT NULL,
    [IsDeleted]           BIT             NOT NULL,
    [CreatedUserID]       INT             NULL,
    [InsertedAt]          DATETIME        NOT NULL,
    [LastUpdatedUserID]   INT             NULL,
    [UpdatedAt]           DATETIME        NOT NULL,
    [ScheduleDescription] NVARCHAR (1000) NULL,
    CONSTRAINT [PK_dbo.Schedule] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.Schedule_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.Schedule_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);





