CREATE TABLE [dbo].[TrackingLoginOperation] (
    [ID]                  INT      IDENTITY (1, 1) NOT NULL,
    [OperationPositionID] INT      NOT NULL,
    [IsDeleted]           BIT      NULL,
    [CreatedUserID]       INT      NOT NULL,
    [InsertedAt]          DATETIME NOT NULL,
    [LastUpdatedUserID]   INT      NULL,
    [UpdatedAt]           DATETIME NOT NULL,
    CONSTRAINT [PK_TrackingLoginOperation] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.TrackingLoginOperation_dbo.OperationPosition_OperationPositionID] FOREIGN KEY ([OperationPositionID]) REFERENCES [dbo].[OperationPosition] ([ID]),
    CONSTRAINT [FK_dbo.TrackingLoginOperation_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.TrackingLoginOperation_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_OperationPositionID]
    ON [dbo].[TrackingLoginOperation]([OperationPositionID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[TrackingLoginOperation]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[TrackingLoginOperation]([CreatedUserID] ASC);

