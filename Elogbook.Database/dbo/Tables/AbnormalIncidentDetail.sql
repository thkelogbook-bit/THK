CREATE TABLE [dbo].[AbnormalIncidentDetail] (
    [ID]                 INT            IDENTITY (1, 1) NOT NULL,
    [Content]            NVARCHAR (MAX) NULL,
    [Attachments]        NVARCHAR (500) NULL,
    [IsComplete]         INT            NOT NULL,
    [AbnormalIncidentID] INT            NULL,
    [IsDeleted]          BIT            NOT NULL,
    [CreatedUserID]      INT            NULL,
    [InsertedAt]         DATETIME       NOT NULL,
    [LastUpdatedUserID]  INT            NULL,
    [UpdatedAt]          DATETIME       NOT NULL,
    [AppID]              NVARCHAR (100) NULL,
    CONSTRAINT [PK_dbo.AbnormalIncidentDetail] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.AbnormalIncidentDetail_dbo.AbnormalIncident_AbnormalIncidentDetailID] FOREIGN KEY ([AbnormalIncidentID]) REFERENCES [dbo].[AbnormalIncident] ([ID]),
    CONSTRAINT [FK_dbo.AbnormalIncidentDetail_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.AbnormalIncidentDetail_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);

