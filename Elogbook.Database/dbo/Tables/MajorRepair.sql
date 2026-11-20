CREATE TABLE [dbo].[MajorRepair] (
    [ID]                   INT            IDENTITY (1, 1) NOT NULL,
    [FactoryID]            INT            NOT NULL,
    [Title]                NVARCHAR (250) NOT NULL,
    [MajorRepairStatusID]  INT            NULL,
    [CreatedDate]          DATETIME       NOT NULL,
    [StartDate]            DATETIME       NOT NULL,
    [EndDate]              DATETIME       NOT NULL,
    [PlanCreateTicketTime] DATETIME       NOT NULL,
    [AttachmentID]         INT            NOT NULL,
    [AttachmentIds]        VARCHAR (500)  NULL,
    [TotalTicket]          INT            NULL,
    [TotalSuccess]         INT            NULL,
    [IsDeleted]            BIT            NOT NULL,
    [CreatedUserID]        INT            NULL,
    [InsertedAt]           DATETIME       NOT NULL,
    [LastUpdatedUserID]    INT            NULL,
    [UpdatedAt]            DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.MajorRepair] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.MajorRepair_dbo.Attachment_AttachmentID] FOREIGN KEY ([AttachmentID]) REFERENCES [dbo].[Attachment] ([ID]),
    CONSTRAINT [FK_dbo.MajorRepair_dbo.Factory_FactoryID] FOREIGN KEY ([FactoryID]) REFERENCES [dbo].[Factory] ([ID]),
    CONSTRAINT [FK_dbo.MajorRepair_dbo.MajorRepairStatus_MajorRepairStatusID] FOREIGN KEY ([MajorRepairStatusID]) REFERENCES [dbo].[MajorRepairStatus] ([ID]),
    CONSTRAINT [FK_dbo.MajorRepair_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.MajorRepair_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);











GO


GO


GO


GO



GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[MajorRepair]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FactoryID]
    ON [dbo].[MajorRepair]([FactoryID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[MajorRepair]([CreatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AttachmentID]
    ON [dbo].[MajorRepair]([AttachmentID] ASC);

