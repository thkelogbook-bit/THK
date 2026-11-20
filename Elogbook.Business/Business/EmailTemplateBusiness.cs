using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System;
using System.Collections.Generic;
using System.Net;

namespace Elogbook.Business
{
    public class EmailTemplateBusiness : IEmailTemplateBusiness
    {
        private readonly IEmailTemplateService _emailTemplateService;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="emailTemplateService"></param>
        public EmailTemplateBusiness(IEmailTemplateService emailTemplateService)
        {
            _emailTemplateService = emailTemplateService;
        }

        public ResponseBaseModel GetEmailTemplates(SearchEmailTemplateRequestModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _emailTemplateService.GetEmailTemplates(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.EmailTemplateBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseModel<EmailTemplateResponseModel> GetEmailTemplateById(int id)
        {
            var responseModel = new ResponseModel<EmailTemplateResponseModel>();

            try
            {
                responseModel.Data = _emailTemplateService.GetEmailTemplateById(id);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.EmailTemplateBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<EmailTemplateResponseModel> AddEmailTemplate(AddEditEmailTemplateRequestModel requestModel)
        {
            var responseModel = new ResponseModel<EmailTemplateResponseModel>();
            _errorMessages = new List<string>();

            try
            {
                if (!IsValidData(requestModel))
                {
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                else
                {
                    if (!_emailTemplateService.CheckDuplicateData(requestModel, true))
                    {
                        responseModel.Data = _emailTemplateService.AddEmailTemplate(requestModel);
                    }
                    else
                    {
                        responseModel.ErrorMessages.Add("Đã tồn tại email template cùng event trong nhà máy.");
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                    }
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.EmailTemplateBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<EmailTemplateResponseModel> UpdateEmailTemplate(AddEditEmailTemplateRequestModel requestModel)
        {
            var responseModel = new ResponseModel<EmailTemplateResponseModel>();
            _errorMessages = new List<string>();

            try
            {
                if (!_emailTemplateService.CheckDuplicateData(requestModel, false))
                {
                    responseModel.Data = _emailTemplateService.UpdateEmailTemplate(requestModel);
                }
                else
                {
                    responseModel.ErrorMessages.Add("Đã tồn tại email template cùng event trong nhà máy.");
                    responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.EmailTemplateBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> DeleteEmailTemplate(int id, int userID)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _emailTemplateService.DeleteEmailTemplate(id, userID);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.EmailTemplateBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> CheckInUseEmailTemplate(int id)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _emailTemplateService.CheckInUseEmailTemplate(id);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.EmailTemplateBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Kiểm tra dữ liệu bắt buộc khi tạo mới/cập nhật email template
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        private bool IsValidData(AddEditEmailTemplateRequestModel requestModel)
        {
            if (requestModel.NotificationTypeID < 0)
            {
                _errorMessages.Add("Sự kiện gửi email là thông tin bắt buộc.");
            }
            if (string.IsNullOrWhiteSpace(requestModel.EmailSubject))
            {
                _errorMessages.Add("Chủ đề email là thông tin bắt buộc.");
            }
            if (string.IsNullOrWhiteSpace(requestModel.EmailContent))
            {
                _errorMessages.Add("Nội dung email là thông tin bắt buộc.");
            }
            if (requestModel.FactoryID < 1)
            {
                _errorMessages.Add(CommonConstant.FACTORY_REQUIRED);
            }

            return _errorMessages.Count <= 0;
        }
    }
}
