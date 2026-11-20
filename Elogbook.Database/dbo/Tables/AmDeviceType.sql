CREATE TABLE [dbo].[AmDeviceType] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [TypeCode]          NVARCHAR (255) COLLATE SQL_Latin1_General_CP1_CI_AI NULL,
    [TypeDesc]          NVARCHAR (200) COLLATE SQL_Latin1_General_CP1_CI_AI NOT NULL,
    [Active]            INT            NULL,
    [IsDeleted]         BIT            NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    [PMISId]            NVARCHAR (25)  NULL,
    [PMISType]          NVARCHAR (20)  NULL,
    CONSTRAINT [PK_dbo.AmDeviceType] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.AmDeviceType_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.AmDeviceType_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);



GO


GO


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[AmDeviceType]([LastUpdatedUserID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[AmDeviceType]([CreatedUserID] ASC);
