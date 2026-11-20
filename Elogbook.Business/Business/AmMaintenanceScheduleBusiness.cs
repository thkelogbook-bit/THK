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
    /// AmMaintenanceScheduleBusiness
    /// </summary>
    public class AmMaintenanceScheduleBusiness : IAmMaintenanceScheduleBusiness
    {
        private readonly IAmMaintenanceScheduleService _maintenanceScheduleService;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="maintenanceScheduleGroupService"></param>
        public AmMaintenanceScheduleBusiness(IAmMaintenanceScheduleService maintenanceScheduleService)
        {
            _maintenanceScheduleService = maintenanceScheduleService;
        }

        public int CreateAM(int maintenanceScheduleId)
        {
            try
            {
                return _maintenanceScheduleService.CreateAM(maintenanceScheduleId);
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmMaintenanceScheduleBusiness.ToString(), ex.ToString());
                return 0;
            }
        }

        public ResponseModel<int> CreateMaintenanceSchedule(AmMaintenanceScheduleAddEditModel requestModel)
        {
            ResponseModel<int> responseModel = new ResponseModel<int>();
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
                    if (!_maintenanceScheduleService.CheckDuplicateMaintenanceSchedule(0, requestModel.MaintenanceScheduleName, true, requestModel.FactoryID))
                    {
                        return _maintenanceScheduleService.CreateMaintenanceSchedule(requestModel);
                    }
                    else
                    {
                        responseModel.ErrorMessages.Add(string.Format(CommonConstant.MSG_ERROR_AM_NAME_DUPLICATE, "lịch bảo trì"));
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                    }
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmMaintenanceScheduleBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseBaseModel DeleteMaintenanceSchedule(int id, int userID)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                return _maintenanceScheduleService.DeleteMaintenanceSchedule(id, userID);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmMaintenanceScheduleBusiness.ToString(),
                        Environment.NewLine + CommonFunction.SerializeObjectToString(id) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseModel<AmMaintenanceScheduleModel> GetMaintenanceScheduleById(int id)
        {
            var responseModel = new ResponseModel<AmMaintenanceScheduleModel>();

            try
            {
                responseModel.Data = _maintenanceScheduleService.GetMaintenanceScheduleById(id);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmMaintenanceScheduleBusiness.ToString(),
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseBaseModel GetMaintenanceSchedules(AmMaintenanceScheduleSearchModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _maintenanceScheduleService.GetMaintenanceSchedules(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmMaintenanceScheduleBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseModel<bool> UpdateMaintenanceSchedule(AmMaintenanceScheduleAddEditModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            _errorMessages = new List<string>();

            try
            {
                if (requestModel.ID < 1)
                {
                    _errorMessages.Add(CommonConstant.MAINTENANCE_SCHEDULE_ID_REQUIRED);
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    return responseModel;
                }

                if (requestModel.FactoryID > 0)
                {
                    bool isExist = _maintenanceScheduleService.CheckDuplicateMaintenanceSchedule(requestModel.ID, requestModel.MaintenanceScheduleName, false, requestModel.FactoryID);
                    if (isExist)
                    {
                        responseModel.ErrorMessages.Add(string.Format(CommonConstant.MSG_ERROR_AM_NAME_DUPLICATE, "lịch bảo trì"));
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                        responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                        return responseModel;
                    }
                }

                responseModel.Data = _maintenanceScheduleService.UpdateMaintenanceSchedule(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AmDeviceBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        private bool IsValidData(AmMaintenanceScheduleAddEditModel requestModel)
        {
            // Nhà máy
            if (requestModel.FactoryID < 1)
            {
                _errorMessages.Add(CommonConstant.FACTORY_REQUIRED);
            }

            // Tên thiết bị là bắt buộc
            if (string.IsNullOrWhiteSpace(requestModel.MaintenanceScheduleName))
            {
                _errorMessages.Add(CommonConstant.MAINTENANCE_SCHEDULE_NAME_REQUIRED);
            }

            // Địa Điểm/ Khu vực
            if (requestModel.SiteDescID < 1)
            {
                _errorMessages.Add(CommonConstant.LOCATION_ID_REQUIRED);
            }

            // Thiết bị
            if (requestModel.AssetDescID < 1)
            {
                _errorMessages.Add(CommonConstant.DEVICE_ID_REQUIRED);
            }

            // Cương vị
            if (requestModel.OperationPositionID < 1)
            {
                _errorMessages.Add(CommonConstant.OPERATION_POSITION_REQUIRED);
            }

            // Ca thực hiện
            if (requestModel.OperationShiftID < 1)
            {
                _errorMessages.Add(CommonConstant.OPERATION_SHIFT_REQUIRED);
            }

            // Danh sách công việc
            if (requestModel.MaintenanceWorks == null || requestModel.MaintenanceWorks.Count == 0)
            {
                _errorMessages.Add(CommonConstant.LIST_MAINTENANCE_WORK_REQUIRED);
            }

            // Thời gian run job
            if (string.IsNullOrWhiteSpace(requestModel.CronJobFormat))
            {
                _errorMessages.Add(CommonConstant.AM_CRON_SCHEDULE_REQUIRED);
            }

            return _errorMessages.Count <= 0;
        }
    }
}
