USE Elogbook

IF OBJECT_ID (N'SigningMethod', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[SigningMethod] (
		[ID]                INT            IDENTITY (1, 1) NOT NULL,
		[SigningMethodName] NVARCHAR (255) NOT NULL,
		[Active]            INT            NOT NULL,
		[IsDeleted]         BIT            NOT NULL,
		[CreatedUserID]     INT            NULL,
		[InsertedAt]        DATETIME       NOT NULL,
		[LastUpdatedUserID] INT            NULL,
		[UpdatedAt]         DATETIME       NOT NULL,
		CONSTRAINT [PK_dbo.SigningMethod] PRIMARY KEY CLUSTERED ([ID] ASC),
		CONSTRAINT [FK_dbo.SigningMethod_dbo.Users_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [dbo].[Users] ([ID]),
		CONSTRAINT [FK_dbo.SigningMethod_dbo.Users_LastUpdatedUserID] FOREIGN KEY ([LastUpdatedUserID]) REFERENCES [dbo].[Users] ([ID])
	);
END
GO

IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'Users'
                 AND COLUMN_NAME = 'AvatarAttachmentID')
BEGIN
	ALTER TABLE dbo.Users ADD [AvatarAttachmentID] [int] NULL
END

GO
IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'Users'
                 AND COLUMN_NAME = 'SigningMethodID')
BEGIN
	ALTER TABLE dbo.Users ADD [SigningMethodID] [int] NOT NULL DEFAULT(2)
END

GO
TRUNCATE TABLE SigningMethod
SET IDENTITY_INSERT [dbo].[SigningMethod] ON 
INSERT [dbo].[SigningMethod] ([ID], [SigningMethodName], [Active], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) VALUES (1, N'Tài khoản người dùng', 1, 0, NULL, CAST(N'2023-03-03T23:59:52.277' AS DateTime), NULL, CAST(N'2023-03-03T23:59:52.277' AS DateTime))
INSERT [dbo].[SigningMethod] ([ID], [SigningMethodName], [Active], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) VALUES (2, N'VIETTEL - CA', 1, 0, NULL, CAST(N'2023-03-03T23:59:52.277' AS DateTime), NULL, CAST(N'2023-03-03T23:59:52.277' AS DateTime))
INSERT [dbo].[SigningMethod] ([ID], [SigningMethodName], [Active], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) VALUES (3, N'MOBILE PHONE - CA', 0, 0, NULL, CAST(N'2023-03-03T23:59:52.277' AS DateTime), NULL, CAST(N'2023-03-03T23:59:52.277' AS DateTime))
INSERT [dbo].[SigningMethod] ([ID], [SigningMethodName], [Active], [IsDeleted], [CreatedUserID], [InsertedAt], [LastUpdatedUserID], [UpdatedAt]) VALUES (4, N'VINA PHONE - CA', 0, 0, NULL, CAST(N'2023-03-03T23:59:52.277' AS DateTime), NULL, CAST(N'2023-03-03T23:59:52.277' AS DateTime))
SET IDENTITY_INSERT [dbo].[SigningMethod] OFF
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('TSVH_GetParameters'))
	DROP PROC [dbo].[TSVH_GetParameters]
