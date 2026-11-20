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
    /// Service import data DCS from excel files
    /// </summary>
    public class DcsJob : IJob
    {
        private readonly IDcsImportDataBusiness _dcsImportDataBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        public DcsJob()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public DcsJob(IDcsImportDataBusiness dcsImportDataBusiness)
        {
            _dcsImportDataBusiness = dcsImportDataBusiness;
        }

        /// <summary>
        /// Execute job
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context)
        {
            LogCustom.LogMessage(TrackingLogLevel.INFO, "TSVH_DCS", "Start run job sync data from DCS to Elogbook.");
            await Console.Out.WriteLineAsync("Start run job sync data PMIS.");

            for (int i = 0; i <= AppSettings.DCS_DAY_READ_DATA; i++)
            {
                var result = _dcsImportDataBusiness.ImportDataFromExcelFiles(i == 0 ? i : -i);
                if (!result.Data)
                {
                    LogCustom.LogMessage(TrackingLogLevel.INFO, "TSVH_DCS", "Import data unsuccessfully!");
                }
                else
                {
                    LogCustom.LogMessage(TrackingLogLevel.INFO, "TSVH_DCS", "Import data successfully!");
                }
            }

            LogCustom.LogMessage(TrackingLogLevel.INFO, "TSVH_DCS", "End run job sync data from DCS to Elogbook.");
        }
    }
}