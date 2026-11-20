CREATE TABLE [dbo].[AmCountryManufacture] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [CountryName]       NVARCHAR (200) COLLATE SQL_Latin1_General_CP1_CI_AI NOT NULL,
    [CountryCode]       NVARCHAR (200) COLLATE SQL_Latin1_General_CP1_CI_AI NOT NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    [PMISId]            NVARCHAR (25)  NULL,
    [Active]            INT            CONSTRAINT [DF__AmCountryManufacture__Active__4924D839] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_AmCountryManufacture] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.AmCountryManufacture_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.AmCountryManufacture_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);




GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[AmCountryManufacture]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[AmCountryManufacture]([CreatedUserID] ASC);

