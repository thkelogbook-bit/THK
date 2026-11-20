CREATE TABLE [dbo].[PMIS_A_LST_TYPE] (
    [TYPEID]            NVARCHAR (25)  NOT NULL,
    [TYPEDESC]          NVARCHAR (255) NULL,
    [TYPEORD]           INT            NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [JobId]             INT            NOT NULL,
    [status]            INT            NULL,
    [IsSync]            BIT            NULL,
    CONSTRAINT [PK_A_DM_TYPE2] PRIMARY KEY CLUSTERED ([ID] ASC)
);

