CREATE TABLE [dbo].[Parameters] (
    [ID]                  INT            IDENTITY (1, 1) NOT NULL,
    [IsGroup]             BIT            NOT NULL,
    [FactoryID]           INT            NULL,
    [OperationPositionID] INT            NULL,
    [CycleID]             INT            NULL,
    [ParameterGroupID]    INT            NULL,
    [RFIDCode]            NVARCHAR (255) NULL,
    [QRCode]              NVARCHAR (255) NULL,
    [ParameterName]       NVARCHAR (255) NOT NULL,
    [KKSCode]             NVARCHAR (255) NULL,
    [UnitMeasure]         NVARCHAR (255) NULL,
    [Setting]             NVARCHAR (255) NULL,
    [FromRange]           REAL           NULL,
    [ToRange]             REAL           NULL,
    [IsNumberType]        BIT            NOT NULL,
    [QLKTCode]            NVARCHAR (255) NULL,
    [Status]              INT            NOT NULL,
    [Position]            INT            NOT NULL,
    [IsDeleted]           BIT            NOT NULL,
    [CreatedUserID]       INT            NULL,
    [InsertedAt]          DATETIME       NOT NULL,
    [LastUpdatedUserID]   INT            NULL,
    [UpdatedAt]           DATETIME       NOT NULL,
    [SyncedDate]          DATETIME       NULL,
    CONSTRAINT [PK_dbo.Parameters] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.Parameters_dbo.CycleManagement_CycleID] FOREIGN KEY ([CycleID]) REFERENCES [dbo].[CycleManagement] ([ID]),
    CONSTRAINT [FK_dbo.Parameters_dbo.Factory_FactoryID] FOREIGN KEY ([FactoryID]) REFERENCES [dbo].[Factory] ([ID]),
    CONSTRAINT [FK_dbo.Parameters_dbo.OperationPosition_OperationPositionID] FOREIGN KEY ([OperationPositionID]) REFERENCES [dbo].[OperationPosition] ([ID]),
    CONSTRAINT [FK_dbo.Parameters_dbo.ParameterGroup_ParameterGroupID] FOREIGN KEY ([ParameterGroupID]) REFERENCES [dbo].[ParameterGroup] ([ID]),
    CONSTRAINT [FK_dbo.Parameters_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.Parameters_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_ParameterGroupID]
    ON [dbo].[Parameters]([ParameterGroupID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_OperationPositionID]
    ON [dbo].[Parameters]([OperationPositionID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[Parameters]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FactoryID]
    ON [dbo].[Parameters]([FactoryID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CycleID]
    ON [dbo].[Parameters]([CycleID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[Parameters]([CreatedUserID] ASC);

