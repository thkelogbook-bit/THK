CREATE TABLE [dbo].[EmailTransmissionRecipients] (
    [ID]                  INT            IDENTITY (1, 1) NOT NULL,
    [EmailTransmissionID] INT            NOT NULL,
    [RecipientEmailAddr]  NVARCHAR (MAX) NOT NULL,
    [RecipientEmailName]  NVARCHAR (MAX) NOT NULL,
    [RecipientType]       INT            NOT NULL,
    [InsertedAt]          DATETIME       NOT NULL,
    [UpdatedAt]           DATETIME       NOT NULL,
    [LastUpdatedUserID]   INT            NULL,
    [CreatedUserID]       INT            NULL,
    [IsDeleted]           BIT            NOT NULL
);



