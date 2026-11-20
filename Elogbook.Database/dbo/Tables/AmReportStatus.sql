CREATE TABLE [dbo].[AmReportStatus] (
    [ID]                INT            NOT NULL,
    [StatusName]        NVARCHAR (100) NOT NULL,
    [StatusType]        INT            NOT NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NULL,
    CONSTRAINT [PK_dbo.AmReportStatus] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.AmReportStatus_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.AmReportStatus_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);

