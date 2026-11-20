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
    /// PmisJob
    /// </summary>
    public class PmisJob : IJob
    {
        private readonly IPmisBusiness _pmisBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        public PmisJob()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pmisBusiness"></param>
        public PmisJob(IPmisBusiness pmisBusiness)
        {
            _pmisBusiness = pmisBusiness;
        }

        /// <summary>
        /// Sync data PMIS
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context)
        {
            LogCustom.LogMessage(TrackingLogLevel.INFO, "TSVH_PMIS", "Start run job sync data PMIS.");
            await Console.Out.WriteLineAsync("Start run job sync data PMIS.");

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

            _pmisBusiness.AutoCreateTableByYear();
            _pmisBusiness.SycnDataForPMIS(AppSettings.TIME_GET_LATEST_DATA_PMIS);

            LogCustom.LogMessage(TrackingLogLevel.INFO, "TSVH_PMIS", "End run job sync data PMIS.");
        }
    }
}