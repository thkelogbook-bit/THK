using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Log;
using Quartz;
using System;
using System.Threading.Tasks;

namespace Elogbook.Api.Jobs
{
    /// <summary>
    /// Quartz.NET job chạy định kỳ để archive dữ liệu cũ hơn 12 tháng
    /// từ bảng OperationParameters sang OperationParametersHistoryCall.
    /// Mặc định chạy mỗi Chủ nhật lúc 2:00 AM (cron: 0 0 2 ? * SUN).
    /// </summary>
    public class HistoryCallJob : IJob
    {
        private readonly IHistoryCallBusiness _historyCallBusiness;

        /// <summary>
        /// Constructor không tham số — Autofac/Quartz yêu cầu
        /// </summary>
        public HistoryCallJob()
        {
        }

        /// <summary>
        /// Constructor inject qua Autofac DI
        /// </summary>
        public HistoryCallJob(IHistoryCallBusiness historyCallBusiness)
        {
            _historyCallBusiness = historyCallBusiness;
        }

        /// <summary>
        /// Entry point của job — được Quartz.NET gọi theo lịch
        /// </summary>
        public async Task Execute(IJobExecutionContext context)
        {
            LogCustom.LogMessage(TrackingLogLevel.INFO, "HistoryCall", "Start run job archive operation parameters history call.");
            await Console.Out.WriteLineAsync("Start run job archive operation parameters history call.");

            try
            {
                _historyCallBusiness.ArchiveData();
            }
            catch (Exception ex)
            {
                // Bắt toàn bộ exception để Quartz không đánh dấu job là failed và dừng scheduler
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "HistoryCall",
                    $"HistoryCallJob.Execute lỗi không mong muốn: {ex}");
            }

            LogCustom.LogMessage(TrackingLogLevel.INFO, "HistoryCall", "End run job archive operation parameters history call.");
        }
    }
}
