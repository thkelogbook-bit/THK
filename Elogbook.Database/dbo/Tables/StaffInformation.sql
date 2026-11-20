CREATE TABLE [dbo].[StaffInformation] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [WorkDailyID]       INT            NOT NULL,
    [FullName]          NVARCHAR (100) NOT NULL,
    [UserName]          NVARCHAR (100) NOT NULL,
    [IsExternal]        BIT            NULL,
    [IsDirectCommander] BIT            CONSTRAINT [DF__StaffInfo__IsDir__37C5420D] DEFAULT ((0)) NOT NULL,
    [CommanderUserID]   INT            CONSTRAINT [DF__StaffInfo__Comma__38B96646] DEFAULT ((0)) NOT NULL,
    [Mission]           NVARCHAR (MAX) NULL,
    [PositionName]      NVARCHAR (255) NULL,
    [ElectricRankCard]  NVARCHAR (MAX) NULL,
    [CheckInTime]       NVARCHAR (100) NULL,
    [CheckInSignature]  INT            NULL,
    [CheckOutTime]      NVARCHAR (100) NULL,
    [CheckOutSignature] INT            NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.StaffInformation] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.StaffInformation_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.StaffInformation_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.StaffInformation_dbo.WorkDaily_WorkDailyID] FOREIGN KEY ([WorkDailyID]) REFERENCES [dbo].[WorkDaily] ([ID]) ON DELETE CASCADE
);










GO
CREATE NONCLUSTERED INDEX [IX_WorkDailyID]
    ON [dbo].[StaffInformation]([WorkDailyID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[StaffInformation]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[StaffInformation]([CreatedUserID] ASC);

