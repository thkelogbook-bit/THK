CREATE TABLE [dbo].[AllowedInfoDetail] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [AllowedInforID]    INT            NOT NULL,
    [Content]           NVARCHAR (MAX) NULL,
    [Position]          INT            NULL,
    [Note]              NVARCHAR (MAX) NULL,
    [IsChecked]         BIT            NULL,
    [Attachments]       NVARCHAR (MAX) NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.AllowedInfoDetail] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.AllowedInfoDetail_dbo.AllowedInfo_AllowedInforID] FOREIGN KEY ([AllowedInforID]) REFERENCES [dbo].[AllowedInfo] ([ID]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.AllowedInfoDetail_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.AllowedInfoDetail_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[AllowedInfoDetail]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[AllowedInfoDetail]([CreatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AllowedInforID]
    ON [dbo].[AllowedInfoDetail]([AllowedInforID] ASC);

