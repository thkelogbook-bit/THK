CREATE TABLE [dbo].[WorkPlacement] (
    [ID]                    INT            IDENTITY (1, 1) NOT NULL,
    [WorkTicketID]          INT            NOT NULL,
    [Place]                 NVARCHAR (MAX) NULL,
    [StartTime]             DATETIME       NULL,
    [EndTime]               DATETIME       NULL,
    [CommanderUserID]       INT            NULL,
    [CommanderName]         NVARCHAR (100) NULL,
    [CommanderSignature]    INT            NULL,
    [CommanderSignatureOut] INT            NULL,
    [AcceptorUserID]        INT            NULL,
    [AcceptorName]          NVARCHAR (100) NULL,
    [AcceptorStartTime]     DATETIME       NULL,
    [AcceptorEndTime]       DATETIME       NULL,
    [AcceptorSignature]     INT            NULL,
    [AcceptorSignatureOut]  INT            NULL,
    [IsDeleted]             BIT            NOT NULL,
    [CreatedUserID]         INT            NULL,
    [InsertedAt]            DATETIME       NOT NULL,
    [LastUpdatedUserID]     INT            NULL,
    [UpdatedAt]             DATETIME       NOT NULL,
    [AcceptorNameOut]       NVARCHAR (100) NULL,
    [AcceptorUserIDOut]     INT            NULL,
    CONSTRAINT [PK_dbo.WorkPlacement] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.WorkPlacement_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.WorkPlacement_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.WorkPlacement_dbo.WorkTicket_WorkTicketID] FOREIGN KEY ([WorkTicketID]) REFERENCES [dbo].[WorkTicket] ([ID]) ON DELETE CASCADE
);










GO
CREATE NONCLUSTERED INDEX [IX_WorkTicketID]
    ON [dbo].[WorkPlacement]([WorkTicketID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[WorkPlacement]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[WorkPlacement]([CreatedUserID] ASC);

