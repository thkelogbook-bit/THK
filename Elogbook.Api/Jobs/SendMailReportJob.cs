using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Log;
using Quartz;
using System;
using System.Threading.Tasks;

namespace Elogbook.Api.Jobs
{
    /// <summary>
    /// Xử lý gửi mail report
    /// </summary>
    public class SendMailReportJob : IJob
    {
        private readonly IAmReportBusiness _amReportBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        public SendMailReportJob()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="amReportBusiness"></param>
        public SendMailReportJob(IAmReportBusiness amReportBusiness)
        {
            _amReportBusiness = amReportBusiness;
        }

        /// <summary>
        /// Tự đồng gửi mail report
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context)
        {
            LogCustom.LogMessage(TrackingLogLevel.INFO, "AmReport Job", "Start send mail report");
            await Console.Out.WriteLineAsync("Start run job auto complete AM");

            _amReportBusiness.SendMailReport();

            LogCustom.LogMessage(TrackingLogLevel.INFO, "AmReport Job", "End send mail report");
        }
    }
}
