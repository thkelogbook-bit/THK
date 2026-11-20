CREATE TABLE [dbo].[AmDeviceCategory] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [CategoryParent]    INT            NULL,
    [CategoryName]      NVARCHAR (200) NOT NULL,
    [CategoryCode]      NVARCHAR (200) NOT NULL,
    [Active]            INT            NOT NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    [PMISId]            NVARCHAR (25)  NULL,
    [OrgId]             INT            NULL,
    CONSTRAINT [PK_AmCategory] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.AmCategory_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.AmCategory_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);





GO


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[AmDeviceCategory]([LastUpdatedUserID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[AmDeviceCategory]([CreatedUserID] ASC);
