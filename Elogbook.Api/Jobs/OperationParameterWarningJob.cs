using Elogbook.Core;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Log;
using Elogbook.Data;
using Quartz;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Elogbook.Api.Jobs
{
    /// <summary>
    /// OperationParameterWarning
    /// </summary>
    public class OperationParameterWarningJob : IJob
    {
        private readonly IOperationParametersBusiness _operationParametersBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        public OperationParameterWarningJob()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="operationParametersBusiness"></param>
        public OperationParameterWarningJob(IOperationParametersBusiness operationParametersBusiness)
        {
            _operationParametersBusiness = operationParametersBusiness;
        }

        /// <summary>
        /// Reset ticket
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context)
        {
            LogCustom.LogMessage(TrackingLogLevel.INFO, "OPW", "Start run job sync data operation parameter warning.");
            await Console.Out.WriteLineAsync("Start run job sync data operation parameter warning.");

            // Thực hiện xóa dữ liệu các token logout cũ để tối ưu dữ liệu
            try
            {
                using (var dbContext = new ElogbookDataContext())
                {
                    var dateTime = DateTime.Now.AddMinutes(-AppSettings.DEFAULT_TOKEN_EXPIRE_MINUTES);
                    var tokenExpires = dbContext.TokenExpire.Where(x => x.InsertedAt < dateTime);
                    if (tokenExpires.Count() > 0)
                    {
                        foreach (var item in tokenExpires)
                        {
                            dbContext.Entry(item).State = EntityState.Deleted;
                        }
                        dbContext.SaveChanges();
                    }
                }
            }
            catch
            {
                //ignore error
            }

            _operationParametersBusiness.SyncDataOperationParameterWarning();

            LogCustom.LogMessage(TrackingLogLevel.INFO, "OPW", "End run job sync data operation parameter warning.");
        }
    }
}