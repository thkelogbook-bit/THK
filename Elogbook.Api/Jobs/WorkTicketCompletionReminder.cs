using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Log;
using Quartz;
using System;
using System.Threading.Tasks;

namespace Elogbook.Api.Jobs
{
    [DisallowConcurrentExecution]
    public class WorkTicketCompletionReminder : IJob
    {
        private readonly IWorkTicketBusiness _workTicketBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        public WorkTicketCompletionReminder()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="workTicketBusiness"></param>
        public WorkTicketCompletionReminder(IWorkTicketBusiness workTicketBusiness)
        {
            _workTicketBusiness = workTicketBusiness;
        }

        /// <summary>
        /// Gửi email nhắc nhở, cảnh báo phiếu công tác chưa hoàn thành theo kế hoạch
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context)
        {
            LogCustom.LogMessage(TrackingLogLevel.INFO, "Ticket", "Send Mail Work Ticket Completion Reminder");
            await Console.Out.WriteLineAsync("Start run job auto Send Mail Work Ticket");

            _workTicketBusiness.SendEmailWorkTicketCompletionReminder();

            LogCustom.LogMessage(TrackingLogLevel.INFO, "Ticket", "Send Mail Work Ticket Completion Reminder");
        }
    }
}