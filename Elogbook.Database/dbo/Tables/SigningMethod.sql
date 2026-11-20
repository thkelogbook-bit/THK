CREATE TABLE [dbo].[SigningMethod] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [SigningMethodName] NVARCHAR (255) NOT NULL,
    [Active]            INT            NOT NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.SigningMethod] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.SigningMethod_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.SigningMethod_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);

