CREATE TABLE [dbo].[AmRequestTicket] (
    [ID]                      INT             IDENTITY (1, 1) NOT NULL,
    [FactoryID]               INT             NOT NULL,
    [AmLocationId]            INT             NULL,
    [AmDeviceId]              INT             NULL,
    [AmEmergencyId]           INT             NOT NULL,
    [OrganizationId]          INT             NULL,
    [RequestTicketNumber]     NVARCHAR (100)  NOT NULL,
    [RequestContent]          NVARCHAR (500)  NOT NULL,
    [Detail]                  NVARCHAR (1000) NULL,
    [Note]                    NVARCHAR (1000) NULL,
    [AmStatusID]              INT             NOT NULL,
    [IsDeleted]               BIT             NOT NULL,
    [CreatedUserID]           INT             NULL,
    [InsertedAt]              DATETIME        NOT NULL,
    [LastUpdatedUserID]       INT             NULL,
    [UpdatedAt]               DATETIME        NOT NULL,
    [AutonomousMaintenanceID] INT             NULL,
    [Attachments]             NVARCHAR (500)  NULL,
    [RegistrationTicketID]    INT             NULL,
    [RegistrationTicketType]  INT             NULL,
    [InsertedAtBySend]        DATETIME        NULL,
    CONSTRAINT [PK_dbo.AmRequestTicket] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.AmRequestTicket_dbo.AmDevice_AmDeviceId] FOREIGN KEY ([AmDeviceId]) REFERENCES [dbo].[AmDevice] ([ID]),
    CONSTRAINT [FK_dbo.AmRequestTicket_dbo.AmEmergency_AmEmergencyId] FOREIGN KEY ([AmEmergencyId]) REFERENCES [dbo].[AmEmergency] ([ID]),
    CONSTRAINT [FK_dbo.AmRequestTicket_dbo.AmLocation_AmLocationId] FOREIGN KEY ([AmLocationId]) REFERENCES [dbo].[AmLocation] ([ID]),
    CONSTRAINT [FK_dbo.AmRequestTicket_dbo.AmRequestTicketStatus_ID] FOREIGN KEY ([AmStatusID]) REFERENCES [dbo].[AmRequestTicketStatus] ([ID]),
    CONSTRAINT [FK_dbo.AmRequestTicket_dbo.Factory_FactoryID] FOREIGN KEY ([FactoryID]) REFERENCES [dbo].[Factory] ([ID]),
    CONSTRAINT [FK_dbo.AmRequestTicket_dbo.Organization_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organization] ([ID]),
    CONSTRAINT [FK_dbo.AmRequestTicket_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.AmRequestTicket_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);





GO
CREATE NONCLUSTERED INDEX [IX_FactoryID]
    ON [dbo].[AmRequestTicket]([FactoryID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_AmLocationId]
    ON [dbo].[AmRequestTicket]([AmLocationId] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_AmEmergencyId]
    ON [dbo].[AmRequestTicket]([AmEmergencyId] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_OrganizationId]
    ON [dbo].[AmRequestTicket]([OrganizationId] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[AmRequestTicket]([LastUpdatedUserID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[AmRequestTicket]([CreatedUserID] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_AmDeviceId]
    ON [dbo].[AmRequestTicket]([AmDeviceId] ASC);

