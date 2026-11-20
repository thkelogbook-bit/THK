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
    /// CountryManufactureBusiness
    /// </summary>
    public class AmCountryManufactureBusiness : IAmCountryManufactureBusiness
    {
        private readonly IAmCountryManufactureService _countryManufactureService;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="CountryManufactureService"></param>
        public AmCountryManufactureBusiness(IAmCountryManufactureService CountryManufactureService)
        {
            _countryManufactureService = CountryManufactureService;
        }

        public ResponseModel<bool> CreateCountryManufacture(AmCountryManufactureAddEditModel requestModel)
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
                    if (_countryManufactureService.CheckDuplicateCountryManufacture(requestModel.CountryName, false))
                    {
                        responseModel.ErrorMessages.Add(string.Format(CommonConstant.MSG_ERROR_AM_NAME_DUPLICATE, "nước sản xuất"));
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                        return responseModel;
                    }

                    if (_countryManufactureService.CheckDuplicateCountryManufacture(requestModel.CountryCode, true))
                    {
                        responseModel.ErrorMessages.Add(string.Format(CommonConstant.MSG_ERROR_AM_CODE_DUPLICATE, "nước sản xuất"));
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                        return responseModel;
                    }

                    responseModel.Data = _countryManufactureService.CreateCountryManufacture(requestModel);
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmCountryManufactureBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseBaseModel DeleteCountryManufacture(int id, int userID)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                return _countryManufactureService.DeleteCountryManufacture(id, userID);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmCountryManufactureBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseModel<AmCountryManufactureModel> GetCountryManufactureById(int id)
        {
            var responseModel = new ResponseModel<AmCountryManufactureModel>();

            try
            {
                responseModel.Data = _countryManufactureService.GetCountryManufactureById(id);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmCountryManufactureBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseBaseModel GetCountryManufactures(AmCountryManufactureSearchModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _countryManufactureService.GetCountryManufactures(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmCountryManufactureBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseModel<bool> UpdateCountryManufacture(AmCountryManufactureAddEditModel requestModel)
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

                responseModel.Data = _countryManufactureService.UpdateCountryManufacture(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmCountryManufactureBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseBaseModel SyncDataPmis()
        {
            ResponseBaseModel responseModel = new ResponseBaseModel();

            try
            {
                return _countryManufactureService.SyncDataPmis();
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmCountryManufactureBusiness.ToString(), Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        private bool IsValidData(AmCountryManufactureAddEditModel requestModel)
        {
            // CountryManufactureName là bắt buộc
            if (string.IsNullOrWhiteSpace(requestModel.CountryName))
            {
                _errorMessages.Add(CommonConstant.DEVICE_TYPE_NAME_REQUIRED);
            }

            if (string.IsNullOrWhiteSpace(requestModel.CountryCode))
            {
                _errorMessages.Add(CommonConstant.DEVICE_TYPE_CODE_REQUIRED);
            }

            return _errorMessages.Count <= 0;
        }

        public List<ComboboxDataModel> GetAllCountryManufacture()
        {
            try
            {
                return _countryManufactureService.GetAllCountryManufacture();
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmCountryManufactureBusiness.ToString(), ex.ToString());
                return new List<ComboboxDataModel>();
            }
        }
    }
}
