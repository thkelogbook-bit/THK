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
    /// Quản lý email template
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/emailTemplate")]
    public class EmailTemplateController : BaseApiController
    {
        private readonly IEmailTemplateBusiness _emailTemplateBusiness;
        private readonly ICommonBusiness _commonBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="emailTemplateBusiness"></param>
        /// <param name="commonBusiness"></param>
        public EmailTemplateController(IEmailTemplateBusiness emailTemplateBusiness,
            ICommonBusiness commonBusiness)
        {
            _emailTemplateBusiness = emailTemplateBusiness;
            _commonBusiness = commonBusiness;
        }

        /// <summary>
        /// Tìm kiếm email template theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_EMAILT
        [Route("find")]
        [HttpPost]
        public ResponseBaseModel GetEmailTemplates([FromBody] SearchEmailTemplateRequestModel requestModel)
        {
            return _emailTemplateBusiness.GetEmailTemplates(requestModel);
        }

        /// <summary>
        /// Thông tin của mỗi email template
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_EMAILT
        [Route("{id}")]
        [HttpGet]
        public ResponseModel<EmailTemplateResponseModel> GetEmailTemplateById([FromUri] int id)
        {
            return _emailTemplateBusiness.GetEmailTemplateById(id);
        }

        /// <summary>
        /// Thêm mới email template
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        public ResponseModel<EmailTemplateResponseModel> AddEmailTemplate([FromBody] AddEditEmailTemplateRequestModel requestModel)
        {
            ResponseModel<EmailTemplateResponseModel> responseModel = new ResponseModel<EmailTemplateResponseModel>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.ADD_EMAILT)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _emailTemplateBusiness.AddEmailTemplate(requestModel);
        }

        /// <summary>
        /// Cập nhật email template
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPut]
        public ResponseModel<EmailTemplateResponseModel> UpdateEmailTemplate([FromBody] AddEditEmailTemplateRequestModel requestModel)
        {
            ResponseModel<EmailTemplateResponseModel> responseModel = new ResponseModel<EmailTemplateResponseModel>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_EMAILT)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _emailTemplateBusiness.UpdateEmailTemplate(requestModel);
        }

        /// <summary>
        /// Xóa email template
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        public ResponseModel<bool> DeleteEmailTemplate([FromUri] int id)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.DELETE_EMAILT)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            return _emailTemplateBusiness.DeleteEmailTemplate(id, CurrentUser.ID);
        }

        /// <summary>
        /// Kiểm tra email template đang sử dụng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_EMAILT
        [Route("checkInUse/{id}")]
        [HttpGet]
        public ResponseModel<bool> CheckInUseEmailTemplate([FromUri] int id)
        {
            return _emailTemplateBusiness.CheckInUseEmailTemplate(id);
        }
    }
}