using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Microsoft.Web.Http;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// SLA Configuration Management
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/slaConfigs")]
    public class SlaConfigController : BaseApiController
    {
        private readonly ISlaConfigBusiness _slaConfigBusiness;
        private readonly ICommonBusiness _commonBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        public SlaConfigController(ISlaConfigBusiness slaConfigBusiness, ICommonBusiness commonBusiness)
        {
            _slaConfigBusiness = slaConfigBusiness;
            _commonBusiness = commonBusiness;
        }

        /// <summary>
        /// Get all SLA configurations with pagination and filtering
        /// </summary>
        [Route("find")]
        [HttpPost]
        public ResponseBaseModel GetSlaConfigs([FromBody] SearchSlaConfigModel requestModel)
        {
            var responseModel = new ResponseBaseModel();
            if (requestModel == null)
            {
                responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                responseModel.Content = null;
                responseModel.Message = "Dữ liệu yêu cầu không hợp lệ";
                return responseModel;
            }

            return _slaConfigBusiness.GetSlaConfigs(requestModel);
        }

        /// <summary>
        /// Get SLA configuration details by ID
        /// </summary>
        [Route("{id}")]
        [HttpGet]
        public ResponseModel<SlaConfigDetailModel> GetSlaConfigById([FromUri] int id)
        {
            return _slaConfigBusiness.GetSlaConfigById(id);
        }

        /// <summary>
        /// Create new SLA configuration
        /// </summary>
        [Route("")]
        [HttpPost]
        public ResponseModel<int> AddSlaConfig([FromBody] AddEditSlaConfigModel requestModel)
        {
            ResponseModel<int> responseModel = new ResponseModel<int>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.ADD_SLA)).Data)
            {
                responseModel.StatusCode = (int)HttpStatusCode.Forbidden;
                responseModel.ErrorMessages.Add("Người dùng không có quyền thêm cấu hình SLA");
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;

            return _slaConfigBusiness.AddSlaConfig(requestModel);
        }

        /// <summary>
        /// Update SLA configuration
        /// </summary>
        [Route("")]
        [HttpPut]
        public ResponseModel<bool> UpdateSlaConfig([FromBody] AddEditSlaConfigModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_SLA)).Data)
            {
                responseModel.StatusCode = (int)HttpStatusCode.Forbidden;
                responseModel.ErrorMessages.Add("Người dùng không có quyền chỉnh sửa cấu hình SLA");
                return responseModel;
            }

            if (requestModel == null)
            {
                responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                responseModel.ErrorMessages.Add("Dữ liệu yêu cầu không hợp lệ");
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _slaConfigBusiness.UpdateSlaConfig(requestModel);
        }

        /// <summary>
        /// Delete SLA configuration
        /// </summary>
        [Route("{id}")]
        [HttpDelete]
        public ResponseModel<bool> DeleteSlaConfig([FromUri] int id)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.DELETE_SLA)).Data)
            {
                responseModel.StatusCode = (int)HttpStatusCode.Forbidden;
                responseModel.ErrorMessages.Add("Người dùng không có quyền xóa cấu hình SLA");
                return responseModel;
            }

            return _slaConfigBusiness.DeleteSlaConfig(id, CurrentUser.ID);
        }

        /// <summary>
        /// Toggle SLA configuration active/inactive status
        /// </summary>
        [Route("{id}/toggle")]
        [HttpPatch]
        public ResponseModel<bool> ToggleSlaConfigStatus([FromUri] int id)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_SLA)).Data)
            {
                responseModel.StatusCode = (int)HttpStatusCode.Forbidden;
                responseModel.ErrorMessages.Add("Người dùng không có quyền chỉnh sửa cấu hình SLA");
                return responseModel;
            }

            return _slaConfigBusiness.ToggleSlaConfigStatus(id, CurrentUser.ID);
        }

        /// <summary>
        /// Get list of SLA conditions
        /// </summary>
        [Route("slaConditions")]
        [HttpGet]
        public List<SlaConditionModel> GetSlaConditions()
        {
            return _slaConfigBusiness.GetSlaConditions();
        }

        /// <summary>
        /// Get list of SLA roles
        /// </summary>
        [Route("slaRoles")]
        [HttpGet]
        public List<SlaRoleModel> GetSlaRoles()
        {
            return _slaConfigBusiness.GetSlaRoles();
        }

        /// <summary>
        /// Get list of SLA categories for combobox
        /// </summary>
        [Route("slaCategories")]
        [HttpGet]
        public List<SlaCategoryModel> GetSlaCategories()
        {
            return _slaConfigBusiness.GetSlaCategories();
        }


        /// <summary>
        /// Get active email templates for SLA
        /// </summary>
        [Route("getEmailTemplates")]
        [HttpGet]
        public List<SlaEmailTemplateModel> GetActiveEmailTemplates()
        {
            return _slaConfigBusiness.GetActiveEmailTemplates();
        }

        /// <summary>
        /// Physically delete a SlaLevel by ID
        /// </summary>
        [Route("slaLevel/{id}")]
        [HttpDelete]
        public ResponseModel<bool> DeleteSlaLevel([FromUri] int id)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.DELETE_SLA)).Data)
            {
                responseModel.StatusCode = (int)HttpStatusCode.Forbidden;
                responseModel.ErrorMessages.Add("Người dùng không có quyền xóa cấp độ SLA");
                return responseModel;
            }

            return _slaConfigBusiness.DeleteSlaLevel(id, CurrentUser.ID);
        }

        /// <summary>
        /// Add a new SlaLevel for a SlaConfig
        /// </summary>
        [Route("slaLevel")]
        [HttpPost]
        public ResponseModel<int> AddSlaLevel([FromBody] AddEditSlaLevelModel model)
        {
            ResponseModel<int> responseModel = new ResponseModel<int>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.ADD_SLA)).Data)
            {
                responseModel.StatusCode = (int)HttpStatusCode.Forbidden;
                responseModel.ErrorMessages.Add("Người dùng không có quyền thêm cấp độ SLA");
                return responseModel;
            }

            if (model == null)
            {
                responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                responseModel.ErrorMessages.Add("Dữ liệu yêu cầu không hợp lệ");
                return responseModel;
            }

            model.CurrentUserID = CurrentUser.ID;
            return _slaConfigBusiness.AddSlaLevel(model);
        }

        /// <summary>
        /// Update an existing SlaLevel (roles will be replaced)
        /// </summary>
        [Route("slaLevel/{id}")]
        [HttpPut]
        public ResponseModel<bool> UpdateSlaLevel([FromUri] int id, [FromBody] AddEditSlaLevelModel model)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_SLA)).Data)
            {
                responseModel.StatusCode = (int)HttpStatusCode.Forbidden;
                responseModel.ErrorMessages.Add("Người dùng không có quyền chỉnh sửa cấp độ SLA");
                return responseModel;
            }

            if (model == null)
            {
                responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                responseModel.ErrorMessages.Add("Dữ liệu yêu cầu không hợp lệ");
                return responseModel;
            }

            model.CurrentUserID = CurrentUser.ID;
            return _slaConfigBusiness.UpdateSlaLevel(id, model);
        }
    }
}
