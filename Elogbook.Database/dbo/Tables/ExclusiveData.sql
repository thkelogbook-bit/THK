CREATE TABLE [dbo].[ExclusiveData] (
    [ID]                INT           IDENTITY (1, 1) NOT NULL,
    [TicketNumber]      INT           NOT NULL,
    [TicketType]        NVARCHAR (50) NULL,
    [IsDeleted]         BIT           NOT NULL,
    [CreatedUserID]     INT           NULL,
    [InsertedAt]        DATETIME      NOT NULL,
    [LastUpdatedUserID] INT           NULL,
    [UpdatedAt]         DATETIME      NOT NULL,
    CONSTRAINT [PK_dbo.ExclusiveData] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.ExclusiveData_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.ExclusiveData_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[ExclusiveData]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[ExclusiveData]([CreatedUserID] ASC);

