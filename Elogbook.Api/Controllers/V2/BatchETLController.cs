using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Microsoft.Web.Http;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V2
{
    /// <summary>
    /// Load ETL
    /// </summary>
    [ApiVersion("2.0")]
    [RoutePrefix("v{version:apiVersion}/batchetl")]
    public class BatchEtlController : BaseApiController
    {
        private readonly IBatchETLBusiness _batchETLBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="batchETLBusiness"></param>
        public BatchEtlController(IBatchETLBusiness batchETLBusiness)
        {
            _batchETLBusiness = batchETLBusiness;
        }

        /// <summary>
        /// Load ETL
        /// </summary>
        /// <returns></returns>
        [Route("runBatch")]
        [HttpGet]
        public ResponseBaseModel RunBatch()
        {
            return _batchETLBusiness.RunBatch();
        }
    }
}