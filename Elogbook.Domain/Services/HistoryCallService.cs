using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Data;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Elogbook.Domain.Services
{
    /// <summary>
    /// Domain service xử lý archive dữ liệu thông số vận hành cũ
    /// từ OperationParameters sang OperationParametersArchive.
    /// </summary>
    public class HistoryCallService : IHistoryCallService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;

        public HistoryCallService(ElogbookUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Gọi SP ArchiveOperationParameters theo batch cho đến khi
        /// HasMore = 0 hoặc đạt giới hạn maxBatches.
        /// SP tự xử lý transaction INSERT + DELETE, đảm bảo toàn vẹn dữ liệu.
        /// </summary>
        public void ArchiveData(int batchSize, int maxBatches)
        {
            int batchCount = 0;
            bool hasMore = true;

            while (hasMore && batchCount < maxBatches)
            {
                try
                {
                    var pBatchSize = new SqlParameter("@BatchSize", SqlDbType.Int) { Value = batchSize };
                    var pCutOffMonths = new SqlParameter("@CutOffMonths", SqlDbType.Int) { Value = 12 };

                    // Dùng .ToList() synchronous — tránh deadlock khi dùng .ToListAsync().Result trong job
                    var results = _unitOfWork.DataContext.Database
                        .SqlQuery<ArchiveResult>(
                            "EXEC dbo.ArchiveOperationParameters @CutOffMonths, @BatchSize",
                            pCutOffMonths, pBatchSize)
                        .ToList();

                    batchCount++;

                    if (results == null || results.Count == 0)
                    {
                        hasMore = false;
                        LogCustom.LogMessage(TrackingLogLevel.INFO, "HistoryCall",
                            $"Batch {batchCount}: SP trả về không có kết quả — dừng vòng lặp.");
                        break;
                    }

                    var result = results[0];
                    hasMore = result.HasMore == 1;

                    LogCustom.LogMessage(TrackingLogLevel.INFO, "HistoryCall",
                        $"Batch {batchCount}: RowsArchived={result.RowsArchived}, HasMore={result.HasMore}");

                    if (result.RowsArchived == 0)
                    {
                        hasMore = false;
                    }
                }
                catch (Exception ex)
                {
                    LogCustom.LogMessage(TrackingLogLevel.ERROR, "HistoryCall",
                        $"Lỗi tại batch {batchCount + 1}: {ex}");
                    break;
                }
            }

            LogCustom.LogMessage(TrackingLogLevel.INFO, "HistoryCall",
                $"ArchiveData hoàn tất: tổng {batchCount} batch đã xử lý.");
        }

        /// <summary>
        /// Model nhận kết quả trả về từ SP ArchiveOperationParametersHistoryCall
        /// </summary>
        private class ArchiveResult
        {
            public int RowsArchived { get; set; }
            public int HasMore { get; set; }
        }
    }
}
