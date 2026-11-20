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
    /// ManufacturerBusiness
    /// </summary>
    public class AmManufacturerBusiness : IAmManufacturerBusiness
    {
        private readonly IAmManufacturerService _manufacturerService;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ManufacturerService"></param>
        public AmManufacturerBusiness(IAmManufacturerService ManufacturerService)
        {
            _manufacturerService = ManufacturerService;
        }

        public ResponseModel<bool> CreateManufacturer(AmManufacturerAddEditModel requestModel)
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
                    if (_manufacturerService.CheckDuplicateManufacturer(requestModel.ManufacturerName, false))
                    {
                        responseModel.ErrorMessages.Add(string.Format(CommonConstant.MSG_ERROR_AM_NAME_DUPLICATE, "nhà chế tạo"));
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                        return responseModel;
                    }

                    if (_manufacturerService.CheckDuplicateManufacturer(requestModel.ManufacturerCode, true))
                    {
                        responseModel.ErrorMessages.Add(string.Format(CommonConstant.MSG_ERROR_AM_CODE_DUPLICATE, "nhà chế tạo"));
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                        return responseModel;
                    }

                    responseModel.Data = _manufacturerService.CreateManufacturer(requestModel);
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmManufacturerBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseBaseModel DeleteManufacturer(int id, int userID)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                return _manufacturerService.DeleteManufacturer(id, userID);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmManufacturerBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseModel<AmManufacturerModel> GetManufacturerById(int id)
        {
            var responseModel = new ResponseModel<AmManufacturerModel>();

            try
            {
                responseModel.Data = _manufacturerService.GetManufacturerById(id);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmManufacturerBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseBaseModel GetManufacturers(AmManufacturerSearchModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _manufacturerService.GetManufacturers(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmManufacturerBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseModel<bool> UpdateManufacturer(AmManufacturerAddEditModel requestModel)
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

                responseModel.Data = _manufacturerService.UpdateManufacturer(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmManufacturerBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseBaseModel SyncDataPmis()
        {
            ResponseBaseModel responseModel = new ResponseBaseModel();

            try
            {
                return _manufacturerService.SyncDataPmis();
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmManufacturerBusiness.ToString(), Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        private bool IsValidData(AmManufacturerAddEditModel requestModel)
        {
            // ManufacturerName là bắt buộc
            if (string.IsNullOrWhiteSpace(requestModel.ManufacturerName))
            {
                _errorMessages.Add(CommonConstant.DEVICE_TYPE_NAME_REQUIRED);
            }

            if (string.IsNullOrWhiteSpace(requestModel.ManufacturerCode))
            {
                _errorMessages.Add(CommonConstant.DEVICE_TYPE_CODE_REQUIRED);
            }

            return _errorMessages.Count <= 0;
        }

        public List<ComboboxDataModel> GetAllManufacturer()
        {
            try
            {
                return _manufacturerService.GetAllManufacturer();
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmManufacturerBusiness.ToString(), ex.ToString());
                return new List<ComboboxDataModel>();
            }
        }
    }
}
