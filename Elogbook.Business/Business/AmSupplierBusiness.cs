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
    /// SupplierBusiness
    /// </summary>
    public class AmSupplierBusiness : IAmSupplierBusiness
    {
        private readonly IAmSupplierService _supplierService;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="SupplierService"></param>
        public AmSupplierBusiness(IAmSupplierService SupplierService)
        {
            _supplierService = SupplierService;
        }

        public ResponseModel<bool> CreateSupplier(AmSupplierAddEditModel requestModel)
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
                    if (_supplierService.CheckDuplicateSupplier(requestModel.SupplierName, false))
                    {
                        responseModel.ErrorMessages.Add(string.Format(CommonConstant.MSG_ERROR_AM_NAME_DUPLICATE, "nhà cung cấp"));
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                        return responseModel;
                    }

                    if (_supplierService.CheckDuplicateSupplier(requestModel.SupplierCode, true))
                    {
                        responseModel.ErrorMessages.Add(string.Format(CommonConstant.MSG_ERROR_AM_CODE_DUPLICATE, "nhà cung cấp"));
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                        return responseModel;
                    }

                    responseModel.Data = _supplierService.CreateSupplier(requestModel);
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmSupplierBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseBaseModel DeleteSupplier(int id, int userID)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                return _supplierService.DeleteSupplier(id, userID);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmSupplierBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseModel<AmSupplierModel> GetSupplierById(int id)
        {
            var responseModel = new ResponseModel<AmSupplierModel>();

            try
            {
                responseModel.Data = _supplierService.GetSupplierById(id);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmSupplierBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseBaseModel GetSuppliers(AmSupplierSearchModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _supplierService.GetSuppliers(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmSupplierBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseModel<bool> UpdateSupplier(AmSupplierAddEditModel requestModel)
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

                responseModel.Data = _supplierService.UpdateSupplier(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmSupplierBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseBaseModel SyncDataPmis()
        {
            ResponseBaseModel responseModel = new ResponseBaseModel();

            try
            {
                return _supplierService.SyncDataPmis();
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmSupplierBusiness.ToString(), Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        private bool IsValidData(AmSupplierAddEditModel requestModel)
        {
            // SupplierName là bắt buộc
            if (string.IsNullOrWhiteSpace(requestModel.SupplierName))
            {
                _errorMessages.Add(CommonConstant.DEVICE_TYPE_NAME_REQUIRED);
            }

            if (string.IsNullOrWhiteSpace(requestModel.SupplierCode))
            {
                _errorMessages.Add(CommonConstant.DEVICE_TYPE_CODE_REQUIRED);
            }

            return _errorMessages.Count <= 0;
        }

        public List<ComboboxDataModel> GetAllSupplier()
        {
            try
            {
                return _supplierService.GetAllSupplier();
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmSupplierBusiness.ToString(), ex.ToString());
                return new List<ComboboxDataModel>();
            }
        }
    }
}
