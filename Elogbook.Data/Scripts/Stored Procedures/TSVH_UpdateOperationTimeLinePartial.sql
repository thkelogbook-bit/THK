/****** Object:  StoredProcedure [dbo].[TSVH_UpdateOperationTimeLinePartial]    Script Date: 1/20/2024 9:50:53 AM ******/
DROP PROCEDURE [dbo].[TSVH_UpdateOperationTimeLinePartial]
GO

/****** Object:  StoredProcedure [dbo].[TSVH_UpdateOperationTimeLinePartial]    Script Date: 1/20/2024 9:50:53 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[TSVH_UpdateOperationTimeLinePartial]
    @FactoryID INT,
    @OperationPositionID INT,
    @CycleID INT,
    @OperationTimeLineIDs NVARCHAR(MAX),   
    @StopParameterIDs NVARCHAR(MAX),
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
		 DECLARE @StopParameterIDsTemp TABLE
    (
        ParameterID INT
    )


    INSERT INTO @StopParameterIDsTemp (ParameterID)
    SELECT value
    FROM STRING_SPLIT(@StopParameterIDs, ',')

    -- Chỉnh sửa điều kiện WHERE và sử dụng INNER JOIN trực tiếp trong câu lệnh SELECT
    INSERT INTO @OperationTimeLineIDTemp
    SELECT otl.ID
    FROM STRING_SPLIT(@OperationTimeLineIDs, ',') temp
    INNER JOIN dbo.OperationTimeLine otl ON temp.[value] = otl.ID

    SET @TotalTimeLine = (SELECT COUNT(OperationTimeLineID) FROM @OperationTimeLineIDTemp)

    IF(ISNULL(@TotalTimeLine, 0) > 0)
    BEGIN
        BEGIN TRANSACTION

        WHILE(@TimeLineIndex <= @TotalTimeLine)
        BEGIN
            SELECT TOP 1 @OperationTimeLineID = OperationTimeLineID FROM @OperationTimeLineIDTemp WHERE RowIndex = @TimeLineIndex
            IF(ISNULL(@OperationTimeLineID, 0) > 0)
            BEGIN
                -- Count số dòng dữ liệu báo bận, báo dừng máy của mỗi khung giờ
                SELECT @TotalParamOther = COUNT(p.ID)
                FROM dbo.OperationParameters p WITH (NOLOCK)
                WHERE p.FactoryID = @FactoryID
                    AND p.OperationPositionID = @OperationPositionID
                    AND p.CycleID = @CycleID
                    AND p.OperationTimeLineID = @OperationTimeLineID
                    AND p.IsChecked = 0
                    AND p.IsDeleted = 0
                    AND ParameterID IN (SELECT ParameterID FROM @StopParameterIDsTemp)

                SET @TotalParamOther = ISNULL(@TotalParamOther, 0)

                -- Cập nhật dữ liệu ghi thông số
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
                    AND ParameterID IN (SELECT ParameterID FROM @StopParameterIDsTemp)

                -- Cập nhật thông tin khung giờ ghi thông số
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

        -- COMMIT TRANSACTION nếu mọi thứ diễn ra suôn sẻ
        COMMIT TRANSACTION
        SET @Result = 'SUCCESS'
    END
    ELSE
    BEGIN
        SET @Result = 'SUCCESS' -- Nếu không có khung giờ nào được chọn, coi đây là thành công
    END

    -- Kết quả tạo danh sách thông số
    SELECT @Result AS Message
END
GO