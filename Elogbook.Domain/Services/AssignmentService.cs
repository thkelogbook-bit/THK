using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Elogbook.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;

namespace Elogbook.Domain.Services
{
    public class AssignmentService : IAssignmentService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;

        public AssignmentService(ElogbookUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Kiểm tra trùng lặp phân công
        /// </summary>
        /// <param name="inputRequest"></param>
        /// <returns></returns>
        public bool CheckDuplicate(CheckAssignmentDuplicateRequestModel inputRequest)
        {
            var workDate = Core.DateTimeConvert.ConvertDateTimeFromString(inputRequest.WorkDate, false);
            return _unitOfWork.ElogbookRepository<AssignmentSlot>()
                    .GetQuery(x => x.FactoryID == inputRequest.FactoryID &&
                                    x.WorkDate == workDate &&
                                    x.OperationPositionID == inputRequest.OperationPositionID &&
                                    x.OperationShiftID == inputRequest.OperationShiftID &&
                                    !x.IsDeleted)
                    .Any();
        }

        /// <summary>
        /// Tạo kế hoạch phân công
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<bool> CreateAssignment(AssignmentRequestModel requestModel)
        {
            var response = new ResponseModel<bool>();
            using (var transaction = _unitOfWork.DataContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
            {
                try
                {
                    if (requestModel == null)
                    {
                        response.ErrorMessages.Add("Invalid request model");
                        response.Data = false;
                        response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                        return response;
                    }

                    if (requestModel.OperationShifts == null || !requestModel.OperationShifts.Any())
                    {
                        response.ErrorMessages.Add("Danh sách ca làm việc là bắt buộc");
                        response.Data = false;
                        response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                        return response;
                    }

                    // Iterate shifts and create slots and employees
                    foreach (var shift in requestModel.OperationShifts)
                    {
                        if (shift == null) continue;

                        if (shift.AssignmentDetails == null || !shift.AssignmentDetails.Any()) continue;

                        foreach (var detail in shift.AssignmentDetails)
                        {
                            if (detail == null) continue;

                            // parse work date
                            var workDate = Core.DateTimeConvert.ConvertDateTimeFromString(detail.WorkDate, false);

                            var slot = new AssignmentSlot
                            {
                                FactoryID = requestModel.FactoryID,
                                OperationPositionID = requestModel.OperationPositionID,
                                WorkDate = workDate,
                                OperationShiftID = shift.OperationShiftID,
                                Active = 1,
                                CreatedUserID = requestModel.CurrentUserID,
                                LastUpdatedUserID = requestModel.CurrentUserID,
                                InsertedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now
                            };

                            _unitOfWork.ElogbookRepository<AssignmentSlot>().Add(slot);

                            // save to get slot ID
                            _unitOfWork.Complete();

                            if (detail.AssignedUsers != null && detail.AssignedUsers.Any())
                            {
                                foreach (var user in detail.AssignedUsers)
                                {
                                    if (user == null || user.UserID <= 0) continue;

                                    var emp = new AssignmentEmployee
                                    {
                                        AssignmentSlotID = slot.ID,
                                        UserID = user.UserID,
                                        Active = 1,
                                        CreatedUserID = requestModel.CurrentUserID,
                                        LastUpdatedUserID = requestModel.CurrentUserID,
                                        InsertedAt = DateTime.Now,
                                        UpdatedAt = DateTime.Now
                                    };

                                    _unitOfWork.ElogbookRepository<AssignmentEmployee>().Add(emp);
                                    _unitOfWork.Complete();

                                    var history = new AssignmentEmployeeHistory
                                    {
                                        AssignmentSlotID = slot.ID,
                                        NewUserID = user.UserID,
                                        ActionType = (int)EnumActionType.Add,
                                        Reason = "Thêm mới nhân sự theo kế hoạch",
                                        Active = 1,
                                        CreatedUserID = requestModel.CurrentUserID,
                                        LastUpdatedUserID = requestModel.CurrentUserID,
                                        InsertedAt = DateTime.Now,
                                        UpdatedAt = DateTime.Now
                                    };
                                    _unitOfWork.ElogbookRepository<AssignmentEmployeeHistory>().Add(history);
                                    _unitOfWork.Complete();
                                }
                            }
                        }
                    }

                    transaction.Commit();
                    response.Data = true;
                    response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                    return response;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    response.ErrorMessages.Add(ex.Message);
                    response.Data = false;
                    response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                    return response;
                }
            }
        }

        /// <summary>
        /// Lấy bảng phân công theo bộ lọc (View 1 - Quản lý phân công)
        /// Trả về đầy đủ cương vị × ca × ngày kể cả chưa có slot nào
        /// </summary>
        public List<AssignmentScheduleResponseModel> GetAssignmentSchedule(GetAssignmentScheduleRequestModel requestModel)
        {
            // Parse fromDate / toDate từ định dạng dd/MM/yyyy
            DateTime.TryParseExact(
                requestModel.FromDate, "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime fromDate);

            DateTime.TryParseExact(
                requestModel.ToDate, "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime toDate);

            if (fromDate == DateTime.MinValue) fromDate = DateTime.Today;
            if (toDate == DateTime.MinValue || toDate < fromDate) toDate = fromDate;

            // Sinh toàn bộ ngày trong khoảng
            var allDates = new List<DateTime>();
            for (var d = fromDate.Date; d <= toDate.Date; d = d.AddDays(1))
                allDates.Add(d);

            // --- Lấy khung cương vị × ca từ DB (không phụ thuộc slot) ---
            // Cương vị theo nhà máy
            var positionQuery = _unitOfWork.ElogbookRepository<OperationPosition>()
                .GetQuery(x => !x.IsDeleted && x.Active == 1 && x.FactoryID == requestModel.FactoryID);

            if (requestModel.OperationPositionID > 0)
                positionQuery = positionQuery.Where(x => x.ID == requestModel.OperationPositionID);

            var positions = positionQuery
                .OrderBy(x => x.Position)
                .ThenBy(x => x.OperationPositionName)
                .Select(x => new { x.ID, x.OperationPositionName, x.Position })
                .ToList();

            // Ca vận hành theo nhà máy
            var shiftQuery = _unitOfWork.ElogbookRepository<OperationShift>()
                .GetQuery(x => !x.IsDeleted && x.Active == 1 && x.FactoryID == requestModel.FactoryID);

            if (requestModel.OperationShiftID > 0)
                shiftQuery = shiftQuery.Where(x => x.ID == requestModel.OperationShiftID);

            var shifts = shiftQuery
                .OrderBy(x => x.Position)
                .Select(x => new { x.ID, x.OperationShiftName, x.Position, x.StartTime, x.EndTime })
                .ToList();

            if (!positions.Any() || !shifts.Any())
                return new List<AssignmentScheduleResponseModel>();

            // --- Lấy data phân công từ SP ---
            var sqlParams = new SqlParameter[]
            {
                new SqlParameter("@FactoryID", SqlDbType.Int)          { Value = requestModel.FactoryID },
                new SqlParameter("@FromDate",  SqlDbType.Date)          { Value = fromDate.Date },
                new SqlParameter("@ToDate",    SqlDbType.Date)          { Value = toDate.Date },
                new SqlParameter("@OperationPositionID", SqlDbType.Int) { Value = requestModel.OperationPositionID },
                new SqlParameter("@OperationShiftID",    SqlDbType.Int) { Value = requestModel.OperationShiftID },
            };

            var rows = _unitOfWork.DataContext.Database
                .SqlQuery<APAssignmentScheduleRawResult>(
                    "PCON_GetAssignmentSchedule @FactoryID, @FromDate, @ToDate, @OperationPositionID, @OperationShiftID",
                    sqlParams)
                .ToList();

            // --- Build response: khung cương vị × ca × ngày, fill data từ rows ---
            var result = positions.Select(position => new AssignmentScheduleResponseModel
            {
                OperationPositionID   = position.ID,
                OperationPositionName = position.OperationPositionName,
                Position              = position.Position,

                OperationShifts = shifts.Select(shift => new AssignmentShiftResponseModel
                {
                    OperationShiftID = shift.ID,
                    OperationShiftName = shift.OperationShiftName,
                    StartTime = shift.StartTime,
                    EndTime = shift.EndTime,

                    // Các rows thuộc cương vị + ca này
                    AssignmentDetails = allDates.Select(date =>
                    {
                        var slotGroup = rows
                            .Where(r => r.OperationPositionID == position.ID
                                     && r.OperationShiftID == shift.ID
                                     && r.WorkDate.Date == date.Date)
                            .GroupBy(r => new
                            {
                                r.AssignmentSlotID,
                                r.WorkDate,
                                r.DayOfWeekCode,
                                r.HasOverride
                            })
                            .FirstOrDefault();

                        if (slotGroup != null)
                        {
                            // Ngày có slot — trả về nhân viên thực tế
                            return new AssignmentDetailResponseModel
                            {
                                AssignmentSlotID = slotGroup.Key.AssignmentSlotID,
                                WorkDate = date.ToString("dd/MM/yyyy"),
                                DayOfWeekCode = slotGroup.Key.DayOfWeekCode,
                                HasOverride = slotGroup.Key.HasOverride,
                                AssignedUsers = slotGroup
                                    .Where(r => r.UserID > 0)
                                    .Select(r => new AssignUserResponseModel
                                    {
                                        AssignmentEmployeeID = r.AssignmentEmployeeID,
                                        UserID = r.UserID,
                                        UserName = r.UserName,
                                        FullName = r.FullName,
                                        AccountType = r.AccountType
                                    })
                                    .OrderBy(r => r.AssignmentEmployeeID)
                                    .ToList()
                            };
                        }

                        // Ngày chưa có slot — trả về ô trống
                        return new AssignmentDetailResponseModel
                        {
                            AssignmentSlotID = 0,
                            WorkDate = date.ToString("dd/MM/yyyy"),
                            DayOfWeekCode = (int)date.DayOfWeek,
                            HasOverride = false,
                            AssignedUsers = new List<AssignUserResponseModel>()
                        };
                    }).ToList()
                }).ToList()
            }).ToList();

            return result;
        }

        public AssignmentSlotResponseModel GetAssignmentSlotInfo(int assignmentSlotID)
        {
            var query = _unitOfWork
                .ElogbookRepository<AssignmentSlot>()
                .GetQuery();

            // Lấy thông tin vị trí phân bố nhân sự
            var assignmentSlot = query
                .Where(x => x.ID == assignmentSlotID && !x.IsDeleted)
                .Select(slot => new AssignmentSlotResponseModel
                {
                    AssignmentSlotID = slot.ID,
                    WorkDate = slot.WorkDate,

                    // Thông tin nhà máy
                    Factory = new ValueLabelModel
                    {
                        Value = slot.Factory.ID,
                        Label = slot.Factory.FactoryName,
                    },

                    // Thông tin cương vị
                    OperationPosition = new ValueLabelModel
                    {
                        Value = slot.OperationPosition.ID,
                        Label = slot.OperationPosition.OperationPositionName,
                    },

                    // Thông tin ca vận hành
                    OperationShift = new ValueLabelModel
                    {
                        Value = slot.OperationShift.ID,
                        Label = slot.OperationShift.OperationShiftName,
                    },

                    // Lịch sử thay đổi nhân sự
                    ReasonHistories = slot.AssignmentEmployeeHistories
                        .Where(h => !h.IsDeleted)
                        .OrderByDescending(h => h.InsertedAt)
                        .Select(h => new AssignmentEmployeeHistoryInfoModel
                        {
                            ID = h.ID,
                            AssignmentSlotID = h.AssignmentSlotID,
                            OldUserID = h.OldUserID,
                            NewUserID = h.NewUserID,
                            ActionType = h.ActionType,
                            Reason = h.Reason,
                            CreatedUserID = h.CreatedUserID,
                            Active = h.Active,
                            InsertedAt = h.InsertedAt,
                        })
                        .ToList(),

                    // Danh sách nhân sự bố trí
                    AssignmentEmployees = slot.AssignmentEmployees
                        .Where(ae => !ae.IsDeleted)
                        .Select(ae => new AssignmentEmployeeInfoModel
                        {
                            ID = ae.ID,
                            UserID = ae.UserID,
                            UserName = ae.User.UserName,
                            FullName = ae.User.FullName,
                        })
                        .ToList()
                })
                .FirstOrDefault();

            if (assignmentSlot == null)
                return null;

            // Lấy danh sách history để lấy UserID
            var historyUserIds = assignmentSlot.ReasonHistories
                .SelectMany(h => new[]
                {
                    h.OldUserID,
                    h.NewUserID,
                    h.CreatedUserID
                })
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .Distinct()
                .ToList();

            // Lấy FullName của toàn bộ User liên quan đến history
            var userDict = _unitOfWork
                .ElogbookRepository<User>()
                .GetQuery()
                .Where(u => historyUserIds.Contains(u.ID))
                .ToDictionary(
                    u => u.ID,
                    u => u.FullName
                );

            string GetUserFullName(int? userId)
            {
                if (!userId.HasValue)
                    return string.Empty;

                return userDict.TryGetValue(userId.Value, out var fullName)
                    ? fullName
                    : string.Empty;
            }

            // Build nội dung History
            foreach (var history in assignmentSlot.ReasonHistories)
            {
                var createdUserName = GetUserFullName(history.CreatedUserID);
                var oldUserName = GetUserFullName(history.OldUserID);
                var newUserName = GetUserFullName(history.NewUserID);

                switch (history.ActionType)
                {
                    case 1:
                        history.Reason = string.IsNullOrWhiteSpace(history.Reason)
                            ? $"{history.InsertedAt:dd/MM/yyyy HH:mm:ss}: {createdUserName} thêm nhân sự {newUserName}."
                            : $"{history.InsertedAt:dd/MM/yyyy HH:mm:ss}: {createdUserName} thêm nhân sự {newUserName} với lý do: {history.Reason}.";

                        break;

                    case 2:
                        history.Reason = string.IsNullOrWhiteSpace(history.Reason)
                            ? $"{history.InsertedAt:dd/MM/yyyy HH:mm:ss}: {createdUserName} thay đổi nhân sự {oldUserName} sang nhân sự {newUserName}."
                            : $"{history.InsertedAt:dd/MM/yyyy HH:mm:ss}: {createdUserName} thay đổi nhân sự {oldUserName} sang nhân sự {newUserName} với lý do: {history.Reason}.";

                        break;

                    case 3:
                        history.Reason = string.IsNullOrWhiteSpace(history.Reason)
                            ? $"{history.InsertedAt:dd/MM/yyyy HH:mm:ss}: {createdUserName} loại bỏ nhân sự {oldUserName} khỏi kế hoạch."
                            : $"{history.InsertedAt:dd/MM/yyyy HH:mm:ss}: {createdUserName} loại bỏ nhân sự {oldUserName} khỏi kế hoạch với lý do: {history.Reason}.";

                        break;
                }
            }

            return assignmentSlot;
        }

        public ResponseModel<bool> UpdateAssignmentSlot(UpdateAssignmentSlotModel requestModel)
        {
            var response = new ResponseModel<bool>();
            using (var transaction = _unitOfWork.DataContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
            {
                try
                {
                    var entity = _unitOfWork.ElogbookRepository<AssignmentSlot>().GetQuery(x => x.ID == requestModel.AssignmentSlotID && !x.IsDeleted).FirstOrDefault();
                    if (entity != null)
                    {
                        foreach (var item in requestModel.AssignmentEmployees)
                        {
                            if (item.ID <= 0)
                            {
                                var newAssignmentEmployee = new AssignmentEmployee
                                {
                                    AssignmentSlotID = requestModel.AssignmentSlotID,
                                    UserID = item.UserID,
                                    Active = (int)AccessStatus.Active,
                                    IsDeleted = false,
                                    CreatedUserID = requestModel.CurrentUserID,
                                    InsertedAt = DateTime.Now,
                                    LastUpdatedUserID = requestModel.CurrentUserID,
                                    UpdatedAt = DateTime.Now,
                                };
                                _unitOfWork.ElogbookRepository<AssignmentEmployee>().Add(newAssignmentEmployee);

                                AddAssignmentEmployeeHistory(requestModel, null, item.UserID, (int)EnumActionType.Add);
                            }
                            else
                            {
                                var assignmentEmployee = _unitOfWork.ElogbookRepository<AssignmentEmployee>()
                                    .GetQuery(x => x.ID == item.ID && x.AssignmentSlotID == requestModel.AssignmentSlotID && !x.IsDeleted).FirstOrDefault();

                                if (assignmentEmployee != null)
                                {
                                    if (item.IsRemove)
                                    {
                                        AddAssignmentEmployeeHistory(requestModel, assignmentEmployee.UserID, assignmentEmployee.UserID, (int)EnumActionType.Delete);

                                        _unitOfWork.ElogbookRepository<AssignmentEmployee>().Delete(assignmentEmployee, requestModel.CurrentUserID, true);
                                    }
                                    else if (assignmentEmployee.UserID != item.UserID)
                                    {
                                        AddAssignmentEmployeeHistory(requestModel, assignmentEmployee.UserID, item.UserID, (int)EnumActionType.Update);

                                        assignmentEmployee.UserID = item.UserID;
                                        assignmentEmployee.LastUpdatedUserID = requestModel.CurrentUserID;
                                        assignmentEmployee.UpdatedAt = DateTime.Now;
                                        _unitOfWork.ElogbookRepository<AssignmentEmployee>().Update(assignmentEmployee);
                                    }
                                }
                            }
                        }

                        _unitOfWork.Complete();
                    }

                    if (requestModel.AssignmentSlotID <= 0)
                    {
                        var workDate = Core.DateTimeConvert.ConvertDateTimeFromString(requestModel.WorkDate, false);

                        var assignmentSlot = new AssignmentSlot
                        {
                            FactoryID = requestModel.FactoryID,
                            OperationPositionID = requestModel.OperationPositionID,
                            WorkDate = workDate,
                            OperationShiftID = requestModel.OperationShiftID,
                            Active = (int)AccessStatus.Active,
                            IsDeleted = false,
                            CreatedUserID = requestModel.CurrentUserID,
                            InsertedAt = DateTime.Now,
                            LastUpdatedUserID = requestModel.CurrentUserID,
                            UpdatedAt = DateTime.Now,
                        };

                        _unitOfWork.ElogbookRepository<AssignmentSlot>().Add(assignmentSlot);
                        // save to get slot ID
                        _unitOfWork.Complete();
                        requestModel.AssignmentSlotID = assignmentSlot.ID;

                        foreach (var (item, newAssignmentEmployee) in from item in requestModel.AssignmentEmployees
                                                                      let newAssignmentEmployee = new AssignmentEmployee
                                                                      {
                                                                          AssignmentSlotID = requestModel.AssignmentSlotID,
                                                                          UserID = item.UserID,
                                                                          Active = (int)AccessStatus.Active,
                                                                          IsDeleted = false,
                                                                          CreatedUserID = requestModel.CurrentUserID,
                                                                          InsertedAt = DateTime.Now,
                                                                          LastUpdatedUserID = requestModel.CurrentUserID,
                                                                          UpdatedAt = DateTime.Now,
                                                                      }
                                                                      select (item, newAssignmentEmployee))
                        {
                            _unitOfWork.ElogbookRepository<AssignmentEmployee>().Add(newAssignmentEmployee);
                            AddAssignmentEmployeeHistory(requestModel, null, item.UserID, (int)EnumActionType.Add);
                        }
                        _unitOfWork.Complete();
                    }

                    transaction.Commit();
                    response.Data = true;
                    response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                    return response;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    LogCustom.LogMessage(TrackingLogLevel.ERROR, "UpdateAssignmentSlot",
                            Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
                    response.ErrorMessages.Add(CommonConstant.INCIDENT);
                    response.Data = false;
                    response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                    return response;
                }
            }
        }

        public void AddAssignmentEmployeeHistory(UpdateAssignmentSlotModel requestModel, int? oldUserID, int? newUserID, int actionType)
        {
            var newAssignmentEmployeeHistory = new AssignmentEmployeeHistory
            {
                AssignmentSlotID = requestModel.AssignmentSlotID,
                OldUserID = oldUserID,
                NewUserID = newUserID,
                ActionType = actionType,
                Reason = requestModel.Reason,
                Active = (int)AccessStatus.Active,
                IsDeleted = false,
                CreatedUserID = requestModel.CurrentUserID,
                InsertedAt = DateTime.Now,
                LastUpdatedUserID = requestModel.CurrentUserID,
                UpdatedAt = DateTime.Now,
            };
            _unitOfWork.ElogbookRepository<AssignmentEmployeeHistory>().Add(newAssignmentEmployeeHistory);
        }

        /// <summary>
        /// Kiểm tra khoảng thời gian phân công đã tồn tại hay chưa
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public ResponseModel<bool> CheckRangeExists(CheckRangeAssignmentRequestModel requestModel)
        {
            var response = new ResponseModel<bool>();

            if (requestModel.FactoryID <= 0 || requestModel.OperationPositionID <= 0)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.ErrorMessages.Add("Nhà máy và Cương vị là bắt buộc.");
                return response;
            }

            if (string.IsNullOrEmpty(requestModel.FromDate) || string.IsNullOrEmpty(requestModel.ToDate))
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.ErrorMessages.Add("Ngày bắt đầu và ngày kết thúc không được để trống.");
                return response;
            }

            var fromDate = Core.DateTimeConvert.ConvertDateTimeFromString(requestModel.FromDate, false);
            var toDate = Core.DateTimeConvert.ConvertDateTimeFromString(requestModel.ToDate, true);

            if (fromDate > toDate)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.ErrorMessages.Add("Ngày bắt đầu không được lớn hơn ngày kết thúc.");
                return response;
            }

            var exists = _unitOfWork.ElogbookRepository<AssignmentSlot>()
                .GetQuery(x => x.FactoryID == requestModel.FactoryID &&
                                x.OperationPositionID == requestModel.OperationPositionID &&
                                x.WorkDate >= fromDate &&
                                x.WorkDate <= toDate &&
                                !x.IsDeleted)
                .Select(x => x.WorkDate)
                .Distinct()
                .ToList();

            if (exists.Any())
            {
                response.StatusCode = (int)HttpStatusCode.OK;
                response.ErrorMessages.Add("Khoảng thời gian phân công đã tồn tại.");
                response.ErrorMessages.Add($"Ngày đã tồn tại: {string.Join(", ", exists.Select(d => d.ToString("dd/MM/yyyy")))}");
                response.Data = false;
                return response;
            }

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = true;
            return response;
        }

        /// <summary>
        /// ActionType enum
        /// </summary>
        private enum EnumActionType
        {
            /// <summary>
            /// Thêm mới
            /// </summary>
            [Description("Thêm mới")]
            Add = 1,

            /// <summary>
            /// Cập nhật
            /// </summary>
            [Description("Cập nhật")]
            Update = 2,

            /// <summary>
            /// Xóa
            /// </summary>
            [Description("Xóa")]
            Delete = 3
        }
    }
}
