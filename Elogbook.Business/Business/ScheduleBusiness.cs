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
    public class ScheduleBusiness : IScheduleBusiness
    {
        private readonly IScheduleService _scheduleService;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="scheduleService"></param>
        public ScheduleBusiness(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        public ResponseBaseModel GetSchedules(SearchScheduleRequestModel requestModel)
        {
            ResponseBaseModel responseBaseModel = new ResponseBaseModel();

            try
            {
                responseBaseModel = _scheduleService.GetSchedules(requestModel);
            }
            catch (Exception ex)
            {
                responseBaseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "ScheduleBusiness",
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseBaseModel;
        }

        public ResponseModel<ScheduleResponseModel> GetScheduleById(int id)
        {
            var responseModel = new ResponseModel<ScheduleResponseModel>();

            try
            {
                responseModel.Data = _scheduleService.GetScheduleById(id);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "ScheduleBusiness",
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<ScheduleResponseModel> AddSchedule(AddEditScheduleRequestModel requestModel)
        {
            var responseModel = new ResponseModel<ScheduleResponseModel>();
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
                    if (!_scheduleService.CheckDuplicateData(0, requestModel.ScheduleName, true))
                    {
                        responseModel.Data = _scheduleService.AddSchedule(requestModel);
                    }
                    else
                    {
                        responseModel.ErrorMessages.Add("Công việc đã tồn tại trong hệ thống.");
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                    }
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "ScheduleBusiness",
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<ScheduleResponseModel> UpdateSchedule(AddEditScheduleRequestModel requestModel)
        {
            var responseModel = new ResponseModel<ScheduleResponseModel>();
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
                    if (!_scheduleService.CheckDuplicateData(requestModel.ID, requestModel.ScheduleName, false))
                    {
                        responseModel.Data = _scheduleService.UpdateSchedule(requestModel);
                    }
                    else
                    {
                        responseModel.ErrorMessages.Add("Công việc đã tồn tại trong hệ thống.");
                        responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                    }
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "ScheduleBusiness",
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> DeleteSchedule(int id, int userID)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _scheduleService.DeleteSchedule(id, userID);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "ScheduleBusiness",
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<bool> CheckInUseSchedule(int id)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                responseModel.Data = _scheduleService.CheckInUseSchedule(id);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "ScheduleBusiness",
                    Environment.NewLine + $"ID: {id} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        public ResponseModel<string> GetCronJobDescription(CronJobDescriptionRequestModel requestModel)
        {
            ResponseModel<string> responseModel = new ResponseModel<string>();

            try
            {
                responseModel.Data = _scheduleService.GetCronJobDescription(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = string.Empty;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "ScheduleBusiness", ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Kiểm tra dữ liệu bắt buộc khi tạo mới/cập nhật schedule
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        private bool IsValidData(AddEditScheduleRequestModel requestModel)
        {
            if (string.IsNullOrWhiteSpace(requestModel.ScheduleName) && (requestModel.ID <= 0 || !requestModel.IsSystem))
            {
                _errorMessages.Add("Tên công việc là thông tin bắt buộc.");
            }

            if (string.IsNullOrWhiteSpace(requestModel.ScheduleValue))
            {
                _errorMessages.Add("Thời gian thực hiện là thông tin bắt buộc.");
            }

            // nếu có SlaConfigID,SlaCategoryID thì kiểm tra tồn tại
            if (requestModel.SlaConfigID.HasValue && requestModel.SlaConfigID.Value > 0
                && requestModel.SlaCategoryID.HasValue && requestModel.SlaCategoryID.Value > 0)
            {
                var exists = _scheduleService.SlaConfigExists(requestModel.SlaConfigID.Value, requestModel.SlaCategoryID.Value);
                if (!exists)
                {
                    _errorMessages.Add("Cấu hình SLA không tồn tại hoặc không hoạt động.");
                }
            }

            return _errorMessages.Count <= 0;
        }
    }
}
