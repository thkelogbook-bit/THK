CREATE TABLE [dbo].[AmWork] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [FactoryID]         INT            NOT NULL,
    [AmWorkTypeID]      INT            NOT NULL,
    [Active]            INT            NULL,
    [WorkName]          NVARCHAR (200) COLLATE SQL_Latin1_General_CP1_CI_AI NOT NULL,
    [WorkDesc]          NVARCHAR (MAX) NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.AmWork] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.AmWork_dbo.AmWorkType_AmWorkTypeID] FOREIGN KEY ([AmWorkTypeID]) REFERENCES [dbo].[AmWorkType] ([ID]),
    CONSTRAINT [FK_dbo.AmWork_dbo.Factory_FactoryID] FOREIGN KEY ([FactoryID]) REFERENCES [dbo].[Factory] ([ID]),
    CONSTRAINT [FK_dbo.AmWork_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.AmWork_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);



GO
CREATE NONCLUSTERED INDEX [IX_FactoryID]
    ON [dbo].[AmWork]([FactoryID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_AmWorkTypeID]
    ON [dbo].[AmWork]([AmWorkTypeID] ASC);

GO


    GO


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[AmWork]([LastUpdatedUserID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[AmWork]([CreatedUserID] ASC);
