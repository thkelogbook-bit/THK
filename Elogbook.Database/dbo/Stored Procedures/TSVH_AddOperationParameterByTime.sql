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