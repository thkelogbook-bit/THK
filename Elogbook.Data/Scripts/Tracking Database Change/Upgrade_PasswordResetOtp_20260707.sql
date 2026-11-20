-- =============================================
-- CR: Reset Password via Email OTP
-- Tạo bảng PasswordResetOtps và ResetSessionTokens
-- =============================================

-- =============================================
-- 1. Bảng PasswordResetOtps
-- =============================================
IF OBJECT_ID(N'PasswordResetOtps', N'U') IS NOT NULL
BEGIN
	DROP TABLE [dbo].[PasswordResetOtps]
END
GO

CREATE TABLE [dbo].[PasswordResetOtps] (
	[ID]              INT              IDENTITY (1, 1) NOT NULL,
	[UserID]          INT              NOT NULL,
	[OtpHash]         VARCHAR (64)     NOT NULL,
	[ExpiresAt]       DATETIME         NOT NULL,
	[Used]            BIT              NOT NULL CONSTRAINT [DF_PasswordResetOtps_Used] DEFAULT (0),
	[Attempts]        SMALLINT         NOT NULL CONSTRAINT [DF_PasswordResetOtps_Attempts] DEFAULT (0),
	[IpAddress]       VARCHAR (45)     NULL,
	[UserAgent]       NVARCHAR (512)   NULL,
	[IsDeleted]       BIT              NOT NULL CONSTRAINT [DF_PasswordResetOtps_IsDeleted] DEFAULT (0),
	[CreatedUserID]   INT              NULL,
	[InsertedAt]      DATETIME         NOT NULL CONSTRAINT [DF_PasswordResetOtps_InsertedAt] DEFAULT (GETDATE()),
	[LastUpdatedUserID] INT            NULL,
	[UpdatedAt]       DATETIME         NOT NULL CONSTRAINT [DF_PasswordResetOtps_UpdatedAt] DEFAULT (GETDATE()),
	CONSTRAINT [PK_dbo.PasswordResetOtps] PRIMARY KEY CLUSTERED ([ID] ASC)
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PasswordResetOtps] WITH CHECK ADD CONSTRAINT [FK_dbo.PasswordResetOtps_dbo.Users_UserID] FOREIGN KEY ([UserID])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[PasswordResetOtps] CHECK CONSTRAINT [FK_dbo.PasswordResetOtps_dbo.Users_UserID]
GO

ALTER TABLE [dbo].[PasswordResetOtps] WITH CHECK ADD CONSTRAINT [FK_dbo.PasswordResetOtps_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[PasswordResetOtps] CHECK CONSTRAINT [FK_dbo.PasswordResetOtps_dbo.Users_CreatedUserID]
GO

ALTER TABLE [dbo].[PasswordResetOtps] WITH CHECK ADD CONSTRAINT [FK_dbo.PasswordResetOtps_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[PasswordResetOtps] CHECK CONSTRAINT [FK_dbo.PasswordResetOtps_dbo.Users_LastUpdatedUserID]
GO

CREATE NONCLUSTERED INDEX [IX_PasswordResetOtps_UserID]
	ON [dbo].[PasswordResetOtps] ([UserID] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_PasswordResetOtps_ExpiresAt]
	ON [dbo].[PasswordResetOtps] ([ExpiresAt] ASC)
GO

-- =============================================
-- 2. Bảng ResetSessionTokens
-- =============================================
IF OBJECT_ID(N'ResetSessionTokens', N'U') IS NOT NULL
BEGIN
	DROP TABLE [dbo].[ResetSessionTokens]
END
GO

CREATE TABLE [dbo].[ResetSessionTokens] (
	[ID]              INT              IDENTITY (1, 1) NOT NULL,
	[UserID]          INT              NOT NULL,
	[TokenHash]       VARCHAR (64)     NOT NULL,
	[ExpiresAt]       DATETIME         NOT NULL,
	[Used]            BIT              NOT NULL CONSTRAINT [DF_ResetSessionTokens_Used] DEFAULT (0),
	[IsDeleted]       BIT              NOT NULL CONSTRAINT [DF_ResetSessionTokens_IsDeleted] DEFAULT (0),
	[CreatedUserID]   INT              NULL,
	[InsertedAt]      DATETIME         NOT NULL CONSTRAINT [DF_ResetSessionTokens_InsertedAt] DEFAULT (GETDATE()),
	[LastUpdatedUserID] INT            NULL,
	[UpdatedAt]       DATETIME         NOT NULL CONSTRAINT [DF_ResetSessionTokens_UpdatedAt] DEFAULT (GETDATE()),
	CONSTRAINT [PK_dbo.ResetSessionTokens] PRIMARY KEY CLUSTERED ([ID] ASC)
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ResetSessionTokens] WITH CHECK ADD CONSTRAINT [FK_dbo.ResetSessionTokens_dbo.Users_UserID] FOREIGN KEY ([UserID])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[ResetSessionTokens] CHECK CONSTRAINT [FK_dbo.ResetSessionTokens_dbo.Users_UserID]
GO

ALTER TABLE [dbo].[ResetSessionTokens] WITH CHECK ADD CONSTRAINT [FK_dbo.ResetSessionTokens_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[ResetSessionTokens] CHECK CONSTRAINT [FK_dbo.ResetSessionTokens_dbo.Users_CreatedUserID]
GO

ALTER TABLE [dbo].[ResetSessionTokens] WITH CHECK ADD CONSTRAINT [FK_dbo.ResetSessionTokens_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[ResetSessionTokens] CHECK CONSTRAINT [FK_dbo.ResetSessionTokens_dbo.Users_LastUpdatedUserID]
GO

CREATE NONCLUSTERED INDEX [IX_ResetSessionTokens_TokenHash]
	ON [dbo].[ResetSessionTokens] ([TokenHash] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_ResetSessionTokens_UserID]
	ON [dbo].[ResetSessionTokens] ([UserID] ASC)
GO