GO
-- =============================================
-- Lấy danh sách ghi thông số theo điều kiện
-- Fix issue performance
-- =============================================
--TEST: EXECUTE [dbo].[TSVH_GetParameters] 1, 2, 3, 2, 45672, 1, 10
CREATE PROCEDURE [dbo].[TSVH_GetParameters]
	@FactoryID INT = 0,
	@CycleID INT = 0,
	@ParameterGroupID INT = 0,
	@OperationPositionID INT = 0,
	@OperationTimeLineID INT = 0,
	@PageNumber INT = 1,
	@PageSize INT = 10
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @IsUseOperationParameterGroup BIT = (SELECT TOP 1 IsUseOperationParameterGroup FROM dbo.Factory WITH(NOLOCK) WHERE ID = @FactoryID)
	IF(@ParameterGroupID IS NULL) SET @ParameterGroupID = 0
	IF(@IsUseOperationParameterGroup IS NULL) SET @IsUseOperationParameterGroup = 0

	DECLARE @TempData TABLE(
		[RowIndex] INT IDENTITY(1,1),
		[ID] INT,
		[IsGroup] BIT,
		[ParameterID] INT,
		[ParameterName] NVARCHAR(255),
		[KKSCode] NVARCHAR(255),
		[UnitMeasure] NVARCHAR(255),
		[PreviousValue] NVARCHAR(MAX),
		[CurrentValue] NVARCHAR(MAX),
		[ErrorCode] INT,
		[FromRange] REAL,
		[ToRange] REAL,
		[IsNumberType] BIT,
		[ParameterGroupID] INT,
		[OprerationTimeLineID] INT,
		[IsChecked] BIT,
		[AlertDetails] NVARCHAR(MAX),
		[CheckedDate] DATETIME,
		[Setting] NVARCHAR(MAX)
	)

	INSERT INTO @TempData
	SELECT
		p.ID,
		0,
		p.ParameterID,
		p.ParameterName,
		p.KKSCode,
		p.UnitMeasure,
		p.PreviousValue,
		p.CurrentValue,
		p.ErrorCode,
		p.FromRange,
		p.ToRange,
		p.IsNumberType,
		p.ParameterGroupID,
		p.OperationTimeLineID,
		p.IsChecked,
		p.AlertDetails,
		p.CheckedDate,
		NULL
	FROM dbo.OperationParameters p WITH(NOLOCK)
	WHERE p.FactoryID = @FactoryID
	AND p.OperationPositionID = @OperationPositionID
	AND p.OperationTimeLineID = @OperationTimeLineID
	AND p.CycleID = @CycleID
	AND p.IsDeleted = 0
	AND ((@IsUseOperationParameterGroup = 1 AND (@ParameterGroupID > 0 AND p.ParameterGroupID = @ParameterGroupID)
		OR (@ParameterGroupID <= 0 AND p.ParameterGroupID IS NULL)) OR @IsUseOperationParameterGroup = 0)

	DECLARE @TotalRecords INT = (SELECT COUNT(ID) FROM @TempData)

	SELECT temp.ID,
		CAST(CASE WHEN @ParameterGroupID > 0 THEN 1 ELSE 0 END AS BIT) AS IsGroup,
		temp.ParameterID,
		temp.ParameterName,
		temp.KKSCode,
		temp.UnitMeasure,
		REVERSE(REPLACE(STUFF((
					SELECT ':' + (CASE WHEN ISNULL(pd.ErrorCode, 0) = 1 THEN 'F' --Thiết bị bị hỏng (F)
										WHEN ISNULL(pd.ErrorCode, 0) = 2 THEN '-' --Dừng thiết bị (-)
										WHEN ISNULL(pd.ErrorCode, 0) = 3 THEN 'S' --Sim thiết bị (S)
										WHEN ISNULL(pd.ErrorCode, 0) = 4 THEN 'J' --Jump thiết bị (J)
										WHEN ISNULL(pd.ErrorCode, 0) = 5 THEN 'x' --Bận công tác
									ELSE ISNULL(pd.CurrentValue, '') END)
					FROM dbo.OperationParameters pd WITH(NOLOCK)
					WHERE pd.FactoryID = @FactoryID
						AND pd.OperationPositionID = @OperationPositionID
						AND pd.OperationTimeLineID < temp.OprerationTimeLineID
						AND pd.CycleID = @CycleID
						AND pd.ParameterID = temp.ParameterID
						AND pd.IsDeleted = 0
					ORDER BY pd.OperationTimeLineID DESC
					OFFSET (1-1) * 5 ROWS
					FETCH NEXT 5 ROWS ONLY
					FOR XML PATH(''), TYPE).value('.', 'VARCHAR(MAX)'), 1, 1, ''), 
			':', ' : ')) AS PreviousValue,
		temp.CurrentValue,
		CASE WHEN temp.ErrorCode = 5 THEN NULL ELSE temp.ErrorCode END AS ErrorCode,
		temp.FromRange,
		temp.ToRange,
		temp.IsNumberType,
		temp.ParameterGroupID,
		temp.OprerationTimeLineID,
		temp.IsChecked,
		temp.AlertDetails,
		temp.CheckedDate,
		REPLACE(STUFF((
					SELECT ',' + ISNULL(Alert,'') + ' ' + ISNULL(Operator,'') + ' ' + ISNULL(CAST(Value AS NVARCHAR(100)),'') + ' : ' + ISNULL(Color,'') + ';'
					FROM dbo.AlertDetails WITH(NOLOCK)
					WHERE ParameterID = temp.ParameterID
					FOR XML PATH(''), TYPE).value('.', 'VARCHAR(MAX)'), 1, 1, ''), 
			';,', ';') AS Setting,
		@TotalRecords AS TotalRecords
	FROM @TempData temp
	ORDER BY temp.ID
	OFFSET (@PageNumber-1) * @PageSize ROWS
	FETCH NEXT @PageSize ROWS ONLY
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('TSVH_AddOperationParameterByTime'))
	DROP PROC [dbo].[TSVH_AddOperationParameterByTime]
