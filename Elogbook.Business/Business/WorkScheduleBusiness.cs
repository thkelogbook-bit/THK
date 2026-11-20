using Elogbook.Core;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Elogbook.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Elogbook.Business
{
    public class WorkScheduleBusiness : IWorkScheduleBusiness
    {
        private readonly IWorkScheduleService _workScheduleService;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        public WorkScheduleBusiness(IWorkScheduleService workScheduleService)
        {
            _workScheduleService = workScheduleService;
        }

        /// <summary>
        /// Thêm mới lịch làm việc
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public ResponseModel<bool> AddEditWorkSchedule(WorkScheduleRequestModel requestModel)
        {
            var responseModel = new ResponseModel<bool>();
            _errorMessages = new List<string>();

            try
            {
                if (!IsValid(requestModel, true))
                {
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                else
                {
                    responseModel.Data = _workScheduleService.AddEditWorkSchedule(requestModel);
                    responseModel.StatusCode = (int)HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkScheduleBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Lấy dữ liệu khởi tạo cho màn hình quản lý lịch làm việc
        /// </summary>
        /// <returns></returns>
        public ResponseModel<WorkScheduleResponseModel> GetInitialData()
        {
            var response = new ResponseModel<WorkScheduleResponseModel>();
            try
            {
                response.Data = _workScheduleService.GetInitialData();
                response.StatusCode = (int)HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                response.ErrorMessages.Add(ex.Message);
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkScheduleBusiness.ToString(), ex.ToString());
            }
            return response;
        }

        public ResponseBaseModel GetAllHolidaySchedule()
        {
            var response = new ResponseBaseModel();
            try
            {
                var list = _workScheduleService.GetAllHolidaySchedule();
                response.Content = list;
                response.StatusCode = (int)HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkScheduleBusiness.ToString(), ex.ToString());
            }
            return response;
        }

        public ResponseModel<Core.Models.Response.HolidayScheduleResponseModel> GetHolidayScheduleById(int id)
        {
            var response = new ResponseModel<Core.Models.Response.HolidayScheduleResponseModel>();
            try
            {
                response.Data = _workScheduleService.GetHolidayScheduleById(id);
                response.StatusCode = (int)HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                response.ErrorMessages.Add(ex.Message);
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkScheduleBusiness.ToString(), ex.ToString());
            }
            return response;
        }

        public ResponseModel<bool> AddEditHolidaySchedule(HolidayScheduleRequestModel requestModel, int currentUserId)
        {
            var response = new ResponseModel<bool>();
            try
            {
                // simple validation
                if (requestModel == null)
                {
                    response.ErrorMessages.Add("Dữ liệu không hợp lệ.");
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return response;
                }

                response.Data = _workScheduleService.AddEditHolidaySchedule(requestModel, currentUserId);
                response.StatusCode = (int)HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                response.ErrorMessages.Add(ex.Message);
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkScheduleBusiness.ToString(), ex.ToString());
            }
            return response;
        }

        public ResponseModel<bool> DeleteHolidaySchedule(int id, int currentUserId)
        {
            var response = new ResponseModel<bool>();
            try
            {
                response.Data = _workScheduleService.DeleteHolidaySchedule(id, currentUserId);
                response.StatusCode = (int)HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                response.ErrorMessages.Add(ex.Message);
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkScheduleBusiness.ToString(), ex.ToString());
            }
            return response;
        }

        private bool IsValid(WorkScheduleRequestModel requestModel, bool isAddMode)
        {
            if (requestModel == null)
            {
                _errorMessages.Add("Dữ liệu đầu vào không hợp lệ.");
                return false;
            }

            if (isAddMode)
            {
                // WorkType must be 1 or 2
                if (requestModel.WorkType != 1 && requestModel.WorkType != 2)
                {
                    _errorMessages.Add("Phân loại lịch làm việc không hợp lệ. Giá trị hợp lệ: 1 (24h) hoặc 2 (Tiêu chuẩn).");
                }

                // Validate standard times if provided
                if (!string.IsNullOrWhiteSpace(requestModel.StandardStartTime) && !string.IsNullOrWhiteSpace(requestModel.StandardEndTime))
                {
                    TimeSpan start;
                    TimeSpan end;
                    var okStart = TimeSpan.TryParse(requestModel.StandardStartTime, out start);
                    var okEnd = TimeSpan.TryParse(requestModel.StandardEndTime, out end);

                    if (!okStart)
                    {
                        _errorMessages.Add("Thời gian chuẩn bắt đầu không đúng định dạng HH:mm.");
                    }
                    if (!okEnd)
                    {
                        _errorMessages.Add("Thời gian chuẩn kết thúc không đúng định dạng HH:mm.");
                    }

                    if (okStart && okEnd && start > end)
                    {
                        _errorMessages.Add("Thời gian chuẩn bắt đầu không được lớn hơn thời gian chuẩn kết thúc.");
                    }
                }

                // Validate breaks
                if (requestModel.BreakDays != null && requestModel.BreakDays.Count > 0)
                {
                    for (int i = 0; i < requestModel.BreakDays.Count; i++)
                    {
                        var b = requestModel.BreakDays[i];
                        if (!string.IsNullOrWhiteSpace(b.BreakStartTime) && !string.IsNullOrWhiteSpace(b.BreakEndTime))
                        {
                            var okBs = TimeSpan.TryParse(b.BreakStartTime, out TimeSpan bs);
                            var okBe = TimeSpan.TryParse(b.BreakEndTime, out TimeSpan be);

                            if (!okBs)
                                _errorMessages.Add($"Ngày nghỉ [{i}].Thời gian bắt đầu nghỉ không đúng định dạng HH:mm.");
                            if (!okBe)
                                _errorMessages.Add($"Ngày nghỉ [{i}].Thời gian kết thúc nghỉ không đúng định dạng HH:mm.");

                            if (okBs && okBe && bs > be)
                                _errorMessages.Add($"Ngày nghỉ [{i}].Thời gian bắt đầu không được lớn hơn thời gian kết thúc.");
                        }
                    }
                }

                // Validate work days
                if (requestModel.WorkDays != null && requestModel.WorkDays.Count > 0)
                {
                    var daySet = new HashSet<int>();
                    for (int i = 0; i < requestModel.WorkDays.Count; i++)
                    {
                        var d = requestModel.WorkDays[i];

                        // WorkingType must be 1,2,3
                        if (d.WorkingType != 1 && d.WorkingType != 2 && d.WorkingType != 3)
                        {
                            _errorMessages.Add($"Ngày làm việc [{i}].Phân loại ngày làm việc không hợp lệ. Giá trị hợp lệ: 1,2,3.");
                        }

                        // DayOfWeek must be 0..6
                        if (d.DayOfWeek < 0 || d.DayOfWeek > 6)
                        {
                            _errorMessages.Add($"Ngày làm việc [{i}].Ngày trong tuần không hợp lệ. Giá trị hợp lệ: 0..6.");
                        }
                        else
                        {
                            if (daySet.Contains(d.DayOfWeek))
                            {
                                _errorMessages.Add($"Ngày làm việc [{i}].Ngày trong tuần {d.DayOfWeek} bị trùng lặp trong WorkDays.");
                            }
                            else
                            {
                                daySet.Add(d.DayOfWeek);
                            }
                        }

                        // Check start/end times (StartTime/EndTime are TimeSpan)
                        try
                        {
                            if (d.StartTime > d.EndTime)
                            {
                                _errorMessages.Add($"Ngày làm việc [{i}].Thời gian bắt đầu không được lớn hơn thời gian kết thúc.");
                            }
                        }
                        catch
                        {
                            // In case StartTime/EndTime are not set, ignore here (assumed default 0)
                        }
                    }
                }
            }
            return _errorMessages.Count == 0;
        }
    }
}
