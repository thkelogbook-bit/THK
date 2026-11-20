using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Log;
using Quartz;
using System;
using System.Threading.Tasks;

namespace Elogbook.Api.Jobs
{
    /// <summary>
    /// AMTaskJob
    /// </summary>
    public class AmCreateJob : IJob
    {

        private readonly IAutonomousMaintenanceBusiness _autonomousMaintenanceBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        public AmCreateJob()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="autonomousMaintenanceBusiness"></param>
        public AmCreateJob(IAutonomousMaintenanceBusiness autonomousMaintenanceBusiness)
        {
            _autonomousMaintenanceBusiness = autonomousMaintenanceBusiness;
        }

        /// <summary>
        /// Create Autonomous Maintenance
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context)
        {
            LogCustom.LogMessage(TrackingLogLevel.INFO, "Autonomous Maintenance", "Start run job create autonomous maintenance record.");
            await Console.Out.WriteLineAsync("Start run job create autonomous maintenance record.");

            _autonomousMaintenanceBusiness.CreateAutonomousMaintenance();

            LogCustom.LogMessage(TrackingLogLevel.INFO, "Autonomous Maintenance", "End run job create autonomous maintenance record.");
        }
    }
 }