using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Microsoft.Web.Http;
using System.Collections.Generic;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Tổ chức
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/organization")]
    public class OrganizationController : BaseApiController
    {
        private readonly IOrganizationBusiness _organizationBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="organizationBusiness"></param>
        public OrganizationController(IOrganizationBusiness organizationBusiness)
        {
            _organizationBusiness = organizationBusiness;
        }

        /// <summary>
        /// Lấy danh sách tổ chức
        /// </summary>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_SDTC
        [Route("find")]
        [HttpPost]
        [Permission(EnumUserPermission.VIEW_SDTC)]
        public ResponseModel<List<OrganizationModel>> GetOrganizations([FromBody] SearchOrganizationModel requestModel)
        {
            return _organizationBusiness.GetOrganizations(requestModel);
        }

        /// <summary>
        /// Thêm mới tổ chức
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        [Permission(EnumUserPermission.ADD_SDTC)]
        public ResponseModel<int> AddOrganization([FromBody] AddEditOrganizationModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _organizationBusiness.AddOrganization(requestModel);
        }

        /// <summary>
        /// Cập nhật tổ chức
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPut]
        [Permission(EnumUserPermission.EDIT_SDTC)]
        public ResponseModel<bool> UpdateOrganization([FromBody] AddEditOrganizationModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _organizationBusiness.UpdateOrganization(requestModel);
        }

        /// <summary>
        /// Thay đổi trạng thái tổ chức
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_SDTC
        [Route("changeStatus")]
        [HttpPut]
        [Permission(EnumUserPermission.EDIT_SDTC)]
        public ResponseModel<int> ChangeStatusOrganization([FromBody] ChangeOrganizationStatusModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _organizationBusiness.ChangeStatusOrganization(requestModel);
        }

        /// <summary>
        /// Xoá tổ chức
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("")]
        [HttpDelete]
        [Permission(EnumUserPermission.DELETE_SDTC)]
        public ResponseModel<int> DeleteOrganization([FromUri] int id)
        {
            return _organizationBusiness.DeleteOrganization(id, CurrentUser.ID);
        }
    }
}