using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Microsoft.Web.Http;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Theo dõi bất thường sự cố
    /// </summary>
    [ApiVersion("2.0")]
    [RoutePrefix("v{version:apiVersion}/abnormalIncident")]
    public class AbnormalIncidentController : BaseApiController
    {
        private readonly IAbnormalIncidentBusiness _abnormalIncidentBusiness;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="abnormalIncidentBusiness"></param>
        public AbnormalIncidentController(IAbnormalIncidentBusiness abnormalIncidentBusiness)
        {
            _abnormalIncidentBusiness = abnormalIncidentBusiness;
        }

        /// <summary>
        /// Tìm kiếm bất thường sự cố
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("find")]
        [HttpPost]
        public ResponseBaseModel Find([FromBody] SearchAbnormalIncidentModel requestModel)
        {
            requestModel.UserID = CurrentUser.ID;
            return _abnormalIncidentBusiness.Find(requestModel);
        }

        /// <summary>
        /// Thêm mới xử lý bất thường sự cố
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("add")]
        [HttpPost]
        public ResponseBaseModel Post([FromBody] AbnormalIncidentDetailAdd requestModel)
        {
            requestModel.CreatedUserID = CurrentUser.ID;
            return _abnormalIncidentBusiness.AddAbnormalIncidentDetail(requestModel);
        }

        /// <summary>
        /// Thông tin xử lý bất thường sự cố
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("get")]
        [HttpGet]
        public ResponseBaseModel Get(int id)
        {
            return _abnormalIncidentBusiness.GetAbnormalIncidentById(id, CurrentUser.ID);
        }

        /// <summary>
        /// Cập nhật xử lý bất thường sự cố
        /// </summary>
        /// <param name="id"></param>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("update")]
        [HttpPut]
        public ResponseBaseModel Put(int id, [FromBody] AbnormalIncidentDetailUpdate requestModel)
        {
            requestModel.AbnormalIncidentID = id;
            requestModel.UpdatedUserID = CurrentUser.ID;
            return _abnormalIncidentBusiness.UpdateAbnormalIncidentDetail(requestModel);
        }

        /// <summary>
        /// Xóa bất thường sự cố
        /// </summary>
        /// <param name="abnormalIncidentID"></param>
        /// <returns></returns>
        [Route("delete")]
        [HttpDelete]
        public ResponseBaseModel Delete(int abnormalIncidentID)
        {
            return _abnormalIncidentBusiness.DeleteAbnormalIncident(abnormalIncidentID, CurrentUser.ID);
        }

        /// <summary>
        /// Xóa chi tiết xử lý bất thường sự cố
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("deleteDetail")]
        [HttpDelete]
        public ResponseBaseModel DeleteDetail(int id)
        {
            return _abnormalIncidentBusiness.DeteleAbnormalIncidentDetail(id, CurrentUser.ID);
        }

        /// <summary>
        /// Đồng bộ dữ liệu với mobile app
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("syncDataAbnormalIncidentDetail")]
        [HttpPost]
        public ResponseBaseModel SyncDataIssueOfShiftDetail([FromBody] AbnormalIncidentDetailSyncDataModel requestModel)
        {
            return _abnormalIncidentBusiness.SyncDataAbnormalIncidentDetail(requestModel);
        }
    }
}
