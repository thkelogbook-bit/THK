using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Microsoft.Web.Http;
using System.Web.Http;
using Elogbook.Core.Models.BaseFilter;
using Hangfire;

namespace Elogbook.Api.Controllers.V2
{
    /// <summary>
    /// Backup data
    /// </summary>
    [ApiVersion("2.0")]
    [RoutePrefix("v{version:apiVersion}/backupData")]
    public class BackupDataController : BaseApiController
    {
        private readonly IBackupDataBusiness _backupDataBusiness;


        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="backupDataBusiness"></param>
        public BackupDataController(IBackupDataBusiness backupDataBusiness)
        {
            _backupDataBusiness = backupDataBusiness;
        }

        /// <summary>
        /// Lấy thông tin backup data
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [Route("find")]
        [HttpPost]
        public ResponseBaseModel Find(BaseListFilter filter)
        {
            return _backupDataBusiness.Find(filter);
        }

        /// <summary>
        /// Delete dashboard
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        public ResponseModel<bool> Delete([FromParameter("id")] int id)
        {
            return _backupDataBusiness.Delete(id, CurrentUser.ID);
        }
    }
}