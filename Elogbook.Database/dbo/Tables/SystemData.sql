CREATE TABLE [dbo].[SystemData] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [DataType]          INT            NOT NULL,
    [Description]       NVARCHAR (500) NOT NULL,
    [Position]          INT            NOT NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedUserID]     INT            NULL,
    [InsertedAt]        DATETIME       NOT NULL,
    [LastUpdatedUserID] INT            NULL,
    [UpdatedAt]         DATETIME       NOT NULL,
    [IsSyncDataMobile]  BIT            CONSTRAINT [DF__SystemDat__IsSyn__4DE98D56] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_dbo.SystemData] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.SystemData_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.SystemData_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);





