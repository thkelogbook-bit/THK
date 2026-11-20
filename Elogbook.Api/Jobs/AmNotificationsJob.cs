using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Log;
using Quartz;
using System;
using System.Threading.Tasks;

namespace Elogbook.Api.Jobs
{
    /// <summary>
    /// Thông báo AM
    /// </summary>
    public class AmNotificationsJob : IJob
    {
        private readonly IAutonomousMaintenanceBusiness _autonomousMaintenanceBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        public AmNotificationsJob()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="autonomousMaintenanceBusiness"></param>
        public AmNotificationsJob(IAutonomousMaintenanceBusiness autonomousMaintenanceBusiness)
        {
            _autonomousMaintenanceBusiness = autonomousMaintenanceBusiness;
        }

        /// <summary>
        /// Send mail notification
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context)
        {
            LogCustom.LogMessage(TrackingLogLevel.INFO, "Autonomous Maintenance", "Start run job send mail notification.");
            await Console.Out.WriteLineAsync("Start run job send mail notification");

            _autonomousMaintenanceBusiness.SendMailNotificationAm();

            LogCustom.LogMessage(TrackingLogLevel.INFO, "Autonomous Maintenance", "End run job send mail notification");
        }
    }
}
