CREATE TABLE [dbo].[TokenExpire] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [UserName]          NVARCHAR (100) NOT NULL,
    [Token]             VARCHAR (MAX)  NOT NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.TokenExpire] PRIMARY KEY CLUSTERED ([ID] ASC)
);

