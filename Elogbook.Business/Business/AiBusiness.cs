using Elogbook.Core;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Elogbook.Domain.Services;
using System;
using System.Collections.Generic;
using System.Net;

namespace Elogbook.Business.Business
{
    public class AiBusiness : IAiBusiness
    {
        private readonly IAiService _aiService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="aiService"></param>
        public AiBusiness(IAiService aiService)
        {
            _aiService = aiService;
        }

        /// <summary>
        /// GetConfigRuleDefaul
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel<ConfigWarningRuleResponseModel> GetConfigRuleDefaul()
        {
            var responseModel = new ResponseModel<ConfigWarningRuleResponseModel>();

            try
            {
                responseModel.Data = _aiService.GetConfigRuleDefaul();
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AiBusiness.ToString(),
                    Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// GetConfigDefaul
        /// </summary>
        /// <returns></returns>
        public ResponseModel<ConfigWarningResponseModel> GetConfigDefaul()
        {
            var responseModel = new ResponseModel<ConfigWarningResponseModel>();

            try
            {
                responseModel.Data = _aiService.GetConfigDefaul();
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AiBusiness.ToString(),
                    Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// SaveConfigWarningRule
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<bool> SaveConfigWarningRule(AddEditConfigWarningRuleRequestModel requestModel)
        {
            try
            {
                return _aiService.SaveConfigWarningRule(requestModel);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AiBusiness.ToString(), ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// SaveConfigWarning
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<bool> SaveConfigWarning(AddEditConfigWarningRequestModel requestModel)
        {
            try
            {
                return _aiService.SaveConfigWarning(requestModel);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AiBusiness.ToString(), ex.ToString());
                return null;
            }
        }

    }
}
