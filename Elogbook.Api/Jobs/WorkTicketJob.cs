using Elogbook.Core;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Log;
using Quartz;
using System;
using System.Threading.Tasks;

namespace Elogbook.Api.Jobs
{
    /// <summary>
    /// PmisJob
    /// </summary>
    public class WorkTicketJob : IJob
    {
        private readonly IMajorRepairBusiness _majorRepairBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        public WorkTicketJob()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="majorRepairBusiness"></param>
        public WorkTicketJob(IMajorRepairBusiness majorRepairBusiness)
        {
            _majorRepairBusiness = majorRepairBusiness;
        }

        /// <summary>
        /// Sync data PMIS
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context)
        {
            LogCustom.LogMessage(TrackingLogLevel.INFO, "WORK_TICKET", "Start run job create work ticket from excel.");
            await Console.Out.WriteLineAsync("Start run job create work ticket from excel.");

            _majorRepairBusiness.InsertWorkTicketFromSchedule();

            LogCustom.LogMessage(TrackingLogLevel.INFO, "WORK_TICKET", "End run job create work ticket from excel.");
        }
    }
}