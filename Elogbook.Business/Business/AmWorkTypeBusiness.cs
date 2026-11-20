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
    /// WorkTypeBusiness
    /// </summary>
    public class AmWorkTypeBusiness : IAmWorkTypeBusiness
    {
        private readonly IAmWorkTypeService _workTypeService;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="workTypeService"></param>
        public AmWorkTypeBusiness(IAmWorkTypeService workTypeService)
        {
            _workTypeService = workTypeService;
        }

        public ResponseModel<bool> CreateWorkType(AmWorkTypeAddEditModel requestModel)
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
                    if (!_workTypeService.CheckDuplicateWorkType(0, requestModel.TypeDesc, true, requestModel.FactoryID))
                    {
                        responseModel.Data = _workTypeService.CreateWorkType(requestModel);
                    }
                    else
                    {
                        responseModel.ErrorMessages.Add(string.Format(CommonConstant.MSG_ERROR_AM_NAME_DUPLICATE, "loại công việc"));
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                    }
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmWorkTypeBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseBaseModel DeleteWorkType(int id, int userID)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                return _workTypeService.DeleteWorkType(id, userID);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmWorkTypeBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public List<ComboboxDataModel> GetWorkTypeByFactoryID(int factoryID)
        {
            try
            {
                return _workTypeService.GetWorkTypeByFactoryID(factoryID);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmDeviceBusiness.ToString(), ex.ToString());
                return new List<ComboboxDataModel>();
            }
        }

        public ResponseModel<AmWorkTypeModel> GetWorkTypeById(int id)
        {
            var responseModel = new ResponseModel<AmWorkTypeModel>();

            try
            {
                responseModel.Data = _workTypeService.GetWorkTypeById(id);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmWorkTypeBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseBaseModel GetWorkTypes(AmWorkTypeSearchModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _workTypeService.GetWorkTypes(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmWorkTypeBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseModel<bool> UpdateWorkType(AmWorkTypeAddEditModel requestModel)
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
                    bool isExists = _workTypeService.CheckDuplicateWorkType(requestModel.ID, requestModel.TypeDesc, false, requestModel.FactoryID);
                    if (isExists)
                    {
                        responseModel.ErrorMessages.Add(string.Format(CommonConstant.MSG_ERROR_AM_NAME_DUPLICATE, "loại công việc"));
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                        responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                        return responseModel;
                    }
                }

                responseModel.Data = _workTypeService.UpdateWorkType(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmWorkTypeBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        private bool IsValidData(AmWorkTypeAddEditModel requestModel)
        {
            // WorkTypeName là bắt buộc
            if (string.IsNullOrWhiteSpace(requestModel.TypeDesc))
            {
                _errorMessages.Add(CommonConstant.WORK_TYPE_NAME_REQUIRED);
            }

            if (requestModel.FactoryID < 1)
            {
                _errorMessages.Add(CommonConstant.FACTORY_REQUIRED);
            }

            return _errorMessages.Count <= 0;
        }
    }
}
