CREATE TABLE [dbo].[WarnAndSafe] (
    [ID]                   INT            IDENTITY (1, 1) NOT NULL,
    [WorkTicketID]         INT            NOT NULL,
    [WarningContent]       NVARCHAR (MAX) NOT NULL,
    [SafetyMeasureContent] NVARCHAR (MAX) NOT NULL,
    [IsDeleted]            BIT            NOT NULL,
    [CreatedUserID]        INT            NULL,
    [InsertedAt]           DATETIME       NOT NULL,
    [LastUpdatedUserID]    INT            NULL,
    [UpdatedAt]            DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.WarnAndSafe] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.WarnAndSafe_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.WarnAndSafe_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.WarnAndSafe_dbo.WorkTicket_WorkTicketID] FOREIGN KEY ([WorkTicketID]) REFERENCES [dbo].[WorkTicket] ([ID]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_WorkTicketID]
    ON [dbo].[WarnAndSafe]([WorkTicketID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[WarnAndSafe]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[WarnAndSafe]([CreatedUserID] ASC);

