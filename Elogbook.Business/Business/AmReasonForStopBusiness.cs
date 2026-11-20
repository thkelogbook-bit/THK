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
    /// ReasonForStopBusiness
    /// </summary>
    public class AmReasonForStopBusiness : IAmReasonForStopBusiness
    {
        private readonly IAmReasonForStopService _amReasonForStop;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="amReasonForStop"></param>
        public AmReasonForStopBusiness(IAmReasonForStopService amReasonForStop)
        {
            _amReasonForStop = amReasonForStop;
        }

        public ResponseModel<bool> CreateReasonForStop(AmReasonForStopAddEditModel requestModel)
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
                    if (!_amReasonForStop.CheckDuplicateReasonForStop(0, requestModel.ReasonDesc, true, requestModel.FactoryID))
                    {
                        responseModel.Data = _amReasonForStop.CreateReasonForStop(requestModel);
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
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmReasonForStopBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseBaseModel DeleteReasonForStop(int id, int userID)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                return _amReasonForStop.DeleteReasonForStop(id, userID);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmReasonForStopBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseModel<AmReasonForStopModel> GetReasonForStopById(int id)
        {
            var responseModel = new ResponseModel<AmReasonForStopModel>();

            try
            {
                responseModel.Data = _amReasonForStop.GetReasonForStopById(id);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmReasonForStopBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseBaseModel GetReasonForStops(AmReasonForStopSearchModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _amReasonForStop.GetReasonForStops(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmReasonForStopBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseModel<bool> UpdateReasonForStop(AmReasonForStopAddEditModel requestModel)
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

                if (requestModel.ReasonType > 2)
                {
                    _errorMessages.Add("Loại nguyên nhân không hợp lệ");
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseModel;
                }

                if (requestModel.Active > 2)
                {
                    _errorMessages.Add("Trạng thái không hợp lệ");
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseModel;
                }

                if (requestModel.FactoryID > 0)
                {
                    bool facInfo = _amReasonForStop.CheckFactoryExist(requestModel.FactoryID);

                    if (!facInfo)
                    {
                        _errorMessages.Add("Nhà máy không tồn tại");
                        responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                        return responseModel;
                    }

                    bool isExists = _amReasonForStop.CheckDuplicateReasonForStop(requestModel.ID, requestModel.ReasonDesc, false, requestModel.FactoryID);
                    if (isExists)
                    {
                        responseModel.ErrorMessages.Add(string.Format(CommonConstant.MSG_ERROR_AM_NAME_DUPLICATE, "nguyên nhân"));
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                        responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                        return responseModel;
                    }
                }

                responseModel.Data = _amReasonForStop.UpdateReasonForStop(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmReasonForStopBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        private bool IsValidData(AmReasonForStopAddEditModel requestModel)
        {
            // ReasonForStopName là bắt buộc
            if (string.IsNullOrWhiteSpace(requestModel.ReasonDesc))
            {
                _errorMessages.Add(CommonConstant.REASON_STOP_REQUIRED);
            }

            if (requestModel.FactoryID < 1)
            {
                _errorMessages.Add(CommonConstant.FACTORY_REQUIRED);
            }
            else
            {
                bool facInfo = _amReasonForStop.CheckFactoryExist(requestModel.FactoryID);

                if (!facInfo)
                {
                    _errorMessages.Add("Nhà máy không tồn tại");
                }

            }

            if (requestModel.ReasonType > 2 || requestModel.ReasonType < 1)
            {
                _errorMessages.Add(CommonConstant.REASON_TYPE_REQUIRED);
            }

            if (requestModel.Active > 2 || requestModel.ReasonType < 1)
            {
                _errorMessages.Add("Trạng thái không hợp lệ");
            }

            return _errorMessages.Count <= 0;
        }
    }
}
