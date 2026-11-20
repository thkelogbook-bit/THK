CREATE TABLE [dbo].[CycleManagement] (
    [ID]                INT             IDENTITY (1, 1) NOT NULL,
    [CycleName]         NVARCHAR (255)  NOT NULL,
    [Position]          INT             NOT NULL,
    [TimeLine]          NVARCHAR (1000) NULL,
    [OpenInputBefore]   INT             NOT NULL,
    [BlockInputAfter]   INT             NOT NULL,
    [Active]            INT             NOT NULL,
    [IsDeleted]         BIT             NOT NULL,
    [CreatedUserID]     INT             NULL,
    [InsertedAt]        DATETIME        NOT NULL,
    [LastUpdatedUserID] INT             NULL,
    [UpdatedAt]         DATETIME        NOT NULL,
    CONSTRAINT [PK_dbo.CycleManagement] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.CycleManagement_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.CycleManagement_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[CycleManagement]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[CycleManagement]([CreatedUserID] ASC);

