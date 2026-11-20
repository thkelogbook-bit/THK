CREATE TABLE [dbo].[OperationSituation] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [OperationDiaryID]  INT            NOT NULL,
    [Hour]              FLOAT (53)     NOT NULL,
    [AssignedFromID]    INT            NULL,
    [AssignedFromName]  NVARCHAR (255) COLLATE SQL_Latin1_General_CP1_CI_AI NULL,
    [AssignedToID]      INT            NULL,
    [Description]       NVARCHAR (MAX) NOT NULL,
    [Attachments]       NVARCHAR (500) NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    [SyncDataKey]       NVARCHAR (100) NULL,
    [AppID]             NVARCHAR (100) NULL,
    [AssignedToName]    NVARCHAR (255) COLLATE SQL_Latin1_General_CP1_CI_AI NULL,
    [SyncedDate]        DATETIME       NULL,
    [FactoryID]         INT            NULL,
    CONSTRAINT [PK_dbo.OperationSituation] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.OperationSituation_dbo.OperationDiary_OperationDiaryID] FOREIGN KEY ([OperationDiaryID]) REFERENCES [dbo].[OperationDiary] ([ID]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.OperationSituation_dbo.Users_AssignedFromID] FOREIGN KEY ([AssignedFromID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.OperationSituation_dbo.Users_AssignedToID] FOREIGN KEY ([AssignedToID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.OperationSituation_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.OperationSituation_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);




GO
CREATE NONCLUSTERED INDEX [IX_Performance_Search]
    ON [dbo].[OperationSituation]([IsDeleted] ASC, [InsertedAt] ASC, [FactoryID] ASC)
    INCLUDE([OperationDiaryID], [Description]);


GO
CREATE NONCLUSTERED INDEX [IX_OperationDiaryID]
    ON [dbo].[OperationSituation]([OperationDiaryID] ASC);

