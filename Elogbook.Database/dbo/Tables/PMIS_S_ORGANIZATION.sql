CREATE TABLE [dbo].[PMIS_S_ORGANIZATION] (
    [ORGID]               NVARCHAR (25)  NOT NULL,
    [ORGDESC]             NVARCHAR (500) NOT NULL,
    [ORGCODE]             NVARCHAR (50)  NULL,
    [ACTIVE]              BIT            NOT NULL,
    [ORGID_PARENT]        NVARCHAR (25)  NULL,
    [ORGORD]              INT            NULL,
    [PARRELTYPEID]        NVARCHAR (25)  NULL,
    [ORGLEVEL]            NVARCHAR (25)  NULL,
    [APPURL]              NVARCHAR (500) NULL,
    [APPURLACTIVE]        BIT            NULL,
    [ACTIVEDATE]          DATETIME       NOT NULL,
    [ISRUNNING]           BIT            NULL,
    [AUTO_LOGIN_CODE]     NVARCHAR (500) NULL,
    [UPD_LOGIN_CODE_TIME] DATETIME       NULL,
    [PARENT_RPT_LEVEL]    NVARCHAR (50)  NULL,
    [VERSION]             NVARCHAR (50)  NULL,
    [FUELTYPEID]          VARCHAR (50)   NULL,
    [IsDeleted]           BIT            NOT NULL,
    [CreatedUserID]       INT            NULL,
    [InsertedAt]          DATETIME       NOT NULL,
    [LastUpdatedUserID]   INT            NULL,
    [UpdatedAt]           DATETIME       NOT NULL,
    [ID]                  INT            IDENTITY (1, 1) NOT NULL,
    [JobID]               INT            NOT NULL,
    [status]              INT            NULL,
    [IsSync]              BIT            NULL,
    [SHORT_NAME]          NVARCHAR (250) NULL,
    CONSTRAINT [PK_S_ORGANIZATION2] PRIMARY KEY CLUSTERED ([ID] ASC)
);



