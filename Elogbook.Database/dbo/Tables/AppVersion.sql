CREATE TABLE [dbo].[AppVersion] (
    [ID]                         INT            IDENTITY (1, 1) NOT NULL,
    [Version]                    NVARCHAR (50)  NOT NULL,
    [ForceUpdate]                BIT            NOT NULL,
    [IsDeleted]                  BIT            NOT NULL,
    [CreatedUserID]              INT            NULL,
    [InsertedAt]                 DATETIME       NOT NULL,
    [LastUpdatedUserID]          INT            NULL,
    [UpdatedAt]                  DATETIME       NOT NULL,
    [AppTitle]                   NVARCHAR (255) NULL,
    [LogoImage]                  INT            NULL,
    [BackgroundImageBeforeLogin] INT            NULL,
    [BackgroundImageAfterLogin]  INT            NULL,
    [LogoImageMobile]            INT            NULL,
    [BackgroundImageMobile]      INT            NULL,
    [BackgroundLoginImage]       INT            NULL,
    [Copyright]                  NVARCHAR (255) NULL,
    [LogoImageIOSMobile]         INT            NULL,
    [BackgroundImageIOSMobile]   INT            NULL,
    [WebVersion]                 NVARCHAR (100) NULL,
    [LeftMenuLogoIcon]           INT            NULL,
    [IsSyncData_PCT_TDTTC]       BIT            CONSTRAINT [DF__AppVersio__IsSyn__20E1DCB5] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_dbo.AppVersion] PRIMARY KEY CLUSTERED ([ID] ASC)
);





