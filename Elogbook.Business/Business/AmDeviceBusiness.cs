using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Elogbook.Core.Models.Response.AmDevice;
using System;
using System.Collections.Generic;
using System.Net;

namespace Elogbook.Business
{
    /// <summary>
    /// DeviceBusiness
    /// </summary>
    public class AmDeviceBusiness : IAmDeviceBusiness
    {
        private readonly IAmDeviceService _deviceService;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="deviceGroupService"></param>
        public AmDeviceBusiness(IAmDeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        public ResponseModel<bool> CreateDevice(AmDeviceAddEditModel requestModel)
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
                    if (_deviceService.CheckDuplicateDevice(requestModel.AssetDesc, false))
                    {
                        responseModel.ErrorMessages.Add(string.Format(CommonConstant.MSG_ERROR_AM_NAME_DUPLICATE, "thiết bị"));
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                        return responseModel;
                    }

                    if (_deviceService.CheckDuplicateDevice(requestModel.AssetCode, true))
                    {
                        responseModel.ErrorMessages.Add(string.Format(CommonConstant.MSG_ERROR_AM_CODE_DUPLICATE, "thiết bị"));
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                        return responseModel;
                    }

                    return _deviceService.CreateDevice(requestModel);
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmDeviceBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseBaseModel DeleteDevice(int id, int userID)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                return _deviceService.DeleteDevice(id, userID);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmDeviceBusiness.ToString(),
                        Environment.NewLine + CommonFunction.SerializeObjectToString(id) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseModel<AmDeviceModel> GetDeviceById(int id)
        {
            var responseModel = new ResponseModel<AmDeviceModel>();

            try
            {
                responseModel.Data = _deviceService.GetDeviceById(id);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmDeviceBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseBaseModel GetDevices(AmDeviceSearchModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _deviceService.GetDevices(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmDeviceBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseModel<bool> UpdateDevice(AmDeviceAddEditModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                return _deviceService.UpdateDevice(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmDeviceBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseBaseModel SyncDataPmis()
        {
            ResponseBaseModel responseModel = new ResponseBaseModel();

            try
            {
                return _deviceService.SyncDataPmis();
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmDeviceBusiness.ToString(), Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public List<ComboboxDataModel> GetDeviceByLocationID(int locationID)
        {
            try
            {
                return _deviceService.GetDeviceByLocationID(locationID);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmDeviceBusiness.ToString(), ex.ToString());
                return new List<ComboboxDataModel>();
            }
        }

        /// <summary>
        /// Danh sách Thiết bị cha
        /// </summary>
        /// <param name="locationID"></param>
        /// <param name="deviceTypeID"></param>
        /// <returns></returns>
        public List<ComboboxDataModel> GetParent(int locationID, int deviceTypeID)
        {
            try
            {
                return _deviceService.GetParent(locationID, deviceTypeID);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmDeviceBusiness.ToString(), ex.ToString());
                return new List<ComboboxDataModel>();
            }
        }

        private bool IsValidData(AmDeviceAddEditModel requestModel)
        {
            // Khu vực
            if (requestModel.SiteID < 1)
            {
                _errorMessages.Add(CommonConstant.LOCATION_ID_REQUIRED);
            }

            // Tên thiết bị là bắt buộc
            if (string.IsNullOrWhiteSpace(requestModel.AssetDesc))
            {
                _errorMessages.Add(CommonConstant.DEVICE_TYPE_NAME_REQUIRED);
            }

            // Mã thiết bị
            if (string.IsNullOrWhiteSpace(requestModel.AssetCode))
            {
                _errorMessages.Add(CommonConstant.DEVICE_CODE_REQUIRED);
            }

            // Tình trạng thiết bị
            if (requestModel.DeviceStatusID < 1)
            {
                _errorMessages.Add(CommonConstant.DEVICE_STATUS_ID_REQUIRED);
            }

            // Số file hình ảnh không được vượt quá 5
            if (!string.IsNullOrWhiteSpace(requestModel.Images))
            {
                string[] lstIdImg = requestModel.Images.Split(',');

                if (lstIdImg.Length > 5)
                {
                    _errorMessages.Add(CommonConstant.IMAGE_LIMITED);
                }
            }

            return _errorMessages.Count <= 0;
        }

        public ResponseBaseModel QuickSearchDevices(AmDeviceQuickSearchModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _deviceService.QuickSearchDevices(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmDeviceBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseBaseModel GetDevicesHierarchy(AmDeviceSearchModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _deviceService.GetDevicesHierarchy(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmDeviceBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseBaseModel GetRootDevices(AmDeviceSearchModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _deviceService.GetRootDevices(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmDeviceBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseModel<List<BaseHierachyModel>> GetLocationDeviceHierarchy()
        {
            ResponseModel<List<BaseHierachyModel>> responseModel = new ResponseModel<List<BaseHierachyModel>>();

            try
            {
                responseModel.Data = _deviceService.GetLocationDeviceHierarchy();
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmDeviceBusiness.ToString(),
                    Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseBaseModel ChangeParent(ChangeParentLocationModel requestModel)
        {
            ResponseBaseModel responseModel = new ResponseBaseModel();

            try
            {
                return _deviceService.ChangeParent(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmDeviceBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<List<AmDeviceModel>> GetDeviceChildren(int id)
        {
            ResponseModel<List<AmDeviceModel>> responseModel = new ResponseModel<List<AmDeviceModel>>();

            try
            {
                responseModel.Data = _deviceService.GetDeviceChildren(id);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmDeviceBusiness.ToString(),
                    Environment.NewLine + $"ID: {id}" + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }
    }
}
