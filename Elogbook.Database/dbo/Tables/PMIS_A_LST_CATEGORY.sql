CREATE TABLE [dbo].[PMIS_A_LST_CATEGORY] (
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
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [JobId]             INT            NOT NULL,
    [status]            INT            NULL,
    [IsSync]            BIT            NULL,
    CONSTRAINT [PK_A_CATEGORY2] PRIMARY KEY CLUSTERED ([ID] ASC)
);

