CREATE TABLE [dbo].[Attachment] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [FilePath]          NVARCHAR (500) NULL,
    [PathThumbnail]     NVARCHAR (500) NULL,
    [FileName]          NVARCHAR (500) NULL,
    [FileType]          NVARCHAR (255) NULL,
    [Description]       NVARCHAR (MAX) NULL,
    [ScreenName]        NVARCHAR (100) NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    [SyncedDate]        DATETIME       NULL,
    CONSTRAINT [PK_dbo.Attachment] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.Attachment_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.Attachment_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[Attachment]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[Attachment]([CreatedUserID] ASC);

