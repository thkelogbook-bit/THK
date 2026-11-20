using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Microsoft.Web.Http;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V2
{
    /// <summary>
    /// Job history
    /// </summary>
    [ApiVersion("2.0")]
    [RoutePrefix("v{version:apiVersion}/jobHistory")]
    public class JobHistoryController : BaseApiController
    {
        private readonly IJobHistoryBusiness _jobHistoryBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="jobHistoryBusiness"></param>
        public JobHistoryController(IJobHistoryBusiness jobHistoryBusiness)
        {
            _jobHistoryBusiness = jobHistoryBusiness;
        }

        /// <summary>
        ///Get History
        /// </summary>
        /// <returns></returns>
        [Route("Find")]
        [HttpPost]
        public ResponseBaseModel Find([FromBody] SearchJobHistoryModel requestModel)
        {
            return _jobHistoryBusiness.Find(requestModel);
        }
    }
}