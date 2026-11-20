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
    /// <summary>
    /// ReasonForPendingBusiness
    /// </summary>
    public class AmReasonForPendingBusiness : IAmReasonForPendingBusiness
    {
        private readonly IAmReasonForPendingService _amReasonForPending;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="amReasonForPending"></param>
        public AmReasonForPendingBusiness(IAmReasonForPendingService amReasonForPending)
        {
            _amReasonForPending = amReasonForPending;
        }

        public ResponseModel<bool> CreateReasonForPending(AmReasonForPendingAddEditModel requestModel)
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
                    if (!_amReasonForPending.CheckDuplicateReasonForPending(0, requestModel.ReasonDesc, true, requestModel.FactoryID))
                    {
                        responseModel.Data = _amReasonForPending.CreateReasonForPending(requestModel);
                    }
                    else
                    {
                        responseModel.ErrorMessages.Add(string.Format(CommonConstant.MSG_ERROR_AM_NAME_DUPLICATE, "nguyên nhân"));
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                    }
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmReasonForPendingBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseBaseModel DeleteReasonForPending(int id, int userID)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                return _amReasonForPending.DeleteReasonForPending(id, userID);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmReasonForPendingBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseModel<AmReasonForPendingModel> GetReasonForPendingById(int id)
        {
            var responseModel = new ResponseModel<AmReasonForPendingModel>();

            try
            {
                responseModel.Data = _amReasonForPending.GetReasonForPendingById(id);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmReasonForPendingBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseBaseModel GetReasonForPendings(AmReasonForPendingSearchModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _amReasonForPending.GetReasonForPendings(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmReasonForPendingBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseModel<bool> UpdateReasonForPending(AmReasonForPendingAddEditModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            _errorMessages = new List<string>();

            try
            {
                if (requestModel.ID < 1)
                {
                    _errorMessages.Add(CommonConstant.WORK_TYPE_ID_REQUIRED);
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseModel;
                }

                if (requestModel.FactoryID > 0)
                {
                    bool isExists = _amReasonForPending.CheckDuplicateReasonForPending(requestModel.ID, requestModel.ReasonDesc, false, requestModel.FactoryID);
                    if (isExists)
                    {
                        responseModel.ErrorMessages.Add(string.Format(CommonConstant.MSG_ERROR_AM_NAME_DUPLICATE, "nguyên nhân"));
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                        responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                        return responseModel;
                    }
                }

                responseModel.Data = _amReasonForPending.UpdateReasonForPending(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmReasonForPendingBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        private bool IsValidData(AmReasonForPendingAddEditModel requestModel)
        {
            // ReasonForPendingName là bắt buộc
            if (string.IsNullOrWhiteSpace(requestModel.ReasonDesc))
            {
                _errorMessages.Add(CommonConstant.REASON_STOP_REQUIRED);
            }

            if (requestModel.FactoryID < 1)
            {
                _errorMessages.Add(CommonConstant.FACTORY_REQUIRED);
            }

            return _errorMessages.Count <= 0;
        }
    }
}
