CREATE TABLE [dbo].[ManipulationTicket] (
    [ID]                               INT            IDENTITY (1, 1) NOT NULL,
    [TicketType]                       INT            NOT NULL,
    [RegistrationManipulationTicketID] INT            NULL,
    [FactoryID]                        INT            NULL,
    [CreatedDate]                      DATETIME       NOT NULL,
    [ApproveUnitID]                    INT            NULL,
    [ApproveUnitView]                  NVARCHAR (500) NULL,
    [TicketName]                       NVARCHAR (500) NULL,
    [TicketNumber]                     NVARCHAR (255) NULL,
    [CreatedUserTicket]                NVARCHAR (100) NULL,
    [ApprovedUserTicket]               NVARCHAR (100) NULL,
    [Purpose]                          NVARCHAR (MAX) NULL,
    [StartTime]                        DATETIME       NOT NULL,
    [EndTime]                          DATETIME       NOT NULL,
    [RecommendedUnit]                  NVARCHAR (MAX) NULL,
    [CaseAbnormal]                     NVARCHAR (MAX) NULL,
    [WorkEndTime]                      DATETIME       NULL,
    [CompletedTime]                    DATETIME       NULL,
    [ReferAttachmentIDs]               NVARCHAR (500) NULL,
    [MapAttachmentIDs]                 NVARCHAR (500) NULL,
    [TicketStatusValue]                INT            NOT NULL,
    [IsDeleted]                        BIT            NOT NULL,
    [CreatedUserID]                    INT            NULL,
    [InsertedAt]                       DATETIME       NOT NULL,
    [LastUpdatedUserID]                INT            NULL,
    [UpdatedAt]                        DATETIME       NOT NULL,
    [SyncedDate]                       DATETIME       NULL,
    CONSTRAINT [PK_dbo.ManipulationTicket] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.ManipulationTicket_dbo.Factory_FactoryID] FOREIGN KEY ([FactoryID]) REFERENCES [dbo].[Factory] ([ID]),
    CONSTRAINT [FK_dbo.ManipulationTicket_dbo.Organization_ApproveUnitID] FOREIGN KEY ([ApproveUnitID]) REFERENCES [dbo].[Organization] ([ID]),
    CONSTRAINT [FK_dbo.ManipulationTicket_dbo.RegistrationManipulationTicket_RegistrationManipulationTicketID] FOREIGN KEY ([RegistrationManipulationTicketID]) REFERENCES [dbo].[RegistrationManipulationTicket] ([ID]),
    CONSTRAINT [FK_dbo.ManipulationTicket_dbo.TicketManipulationStatus_TicketStatusValue] FOREIGN KEY ([TicketStatusValue]) REFERENCES [dbo].[TicketManipulationStatus] ([ID]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.ManipulationTicket_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.ManipulationTicket_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_TicketStatusValue]
    ON [dbo].[ManipulationTicket]([TicketStatusValue] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_RegistrationManipulationTicketID]
    ON [dbo].[ManipulationTicket]([RegistrationManipulationTicketID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[ManipulationTicket]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FactoryID]
    ON [dbo].[ManipulationTicket]([FactoryID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[ManipulationTicket]([CreatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ApproveUnitID]
    ON [dbo].[ManipulationTicket]([ApproveUnitID] ASC);

