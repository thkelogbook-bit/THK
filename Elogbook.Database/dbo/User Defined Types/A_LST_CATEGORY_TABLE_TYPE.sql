CREATE TYPE [dbo].[A_LST_CATEGORY_TABLE_TYPE] AS TABLE (
    [CATEGORYID]        NVARCHAR (25)  NOT NULL,
    [CATEGORYDESC]      NVARCHAR (255) NOT NULL,
    [CATEGORYID_PARENT] NVARCHAR (25)  NULL,
    [ORGID]             NVARCHAR (25)  NULL,
    [CATEGORYORD]       INT            NULL,
    [ASSETIDPREFIX]     NVARCHAR (25)  NULL,
    [USER_CR_ID]        NVARCHAR (25)  NULL,
    [USER_CR_DTIME]     DATETIME       NULL,
    [USER_MDF_ID]       NVARCHAR (25)  NULL,
    [USER_MDF_DTIME]    DATETIME       NULL,
    [JobId]             INT            NULL,
    [IsDeleted]         BIT            NULL,
    [InsertedAt]        DATETIME       NULL,
    [UpdatedAt]         DATETIME       NULL);

