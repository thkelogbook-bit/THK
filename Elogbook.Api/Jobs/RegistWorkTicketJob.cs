using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Log;
using Quartz;
using System;
using System.Threading.Tasks;

namespace Elogbook.Api.Jobs
{
    /// <summary>
    /// RegistWorkTicketJob
    /// </summary>
    public class RegistWorkTicketJob : IJob
    {
        private readonly IBatchWorkRegBusiness _batchWorkRegBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        public RegistWorkTicketJob()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="batchWorkRegBusiness"></param>
        public RegistWorkTicketJob(IBatchWorkRegBusiness batchWorkRegBusiness)
        {
            _batchWorkRegBusiness = batchWorkRegBusiness;
        }

        /// <summary>
        /// Tạo Phiếu đăng ký công việc từ file excel theo lịch trình
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context)
        {
            LogCustom.LogMessage(TrackingLogLevel.INFO, "RegistrationTicket", "Start run job create RegistrationTicket from excel.");
            await Console.Out.WriteLineAsync("Start run job create work ticket from excel.");

            _batchWorkRegBusiness.InsertWorkRegFromSchedule();

            LogCustom.LogMessage(TrackingLogLevel.INFO, "RegistrationTicket", "End run job create RegistrationTicket from excel.");
        }
    }
}