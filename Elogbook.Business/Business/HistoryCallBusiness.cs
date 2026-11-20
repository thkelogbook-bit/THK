using Elogbook.Core;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using System;

namespace Elogbook.Business.Business
{
    /// <summary>
    /// Business layer cho chức năng HistoryCall.
    /// Đọc cấu hình từ AppSettings và delegate xuống HistoryCallService.
    /// </summary>
    public class HistoryCallBusiness : IHistoryCallBusiness
    {
        private readonly IHistoryCallService _historyCallService;

        public HistoryCallBusiness(IHistoryCallService historyCallService)
        {
            _historyCallService = historyCallService;
        }

        /// <summary>
        /// Khởi chạy archive. Đọc batchSize và maxBatches từ AppSettings,
        /// sau đó gọi service thực hiện.
        /// </summary>
        public void ArchiveData()
        {
            try
            {
                int batchSize = AppSettings.HISTORY_CALL_BATCH_SIZE;
                int maxBatches = AppSettings.HISTORY_CALL_MAX_BATCHES;

                LogCustom.LogMessage(TrackingLogLevel.INFO, "HistoryCall",
                    $"ArchiveData bắt đầu: batchSize={batchSize}, maxBatches={maxBatches}");

                _historyCallService.ArchiveData(batchSize, maxBatches);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "HistoryCall",
                    $"ArchiveData lỗi không mong muốn: {ex}");
                // Không ném lại — tránh làm crash HistoryCallJob
            }
        }
    }
}
