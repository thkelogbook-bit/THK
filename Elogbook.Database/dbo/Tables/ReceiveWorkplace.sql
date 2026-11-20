CREATE TABLE [dbo].[ReceiveWorkplace] (
    [ID]                  INT            IDENTITY (1, 1) NOT NULL,
    [WorkTicketID]        INT            NULL,
    [WorkUnitName]        NVARCHAR (255) NULL,
    [WorkUnitID]          INT            NULL,
    [AmountPersons]       NVARCHAR (255) NULL,
    [StartWorkingDate]    DATETIME       NOT NULL,
    [LeaderSignature]     INT            NULL,
    [CommanderSignature]  INT            NULL,
    [SupervisorSignature] INT            NULL,
    [IsDeleted]           BIT            NOT NULL,
    [CreatedUserID]       INT            NULL,
    [InsertedAt]          DATETIME       NOT NULL,
    [LastUpdatedUserID]   INT            NULL,
    [UpdatedAt]           DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.ReceiveWorkplace] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.ReceiveWorkplace_dbo.Organization_WorkUnitID] FOREIGN KEY ([WorkUnitID]) REFERENCES [dbo].[Organization] ([ID]),
    CONSTRAINT [FK_dbo.ReceiveWorkplace_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.ReceiveWorkplace_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.ReceiveWorkplace_dbo.WorkTicket_WorkTicketID] FOREIGN KEY ([WorkTicketID]) REFERENCES [dbo].[WorkTicket] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_WorkUnitID]
    ON [dbo].[ReceiveWorkplace]([WorkUnitID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_WorkTicketID]
    ON [dbo].[ReceiveWorkplace]([WorkTicketID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[ReceiveWorkplace]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[ReceiveWorkplace]([CreatedUserID] ASC);

