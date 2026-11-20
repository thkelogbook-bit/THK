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
    public class AutonomousMaintenanceBusiness : IAutonomousMaintenanceBusiness
    {
        private readonly IAutonomousMaintenanceService _autonomousMaintenanceService;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="majorRepairService"></param>
        public AutonomousMaintenanceBusiness(IAutonomousMaintenanceService autonomousMaintenanceService)
        {
            _autonomousMaintenanceService = autonomousMaintenanceService;
        }

        public int CreateAutonomousMaintenance()
        {
            try
            {
                return _autonomousMaintenanceService.CreateAutonomousMaintenance();
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AutonomousMaintenanceBusiness.ToString(), ex.ToString());
                return 0;
            }
        }

        public ResponseModel<AutonomousMaintenanceModel> GetAutonomousMaintenanceById(int id)
        {
            var responseModel = new ResponseModel<AutonomousMaintenanceModel>();

            try
            {
                responseModel.Data = _autonomousMaintenanceService.GetAutonomousMaintenanceById(id);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AutonomousMaintenanceBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseBaseModel GetAutonomousMaintenances(AutonomousMaintenanceSearchModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _autonomousMaintenanceService.GetAutonomousMaintenances(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AutonomousMaintenanceBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseModel<bool> CreateAmStopMaintenance(AmStopMaintenanceAddEditModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            _errorMessages = new List<string>();

            try
            {
                if (!IsValidDataAmStopMaintenance(requestModel))
                {
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                else
                {
                    return _autonomousMaintenanceService.CreateAmStopMaintenance(requestModel);
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AutonomousMaintenanceBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<AmWorkDetailModel> GetAmWorkDetailById(int id)
        {
            var responseModel = new ResponseModel<AmWorkDetailModel>();

            try
            {
                responseModel.Data = _autonomousMaintenanceService.GetAmWorkDetailById(id);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AutonomousMaintenanceBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<AmWorkDetailModel> WorkProcessing(AmWorkingProgressAddModel requestModel)
        {
            var responseModel = new ResponseModel<AmWorkDetailModel>();
            _errorMessages = new List<string>();
            try
            {
                if (!IsValidAmWorkingProgress(requestModel))
                {
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                else
                {
                    return _autonomousMaintenanceService.WorkProcessing(requestModel);
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AutonomousMaintenanceBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<AmWorkPendingModel> GetAmWorkPendingById(int amWorkDetailID)
        {
            var responseModel = new ResponseModel<AmWorkPendingModel>();

            try
            {
                responseModel.Data = _autonomousMaintenanceService.GetAmWorkPendingById(amWorkDetailID);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AutonomousMaintenanceBusiness.ToString(),
                    Environment.NewLine + $"ID: {amWorkDetailID} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> UpdateDeviceAbnormal(AmDeviceAbnormalUpdateModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                return _autonomousMaintenanceService.UpdateDeviceAbnormal(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AutonomousMaintenanceBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> SignatureCompleteAm(AmSignatureCompleteModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                if (requestModel.CompleterUserID.HasValue)
                {
                    return _autonomousMaintenanceService.SignatureCompleteAm(requestModel);
                }
                else
                {
                    responseModel.Data = false;
                    responseModel.ErrorMessages.Add(CommonConstant.COMPLETE_USERID_REQUIRED);
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AutonomousMaintenanceBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<AmStopMaintenanceModel> GetAmStopMaintenanceById(int autonomousMaintenanceID)
        {
            var responseModel = new ResponseModel<AmStopMaintenanceModel>();

            try
            {
                responseModel.Data = _autonomousMaintenanceService.GetAmStopMaintenanceById(autonomousMaintenanceID);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AutonomousMaintenanceBusiness.ToString(),
                    Environment.NewLine + $"ID: {autonomousMaintenanceID} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public UploadFileModel UploadFile(FileInfoModel fileInfoModel)
        {
            try
            {
                return _autonomousMaintenanceService.UploadFile(fileInfoModel);
            }
            catch (Exception ex)
            {
                return new UploadFileModel()
                {
                    Description = ex.Message,
                    IsError = true
                };
            }
        }

        /// <summary>
        /// lịch sử vận hành sửa chữa
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public ResponseBaseModel FindHistory(int deviceId)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _autonomousMaintenanceService.FindHistory(deviceId);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AutonomousMaintenanceBusiness.ToString(),
                    Environment.NewLine + "DeviceId:" + deviceId + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public int SendMailNotificationAm()
        {
            try
            {
                return _autonomousMaintenanceService.SendMailNotificationAm();
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AutonomousMaintenanceBusiness.ToString(), ex.ToString());
                return 0;
            }
        }

        public int AutoCompleteAm()
        {
            try
            {
                return _autonomousMaintenanceService.AutoCompleteAm();
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AutonomousMaintenanceBusiness.ToString(), ex.ToString());
                return 0;
            }
        }

        private bool IsValidAmWorkingProgress(AmWorkingProgressAddModel requestModel)
        {
            if (requestModel.AutonomousMaintenanceID < 1)
            {
                _errorMessages.Add(CommonConstant.AM_WORKING_REQUIRED);
            }

            if (requestModel.WorkDetailID < 1)
            {
                _errorMessages.Add(CommonConstant.AM_WORKDETAIL_REQUIRED);
            }

            return _errorMessages.Count <= 0;
        }

        private bool IsValidDataAmStopMaintenance(AmStopMaintenanceAddEditModel requestModel)
        {
            if (requestModel.AutonomousMaintenanceID < 1)
            {
                _errorMessages.Add(CommonConstant.AM_REQUIRED);
            }

            if (requestModel.ReasonForStopID < 1)
            {
                _errorMessages.Add(CommonConstant.AM_REASON_STOP_REQUIRED);
            }

            if (requestModel.ImplementerUserID < 1)
            {
                _errorMessages.Add(CommonConstant.IMPLEMENT_USERID_REQUIRED);
            }

            return _errorMessages.Count <= 0;
        }

        public ResponseBaseModel SaveEvidenceAm(SaveImageInfoModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                return _autonomousMaintenanceService.SaveEvidenceAm(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AutonomousMaintenanceBusiness.ToString(),
                        Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }
    }
}
