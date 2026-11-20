-- Bước 4/5: Tạo Stored Procedure ArchiveOperationParameters
-- Chạy sau file 03_Indexes.sql
-- QUAN TRỌNG: File này CHỈ chứa mình CREATE PROCEDURE — không có statement nào khác trước nó

IF EXISTS (
    SELECT 1 FROM sys.objects
    WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('dbo.ArchiveOperationParameters')
)
    DROP PROCEDURE [dbo].[ArchiveOperationParameters]

CREATE PROCEDURE [dbo].[ArchiveOperationParameters]
    @CutOffMonths   INT = 12,
    @BatchSize      INT = 50000
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CutOffDate     DATETIME = DATEADD(MONTH, -@CutOffMonths, CAST(GETDATE() AS DATE));
    DECLARE @RowsArchived   INT = 0;
    DECLARE @HasMore        BIT = 0;

    CREATE TABLE #BatchIDs (
        ID          BIGINT   NOT NULL,
        InsertedAt  DATETIME NOT NULL,
        PRIMARY KEY (ID)
    );

    -- Chọn batch cũ nhất chưa được archive
    INSERT INTO #BatchIDs (ID, InsertedAt)
    SELECT TOP (@BatchSize)
        op.ID,
        op.InsertedAt
    FROM dbo.OperationParameters op WITH (ROWLOCK, READPAST)
    WHERE op.InsertedAt < @CutOffDate
      AND NOT EXISTS (
            SELECT 1 FROM dbo.OperationParametersArchive a
            WHERE a.InsertedAt = op.InsertedAt AND a.ID = op.ID
      )
    ORDER BY op.InsertedAt ASC, op.ID ASC;

    SET @RowsArchived = @@ROWCOUNT;

    IF @RowsArchived > 0
    BEGIN
        BEGIN TRY
            BEGIN TRANSACTION;

            -- INSERT vào archive — snapshot TimeLineName từ OperationTimeLine
            INSERT INTO dbo.OperationParametersArchive (
                [InsertedAt], [ID], [ArchivedAt],
                [ParameterName], [ParameterID], [ParameterGroupID], [OperationTimeLineID],
                [FactoryID], [OperationPositionID], [CycleID],
                [CurrentValue], [KKSCode], [UnitMeasure], [Setting],
                [FromRange], [ToRange], [IsNumberType], [PreviousValue],
                [ErrorCode], [IsChecked], [AlertDetails],
                [IsDeleted], [CreatedUserID], [LastUpdatedUserID],
                [UpdatedAt], [SyncDataKey], [AppID], [CheckedDate], [SyncedDate],
                [ExcelAIColIndex], [TimeLineName]
            )
            SELECT
                op.[InsertedAt], op.[ID], GETDATE(),
                op.[ParameterName], op.[ParameterID], op.[ParameterGroupID], op.[OperationTimeLineID],
                op.[FactoryID], op.[OperationPositionID], op.[CycleID],
                op.[CurrentValue], op.[KKSCode], op.[UnitMeasure], op.[Setting],
                op.[FromRange], op.[ToRange], op.[IsNumberType], op.[PreviousValue],
                op.[ErrorCode], op.[IsChecked], op.[AlertDetails],
                op.[IsDeleted], op.[CreatedUserID], op.[LastUpdatedUserID],
                op.[UpdatedAt], op.[SyncDataKey], op.[AppID], op.[CheckedDate], op.[SyncedDate],
                op.[ExcelAIColIndex],
                tl.[TimeLineName]
            FROM dbo.OperationParameters op
            INNER JOIN #BatchIDs b ON b.ID = op.ID
            LEFT JOIN dbo.OperationTimeLine tl ON tl.ID = op.OperationTimeLineID;

            -- DELETE khỏi bảng gốc sau khi INSERT thành công
            DELETE op
            FROM dbo.OperationParameters op
            INNER JOIN #BatchIDs b ON b.ID = op.ID;

            COMMIT TRANSACTION;

            -- Kiểm tra còn data cần archive không
            IF EXISTS (
                SELECT 1 FROM dbo.OperationParameters WITH (NOLOCK)
                WHERE InsertedAt < @CutOffDate
                  AND NOT EXISTS (
                        SELECT 1 FROM dbo.OperationParametersArchive a
                        WHERE a.InsertedAt = InsertedAt AND a.ID = ID
                  )
            )
                SET @HasMore = 1;

        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0
                ROLLBACK TRANSACTION;

            DECLARE @ErrMsg  NVARCHAR(2048) = ERROR_MESSAGE();
            DECLARE @ErrLine INT            = ERROR_LINE();
            DROP TABLE IF EXISTS #BatchIDs;
            RAISERROR(N'ArchiveOperationParameters lỗi tại dòng %d: %s', 16, 1, @ErrLine, @ErrMsg);
            RETURN;
        END CATCH;
    END

    DROP TABLE IF EXISTS #BatchIDs;

    -- Trả về cho C# job
    SELECT
        @RowsArchived   AS RowsArchived,
        @HasMore        AS HasMore;
END
