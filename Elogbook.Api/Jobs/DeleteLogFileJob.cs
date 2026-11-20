using Elogbook.Core;
using Elogbook.Core.Enums;
using Elogbook.Core.Log;
using Quartz;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Elogbook.Api.Jobs
{
    /// <summary>
    /// DeleteLogFileJob
    /// </summary>
    public class DeleteLogFileJob : IJob
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public DeleteLogFileJob()
        {
        }

        /// <summary>
        /// Execute job
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context)
        {
            LogCustom.LogMessage(TrackingLogLevel.INFO, "DELETE_LOG", "Start run job delete file log.");
            await Console.Out.WriteLineAsync("Start run job delete file log.");

            try
            {
                var filePath = LogCustom.GetLogFilePath();
                LogCustom.LogMessage(TrackingLogLevel.INFO, "DELETE_LOG", $"File Path: {filePath}");
                var di = new DirectoryInfo(filePath);
                foreach (FileInfo file in di.GetFiles())
                {
                    try
                    {
                        if (file.CreationTime < DateTime.Now.AddDays(AppSettings.LAST_TIME_DELETE_LOG))
                            file.Delete();
                    }
                    catch (Exception ex)
                    {
                        LogCustom.LogMessage(TrackingLogLevel.ERROR, "DELETE_LOG", ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "DELETE_LOG", ex.ToString());
            }

            LogCustom.LogMessage(TrackingLogLevel.INFO, "DELETE_LOG", "End run job delete file log.");
        }
    }
}