GO
-- =============================================
-- Thêm mới danh sách ghi thông số theo chu kỳ
-- Fix issue performance: giảm từ 1 phút còn 3 giây
-- =============================================
CREATE PROCEDURE [dbo].[TSVH_AddOperationParameterByTime]
	@FactoryID INT,
	@OperationPositionID INT,
	@CycleID INT,
	@DateCreate DATE,
	@CurrentUserID INT
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @TotalParamOther INT = 0,
		@CurrentDate DATETIME = GETDATE(),
		@Result NVARCHAR(20) = 'ERROR'

	--Danh sách tất cả thông số theo Nhà máy, Cương vị, Chu kỳ đang sử dụng
	DECLARE @OperationParametersTemp TABLE
	(
		[RowIndex] INT IDENTITY(1,1) NOT NULL,
		[ParameterName] [nvarchar](255) NOT NULL,
		[ParameterID] [int] NOT NULL,
		[ParameterGroupID] [int] NULL,
		[KKSCode] [nvarchar](255) NULL,
		[UnitMeasure] [nvarchar](255) NULL,
		[Setting] [nvarchar](max) NULL,
		[FromRange] [real] NULL,
		[ToRange] [real] NULL,
		[IsNumberType] [bit] NOT NULL,
		[AlertDetails] [nvarchar](max) NULL
	)
	INSERT INTO @OperationParametersTemp
	SELECT
		p.ParameterName,
		p.ID,
		p.ParameterGroupID,
		p.KKSCode,
		p.UnitMeasure,
		p.Setting,
		p.FromRange,
		p.ToRange,
		p.IsNumberType,
		REPLACE(STUFF((
				SELECT ',' + ISNULL(Alert,'') + ' ' + ISNULL(Operator,'') + ' ' + ISNULL(CAST(Value AS NVARCHAR(100)),'') + ' : ' + ISNULL(Color,'') + ';'
				FROM dbo.AlertDetails WITH(NOLOCK)
				WHERE ParameterID = p.ID
				FOR XML PATH(''), TYPE).value('.', 'VARCHAR(MAX)'), 1, 1, ''), 
		';,', ';') AS AlertDetails
	FROM dbo.Parameters p WITH(NOLOCK)
	WHERE p.FactoryID = @FactoryID
		AND p.OperationPositionID = @OperationPositionID
		AND p.CycleID = @CycleID
		AND p.Status = 1
		AND p.IsDeleted = 0
	ORDER BY p.Position ASC, p.ID ASC

	DECLARE @OperationTimeLineTemp TABLE
	(
		RowIndex INT IDENTITY(1,1), 
		OperationTimeLineID INT, 
		TimeLineSyncDataKey NVARCHAR(100)
	)
	SET @TotalParamOther = (SELECT COUNT(ParameterID) FROM @OperationParametersTemp)
	--Tạo các khung giờ để tạo dữ liệu ghi thông số vận hành
	IF(ISNULL(@TotalParamOther, 0) > 0 AND EXISTS(SELECT TOP 1 ID FROM dbo.TimeLineCycle WITH(NOLOCK) WHERE CycleID = @CycleID AND Active = 1 AND IsDeleted = 0))
	BEGIN
		INSERT INTO dbo.OperationTimeLine
		(
			[TimeLineSyncDataKey], 
			[TimeLineName], 
			[FactoryID], 
			[OperationPositionID], 
			[CycleID], 
			[TotalParametersDefault], 
			[TotalParametersChecked], 
			[TotalParametersSynchronized], 
			[IsDeleted], 
			[CreatedUserID], 
			[InsertedAt], 
			[LastUpdatedUserID], 
			[UpdatedAt], 
			[AppID], 
			[SyncedDate]
		)
		OUTPUT INSERTED.[ID], INSERTED.[TimeLineSyncDataKey]
		INTO @OperationTimeLineTemp
		(
			OperationTimeLineID,
			TimeLineSyncDataKey
		)
		SELECT 
			CAST(Hour AS VARCHAR(20)) + '_' + Replace(CONVERT(VARCHAR(20), @DateCreate, 103),'/','') + '_' + 
				CAST(@FactoryID AS VARCHAR(20)) + '_' + CAST(@OperationPositionID AS VARCHAR(20)) + '_' + CAST(@CycleID AS VARCHAR(20)), --[TimeLineSyncDataKey]
			Hour, --[TimeLineName]
			@FactoryID, --[FactoryID]
			@OperationPositionID, --[OperationPositionID]
			@CycleID, --[CycleID]
			@TotalParamOther, --[TotalParametersDefault]
			0, --[TotalParametersChecked]
			0, --[TotalParametersSynchronized]
			0, --[IsDeleted]
			@CurrentUserID, --[CreatedUserID]
			@DateCreate, --[InsertedAt]
			@CurrentUserID, --[LastUpdatedUserID]
			@CurrentDate, --[UpdatedAt]
			NULL, --[AppID]
			@CurrentDate --[SyncedDate]
		FROM dbo.TimeLineCycle WITH(NOLOCK)
		WHERE CycleID = @CycleID AND Active = 1 AND IsDeleted = 0
		ORDER BY Hour ASC

		DECLARE @TotalTimeLine INT = (SELECT COUNT(OperationTimeLineID) OperationTimeLineID FROM @OperationTimeLineTemp),
			@TimeLineIndex INT = 1,
			@OperationTimeLineID INT,
			@TimeLineSyncDataKey NVARCHAR(100)

		IF(ISNULL(@TotalTimeLine, 0) > 0)
		BEGIN
			--Duyệt qua các khung giờ để tạo danh sách thông số
			WHILE(@TimeLineIndex <= @TotalTimeLine)
			BEGIN
				SELECT @OperationTimeLineID = OperationTimeLineID, @TimeLineSyncDataKey = TimeLineSyncDataKey FROM @OperationTimeLineTemp WHERE RowIndex = @TimeLineIndex
				IF(ISNULL(@OperationTimeLineID, 0) > 0 AND ISNULL(@TimeLineSyncDataKey, '') != '')
				BEGIN
					INSERT INTO dbo.OperationParameters
					(
						[ParameterName], 
						[ParameterID], 
						[ParameterGroupID], 
						[OperationTimeLineID], 
						[FactoryID], 
						[OperationPositionID], 
						[CycleID], 
						[CurrentValue], 
						[KKSCode], 
						[UnitMeasure], 
						[Setting], 
						[FromRange], 
						[ToRange], 
						[IsNumberType], 
						[PreviousValue], 
						[ErrorCode], 
						[IsChecked], 
						[AlertDetails], 
						[IsDeleted], 
						[CreatedUserID], 
						[InsertedAt], 
						[LastUpdatedUserID], 
						[UpdatedAt], 
						[SyncDataKey], 
						[AppID], 
						[CheckedDate], 
						[SyncedDate]
					)
					SELECT
						[ParameterName], 
						[ParameterID], 
						[ParameterGroupID], 
						@OperationTimeLineID, 
						@FactoryID, 
						@OperationPositionID, 
						@CycleID, 
						NULL, 
						[KKSCode], 
						[UnitMeasure], 
						[Setting], 
						[FromRange], 
						[ToRange], 
						[IsNumberType], 
						'', 
						NULL, 
						0, 
						[AlertDetails], 
						0, --[IsDeleted] 
						@CurrentUserID, --[CreatedUserID] 
						@CurrentDate, --[InsertedAt] 
						@CurrentUserID, --[LastUpdatedUserID] 
						@CurrentDate, --[UpdatedAt] 
						@TimeLineSyncDataKey, --[SyncDataKey] 
						NULL, --[AppID] 
						NULL, --[CheckedDate] 
						@CurrentDate --[SyncedDate]
					FROM @OperationParametersTemp
					ORDER BY RowIndex
				END

				SET @OperationTimeLineID = NULL
				SET @TimeLineSyncDataKey = NULL
				SET @TimeLineIndex = @TimeLineIndex + 1
			END

			SET @Result = 'SUCCESS'
		END
	END

	--Kết quả tạo danh sách thông số
	SELECT @Result AS Message
END