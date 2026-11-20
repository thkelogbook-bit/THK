using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Log;
using Quartz;
using System;
using System.Threading.Tasks;

namespace Elogbook.Api.Jobs
{
    /// <summary>
    /// PmisToElogbookJob
    /// </summary>
    public class PmisToElogbookJob : IJob
    {
        private readonly IBatchETLBusiness _batchETLBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        public PmisToElogbookJob()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public PmisToElogbookJob(IBatchETLBusiness batchETLBusiness)
        {
            _batchETLBusiness = batchETLBusiness;
        }

        /// <summary>
        /// Sync data PMIS
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context)
        {
            LogCustom.LogMessage(TrackingLogLevel.INFO, "DEVICE_PMIS", "Start run job sync PMIS to Elogbook.");
            await Console.Out.WriteLineAsync("Start run job sync PMIS to Elogbook.");
           
            _batchETLBusiness.RunBatch();

            LogCustom.LogMessage(TrackingLogLevel.INFO, "DEVICE_PMIS", "Start run job sync PMIS to Elogbook.");
        }
    }
}