CREATE TABLE [dbo].[AmLocation] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [OrganizationID]    INT            NULL,
    [SiteParent]        NVARCHAR (255) NULL,
    [SiteDesc]          NVARCHAR (500) COLLATE SQL_Latin1_General_CP1_CI_AI NOT NULL,
    [SiteCode]          NVARCHAR (255) COLLATE SQL_Latin1_General_CP1_CI_AI NULL,
    [Active]            INT            NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    [PMISId]            NVARCHAR (25)  NULL,
    CONSTRAINT [PK_dbo.AmLocation] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.AmLocation_dbo.Organization_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [dbo].[Organization] ([ID]),
    CONSTRAINT [FK_dbo.AmLocation_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.AmLocation_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);



GO


GO


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[AmLocation]([LastUpdatedUserID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[AmLocation]([CreatedUserID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_OrganizationID]
    ON [dbo].[AmLocation]([OrganizationID] ASC);

