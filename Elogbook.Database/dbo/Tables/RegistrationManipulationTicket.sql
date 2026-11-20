CREATE TABLE [dbo].[RegistrationManipulationTicket] (
    [ID]                       INT            IDENTITY (1, 1) NOT NULL,
    [TicketTypeValue]          INT            NOT NULL,
    [FactoryID]                INT            NULL,
    [CreateDate]               DATETIME       NOT NULL,
    [ApproveUnitID]            INT            NULL,
    [ApproveUnitView]          NVARCHAR (255) NULL,
    [TicketName]               NVARCHAR (500) NULL,
    [CreatedUserPositionName]  NVARCHAR (255) NULL,
    [ApprovedUserID]           INT            NOT NULL,
    [ApprovedUserPositionName] NVARCHAR (255) NULL,
    [Purpose]                  NVARCHAR (MAX) NOT NULL,
    [StartTime]                DATETIME       NOT NULL,
    [EndTime]                  DATETIME       NOT NULL,
    [RecommendedUnit]          NVARCHAR (MAX) NULL,
    [ReferAttachmentIDs]       NVARCHAR (500) NULL,
    [TicketStatusValue]        INT            NOT NULL,
    [RegistrationDate]         DATETIME       NULL,
    [IsDeleted]                BIT            NOT NULL,
    [CreatedUserID]            INT            NULL,
    [InsertedAt]               DATETIME       NOT NULL,
    [LastUpdatedUserID]        INT            NULL,
    [UpdatedAt]                DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.RegistrationManipulationTicket] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.RegistrationManipulationTicket_dbo.Factory_FactoryID] FOREIGN KEY ([FactoryID]) REFERENCES [dbo].[Factory] ([ID]),
    CONSTRAINT [FK_dbo.RegistrationManipulationTicket_dbo.Organization_ApproveUnitID] FOREIGN KEY ([ApproveUnitID]) REFERENCES [dbo].[Organization] ([ID]),
    CONSTRAINT [FK_dbo.RegistrationManipulationTicket_dbo.TicketManipulationStatus_TicketStatusValue] FOREIGN KEY ([TicketStatusValue]) REFERENCES [dbo].[TicketManipulationStatus] ([ID]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.RegistrationManipulationTicket_dbo.Users_ApprovedUserID] FOREIGN KEY ([ApprovedUserID]) REFERENCES [dbo].[Users] ([ID]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.RegistrationManipulationTicket_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.RegistrationManipulationTicket_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_TicketStatusValue]
    ON [dbo].[RegistrationManipulationTicket]([TicketStatusValue] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[RegistrationManipulationTicket]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FactoryID]
    ON [dbo].[RegistrationManipulationTicket]([FactoryID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[RegistrationManipulationTicket]([CreatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ApproveUnitID]
    ON [dbo].[RegistrationManipulationTicket]([ApproveUnitID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ApprovedUserID]
    ON [dbo].[RegistrationManipulationTicket]([ApprovedUserID] ASC);

