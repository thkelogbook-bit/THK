CREATE TABLE [dbo].[HazardsAndSafetyMeasures] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [Active]            INT            NULL,
    [ContentText]       NVARCHAR (500) COLLATE SQL_Latin1_General_CP1_CI_AI NOT NULL,
    [ContentType]       INT            NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.HazardsAndSafetyMeasures] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.HazardsAndSafetyMeasures_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.HazardsAndSafetyMeasures_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);

