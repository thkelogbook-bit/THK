CREATE TABLE [dbo].[WorkFinish] (
    [ID]                             INT            IDENTITY (1, 1) NOT NULL,
    [WorkTicketID]                   INT            NOT NULL,
    [ReturnWorkplaceTime]            DATETIME       NULL,
    [CheckReceivedTime]              DATETIME       NULL,
    [CheckCompletedTime]             DATETIME       NULL,
    [LeaderSignature]                INT            NULL,
    [CommanderSignature]             INT            NULL,
    [AllowedSignature]               INT            NULL,
    [ApproverSignature]              INT            NULL,
    [RepresentativeUnit]             NVARCHAR (255) NULL,
    [RepresentativeUnitPositionName] NVARCHAR (255) NULL,
    [OperatorConfirmed1]             BIT            NULL,
    [OperatorConfirmed2]             BIT            NULL,
    [OperatorConfirmed3]             BIT            NULL,
    [IsDeleted]                      BIT            NOT NULL,
    [CreatedUserID]                  INT            NULL,
    [InsertedAt]                     DATETIME       NOT NULL,
    [LastUpdatedUserID]              INT            NULL,
    [UpdatedAt]                      DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.WorkFinish] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.WorkFinish_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.WorkFinish_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.WorkFinish_dbo.WorkTicket_WorkTicketID] FOREIGN KEY ([WorkTicketID]) REFERENCES [dbo].[WorkTicket] ([ID]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_WorkTicketID]
    ON [dbo].[WorkFinish]([WorkTicketID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[WorkFinish]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[WorkFinish]([CreatedUserID] ASC);

