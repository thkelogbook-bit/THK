CREATE TABLE [dbo].[ProbationaryStaff] (
    [ID]                  INT            IDENTITY (1, 1) NOT NULL,
    [OperationDiaryID]    INT            NOT NULL,
    [OperationPositionID] INT            NOT NULL,
    [SyncDataKey]         NVARCHAR (100) NULL,
    [SyncedDate]          DATETIME       NULL,
    [AppID]               NVARCHAR (100) NULL,
    [AssignedUserID]      INT            NULL,
    [AssignedUserName]    NVARCHAR (255) COLLATE SQL_Latin1_General_CP1_CI_AI NULL,
    [PositionName]        NVARCHAR (255) NULL,
    [Description]         NVARCHAR (MAX) NULL,
    [IsDeleted]           BIT            NOT NULL,
    [CreatedUserID]       INT            NULL,
    [InsertedAt]          DATETIME       NOT NULL,
    [LastUpdatedUserID]   INT            NULL,
    [UpdatedAt]           DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.ProbationaryStaff] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.ProbationaryStaff_dbo.OperationDiary_OperationDiaryID] FOREIGN KEY ([OperationDiaryID]) REFERENCES [dbo].[OperationDiary] ([ID]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.ProbationaryStaff_dbo.PositionInShift_PositionInShiftID] FOREIGN KEY ([OperationPositionID]) REFERENCES [dbo].[PositionInShift] ([ID]),
    CONSTRAINT [FK_dbo.ProbationaryStaff_dbo.Users_AssignedUserID] FOREIGN KEY ([AssignedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.ProbationaryStaff_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.ProbationaryStaff_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);





