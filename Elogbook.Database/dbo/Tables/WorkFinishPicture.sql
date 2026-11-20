CREATE TABLE [dbo].[WorkFinishPicture] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [WorkFinishID]      INT            NULL,
    [Attachments]       NVARCHAR (500) NULL,
    [Position]          INT            NULL,
    [IsChecked]         BIT            NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.WorkFinishPicture] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.WorkFinishPicture_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.WorkFinishPicture_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.WorkFinishPicture_dbo.WorkFinish_WorkFinishID] FOREIGN KEY ([WorkFinishID]) REFERENCES [dbo].[WorkFinish] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_WorkFinishID]
    ON [dbo].[WorkFinishPicture]([WorkFinishID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[WorkFinishPicture]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[WorkFinishPicture]([CreatedUserID] ASC);

