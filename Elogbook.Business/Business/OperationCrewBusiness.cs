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
    public class OperationCrewBusiness : IOperationCrewBusiness
    {
        private readonly IOperationCrewService _operationCrewService;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userService"></param>
        public OperationCrewBusiness(IOperationCrewService operationCrewService)
        {
            _operationCrewService = operationCrewService;
        }

        public ResponseModel<OperationCrewModel> AddOperationCrew(AddEditOperationCrewModel requestModel)
        {
            var responseModel = new ResponseModel<OperationCrewModel>();
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
                    if (!_operationCrewService.CheckDuplicateData(0, requestModel.OperationCrewName, requestModel.FactoryID, true))
                    {
                        responseModel.Data = _operationCrewService.AddOperationCrew(requestModel);
                    }
                    else
                    {
                        responseModel.ErrorMessages.Add(CommonConstant.DUPLICATE_OPERATION_CREW);
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                    }
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationCrewBusiness.ToString(), 
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> CheckInUseOperationCrew(int id)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _operationCrewService.CheckInUseOperationCrew(id);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationCrewBusiness.ToString(), 
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> DeleteOperationCrew(int id, int userID)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                if (!_operationCrewService.CheckInUseOperationCrew(id))
                {
                    responseModel.Data = _operationCrewService.DeleteOperationCrew(id, userID);
                }
                else
                {
                    responseModel.Data = false;
                    responseModel.ErrorMessages.Add(CommonConstant.MSG_CANNOT_DELETE_OPERATION_CREW);
                }
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationCrewBusiness.ToString(), 
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<OperationCrewModel> GetOperationCrewById(int id)
        {
            var responseModel = new ResponseModel<OperationCrewModel>();

            try
            {
                responseModel.Data = _operationCrewService.GetOperationCrewById(id);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationCrewBusiness.ToString(), 
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseBaseModel GetOperationCrews(SearchOperationCrewModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _operationCrewService.GetOperationCrews(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationCrewBusiness.ToString(), 
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseModel<OperationCrewModel> UpdateOperationCrew(AddEditOperationCrewModel requestModel)
        {
            var responseModel = new ResponseModel<OperationCrewModel>();
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
                    if (!_operationCrewService.CheckDuplicateData(requestModel.ID, requestModel.OperationCrewName, requestModel.FactoryID, false))
                    {
                        responseModel.Data = _operationCrewService.UpdateOperationCrew(requestModel);
                    }
                    else
                    {
                        responseModel.ErrorMessages.Add(CommonConstant.DUPLICATE_OPERATION_CREW);
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                    }
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.OperationCrewBusiness.ToString(), 
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Kiểm tra dữ liệu bắt buộc khi tạo mới/cập nhật Kíp vận hành
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        private bool IsValidData(AddEditOperationCrewModel requestModel)
        {
            if (string.IsNullOrWhiteSpace(requestModel.OperationCrewName))
            {
                _errorMessages.Add(CommonConstant.OPERATION_CREW_NAME_REQUIRED);
            }
            if (requestModel.FactoryID < 1)
            {
                _errorMessages.Add(CommonConstant.FACTORY_REQUIRED);
            }

            return _errorMessages.Count <= 0;
        }
    }
}
