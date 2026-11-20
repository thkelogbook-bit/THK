CREATE TABLE [dbo].[Stakeholders] (
    [ID]                   INT            IDENTITY (1, 1) NOT NULL,
    [AssignToUserID]       INT            NULL,
    [FullName]             NVARCHAR (100) COLLATE SQL_Latin1_General_CP1_CI_AI NULL,
    [IsExternal]           BIT            NOT NULL,
    [UserName]             NVARCHAR (100) NULL,
    [ElectricRank]         NVARCHAR (10)  NULL,
    [ElectricRankCard]     NVARCHAR (255) NULL,
    [Roles]                INT            NOT NULL,
    [PositionName]         NVARCHAR (500) NULL,
    [Mission]              NVARCHAR (MAX) NULL,
    [RegistrationTicketID] INT            NULL,
    [ContentPositionID]    INT            NULL,
    [JobRank]              NVARCHAR (500) NULL,
    [Location]             NVARCHAR (MAX) NULL,
    [IsDeleted]            BIT            NOT NULL,
    [CreatedUserID]        INT            NULL,
    [InsertedAt]           DATETIME       NOT NULL,
    [LastUpdatedUserID]    INT            NULL,
    [UpdatedAt]            DATETIME       NOT NULL,
    [LaborCard]            NVARCHAR (255) NULL,
    [LaborRank]            NVARCHAR (10)  NULL,
    CONSTRAINT [PK_dbo.Stakeholders] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.Stakeholders_dbo.ContentPosition_ContentPositionID] FOREIGN KEY ([ContentPositionID]) REFERENCES [dbo].[ContentPosition] ([ID]),
    CONSTRAINT [FK_dbo.Stakeholders_dbo.RegistrationTicket_RegistrationTicketID] FOREIGN KEY ([RegistrationTicketID]) REFERENCES [dbo].[RegistrationTicket] ([ID]),
    CONSTRAINT [FK_dbo.Stakeholders_dbo.Users_AssignToUserID] FOREIGN KEY ([AssignToUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.Stakeholders_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.Stakeholders_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);






GO
CREATE NONCLUSTERED INDEX [IX_RegistrationTicketID]
    ON [dbo].[Stakeholders]([RegistrationTicketID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[Stakeholders]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[Stakeholders]([CreatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ContentPositionID]
    ON [dbo].[Stakeholders]([ContentPositionID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AssignToUserID]
    ON [dbo].[Stakeholders]([AssignToUserID] ASC);

