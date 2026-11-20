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
using System.Net.Http;

namespace Elogbook.Business
{
    /// <summary>
    /// DeviceTypeBusiness
    /// </summary>
    public class AmDeviceTypeBusiness : IAmDeviceTypeBusiness
    {
        private readonly IAmDeviceTypeService _deviceTypeService;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="deviceTypeService"></param>
        public AmDeviceTypeBusiness(IAmDeviceTypeService deviceTypeService)
        {
            _deviceTypeService = deviceTypeService;
        }

        public ResponseModel<bool> CreateDeviceType(AmDeviceTypeAddEditModel requestModel)
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
                    if (_deviceTypeService.CheckDuplicateDeviceType(requestModel.TypeDesc, false))
                    {
                        responseModel.ErrorMessages.Add(string.Format(CommonConstant.MSG_ERROR_AM_NAME_DUPLICATE, "loại thiết bị"));
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                        return responseModel;
                    }

                    if (_deviceTypeService.CheckDuplicateDeviceType(requestModel.TypeCode, true))
                    {
                        responseModel.ErrorMessages.Add(string.Format(CommonConstant.MSG_ERROR_AM_CODE_DUPLICATE, "loại thiết bị"));
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                        return responseModel;
                    }

                    responseModel.Data = _deviceTypeService.CreateDeviceType(requestModel);
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmDeviceTypeBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseBaseModel DeleteDeviceType(int id, int userID)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                return _deviceTypeService.DeleteDeviceType(id, userID);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmDeviceTypeBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseModel<AmDeviceTypeModel> GetDeviceTypeById(int id)
        {
            var responseModel = new ResponseModel<AmDeviceTypeModel>();

            try
            {
                responseModel.Data = _deviceTypeService.GetDeviceTypeById(id);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmDeviceTypeBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseBaseModel GetDeviceTypes(AmDeviceTypeSearchModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _deviceTypeService.GetDeviceTypes(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmDeviceTypeBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseModel<bool> UpdateDeviceType(AmDeviceTypeAddEditModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            _errorMessages = new List<string>();

            try
            {
                if (requestModel.ID < 1)
                {
                    _errorMessages.Add(CommonConstant.DEVICE_TYPE_ID_REQUIRED);
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseModel;
                }

                responseModel.Data = _deviceTypeService.UpdateDeviceType(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmDeviceTypeBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseBaseModel SyncDataPmis()
        {
            ResponseBaseModel responseModel = new ResponseBaseModel();

            try
            {
                return _deviceTypeService.SyncDataPmis();
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmDeviceTypeBusiness.ToString(), Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        private bool IsValidData(AmDeviceTypeAddEditModel requestModel)
        {
            // DeviceTypeName là bắt buộc
            if (string.IsNullOrWhiteSpace(requestModel.TypeDesc))
            {
                _errorMessages.Add(CommonConstant.DEVICE_TYPE_NAME_REQUIRED);
            }

            if (string.IsNullOrWhiteSpace(requestModel.TypeCode))
            {
                _errorMessages.Add(CommonConstant.DEVICE_TYPE_CODE_REQUIRED);
            }

            return _errorMessages.Count <= 0;
        }

        public List<ComboboxDataModel> GetAllDeviceType()
        {
            try
            {
                return _deviceTypeService.GetAllDeviceType();
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmDeviceTypeBusiness.ToString(), ex.ToString());
                return new List<ComboboxDataModel>();
            }
        }
    }
}
