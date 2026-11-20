using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Log;
using Quartz;
using System;
using System.Threading.Tasks;

namespace Elogbook.Api.Jobs
{
    /// <summary>
    /// MailServicesJob
    /// </summary>
    public class MailServicesJob : IJob
    {
        private readonly IEmailServiceBusiness _emailServiceBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        public MailServicesJob()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="emailServiceBusiness"></param>
        public MailServicesJob(IEmailServiceBusiness emailServiceBusiness)
        {
            _emailServiceBusiness = emailServiceBusiness;
        }

        /// <summary>
        /// Execute job
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context)
        {
            LogCustom.LogMessage(TrackingLogLevel.INFO, "MAIL_SERVICE", "Start run job service send mail.");
            await Console.Out.WriteLineAsync("Start run job service send mail.");
            _emailServiceBusiness.HandleEmailTransmission();

            LogCustom.LogMessage(TrackingLogLevel.INFO, "MAIL_SERVICE", "End run job service send mail.");
        }
    }
}