CREATE TABLE [dbo].[TrackingSyncMobileData] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [DeviceID]          NVARCHAR (MAX) NOT NULL,
    [SyncDate]          DATETIME       NOT NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    [SyncDataModule]    DATETIME       CONSTRAINT [DF__TrackingS__SyncD__10566F31] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_dbo.TrackingSyncMobileData] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.TrackingSyncMobileData_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.TrackingSyncMobileData_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);




GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[TrackingSyncMobileData]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[TrackingSyncMobileData]([CreatedUserID] ASC);

