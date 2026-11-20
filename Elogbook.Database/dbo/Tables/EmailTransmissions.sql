CREATE TABLE [dbo].[EmailTransmissions] (
    [ID]                       INT            IDENTITY (1, 1) NOT NULL,
    [EmailReplyToAddr]         NVARCHAR (255) NOT NULL,
    [Subject]                  NVARCHAR (300) NOT NULL,
    [MessageBody]              NVARCHAR (MAX) NOT NULL,
    [IsBodyHtml]               BIT            NULL,
    [Retry]                    INT            NULL,
    [DetailFailure]            NVARCHAR (MAX) NULL,
    [IsSent]                   BIT            NULL,
    [AttackmentID]             INT            NULL,
    [TransmissionCompleteFlag] BIT            NULL,
    [InsertedAt]               DATETIME       NOT NULL,
    [UpdatedAt]                DATETIME       NOT NULL,
    [LastUpdatedUserID]        INT            NULL,
    [CreatedUserID]            INT            NULL,
    [IsDeleted]                BIT            NOT NULL,
    [RegistrationTicketId]     INT            NULL,
    [WorkTicketId]             INT            NULL,
    [AutonomousMaintenanceId]  INT            NULL,
    [AmRequestTicketId]        INT            NULL
);





