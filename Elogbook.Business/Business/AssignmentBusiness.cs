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
using System.Linq;
using System.Net;

namespace Elogbook.Business.Business
{
    public class AssignmentBusiness : IAssignmentBusiness
    {
        private readonly IAssignmentService _assignmentService;
        private List<string> _errorMessages;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="assignmentPlanService"></param>
        public AssignmentBusiness(IAssignmentService assignmentPlanService)
        {
            _assignmentService = assignmentPlanService;
        }

        /// <summary>
        /// Tạo phân công
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<bool> CreateAssignment(AssignmentRequestModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            _errorMessages = new List<string>();

            try
            {
                if (!IsValidData(requestModel, true))
                {
                    responseModel.ErrorMessages = _errorMessages;
                    responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                else
                {
                    //if (_assignmentPlanService.CheckDuplicate(requestModel, false))
                    //{
                    //    responseModel.ErrorMessages.Add("");
                    //    responseModel.StatusCode = StatusCodeCustom.DuplicateData;
                    //    return responseModel;
                    //}

                    return _assignmentService.CreateAssignment(requestModel);
                }
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

        private bool IsValidData(AssignmentRequestModel requestModel, bool isCreate)
        {
            if (requestModel == null)
            {
                _errorMessages.Add("Invalid request model");
            }
            else
            {
                if (isCreate)
                {
                    // Check nhà máy
                    if (requestModel.FactoryID <= 0)
                    {
                        _errorMessages.Add("Nhà máy là bắt buộc");
                    }

                    // Check Cương vị
                    if (requestModel.OperationPositionID <= 0)
                    {
                        _errorMessages.Add("Cương vị là bắt buộc");
                    }

                    // Check danh sách ca làm việc
                    if (requestModel.OperationShifts == null || !requestModel.OperationShifts.Any())
                    {
                        _errorMessages.Add("Danh sách ca làm việc là bắt buộc");
                    }
                    else
                    {
                        // Check duplicate operation shifts
                        var duplicateShifts = requestModel.OperationShifts
                            .Where(s => s != null)
                            .GroupBy(s => s.OperationShiftID)
                            .Where(g => g.Key > 0 && g.Count() > 1)
                            .Select(g => g.Key)
                            .ToList();

                        if (duplicateShifts.Any())
                        {
                            _errorMessages.Add($"Các ca vận hành không được trùng lặp.");
                        }

                        foreach (var shift in requestModel.OperationShifts)
                        {
                           
                            if (shift.OperationShiftID <= 0)
                            {
                                _errorMessages.Add("Ca làm việc là bắt buộc");
                                continue;
                            }

                            if (shift.AssignmentDetails == null || !shift.AssignmentDetails.Any())
                            {
                                _errorMessages.Add($"Danh sách chi tiết phân công cho ca làm việc {shift.OperationShiftID} là bắt buộc");
                                continue;
                            }

                            // Check duplicate work dates within the same shift
                            var duplicateDates = shift.AssignmentDetails
                                .Where(d => d != null && !string.IsNullOrWhiteSpace(d.WorkDate))
                                .GroupBy(d => d.WorkDate.Trim())
                                .Where(g => g.Count() > 1)
                                .Select(g => g.Key)
                                .ToList();

                            if (duplicateDates.Any())
                            {
                                _errorMessages.Add($"Trong ca {shift.OperationShiftID}, các ngày làm việc không được trùng nhau.");
                            }

                            foreach (var detail in shift.AssignmentDetails)
                            {
                                if (string.IsNullOrWhiteSpace(detail.WorkDate))
                                {
                                    _errorMessages.Add($"Ngày làm việc là bắt buộc.");
                                    continue;
                                }

                                // Check duplicate in database: same factory, same position, same shift, same date
                                if (!string.IsNullOrWhiteSpace(detail.WorkDate)
                                    && requestModel.FactoryID > 0
                                    && requestModel.OperationPositionID > 0
                                    && shift.OperationShiftID > 0)
                                {
                                    try
                                    {
                                        var inputRequest = new CheckAssignmentDuplicateRequestModel
                                        {
                                            FactoryID = requestModel.FactoryID,
                                            WorkDate = detail.WorkDate.Trim(),
                                            OperationPositionID = requestModel.OperationPositionID,
                                            OperationShiftID = shift.OperationShiftID
                                        };

                                        var existing = _assignmentService.CheckDuplicate(inputRequest);
                                        if (existing)
                                        {
                                            // normalize date for message
                                            string dateStr = detail.WorkDate.Trim();
                                            if (DateTime.TryParseExact(dateStr, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime dt))
                                            {
                                                dateStr = dt.ToString("dd/MM/yyyy");
                                            }

                                            _errorMessages.Add($"Đã tồn tại nhà máy Id:[{requestModel.FactoryID}], Cương vị Id:[{requestModel.OperationPositionID}], Ca Id:{shift.OperationShiftID}, Ngày:[{dateStr}]");
                                        }
                                    }
                                    catch
                                    {
                                        // ignore errors from checking existence to avoid failing validation flow
                                    }
                                }

                                //if (detail.AssignedUsers == null || !detail.AssignedUsers.Any())
                                //{
                                //    _errorMessages.Add($"Danh sách nhân viên được phân công cho ngày {detail.WorkDate} là bắt buộc.");
                                //}
                                //else
                                //{
                                //    // Check duplicate users within assigned users for the same detail
                                //    var duplicateUsers = detail.AssignedUsers
                                //        .Where(u => u != null)
                                //        .GroupBy(u => u.UserID)
                                //        .Where(g => g.Key > 0 && g.Count() > 1)
                                //        .Select(g => g.Key)
                                //        .ToList();

                                //    if (duplicateUsers.Any())
                                //    {
                                //        _errorMessages.Add($"Trong ngày {detail.WorkDate}, danh sách nhân viên phân công không được trùng lặp.");
                                //    }

                                //    foreach (var user in detail.AssignedUsers)
                                //    {
                                //        if (user == null || user.UserID <= 0)
                                //        {
                                //            _errorMessages.Add($"ID nhân viên là bắt buộc cho ngày {detail.WorkDate}.");
                                //        }
                                //    }
                                //}
                            }
                        }
                    }
                }
            }
            // Add more validation logic here if needed
            return _errorMessages.Count <= 0;
        }

        /// <summary>
        /// Lấy bảng phân công theo bộ lọc (View 1 - Quản lý phân công)
        /// </summary>
        /// <param name="requestModel">Bộ lọc: nhà máy, khoảng ngày, cương vị, ca</param>
        /// <returns>Danh sách phân công nhóm theo cương vị → ca → ngày</returns>
        public ResponseModel<List<AssignmentScheduleResponseModel>> GetAssignmentSchedule(GetAssignmentScheduleRequestModel requestModel)
        {
            var responseModel = new ResponseModel<List<AssignmentScheduleResponseModel>>();

            try
            {
                responseModel.Data = _assignmentService.GetAssignmentSchedule(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.ErrorMessages.Add(ex.Message);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AssignmentBusiness.ToString(), ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Lấy thông tin vị trí nhân sự
        /// </summary>
        /// <param name="assignmentSlotID"></param>
        /// <returns></returns>
        public ResponseModel<AssignmentSlotResponseModel> GetAssignmentSlotInfo(int assignmentSlotID)
        {
            var responseModel = new ResponseModel<AssignmentSlotResponseModel>();

            try
            {
                responseModel.Data = _assignmentService.GetAssignmentSlotInfo(assignmentSlotID);
            }
            catch (Exception ex)
            {
                responseModel.ErrorMessages.Add(ex.Message);
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AssignmentBusiness.ToString(),
                        Environment.NewLine + $"ID: {assignmentSlotID} " + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Cập nhật thông tin nhân sự cho vị trí
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<bool> UpdateAssignmentSlot(UpdateAssignmentSlotModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                return _assignmentService.UpdateAssignmentSlot(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(CommonConstant.INCIDENT);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AssignmentBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }

        /// <summary>
        /// Kiểm tra khoảng thời gian đã tồn tại hay chưa
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public ResponseModel<bool> CheckRangeExists(CheckRangeAssignmentRequestModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                return _assignmentService.CheckRangeExists(requestModel);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(CommonConstant.INCIDENT);
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.AssignmentBusiness.ToString(),
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
            }

            return responseModel;
        }
    }
}
