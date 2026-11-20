using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Log;
using Quartz;
using System;
using System.Threading.Tasks;

namespace Elogbook.Api.Jobs
{
    /// <summary>
    /// BackupDataJob
    /// </summary>
    public class BackupDataJob : IJob
    {
        private readonly IBackupDataBusiness _backupDataBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        public BackupDataJob(IBackupDataBusiness backupDataBusiness)
        {
            _backupDataBusiness = backupDataBusiness;
        }

        /// <summary>
        /// Backup data
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context)
        {
            LogCustom.LogMessage(TrackingLogLevel.INFO, "System", "Start run job backup data.");
            await Console.Out.WriteLineAsync("Start run job backup data.");

            _backupDataBusiness.BackupData();
            LogCustom.LogMessage(TrackingLogLevel.INFO, "System", "End run job backup data.");
        }
    }
}