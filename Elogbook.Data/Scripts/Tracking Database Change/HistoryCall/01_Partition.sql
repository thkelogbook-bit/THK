-- Bước 1/5: Tạo Partition Function và Partition Scheme
-- Chạy file này trước tiên

IF NOT EXISTS (
    SELECT 1 FROM sys.partition_functions WHERE name = N'PF_YearlyPartitionArchive'
)
BEGIN
    CREATE PARTITION FUNCTION [PF_YearlyPartitionArchive](DATETIME)
        AS RANGE LEFT
        FOR VALUES (
            '2019-12-31 23:59:59',
            '2020-12-31 23:59:59',
            '2021-12-31 23:59:59',
            '2022-12-31 23:59:59',
            '2023-12-31 23:59:59',
            '2024-12-31 23:59:59',
            '2025-12-31 23:59:59'
        )
    PRINT N'Tạo PF_YearlyPartitionArchive thành công'
END
ELSE
BEGIN
    PRINT N'PF_YearlyPartitionArchive đã tồn tại — bỏ qua'
END

IF NOT EXISTS (
    SELECT 1 FROM sys.partition_schemes WHERE name = N'PS_YearWiseArchive'
)
BEGIN
    CREATE PARTITION SCHEME [PS_YearWiseArchive]
        AS PARTITION [PF_YearlyPartitionArchive]
        ALL TO ([PRIMARY])
    PRINT N'Tạo PS_YearWiseArchive thành công'
END
ELSE
BEGIN
    PRINT N'PS_YearWiseArchive đã tồn tại — bỏ qua'
END
