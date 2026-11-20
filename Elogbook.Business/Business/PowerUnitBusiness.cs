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
    public class PowerUnitBusiness : IPowerUnitBusiness
    {
        private readonly IPowerUnitService _powerUnitService;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="powerUnitService"></param>
        public PowerUnitBusiness(IPowerUnitService powerUnitService)
        {
            _powerUnitService = powerUnitService;
        }

        public ResponseBaseModel GetPowerUnits(SearchPowerUnitModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _powerUnitService.GetPowerUnits(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.PowerUnitBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseModel<PowerUnitModel> GetPowerUnitById(int id)
        {
            var responseModel = new ResponseModel<PowerUnitModel>();

            try
            {
                responseModel.Data = _powerUnitService.GetPowerUnitById(id);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.PowerUnitBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<PowerUnitModel> AddPowerUnit(AddEditPowerUnitModel requestModel)
        {
            var responseModel = new ResponseModel<PowerUnitModel>();
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
                    if (!_powerUnitService.CheckDuplicateData(0, requestModel.PowerUnitName, requestModel.FactoryID, true))
                    {
                        responseModel.Data = _powerUnitService.AddPowerUnit(requestModel);
                    }
                    else
                    {
                        responseModel.ErrorMessages.Add(CommonConstant.DUPLICATE_POWERUNIT);
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                    }
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.PowerUnitBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<PowerUnitModel> UpdatePowerUnit(AddEditPowerUnitModel requestModel)
        {
            var responseModel = new ResponseModel<PowerUnitModel>();
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
                    if (!_powerUnitService.CheckDuplicateData(requestModel.ID, requestModel.PowerUnitName, requestModel.FactoryID, false))
                    {
                        responseModel.Data = _powerUnitService.UpdatePowerUnit(requestModel);
                    }
                    else
                    {
                        responseModel.ErrorMessages.Add(CommonConstant.DUPLICATE_POWERUNIT);
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                    }
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.PowerUnitBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> DeletePowerUnit(int id, int userID)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _powerUnitService.DeletePowerUnit(id, userID);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.PowerUnitBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> CheckInUsePowerUnit(int id)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _powerUnitService.CheckInUsePowerUnit(id);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.PowerUnitBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Kiểm tra dữ liệu bắt buộc khi tạo mới/cập nhật Tổ máy
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        private bool IsValidData(AddEditPowerUnitModel requestModel)
        {
            if (string.IsNullOrWhiteSpace(requestModel.PowerUnitName))
            {
                _errorMessages.Add(CommonConstant.POWERUNITNAME_REQUIRED);
            }
            if (requestModel.FactoryID < 1)
            {
                _errorMessages.Add(CommonConstant.FACTORY_REQUIRED);
            }

            return _errorMessages.Count <= 0;
        }
    }
}
