CREATE TABLE [dbo].[UsedTimes] (
    [ID]                INT      IDENTITY (1, 1) NOT NULL,
    [ActiveTime]        DATETIME NULL,
    [InactiveTime]      DATETIME NULL,
    [OperationShiftID]  INT      NULL,
    [IsDeleted]         BIT      NOT NULL,
    [CreatedUserID]     INT      NULL,
    [InsertedAt]        DATETIME NOT NULL,
    [LastUpdatedUserID] INT      NULL,
    [UpdatedAt]         DATETIME NOT NULL,
    CONSTRAINT [PK_dbo.UsedTimes] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.UsedTimes_dbo.OperationShift_OperationShiftID] FOREIGN KEY ([OperationShiftID]) REFERENCES [dbo].[OperationShift] ([ID]),
    CONSTRAINT [FK_dbo.UsedTimes_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.UsedTimes_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_OperationShiftID]
    ON [dbo].[UsedTimes]([OperationShiftID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[UsedTimes]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[UsedTimes]([CreatedUserID] ASC);

