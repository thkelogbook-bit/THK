CREATE TABLE [dbo].[EmailTemplate] (
    [ID]                 INT            IDENTITY (1, 1) NOT NULL,
    [NotificationTypeID] INT            NOT NULL,
    [ModuleID]           INT            NOT NULL,
    [EmailSubject]       NVARCHAR (500) NOT NULL,
    [EmailContent]       NTEXT          NOT NULL,
    [EmailCode]          VARCHAR (50)   NULL,
    [Active]             INT            NOT NULL,
    [FactoryID]          INT            NULL,
    [IsDeleted]          BIT            NOT NULL,
    [CreatedUserID]      INT            NULL,
    [InsertedAt]         DATETIME       NOT NULL,
    [LastUpdatedUserID]  INT            NULL,
    [UpdatedAt]          DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.EmailTemplate] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.EmailTemplate_dbo.Factory_FactoryID] FOREIGN KEY ([FactoryID]) REFERENCES [dbo].[Factory] ([ID]),
    CONSTRAINT [FK_dbo.EmailTemplate_dbo.SystemData_ModuleID] FOREIGN KEY ([ModuleID]) REFERENCES [dbo].[SystemData] ([ID]),
    CONSTRAINT [FK_dbo.EmailTemplate_dbo.SystemData_NotificationTypeID] FOREIGN KEY ([NotificationTypeID]) REFERENCES [dbo].[SystemData] ([ID]),
    CONSTRAINT [FK_dbo.EmailTemplate_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.EmailTemplate_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);







