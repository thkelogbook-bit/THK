-- =============================================
-- Báo bận công tác, Báo dừng máy cho mỗi khung giờ ghi thông số
-- Fix issue performance: current 58s for one time line
-- =============================================
CREATE PROCEDURE [dbo].[TSVH_UpdateOperationTimeLine]
	@FactoryID INT,
	@OperationPositionID INT,
	@CycleID INT,
	@OperationTimeLineIDs NVARCHAR(MAX),
	@ErrorCode INT,
	@CurrentUserID INT
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @TotalParamOther INT = 0,
		@CurrentDate DATETIME = GETDATE(),
		@Result NVARCHAR(20) = 'ERROR',
		@TotalTimeLine INT = 0,
		@TimeLineIndex INT = 1,
		@OperationTimeLineID INT

	DECLARE @OperationTimeLineIDTemp TABLE
	(
		RowIndex INT IDENTITY(1,1), 
		OperationTimeLineID INT
	)
	INSERT INTO @OperationTimeLineIDTemp
	SELECT 
		CAST([value] AS INT) 
	FROM STRING_SPLIT(@OperationTimeLineIDs, ',') temp
	INNER JOIN dbo.OperationTimeLine otl ON temp.[value] = otl.ID

	SET @TotalTimeLine = (SELECT COUNT(OperationTimeLineID) FROM @OperationTimeLineIDTemp)

	IF(ISNULL(@TotalTimeLine, 0) > 0)
	BEGIN
		--Duyệt qua các khung giờ để tạo danh sách thông số
		WHILE(@TimeLineIndex <= @TotalTimeLine)
		BEGIN
			SELECT TOP 1 @OperationTimeLineID = OperationTimeLineID FROM @OperationTimeLineIDTemp WHERE RowIndex = @TimeLineIndex
			IF(ISNULL(@OperationTimeLineID, 0) > 0)
			BEGIN
				--Count số dòng dữ liệu báo bận, báo dừng máy của mỗi khung giờ
				SELECT
					@TotalParamOther = COUNT(p.ID)
				FROM dbo.OperationParameters p WITH(NOLOCK)
				WHERE p.FactoryID = @FactoryID
					AND p.OperationPositionID = @OperationPositionID
					AND p.CycleID = @CycleID
					AND p.OperationTimeLineID = @OperationTimeLineID
					AND p.IsChecked = 0
					AND p.IsDeleted = 0

				SET @TotalParamOther = ISNULL(@TotalParamOther, 0)

				--Cập nhật dữ liệu ghi thông số
				UPDATE p
				SET IsChecked = 1,
					ErrorCode = CASE WHEN @ErrorCode < 1 AND @ErrorCode > 5 THEN 0 ELSE @ErrorCode END,
					LastUpdatedUserID = @CurrentUserID,
					CheckedDate = @CurrentDate,
					SyncedDate = @CurrentDate
				FROM dbo.OperationParameters p
				WHERE p.FactoryID = @FactoryID
					AND p.OperationPositionID = @OperationPositionID
					AND p.CycleID = @CycleID
					AND p.OperationTimeLineID = @OperationTimeLineID
					AND p.IsChecked = 0
					AND p.IsDeleted = 0
				
				--Cập nhật thông tin khung giờ ghi thông số
				UPDATE dbo.OperationTimeLine
				SET TotalParametersChecked = TotalParametersChecked + @TotalParamOther,
					TotalParametersSynchronized = TotalParametersChecked + @TotalParamOther,
					LastUpdatedUserID = @CurrentUserID,
					SyncedDate = @CurrentDate
				WHERE ID = @OperationTimeLineID
			END

			SET @OperationTimeLineID = NULL
			SET @TimeLineIndex = @TimeLineIndex + 1
		END

		SET @Result = 'SUCCESS'
	END

	--Kết quả tạo danh sách thông số
	SELECT @Result AS Message
END