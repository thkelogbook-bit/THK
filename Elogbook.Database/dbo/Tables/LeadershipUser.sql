CREATE TABLE [dbo].[LeadershipUser] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [WorkTicketID]      INT            NOT NULL,
    [UserName]          NVARCHAR (100) NULL,
    [FullName]          NVARCHAR (100) COLLATE SQL_Latin1_General_CP1_CI_AI NULL,
    [PositionName]      NVARCHAR (500) NULL,
    [Roles]             INT            NULL,
    [ElectricRank]      NVARCHAR (10)  NULL,
    [ElectricRankCard]  NVARCHAR (255) NULL,
    [Signature]         NVARCHAR (MAX) NULL,
    [IsExternal]        BIT            NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    [LaborCard]         NVARCHAR (255) NULL,
    [LaborRank]         NVARCHAR (10)  NULL,
    CONSTRAINT [PK_dbo.LeadershipUser] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.LeadershipUser_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.LeadershipUser_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.LeadershipUser_dbo.WorkTicket_WorkTicketID] FOREIGN KEY ([WorkTicketID]) REFERENCES [dbo].[WorkTicket] ([ID]) ON DELETE CASCADE
);






GO
CREATE NONCLUSTERED INDEX [IX_WorkTicketID]
    ON [dbo].[LeadershipUser]([WorkTicketID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[LeadershipUser]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[LeadershipUser]([CreatedUserID] ASC);

