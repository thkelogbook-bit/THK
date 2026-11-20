CREATE TABLE [dbo].[AmSupplier] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [SupplierName]      NVARCHAR (200) COLLATE SQL_Latin1_General_CP1_CI_AI NOT NULL,
    [SupplierCode]      NVARCHAR (200) COLLATE SQL_Latin1_General_CP1_CI_AI NOT NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    [PMISId]            NVARCHAR (25)  NULL,
    [Active]            INT            CONSTRAINT [DF__AmSupplier__Active__4924D839] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_AmSupplier] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.AmSupplier_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.AmSupplier_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);




GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[AmSupplier]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[AmSupplier]([CreatedUserID] ASC);

