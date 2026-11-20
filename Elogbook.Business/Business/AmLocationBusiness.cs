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
    /// xử lý nghiệp vụ
    /// </summary>
    public class AmLocationBusiness : IAmLocationBusiness
    {
        private readonly IAmLocationService _locationService;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="locationService"></param>
        public AmLocationBusiness(IAmLocationService locationService)
        {
            _locationService = locationService;
        }

        public ResponseModel<bool> CreateLocation(AmLocationAddEditModel requestModel)
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
                    if (_locationService.CheckDuplicateLocation(requestModel.SiteDesc, false))
                    {
                        responseModel.ErrorMessages.Add(string.Format(CommonConstant.MSG_ERROR_AM_NAME_DUPLICATE, "khu vực"));
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                        return responseModel;
                    }
                    if (_locationService.CheckDuplicateLocation(requestModel.SiteCode, true))
                    {
                        responseModel.ErrorMessages.Add(string.Format(CommonConstant.MSG_ERROR_AM_CODE_DUPLICATE, "khu vực"));
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                        return responseModel;
                    }

                    responseModel.Data = _locationService.CreateLocation(requestModel);
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmLocationBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseBaseModel DeleteLocation(int id, int userID)
        {
            ResponseBaseModel responseModel = new ResponseBaseModel();

            try
            {
                return _locationService.DeleteLocation(id, userID);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmLocationBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public List<ComboboxDataModel> GetLocationByCompanyID(int companyID, bool isParent)
        {
            try
            {
                return _locationService.GetLocationByCompanyID(companyID, isParent);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmLocationBusiness.ToString(), ex.ToString());
                return new List<ComboboxDataModel>();
            }
        }

        /// <summary>
        /// get parent location
        /// </summary>
        /// <param name="companyID"></param>
        /// <returns></returns>
        public List<ComboboxDataModel> GetParent(int companyID)
        {
            try
            {
                return _locationService.GetParent(companyID);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmLocationBusiness.ToString(), ex.ToString());
                return new List<ComboboxDataModel>();
            }
        }

        public ResponseModel<AmLocationModel> GetLocationById(int id)
        {
            var responseModel = new ResponseModel<AmLocationModel>();

            try
            {
                responseModel.Data = _locationService.GetLocationById(id);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmLocationBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseBaseModel GetLocations(AmLocationSearchModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _locationService.GetLocations(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmLocationBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseBaseModel SyncDataPmis()
        {
            ResponseBaseModel responseModel = new ResponseBaseModel();

            try
            {
                return _locationService.SyncDataPmis();
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmLocationBusiness.ToString(), Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> UpdateLocation(AmLocationAddEditModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                return _locationService.UpdateLocation(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmLocationBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        private bool IsValidData(AmLocationAddEditModel requestModel)
        {
            // Tên khu vực là bắt buộc
            if (string.IsNullOrWhiteSpace(requestModel.SiteDesc))
            {
                _errorMessages.Add(CommonConstant.LOCATION_NAME_REQUIRED);
            }

            // Mã khu vực là bắt buộc
            if (string.IsNullOrWhiteSpace(requestModel.SiteCode))
            {
                _errorMessages.Add(CommonConstant.LOCATION_CODE_REQUIRED);
            }

            // Công ty là bắt buộc
            if (requestModel.OrganizationID < 1)
            {
                _errorMessages.Add(string.Format(CommonConstant.ORGANIZATION_ID_REQUIRED, "Công ty"));
            }

            return _errorMessages.Count <= 0;
        }
    }
}
