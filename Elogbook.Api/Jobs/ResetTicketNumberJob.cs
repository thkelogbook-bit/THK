using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Log;
using Quartz;
using System;
using System.Threading.Tasks;

namespace Elogbook.Api.Jobs
{
    /// <summary>
    /// ResetTicketNumberJob
    /// </summary>
    public class ResetTicketNumberJob : IJob
    {
        private readonly IWorkTicketBusiness _workTicketBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        public ResetTicketNumberJob()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="workTicketBusiness"></param>
        public ResetTicketNumberJob(IWorkTicketBusiness workTicketBusiness)
        {
            _workTicketBusiness = workTicketBusiness;
        }

        /// <summary>
        /// Reset ticket
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context)
        {
            LogCustom.LogMessage(TrackingLogLevel.INFO, "PCT", "Start run job reset ticket number of PCT.");
            await Console.Out.WriteLineAsync("Start run job reset ticket number of PCT.");

            _workTicketBusiness.ResetTicketNumber();
            LogCustom.LogMessage(TrackingLogLevel.INFO, "PCT", "End run job reset ticket number of PCT.");
        }
    }
}