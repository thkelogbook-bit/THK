CREATE TABLE [dbo].[WorkerInformation] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [WorkTicketID]      INT            NOT NULL,
    [FullName]          NVARCHAR (100) NOT NULL,
    [CheckInTime]       DATETIME       NOT NULL,
    [CheckInSignature]  INT            NULL,
    [CheckOutTime]      DATETIME       NOT NULL,
    [CheckOutSignature] INT            NULL,
    [UserName]          NVARCHAR (100) NULL,
    [IsExternal]        BIT            NOT NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.WorkerInformation] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.WorkerInformation_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.WorkerInformation_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.WorkerInformation_dbo.WorkTicket_WorkTicketID] FOREIGN KEY ([WorkTicketID]) REFERENCES [dbo].[WorkTicket] ([ID]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_WorkTicketID]
    ON [dbo].[WorkerInformation]([WorkTicketID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[WorkerInformation]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[WorkerInformation]([CreatedUserID] ASC);

