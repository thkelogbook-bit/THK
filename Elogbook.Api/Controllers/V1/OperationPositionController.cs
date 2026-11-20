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
    /// Quản lý Cương vị
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/operationPosition")]
    public class OperationPositionController : BaseApiController
    {
        private readonly IOperationPositionBusiness _operationPositionBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="operationPositionBusiness"></param>
        public OperationPositionController(IOperationPositionBusiness operationPositionBusiness)
        {
            _operationPositionBusiness = operationPositionBusiness;
        }

        /// <summary>
        /// Trả về tất cả Cương vị theo điều kiện truyền vào
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("find")]
        [HttpPost]
        [Permission(EnumUserPermission.VIEW_CV)]
        public ResponseBaseModel GetOperationPositions([FromBody] SearchOperationPositionModel requestModel)
        {
            return _operationPositionBusiness.GetOperationPositions(requestModel);
        }

        /// <summary>
        /// Trả về một Cương vị theo mã Cương vị
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpGet]
        [Permission(EnumUserPermission.EDIT_CV)]
        public ResponseModel<OperationPositionModel> GetOperationPositionById([FromUri] int id)
        {
            return _operationPositionBusiness.GetOperationPositionById(id);
        }

        /// <summary>
        /// Thêm mới một Cương vị
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        [Permission(EnumUserPermission.ADD_CV)]
        public ResponseModel<int> AddOperationPosition([FromBody] AddEditOperationPositionModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _operationPositionBusiness.AddOperationPosition(requestModel);
        }

        /// <summary>
        /// Cập nhập một Cương vị
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPut]
        [Permission(EnumUserPermission.EDIT_CV)]
        public ResponseModel<bool> UpdateOperationPosition([FromBody] AddEditOperationPositionModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _operationPositionBusiness.UpdateOperationPosition(requestModel);
        }

        /// <summary>
        /// Xóa một Cương vị
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        [Permission(EnumUserPermission.DELETE_CV)]
        public ResponseModel<bool> DeleteOperationPosition([FromUri] int id)
        {
            return _operationPositionBusiness.DeleteOperationPosition(id, CurrentUser.ID);
        }

        /// <summary>
        /// Kiểm tra Cương vị có đang sử dụng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("checkInUse/{id}")]
        [HttpGet]
        [Permission(EnumUserPermission.DELETE_CV)]
        public ResponseModel<bool> CheckInUseOperationPosition([FromUri] int id)
        {
            return _operationPositionBusiness.CheckInUseOperationPosition(id);
        }
    }
}