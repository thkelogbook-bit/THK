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
    public class OrganizationBusiness : IOrganizationBusiness
    {
        private readonly IOrganizationService _organizationService;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userService"></param>
        public OrganizationBusiness(IOrganizationService organizationService)
        {
            _organizationService = organizationService;
        }

        public ResponseModel<List<OrganizationModel>> GetOrganizations(SearchOrganizationModel requestModel)
        {
            var responseModel = new ResponseModel<List<OrganizationModel>>();

            try
            {
                responseModel.Data = _organizationService.GetOrganizations(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OrganizationBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<int> AddOrganization(AddEditOrganizationModel requestModel)
        {
            ResponseModel<int> responseModel = new ResponseModel<int>();
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
                    if (!_organizationService.CheckDuplicateData(requestModel.Level, requestModel.Name, requestModel.ShortName, 0, requestModel.ParentID, true))
                    {
                        responseModel.Data = _organizationService.AddOrganization(requestModel);
                    }
                    else
                    {
                        responseModel.ErrorMessages.Add(string.Format(CommonConstant.DUPLICATE_ORGANIZATION, requestModel.Level));
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                    }
                }
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = 0;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OrganizationBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> UpdateOrganization(AddEditOrganizationModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
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
                    if (!_organizationService.CheckDuplicateData(requestModel.Level, requestModel.Name, requestModel.ShortName, requestModel.ID, requestModel.ParentID, false))
                    {
                        responseModel.Data = _organizationService.UpdateOrganization(requestModel);
                    }
                    else
                    {
                        responseModel.ErrorMessages.Add(string.Format(CommonConstant.DUPLICATE_ORGANIZATION, requestModel.Level));
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                    }
                }
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OrganizationBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<int> DeleteOrganization(int id, int userID)
        {
            ResponseModel<int> responseModel = new ResponseModel<int>();

            try
            {
                var deleteStatus = _organizationService.DeleteOrganization(id, userID);
                responseModel.Data = (int)deleteStatus;
                switch (deleteStatus)
                {
                    case UpdateStatus.Success:
                        responseModel.ErrorMessages.Add("Xóa dữ liệu thành công.");
                        responseModel.StatusCode = (int)HttpStatusCode.OK;
                        break;
                    case UpdateStatus.InUse:
                        responseModel.ErrorMessages.Add(CommonConstant.CHECK_STATUS_USE_ORGANIZATION);
                        break;
                    case UpdateStatus.NotFound:
                        responseModel.ErrorMessages.Add("Không tìm thấy dữ liệu cần xóa.");
                        break;
                    case UpdateStatus.Error:
                        responseModel.ErrorMessages.Add("Xóa dữ liệu không thành công.");
                        break;
                }
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = (int)UpdateStatus.Error;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OrganizationBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<int> ChangeStatusOrganization(ChangeOrganizationStatusModel requestModel)
        {
            ResponseModel<int> responseModel = new ResponseModel<int>();

            try
            {
                var deleteStatus = _organizationService.ChangeStatusOrganization(requestModel);
                responseModel.Data = (int)deleteStatus;
                switch (deleteStatus)
                {
                    case UpdateStatus.Success:
                        responseModel.ErrorMessages.Add("Cập nhật trạng thái thành công.");
                        break;
                    case UpdateStatus.NotFound:
                        responseModel.ErrorMessages.Add("Không tìm thấy dữ liệu cần cập nhật trạng thái.");
                        break;
                    case UpdateStatus.Error:
                        responseModel.ErrorMessages.Add("Cập nhật trạng thái dữ liệu không thành công.");
                        break;
                }
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = (int)UpdateStatus.Error;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OrganizationBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        private bool IsValidData(AddEditOrganizationModel requestModel)
        {
            if (string.IsNullOrWhiteSpace(requestModel.Name))
            {
                _errorMessages.Add(string.Format(CommonConstant.ORGANIZATION_NAME, requestModel.Level));
            }
            if (string.IsNullOrWhiteSpace(requestModel.ShortName))
            {
                _errorMessages.Add(string.Format(CommonConstant.ORGANIZATION_SHORTNAME, requestModel.Level));
            }

            return _errorMessages.Count <= 0;
        }
    }
}
