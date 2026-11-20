-- Bước 5/5: Insert Schedule record cho HistoryCallJob
-- Chạy sau file 04_StoredProcedure.sql

IF NOT EXISTS (
    SELECT 1 FROM dbo.Schedule WITH (NOLOCK)
    WHERE ScheduleName = 'historycall'
)
BEGIN
    INSERT INTO dbo.Schedule (
        [ScheduleName],
        [ScheduleContent],
        [ScheduleValue],
        [Active],
        [IsSystem],
        [IsSendEmail],
        [EmailTo],
        [EmailCC],
        [EmailSubject],
        [EmailContent],
        [IsDeleted],
        [InsertedAt],
        [UpdatedAt],
        [ScheduleDescription]
    )
    VALUES (
        'historycall',
        'Archive thông số vận hành cũ hơn 12 tháng sang OperationParametersArchive',
        '0 0 2 ? * SAT,SUN',    -- Thứ 7 và Chủ nhật 2:00 AM
        1,                       -- Active = 1 (bật)
        0,                       -- IsSystem = false
        0,                       -- IsSendEmail = false
        '',                      -- EmailTo
        '',                      -- EmailCC
        '',                      -- EmailSubject
        '',                      -- EmailContent
        0,                       -- IsDeleted = false
        GETDATE(),
        GETDATE(),
        N'Job archive dữ liệu thông số vận hành (OperationParameters) cũ hơn 12 tháng sang bảng OperationParametersArchive. Chạy mỗi Thứ 7 và Chủ nhật lúc 2:00 AM.'
    )
    PRINT N'Thêm Schedule historycall thành công'
END
ELSE
BEGIN
    PRINT N'Schedule historycall đã tồn tại — bỏ qua'
END
