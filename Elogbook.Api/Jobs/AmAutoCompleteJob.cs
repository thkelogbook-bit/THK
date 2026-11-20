using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Log;
using Quartz;
using System;
using System.Threading.Tasks;

namespace Elogbook.Api.Jobs
{
    /// <summary>
    /// Xử lý hoàn thành AM
    /// </summary>
    public class AmAutoCompleteJob : IJob
    {
        private readonly IAutonomousMaintenanceBusiness _autonomousMaintenanceBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        public AmAutoCompleteJob()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="autonomousMaintenanceBusiness"></param>
        public AmAutoCompleteJob(IAutonomousMaintenanceBusiness autonomousMaintenanceBusiness)
        {
            _autonomousMaintenanceBusiness = autonomousMaintenanceBusiness;
        }

        /// <summary>
        /// Tự đồng hoàn thành việc bảo trì tự quản
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context)
        {
            LogCustom.LogMessage(TrackingLogLevel.INFO, "Autonomous Maintenance", "Start auto complete AM");
            await Console.Out.WriteLineAsync("Start run job auto complete AM");

            _autonomousMaintenanceBusiness.AutoCompleteAm();

            LogCustom.LogMessage(TrackingLogLevel.INFO, "Autonomous Maintenance", "End auto complete AM");
        }
    }
}
