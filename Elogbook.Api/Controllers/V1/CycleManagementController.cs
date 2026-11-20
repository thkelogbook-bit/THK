using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Microsoft.Web.Http;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Quản lý Chu kỳ
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/cycleManagement")]
    public class CycleManagementController : BaseApiController
    {
        private readonly ICycleManagementBusiness _cycleManagementBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cycleManagementBusiness"></param>
        public CycleManagementController(ICycleManagementBusiness cycleManagementBusiness)
        {
            _cycleManagementBusiness = cycleManagementBusiness;
        }

        /// <summary>
        /// Thêm Chu kỳ mới
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        [Permission(EnumUserPermission.ADD_CK)]
        public ResponseModel<int> AddCycleManagement([FromBody] AddEditCycleManagerModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _cycleManagementBusiness.AddCycleManagement(requestModel);
        }

        /// <summary>
        /// Trả về tất cả Chu kỳ theo điều kiện truyền vào
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("find")]
        [HttpPost]
        [Permission(EnumUserPermission.VIEW_CK)]
        public ResponseBaseModel GetCycleManagements([FromBody] SearchCycleManagementModel requestModel)
        {
            return _cycleManagementBusiness.GetCycleManagements(requestModel);
        }

        /// <summary>
        /// Trả về chu kỳ theo mã id Chu kỳ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpGet]
        [Permission(EnumUserPermission.EDIT_CK)]
        public ResponseModel<CycleManagementModel> GetCycleManagementById([FromUri] int id)
        {
            return _cycleManagementBusiness.GetCycleManagementById(id);
        }

        /// <summary>
        /// Cập nhập Chu kỳ
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPut]
        [Permission(EnumUserPermission.EDIT_CK)]
        public ResponseModel<bool> UpdateCycleManagement([FromBody] AddEditCycleManagerModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _cycleManagementBusiness.UpdateCycleManagement(requestModel);
        }

        /// <summary>
        /// Kiểm Chu kỳ có đang sử dụng để cập nhật vận hành
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_CK
        [Route("checkInUse/{id}")]
        [HttpGet]
        [Permission(EnumUserPermission.DELETE_CK)]
        public ResponseModel<bool> CheckInUseCycleManagement([FromUri] int id)
        {
            return _cycleManagementBusiness.CheckInUseCycleManagement(id);
        }

        /// <summary>
        /// Xóa Chu kỳ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        [Permission(EnumUserPermission.DELETE_CK)]
        public ResponseModel<bool> DeleteCycleManagement([FromUri] int id)
        {
            return _cycleManagementBusiness.DeleteCycleManagement(id, CurrentUser.ID);
        }
    }
}