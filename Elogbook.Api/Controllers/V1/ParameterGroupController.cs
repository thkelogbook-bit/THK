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
    /// Quản lý Nhóm thông số
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/parameterGroup")]
    public class ParameterGroupController : BaseApiController
    {
        private readonly IParameterGroupBusiness _parameterGroupBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameterGroupBusiness"></param>
        public ParameterGroupController(IParameterGroupBusiness parameterGroupBusiness)
        {
            _parameterGroupBusiness = parameterGroupBusiness;
        }

        /// <summary>
        /// Trả về tất cả Nhóm thông số theo điều kiện truyền vào
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("find")]
        [HttpPost]
        [Permission(EnumUserPermission.VIEW_NTS)]
        public ResponseBaseModel GetParameterGroups([FromBody] SearchParameterGroupModel requestModel)
        {
            return _parameterGroupBusiness.GetParameterGroups(requestModel);
        }

        /// <summary>
        /// Trả về một Nhóm thông số theo mã Nhóm thông số
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpGet]
        [Permission(EnumUserPermission.EDIT_NTS)]
        public ResponseModel<ParameterGroupModel> GetParameterGroupById([FromUri] int id)
        {
            return _parameterGroupBusiness.GetParameterGroupById(id);
        }

        /// <summary>
        /// Thêm mới một Nhóm thống số
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        [Permission(EnumUserPermission.ADD_NTS)]
        public ResponseModel<int> AddParameterGroup([FromBody] AddEditParameterGroupModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _parameterGroupBusiness.AddParameterGroup(requestModel);
        }

        /// <summary>
        /// Cập nhập một Nhóm thống số
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPut]
        [Permission(EnumUserPermission.EDIT_NTS)]
        public ResponseModel<bool> UpdateParameterGroup([FromBody] AddEditParameterGroupModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _parameterGroupBusiness.UpdateParameterGroup(requestModel);
        }

        /// <summary>
        /// Xóa một Nhóm thống số
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        [Permission(EnumUserPermission.DELETE_NTS)]
        public ResponseModel<bool> DeleteParameterGroup([FromUri] int id)
        {
            return _parameterGroupBusiness.DeleteParameterGroup(id, CurrentUser.ID);
        }

        /// <summary>
        /// Kiểm tra Nhóm thông số có đang sử dụng hay chưa
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("checkInUse/{id}")]
        [HttpGet]
        [Permission(EnumUserPermission.DELETE_NTS)]
        public ResponseModel<bool> CheckInUseParameterGroup([FromUri] int id)
        {
            return _parameterGroupBusiness.CheckInUseParameterGroup(id);
        }
    }
}