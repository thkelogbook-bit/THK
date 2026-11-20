CREATE TABLE [dbo].[UserRoles] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [RoleName]          NVARCHAR (255) NOT NULL,
    [Description]       NVARCHAR (MAX) NULL,
    [Active]            INT            NOT NULL,
    [DataPermission]    INT            NOT NULL,
    [Permissions]       NVARCHAR (MAX) NULL,
    [IsAdmin]           BIT            NOT NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.UserRoles] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.UserRoles_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.UserRoles_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[UserRoles]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[UserRoles]([CreatedUserID] ASC);

