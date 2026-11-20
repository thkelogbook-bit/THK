CREATE TABLE [dbo].[RegistrationTicket] (
    [ID]                     INT            IDENTITY (1, 1) NOT NULL,
    [TicketType]             INT            NOT NULL,
    [FactoryID]              INT            NOT NULL,
    [TicketNumber]           NVARCHAR (255) NOT NULL,
    [RegistrationDate]       DATETIME       NULL,
    [ReceiveUnitID]          INT            NOT NULL,
    [RegulationNumber]       NVARCHAR (255) NULL,
    [RegistrationUnitID]     INT            NULL,
    [RegistrationUnitView]   NVARCHAR (500) NULL,
    [Position]               NVARCHAR (500) NULL,
    [PhoneNumber]            NVARCHAR (20)  NULL,
    [Email]                  NVARCHAR (255) NULL,
    [WorkContent]            NVARCHAR (MAX) NOT NULL,
    [WorkPlacement]          NVARCHAR (MAX) NOT NULL,
    [WorkCondition]          NVARCHAR (MAX) NOT NULL,
    [StartDate]              DATETIME       NOT NULL,
    [EndDate]                DATETIME       NOT NULL,
    [UnitNumberAndUnitStaff] NVARCHAR (255) NULL,
    [Units]                  NVARCHAR (MAX) NULL,
    [StatusValue]            INT            NOT NULL,
    [Attachments]            NVARCHAR (500) NULL,
    [LocationDevice]         NVARCHAR (MAX) NULL,
    [Reason]                 INT            NOT NULL,
    [ContentType]            INT            NOT NULL,
    [IsWorkDaily]            BIT            NOT NULL,
    [StartTimeDaily]         NVARCHAR (100) NULL,
    [EndTimeDaily]           NVARCHAR (100) NULL,
    [OtherContents]          NVARCHAR (MAX) NULL,
    [IsCollaborateTicket]    BIT            NOT NULL,
    [RegistrationTicketType] INT            NOT NULL,
    [IsDeleted]              BIT            NOT NULL,
    [CreatedUserID]          INT            NULL,
    [InsertedAt]             DATETIME       NOT NULL,
    [LastUpdatedUserID]      INT            NULL,
    [UpdatedAt]              DATETIME       NOT NULL,
    [WorkTicketID]           INT            NULL,
    [MajorRepairID]          INT            NULL,
    [RequestTicketID]        INT            NULL,
    CONSTRAINT [PK_dbo.RegistrationTicket] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.RegistrationTicket_dbo.Factory_FactoryID] FOREIGN KEY ([FactoryID]) REFERENCES [dbo].[Factory] ([ID]),
    CONSTRAINT [FK_dbo.RegistrationTicket_dbo.Organization_ReceiveUnitID] FOREIGN KEY ([ReceiveUnitID]) REFERENCES [dbo].[Organization] ([ID]),
    CONSTRAINT [FK_dbo.RegistrationTicket_dbo.Organization_RegistrationUnitID] FOREIGN KEY ([RegistrationUnitID]) REFERENCES [dbo].[Organization] ([ID]),
    CONSTRAINT [FK_dbo.RegistrationTicket_dbo.TicketStatus_StatusValue] FOREIGN KEY ([StatusValue]) REFERENCES [dbo].[TicketStatus] ([ID]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.RegistrationTicket_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.RegistrationTicket_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);








GO
CREATE NONCLUSTERED INDEX [IX_StatusValue]
    ON [dbo].[RegistrationTicket]([StatusValue] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_RegistrationUnitID]
    ON [dbo].[RegistrationTicket]([RegistrationUnitID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ReceiveUnitID]
    ON [dbo].[RegistrationTicket]([ReceiveUnitID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[RegistrationTicket]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FactoryID]
    ON [dbo].[RegistrationTicket]([FactoryID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[RegistrationTicket]([CreatedUserID] ASC);

