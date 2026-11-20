CREATE TABLE [dbo].[UserPermission] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [PermissionKey]     NVARCHAR (50)  NOT NULL,
    [PermissionName]    NVARCHAR (255) NOT NULL,
    [Active]            INT            NOT NULL,
    [ModuleValue]       NVARCHAR (255) NOT NULL,
    [SystemType]        NVARCHAR (50)  NOT NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.UserPermission] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.UserPermission_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.UserPermission_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[UserPermission]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[UserPermission]([CreatedUserID] ASC);

