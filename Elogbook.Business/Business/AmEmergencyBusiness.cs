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
    public class AmEmergencyBusiness : IAmEmergencyBusiness
    {
        private readonly IAmEmergencyService _emergencyService;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userService"></param>
        public AmEmergencyBusiness(IAmEmergencyService emergencyService)
        {
            _emergencyService = emergencyService;
        }

        public ResponseModel<AmEmergencyModel> Add(AmAddEditEmergencyModel requestModel)
        {
            var responseModel = new ResponseModel<AmEmergencyModel>();
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
                    if (!_emergencyService.CheckDuplicateData(requestModel, true))
                    {
                        responseModel.Data = _emergencyService.Add(requestModel);
                    }
                    else
                    {
                        responseModel.ErrorMessages.Add(string.Format(CommonConstant.MSG_ERROR_AM_NAME_DUPLICATE, "mức độ khẩn cấp"));
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                    }
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.EmergencyBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        private bool IsValidData(AmAddEditEmergencyModel requestModel)
        {
            if (string.IsNullOrWhiteSpace(requestModel.Name))
            {
                _errorMessages.Add(CommonConstant.EMERGENCY_MANAGEMENT_NAME_REQUIRED);
            }
            if (requestModel.FactoryID < 1)
            {
                _errorMessages.Add(CommonConstant.FACTORY_REQUIRED);
            }

            return _errorMessages.Count <= 0;
        }

        public ResponseModel<AmEmergencyModel> GetById(int id)
        {
            var responseModel = new ResponseModel<AmEmergencyModel>();

            try
            {
                responseModel.Data = _emergencyService.GetById(id);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.EmergencyBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseBaseModel Find(AmSearchEmergencyModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _emergencyService.Find(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.EmergencyBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseModel<AmEmergencyModel> Update(AmAddEditEmergencyModel requestModel)
        {
            var responseModel = new ResponseModel<AmEmergencyModel>();
            _errorMessages = new List<string>();

            try
            {
                if (!_emergencyService.CheckDuplicateData(requestModel, false))
                {
                    responseModel.Data = _emergencyService.Update(requestModel);
                }
                else
                {
                    responseModel.ErrorMessages.Add(string.Format(CommonConstant.MSG_ERROR_AM_NAME_DUPLICATE, "mức độ khẩn cấp"));
                    responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.EmergencyBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> Delete(int id, int currentUserId)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                if (_emergencyService.CheckDataIsUse(id))
                {
                    responseModel.ErrorMessages.Add("Mức độ khẩn cấp này đang được sử dụng không thể xóa");
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    responseModel.Data = false;
                    return responseModel;
                }
                responseModel.Data = _emergencyService.Delete(id, currentUserId);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.EmergencyBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }
    }
}
