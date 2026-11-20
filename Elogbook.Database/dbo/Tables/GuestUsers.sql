CREATE TABLE [dbo].[GuestUsers] (
    [ID]                       INT            IDENTITY (1, 1) NOT NULL,
    [UserName]                 NVARCHAR (255) NULL,
    [FullName]                 NVARCHAR (255) COLLATE SQL_Latin1_General_CP1_CI_AI NOT NULL,
    [DateOfBirth]              DATETIME       NULL,
    [NationalID]               NVARCHAR (50)  NULL,
    [NationalAttachmentID]     INT            NULL,
    [Notes]                    NVARCHAR (MAX) NULL,
    [ElectricRankCard]         NVARCHAR (255) NULL,
    [ElectricRank]             NVARCHAR (10)  NULL,
    [ElectricRankAttachmentID] INT            NULL,
    [LaborCard]                NVARCHAR (255) NULL,
    [LaborRank]                NVARCHAR (10)  NULL,
    [LaborCardAttachmentID]    INT            NULL,
    [IsDeleted]                BIT            NOT NULL,
    [CreatedUserID]            INT            NULL,
    [InsertedAt]               DATETIME       NOT NULL,
    [LastUpdatedUserID]        INT            NULL,
    [UpdatedAt]                DATETIME       NOT NULL,
    CONSTRAINT [PK_dbo.GuestUsers] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.GuestUsers_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
    CONSTRAINT [FK_dbo.GuestUsers_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
);






GO
CREATE NONCLUSTERED INDEX [IX_LastUpdatedUserID]
    ON [dbo].[GuestUsers]([LastUpdatedUserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedUserID]
    ON [dbo].[GuestUsers]([CreatedUserID] ASC);

