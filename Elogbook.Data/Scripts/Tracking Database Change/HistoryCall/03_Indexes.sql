-- Bước 3/5: Tạo indexes cho OperationParametersArchive
-- Chạy sau file 02_CreateTable.sql

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID('dbo.OperationParametersArchive')
      AND name = N'IDX_Archive_ParameterID_InsertedAt'
)
BEGIN
    CREATE NONCLUSTERED INDEX [IDX_Archive_ParameterID_InsertedAt]
        ON [dbo].[OperationParametersArchive] ([ParameterID] ASC, [InsertedAt] ASC)
        INCLUDE ([CurrentValue], [ErrorCode], [IsChecked], [FromRange], [ToRange], [FactoryID], [OperationPositionID], [CycleID])
    PRINT N'Tạo IDX_Archive_ParameterID_InsertedAt thành công'
END

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID('dbo.OperationParametersArchive')
      AND name = N'IDX_Archive_Factory_Position'
)
BEGIN
    CREATE NONCLUSTERED INDEX [IDX_Archive_Factory_Position]
        ON [dbo].[OperationParametersArchive] ([FactoryID] ASC, [OperationPositionID] ASC, [InsertedAt] ASC)
    PRINT N'Tạo IDX_Archive_Factory_Position thành công'
END

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID('dbo.OperationParametersArchive')
      AND name = N'IDX_Archive_OperationTimeLineID'
)
BEGIN
    CREATE NONCLUSTERED INDEX [IDX_Archive_OperationTimeLineID]
        ON [dbo].[OperationParametersArchive] ([OperationTimeLineID] ASC, [InsertedAt] ASC)
    PRINT N'Tạo IDX_Archive_OperationTimeLineID thành công'
END

PRINT N'Tạo indexes hoàn tất'
