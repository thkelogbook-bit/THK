CREATE TABLE [dbo].[OperationShift] (
    [ID]                 INT            IDENTITY (1, 1) NOT NULL,
    [OperationShiftName] NVARCHAR (255) NOT NULL,
    [Position]           INT            NOT NULL,
    [FactoryID]          INT            NOT NULL,
    [StartTime]          FLOAT (53)     NOT NULL,
    [EndTime]            FLOAT (53)     NOT NULL,
    [Active]             INT            NOT NULL,
    [IsDeleted]          BIT            NOT NULL,
    [CreatedUserID]      INT            NULL,
    [InsertedAt]         DATETIME       NOT NULL,
    [LastUpdatedUserID]  INT            NULL,
    [UpdatedAt]          DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.OperationShift] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.OperationShift_dbo.Factory_FactoryID] FOREIGN KEY ([FactoryID]) REFERENCES [dbo].[Factory] ([ID]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.OperationShift_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.OperationShift_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[OperationShift]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FactoryID]
    ON [dbo].[OperationShift]([FactoryID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[OperationShift]([CreatedUserID] ASC);

