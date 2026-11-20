CREATE TABLE [dbo].[Organization] (
    [ID]                  INT            IDENTITY (1, 1) NOT NULL,
    [ParentID]            INT            NULL,
    [Name]                NVARCHAR (255) NOT NULL,
    [ShortName]           NVARCHAR (100) NOT NULL,
    [Level]               NVARCHAR (100) NOT NULL,
    [CompanyType]         NVARCHAR (255) NULL,
    [ProductionPlantType] NVARCHAR (100) NULL,
    [Active]              INT            NOT NULL,
    [AttachmentID]        INT            NULL,
    [IsDeleted]           BIT            NOT NULL,
    [CreatedUserID]       INT            NULL,
    [InsertedAt]          DATETIME       NOT NULL,
    [LastUpdatedUserID]   INT            NULL,
    [UpdatedAt]           DATETIME       NOT NULL,
    [TicketDescription]   NVARCHAR (255) NULL,
    [PMIS_ORGID]          NVARCHAR (25)  NULL,
    [PMIS_ORGCODE]        NVARCHAR (255) NULL,
    [PMIS_ORGID_PARENT]   NVARCHAR (255) NULL,
    CONSTRAINT [PK_dbo.Organization] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.Organization_dbo.Attachment_AttachmentID] FOREIGN KEY ([AttachmentID]) REFERENCES [dbo].[Attachment] ([ID]),
    CONSTRAINT [FK_dbo.Organization_dbo.Organization_ParentID] FOREIGN KEY ([ParentID]) REFERENCES [dbo].[Organization] ([ID]),
    CONSTRAINT [FK_dbo.Organization_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.Organization_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);






GO
CREATE NONCLUSTERED INDEX [IX_ParentID]
    ON [dbo].[Organization]([ParentID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[Organization]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[Organization]([CreatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AttachmentID]
    ON [dbo].[Organization]([AttachmentID] ASC);

