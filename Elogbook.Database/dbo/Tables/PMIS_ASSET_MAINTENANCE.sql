CREATE TABLE [dbo].[PMIS_ASSET_MAINTENANCE] (
    [ID]                INT             IDENTITY (1, 1) NOT NULL,
    [AMID]              BIGINT          NULL,
    [ASSETID]           NVARCHAR (25)   NULL,
    [WORDERID]          NVARCHAR (25)   NULL,
    [TASKID]            NVARCHAR (25)   NULL,
    [TASKDESC]          NVARCHAR (1000) NULL,
    [TPLSTARTDATE]      DATETIME        NULL,
    [TPLFINISHDATE]     DATETIME        NULL,
    [TACSTARTDATE]      DATETIME        NULL,
    [TACFINISHDATE]     DATETIME        NULL,
    [WTYPEID]           NVARCHAR (25)   NULL,
    [IsDeleted]         BIT             NOT NULL,
    [CreatedUserID]     INT             NULL,
    [InsertedAt]        DATETIME        NOT NULL,
    [LastUpdatedUserID] INT             NULL,
    [UpdatedAt]         DATETIME        NOT NULL,
    [JobId]             INT             NULL,
    [status]            INT             NULL,
    [IsSync]            INT             NULL,
    CONSTRAINT [PMIS_ASSET_MAINTENANCE_PK2] PRIMARY KEY CLUSTERED ([ID] ASC)
);

