CREATE TABLE [dbo].[WorkTicket] (
    [ID]                   INT            IDENTITY (1, 1) NOT NULL,
    [ScreenType]           INT            NULL,
    [WorkTicketNumber]     NVARCHAR (100) NOT NULL,
    [RegistrationTicketID] INT            NULL,
    [FactoryID]            INT            NULL,
    [ApprovedUnitID]       INT            NULL,
    [ApprovedDate]         DATETIME       NULL,
    [WorkTicketType]       INT            NOT NULL,
    [WorkTicketStatus]     INT            NOT NULL,
    [UnitNumber]           INT            NULL,
    [WorkPlacement]        NVARCHAR (MAX) NULL,
    [WorkContent]          NVARCHAR (MAX) NULL,
    [WorkScope]            NVARCHAR (MAX) NULL,
    [StartDate]            DATETIME       NOT NULL,
    [EndDate]              DATETIME       NOT NULL,
    [WorkCondition]        NVARCHAR (MAX) NULL,
    [Attachments]          NVARCHAR (500) NULL,
    [WorkPictures]         NVARCHAR (MAX) NULL,
    [WorkResult]           NVARCHAR (MAX) NULL,
    [WorkTicketFeatureID]  INT            NULL,
    [IsFlagNote]           BIT            NOT NULL,
    [TicketPdfID]          INT            NULL,
    [TrackingStatusID]     INT            NULL,
    [IsDeleted]            BIT            NOT NULL,
    [CreatedUserID]        INT            NULL,
    [InsertedAt]           DATETIME       NOT NULL,
    [LastUpdatedUserID]    INT            NULL,
    [UpdatedAt]            DATETIME       NOT NULL,
    [MajorRepairID]        INT            NULL,
    [RequestTicketID]      INT            NULL,
    CONSTRAINT [PK_dbo.WorkTicket] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK__WorkTicke__Major__4F87BD05] FOREIGN KEY ([MajorRepairID]) REFERENCES [dbo].[MajorRepair] ([ID]),
    CONSTRAINT [FK_dbo.WorkTicket_dbo.Factory_FactoryID] FOREIGN KEY ([FactoryID]) REFERENCES [dbo].[Factory] ([ID]),
    CONSTRAINT [FK_dbo.WorkTicket_dbo.Organization_ApprovedUnitID] FOREIGN KEY ([ApprovedUnitID]) REFERENCES [dbo].[Organization] ([ID]),
    CONSTRAINT [FK_dbo.WorkTicket_dbo.RegistrationTicket_RegistrationTicketID] FOREIGN KEY ([RegistrationTicketID]) REFERENCES [dbo].[RegistrationTicket] ([ID]),
    CONSTRAINT [FK_dbo.WorkTicket_dbo.TicketStatus_WorkTicketStatus] FOREIGN KEY ([WorkTicketStatus]) REFERENCES [dbo].[TicketStatus] ([ID]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.WorkTicket_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.WorkTicket_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.WorkTicket_dbo.WorkTicketFeatures_WorkTicketFeatureID] FOREIGN KEY ([WorkTicketFeatureID]) REFERENCES [dbo].[WorkTicketFeatures] ([ID])
);
















GO
CREATE NONCLUSTERED INDEX [IX_WorkTicketStatus]
    ON [dbo].[WorkTicket]([WorkTicketStatus] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_WorkTicketFeatureID]
    ON [dbo].[WorkTicket]([WorkTicketFeatureID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_RegistrationTicketID]
    ON [dbo].[WorkTicket]([RegistrationTicketID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[WorkTicket]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FactoryID]
    ON [dbo].[WorkTicket]([FactoryID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[WorkTicket]([CreatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ApprovedUnitID]
    ON [dbo].[WorkTicket]([ApprovedUnitID] ASC);

