CREATE TABLE [dbo].[TrackingStatus] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [Status]            NVARCHAR (255) NOT NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.TrackingStatus] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.TrackingStatus_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.TrackingStatus_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);




GO



GO


