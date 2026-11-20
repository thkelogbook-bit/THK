CREATE TABLE [dbo].[PMIS_G_ERP_COUNTRY] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [COUNTRY_CODE]      NVARCHAR (5)   NOT NULL,
    [COUNTRY_NAME]      NVARCHAR (100) NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    [jobId]             INT            NULL,
    [status]            INT            NULL,
    [IsSync]            BIT            NULL,
    CONSTRAINT [PK_G_ERP_COUNTRY3] PRIMARY KEY CLUSTERED ([ID] ASC)
);

