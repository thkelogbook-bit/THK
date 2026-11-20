-- Bước 2/5: Tạo bảng OperationParametersArchive
-- Chạy sau file 01_Partition.sql

IF NOT EXISTS (
    SELECT 1 FROM sys.objects WHERE type = 'U' AND OBJECT_ID = OBJECT_ID('dbo.OperationParametersArchive')
)
BEGIN
    CREATE TABLE [dbo].[OperationParametersArchive] (
        [InsertedAt]            DATETIME        NOT NULL,
        [ID]                    BIGINT          NOT NULL,
        [ArchivedAt]            DATETIME        NOT NULL DEFAULT GETDATE(),
        [ParameterName]         NVARCHAR (255)  NOT NULL,
        [ParameterID]           INT             NOT NULL,
        [ParameterGroupID]      INT             NULL,
        [OperationTimeLineID]   INT             NULL,
        [FactoryID]             INT             NOT NULL,
        [OperationPositionID]   INT             NULL,
        [CycleID]               INT             NULL,
        [CurrentValue]          NVARCHAR (MAX)  NULL,
        [KKSCode]               NVARCHAR (255)  NULL,
        [UnitMeasure]           NVARCHAR (255)  NULL,
        [Setting]               NVARCHAR (MAX)  NULL,
        [FromRange]             REAL            NULL,
        [ToRange]               REAL            NULL,
        [IsNumberType]          BIT             NOT NULL DEFAULT 0,
        [PreviousValue]         NVARCHAR (MAX)  NULL,
        [ErrorCode]             INT             NULL,
        [IsChecked]             BIT             NOT NULL DEFAULT 0,
        [AlertDetails]          NVARCHAR (MAX)  NULL,
        [IsDeleted]             BIT             NOT NULL DEFAULT 0,
        [CreatedUserID]         INT             NULL,
        [LastUpdatedUserID]     INT             NULL,
        [UpdatedAt]             DATETIME        NOT NULL,
        [SyncDataKey]           NVARCHAR (100)  NULL,
        [AppID]                 NVARCHAR (100)  NULL,
        [CheckedDate]           DATETIME        NULL,
        [SyncedDate]            DATETIME        NULL,
        [ExcelAIColIndex]       INT             NULL,
        [TimeLineName]          INT             NULL,

        CONSTRAINT [PK_OperationParametersArchive]
            PRIMARY KEY CLUSTERED ([InsertedAt] ASC, [ID] ASC)
            ON [PS_YearWiseArchive] ([InsertedAt])
    )
    ON [PS_YearWiseArchive] ([InsertedAt])

    PRINT N'Tạo bảng OperationParametersArchive thành công'
END
ELSE
BEGIN
    PRINT N'Bảng OperationParametersArchive đã tồn tại — bỏ qua'
END
