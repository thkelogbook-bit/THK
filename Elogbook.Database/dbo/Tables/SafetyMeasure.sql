CREATE TABLE [dbo].[SafetyMeasure] (
    [ID]                 INT            IDENTITY (1, 1) NOT NULL,
    [ReceiveWorkplaceID] INT            NULL,
    [Content]            NVARCHAR (MAX) NULL,
    [Position]           INT            NULL,
    [Note]               NVARCHAR (MAX) NULL,
    [IsChecked]          BIT            NULL,
    [Attachments]        NVARCHAR (500) NULL,
    [IsDeleted]          BIT            NOT NULL,
    [CreatedUserID]      INT            NULL,
    [InsertedAt]         DATETIME       NOT NULL,
    [LastUpdatedUserID]  INT            NULL,
    [UpdatedAt]          DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.SafetyMeasure] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.SafetyMeasure_dbo.ReceiveWorkplace_ReceiveWorkplaceID] FOREIGN KEY ([ReceiveWorkplaceID]) REFERENCES [dbo].[ReceiveWorkplace] ([ID]),
    CONSTRAINT [FK_dbo.SafetyMeasure_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.SafetyMeasure_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_ReceiveWorkplaceID]
    ON [dbo].[SafetyMeasure]([ReceiveWorkplaceID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[SafetyMeasure]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[SafetyMeasure]([CreatedUserID] ASC);

