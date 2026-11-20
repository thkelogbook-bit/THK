using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using System;
using System.Net;

namespace Elogbook.Business
{
    /// <summary>
    /// Job Service
    /// </summary>
    public class JobHistoryBusiness : IJobHistoryBusiness
    {
        private readonly IJobHistoryService _jobHistoryService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="jobHistoryService"></param>
        public JobHistoryBusiness(IJobHistoryService jobHistoryService)
        {
            _jobHistoryService = jobHistoryService;
        }

        public ResponseBaseModel Find(SearchJobHistoryModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _jobHistoryService.Find(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.Message = CommonConstant.JOB_HISTORY_COMMON;
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.JobHistoryBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }
    }
}
