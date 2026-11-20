CREATE TABLE [dbo].[Signature] (
    [ID]                   INT             IDENTITY (1, 1) NOT NULL,
    [SignatureContent]     NVARCHAR (MAX)  NULL,
    [Type]                 INT             NOT NULL,
    [IsDeleted]            BIT             NOT NULL,
    [CreatedUserID]        INT             NULL,
    [InsertedAt]           DATETIME        NOT NULL,
    [LastUpdatedUserID]    INT             NULL,
    [UpdatedAt]            DATETIME        NOT NULL,
    [SignatureContentByte] VARBINARY (MAX) NULL,
    CONSTRAINT [PK_dbo.Signature] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.Signature_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.Signature_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[Signature]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[Signature]([CreatedUserID] ASC);

