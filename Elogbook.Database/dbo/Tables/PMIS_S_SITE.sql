CREATE TABLE [dbo].[PMIS_S_SITE] (
    [SITEID]            NVARCHAR (25)  NOT NULL,
    [ORGID]             NVARCHAR (25)  NOT NULL,
    [SITEDESC]          NVARCHAR (500) NOT NULL,
    [ACTIVE]            BIT            NOT NULL,
    [SITEID_PARENT]     NVARCHAR (25)  NULL,
    [SITEORD]           INT            NULL,
    [LST_DEPTID]        NVARCHAR (250) NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [JobID]             INT            NOT NULL,
    [status]            INT            NULL,
    [IsSync]            BIT            NULL,
    CONSTRAINT [PK_S_SITE1] PRIMARY KEY CLUSTERED ([ID] ASC)
);

