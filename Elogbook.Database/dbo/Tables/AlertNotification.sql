CREATE TABLE [dbo].[AlertNotification] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [UserID]            INT            NOT NULL,
    [NoticeTitle]       NVARCHAR (500) COLLATE SQL_Latin1_General_CP1_CI_AI NULL,
    [NoticeContent]     NVARCHAR (MAX) COLLATE SQL_Latin1_General_CP1_CI_AI NULL,
    [Status]            INT            NOT NULL,
    [IsImportant]       BIT            NULL,
    [IsDeleted]         BIT            NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.AlertNotification] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.AlertNotification_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.AlertNotification_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.AlertNotification_dbo.Users_UserID] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([ID])
);

