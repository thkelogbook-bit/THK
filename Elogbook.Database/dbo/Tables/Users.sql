CREATE TABLE [dbo].[Users] (
    [ID]                       INT            IDENTITY (1, 1) NOT NULL,
    [FullName]                 NVARCHAR (255) COLLATE SQL_Latin1_General_CP1_CI_AI NOT NULL,
    [UserName]                 NVARCHAR (255) NOT NULL,
    [Password]                 NVARCHAR (100) NULL,
    [PositionName]             NVARCHAR (255) NULL,
    [DateOfBirth]              DATETIME       NULL,
    [PhoneNumber]              NVARCHAR (30)  NULL,
    [Email]                    NVARCHAR (255) NULL,
    [NationalID]               NVARCHAR (50)  NULL,
    [NationalAttachmentID]     INT            NOT NULL,
    [ElectricRank]             NVARCHAR (10)  NULL,
    [ElectricRankCard]         NVARCHAR (255) NULL,
    [ElectricRankAttachmentID] INT            NOT NULL,
    [LaborCard]                NVARCHAR (255) NULL,
    [LaborRank]                NVARCHAR (10)  NULL,
    [LaborCardAttachmentID]    INT            NOT NULL,
    [Signature]                NVARCHAR (MAX) NULL,
    [CompanyID]                INT            NOT NULL,
    [ProductionPlantID]        INT            NULL,
    [TeamID]                   INT            NULL,
    [GroupID]                  INT            NULL,
    [FactoryID]                INT            NULL,
    [AccountType]              INT            NOT NULL,
    [SourceType]               INT            NOT NULL,
    [Status]                   INT            NOT NULL,
    [IsDeleted]                BIT            NOT NULL,
    [CreatedUserID]            INT            NULL,
    [InsertedAt]               DATETIME       NOT NULL,
    [LastUpdatedUserID]        INT            NULL,
    [UpdatedAt]                DATETIME       NOT NULL,
    [AvatarAttachmentID]       INT            NULL,
    [SigningMethodID]          INT            CONSTRAINT [DF__Users__SigningMe__4924D839] DEFAULT ((2)) NOT NULL,
    [NumberLoginFailed]        INT            CONSTRAINT [DF_Users_NumberLoginFailed] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_dbo.Users] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.Users_dbo.Factory_FactoryID] FOREIGN KEY ([FactoryID]) REFERENCES [dbo].[Factory] ([ID]),
    CONSTRAINT [FK_dbo.Users_dbo.Organization_CompanyID] FOREIGN KEY ([CompanyID]) REFERENCES [dbo].[Organization] ([ID]),
    CONSTRAINT [FK_dbo.Users_dbo.Organization_GroupID] FOREIGN KEY ([GroupID]) REFERENCES [dbo].[Organization] ([ID]),
    CONSTRAINT [FK_dbo.Users_dbo.Organization_ProductionPlantID] FOREIGN KEY ([ProductionPlantID]) REFERENCES [dbo].[Organization] ([ID]),
    CONSTRAINT [FK_dbo.Users_dbo.Organization_TeamID] FOREIGN KEY ([TeamID]) REFERENCES [dbo].[Organization] ([ID])
);
















GO
CREATE NONCLUSTERED INDEX [IX_TeamID]
    ON [dbo].[Users]([TeamID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ProductionPlantID]
    ON [dbo].[Users]([ProductionPlantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_GroupID]
    ON [dbo].[Users]([GroupID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FactoryID]
    ON [dbo].[Users]([FactoryID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CompanyID]
    ON [dbo].[Users]([CompanyID] ASC);

