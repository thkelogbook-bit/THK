CREATE TABLE [dbo].[AlertDetails] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [ParameterID]       INT            NULL,
    [Alert]             NVARCHAR (100) NULL,
    [Operator]          NVARCHAR (100) NULL,
    [Value]             REAL           NOT NULL,
    [Color]             NVARCHAR (100) NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    [SyncedDate]        DATETIME       NULL,
    CONSTRAINT [PK_dbo.AlertDetails] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.AlertDetails_dbo.Parameters_ParameterID] FOREIGN KEY ([ParameterID]) REFERENCES [dbo].[Parameters] ([ID]),
    CONSTRAINT [FK_dbo.AlertDetails_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.AlertDetails_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_ParameterID]
    ON [dbo].[AlertDetails]([ParameterID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[AlertDetails]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[AlertDetails]([CreatedUserID] ASC);

