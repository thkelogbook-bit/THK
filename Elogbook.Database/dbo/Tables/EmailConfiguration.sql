CREATE TABLE [dbo].[EmailConfiguration] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [EmailAddress]      NVARCHAR (255) NOT NULL,
    [EmailServer]       NVARCHAR (255) NOT NULL,
    [Port]              INT            NOT NULL,
    [EmailType]         INT            NOT NULL,
    [UserName]          NVARCHAR (255) NOT NULL,
    [PassWord]          NVARCHAR (255) NOT NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.EmailConfiguration] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.EmailConfiguration_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.EmailConfiguration_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);

