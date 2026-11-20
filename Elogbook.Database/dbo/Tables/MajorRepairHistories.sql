CREATE TABLE [dbo].[MajorRepairHistories] (
    [ID]                  INT            IDENTITY (1, 1) NOT NULL,
    [MajorRepairID]       INT            NULL,
    [WorkTicketID]        INT            NULL,
    [WorkTicketNumber]    NVARCHAR (255) NULL,
    [MajorRepairStatusID] INT            NULL,
    [Content]             NVARCHAR (255) NOT NULL,
    [AttachmentID]        INT            NULL,
    [IsDeleted]           BIT            NOT NULL,
    [CreatedUserID]       INT            NULL,
    [InsertedAt]          DATETIME       NOT NULL,
    [LastUpdatedUserID]   INT            NULL,
    [UpdatedAt]           DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.MajorRepairHistories] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.MajorRepairHistories_dbo.Attachment_AttachmentID] FOREIGN KEY ([AttachmentID]) REFERENCES [dbo].[Attachment] ([ID]),
    CONSTRAINT [FK_dbo.MajorRepairHistories_dbo.MajorRepair_MajorRepairID] FOREIGN KEY ([MajorRepairID]) REFERENCES [dbo].[MajorRepair] ([ID]),
    CONSTRAINT [FK_dbo.MajorRepairHistories_dbo.MajorRepairStatus_MajorRepairStatusID] FOREIGN KEY ([MajorRepairStatusID]) REFERENCES [dbo].[MajorRepairStatus] ([ID]),
    CONSTRAINT [FK_dbo.MajorRepairHistories_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.MajorRepairHistories_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.MajorRepairHistories_dbo.WorkTicket_WorkTicketID] FOREIGN KEY ([WorkTicketID]) REFERENCES [dbo].[WorkTicket] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_WorkTicketID]
    ON [dbo].[MajorRepairHistories]([WorkTicketID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_MajorRepairStatusID]
    ON [dbo].[MajorRepairHistories]([MajorRepairStatusID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_MajorRepairID]
    ON [dbo].[MajorRepairHistories]([MajorRepairID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[MajorRepairHistories]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[MajorRepairHistories]([CreatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AttachmentID]
    ON [dbo].[MajorRepairHistories]([AttachmentID] ASC);

