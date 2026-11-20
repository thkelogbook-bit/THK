CREATE TABLE [dbo].[AmReasonForPending] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [FactoryID]         INT            NOT NULL,
    [ReasonDesc]        NVARCHAR (500) COLLATE SQL_Latin1_General_CP1_CI_AI NOT NULL,
    [Active]            INT            NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.AmReasonForPending] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.AmReasonForPending_dbo.Factory_FactoryID] FOREIGN KEY ([FactoryID]) REFERENCES [dbo].[Factory] ([ID]),
    CONSTRAINT [FK_dbo.AmReasonForPending_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.AmReasonForPending_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);



GO
CREATE NONCLUSTERED INDEX [IX_FactoryID]
    ON [dbo].[AmReasonForPending]([FactoryID] ASC);

GO


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[AmReasonForPending]([LastUpdatedUserID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[AmReasonForPending]([CreatedUserID] ASC);
