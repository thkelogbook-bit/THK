using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Microsoft.Web.Http;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V2
{
    /// <summary>
    /// ConfigurationWarning
    /// </summary>
    [ApiVersion("2.0")]
    [RoutePrefix("v{version:apiVersion}/configurationWarning")]
    public class AiController : BaseApiController
    {
        private readonly IAiBusiness _aiBusiness;
        private readonly ICommonBusiness _commonBusiness;

       /// <summary>
       /// Contruction
       /// </summary>
       /// <param name="aiBusiness"></param>
       /// <param name="commonBusiness"></param>
        public AiController(IAiBusiness aiBusiness, ICommonBusiness commonBusiness)
        {
            _aiBusiness = aiBusiness;
            _commonBusiness = commonBusiness;
        }

        /// <summary>
        /// GetConfigRuleDefaul
        /// </summary>
        /// <returns></returns>
        [Route("rule/get")]
        [HttpGet]
        public ResponseModel<ConfigWarningRuleResponseModel> GetConfigRuleDefaul()
        {
            return _aiBusiness.GetConfigRuleDefaul();
        }

        /// <summary>
        /// UpdateConfigWarningRule
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("rule/update")]
        [HttpPost]
        public ResponseModel<bool> UpdateConfigWarningRule([FromBody] AddEditConfigWarningRuleRequestModel requestModel)
        {
            //ResponseModel<bool> response = new ResponseModel<bool>();
            //if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_CONFIG_WARNING)).Data)
            //{
            //    response.ErrorMessages.Add("Bạn chưa được cấp quyền thao tác");
            //    response.StatusCode = (int)HttpStatusCode.Unauthorized;
            //    return response;
            //}

            requestModel.CurrentUserID = CurrentUser.ID;
            return _aiBusiness.SaveConfigWarningRule(requestModel);
        }

        /// <summary>
        /// GetConfigDefaul
        /// </summary>
        /// <returns></returns>
        [Route("common/get")]
        [HttpGet]
        public ResponseModel<ConfigWarningResponseModel> GetConfigDefaul()
        {
            return _aiBusiness.GetConfigDefaul();
        }

        /// <summary>
        /// UpdateConfigWarning
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("common/update")]
        [HttpPost]
        public ResponseModel<bool> UpdateConfigWarning([FromBody] AddEditConfigWarningRequestModel requestModel)
        {
            //ResponseModel<bool> response = new ResponseModel<bool>();
            //if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_CONFIG_WARNING)).Data)
            //{
            //    response.ErrorMessages.Add("Bạn chưa được cấp quyền thao tác");
            //    response.StatusCode = (int)HttpStatusCode.Unauthorized;
            //    return response;
            //}

            requestModel.CurrentUserID = CurrentUser.ID;
            return _aiBusiness.SaveConfigWarning(requestModel);
        }
    }
}