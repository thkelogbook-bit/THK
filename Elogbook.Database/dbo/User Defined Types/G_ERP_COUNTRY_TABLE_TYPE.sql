CREATE TYPE [dbo].[G_ERP_COUNTRY_TABLE_TYPE] AS TABLE (
    [ID]           INT            NOT NULL,
    [COUNTRY_CODE] NVARCHAR (5)   NOT NULL,
    [COUNTRY_NAME] NVARCHAR (100) NULL,
    [JobId]        INT            NULL,
    [IsDeleted]    BIT            NULL,
    [InsertedAt]   DATETIME       NULL,
    [UpdatedAt]    DATETIME       NULL);

