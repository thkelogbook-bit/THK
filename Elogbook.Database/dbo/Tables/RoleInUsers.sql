CREATE TABLE [dbo].[RoleInUsers] (
    [ID]                INT      IDENTITY (1, 1) NOT NULL,
    [UserRoleID]        INT      NOT NULL,
    [UserID]            INT      NOT NULL,
    [IsDeleted]         BIT      NOT NULL,
    [CreatedUserID]     INT      NULL,
    [InsertedAt]        DATETIME NOT NULL,
    [LastUpdatedUserID] INT      NULL,
    [UpdatedAt]         DATETIME NOT NULL,
    CONSTRAINT [PK_dbo.RoleInUsers] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.RoleInUsers_dbo.UserRoles_UserRoleID] FOREIGN KEY ([UserRoleID]) REFERENCES [dbo].[UserRoles] ([ID]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.RoleInUsers_dbo.Users_UserID] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([ID]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_UserRoleID]
    ON [dbo].[RoleInUsers]([UserRoleID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_UserID]
    ON [dbo].[RoleInUsers]([UserID] ASC);

