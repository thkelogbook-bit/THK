using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request.OperationShift;
using Elogbook.Core.Models.Response;
using Microsoft.Web.Http;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Quản lý ca vận hành
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/operationShift")]
    public class OperationShiftController : BaseApiController
    {
        private readonly IOperationShiftBusiness _operationShiftBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="operationShiftBusiness"></param>
        public OperationShiftController(IOperationShiftBusiness operationShiftBusiness)
        {
            _operationShiftBusiness = operationShiftBusiness;
        }

        /// <summary>
        /// Tìm kiếm ca vận hành theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("find")]
        [HttpPost]
        [Permission(EnumUserPermission.VIEW_CVH)]
        public ResponseBaseModel GetOperationShifts([FromBody] SearchOperationShiftModel requestModel)
        {
            return _operationShiftBusiness.GetOperationShifts(requestModel);
        }

        /// <summary>
        /// Tìm kiếm ca vận hành theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpGet]
        [Permission(EnumUserPermission.EDIT_CVH)]
        public ResponseModel<OperationShiftModel> GetOperationShiftById([FromUri] int id)
        {
            return _operationShiftBusiness.GetOperationShiftById(id);
        }

        /// <summary>
        /// Thêm mới ca vận hành
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        [Permission(EnumUserPermission.ADD_CVH)]
        public ResponseModel<OperationShiftModel> AddOperationShift([FromBody] AddEditOperationShiftModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _operationShiftBusiness.AddOperationShift(requestModel);
        }

        /// <summary>
        /// Cập nhật ca vận hành
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPut]
        [Permission(EnumUserPermission.EDIT_CVH)]
        public ResponseModel<OperationShiftModel> UpdateOperationShift([FromBody] AddEditOperationShiftModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _operationShiftBusiness.UpdateOperationShift(requestModel);
        }

        /// <summary>
        /// Kiểm tra ca vận hành có đang được sử dụng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("CheckInUse/{id}")]
        [HttpGet]
        [Permission(EnumUserPermission.DELETE_CVH)]
        public ResponseModel<bool> CheckInUseOperationShift([FromUri] int id)
        {
            return _operationShiftBusiness.CheckInUseOperationShift(id);
        }

        /// <summary>
        /// Xoá ca vận hành
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        [Permission(EnumUserPermission.DELETE_CVH)]
        public ResponseModel<bool> DeleteOperationShift([FromUri] int id)
        {
            return _operationShiftBusiness.DeleteOperationShift(id, CurrentUser.ID);
        }
    }
}