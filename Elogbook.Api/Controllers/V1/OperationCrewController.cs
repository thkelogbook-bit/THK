using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Microsoft.Web.Http;
using System.Net;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Quản lý Kíp vận hành
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/operationCrew")]
    public class OperationCrewController : BaseApiController
    {
        private readonly IOperationCrewBusiness _operationCrewBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="operationCrewBusiness"></param>
        public OperationCrewController(IOperationCrewBusiness operationCrewBusiness)
        {
            _operationCrewBusiness = operationCrewBusiness;
        }

        /// <summary>
        /// Trả về tất cả Kíp vận hành theo điều kiện tìm kiếm
        /// </summary>
        /// <param name="requestModel"></param>
        /// EnumUserPermission.VIEW_KVH
        /// <returns></returns>
        [Route("find")]
        [HttpPost]
        [Permission(EnumUserPermission.VIEW_KVH)]
        public ResponseBaseModel GetOperationCrews([FromBody] SearchOperationCrewModel requestModel)
        {
            return _operationCrewBusiness.GetOperationCrews(requestModel);
        }

        /// <summary>
        /// Trả về Kíp vận hành theo mã Kíp vận hành
        /// </summary>
        /// <param name="id"></param>
        /// EnumUserPermission.VIEW_KVH
        /// <returns></returns>
        [Route("{id}")]
        [HttpGet]
        [Permission(EnumUserPermission.EDIT_KVH)]
        public ResponseModel<OperationCrewModel> GetOperationCrewById([FromUri] int id)
        {
            return _operationCrewBusiness.GetOperationCrewById(id);
        }

        /// <summary>
        /// Thêm mới Kíp vận hành
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        [Permission(EnumUserPermission.ADD_KVH)]
        public ResponseModel<OperationCrewModel> AddOperationCrew([FromBody] AddEditOperationCrewModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _operationCrewBusiness.AddOperationCrew(requestModel);
        }

        /// <summary>
        /// Cập nhập Kíp vận hành
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPut]
        [Permission(EnumUserPermission.EDIT_KVH)]
        public ResponseModel<OperationCrewModel> UpdateOperationCrew([FromBody] AddEditOperationCrewModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _operationCrewBusiness.UpdateOperationCrew(requestModel);
        }

        /// <summary>
        /// Xóa Kíp vận hành
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        [Permission(EnumUserPermission.DELETE_KVH)]
        public ResponseModel<bool> DeleteOperationCrew([FromUri] int id)
        {
            return _operationCrewBusiness.DeleteOperationCrew(id, CurrentUser.ID);
        }

        /// <summary>
        /// Kiểm tra Kíp vận hành đang sử dụng
        /// </summary>
        /// <param name="id"></param>
        /// EnumUserPermission.VIEW_KVH
        /// <returns></returns>
        [Route("checkInUse/{id}")]
        [HttpGet]
        [Permission(EnumUserPermission.DELETE_KVH)]
        public ResponseModel<bool> CheckInUseOperationCrew([FromUri] int id)
        {
            return _operationCrewBusiness.CheckInUseOperationCrew(id);
        }
    }
}