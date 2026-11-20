using Elogbook.Core;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Elogbook.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Elogbook.Domain
{
    public class WorkScheduleService : IWorkScheduleService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;

        public WorkScheduleService(ElogbookUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Thêm mới hoặc chỉnh sửa một lịch làm việc
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public bool AddEditWorkSchedule(WorkScheduleRequestModel requestModel)
        {
            using (var transaction = _unitOfWork.DataContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
            {
                try
                {
                    // Create or update WorkSchedule
                    WorkSchedule entity = null;
                    if (requestModel.ID <= 0)
                    {
                        entity = new WorkSchedule
                        {
                            ScheduleName = requestModel.ScheduleName,
                            WorkType = requestModel.WorkType,
                            StandardWorkDurationMinutes = requestModel.StandardWorkDurationMinutes,
                            IsActive = requestModel.IsActive,
                            CreatedUserID = requestModel.CurrentUserID,
                            LastUpdatedUserID = requestModel.CurrentUserID,
                            InsertedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        };

                        // parse standard times
                        if (!string.IsNullOrWhiteSpace(requestModel.StandardStartTime))
                        {
                            if (TimeSpan.TryParse(requestModel.StandardStartTime, out TimeSpan s))
                            {
                                entity.StandardStartTime = s;
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(requestModel.StandardEndTime))
                        {
                            if (TimeSpan.TryParse(requestModel.StandardEndTime, out TimeSpan e))
                            {
                                entity.StandardEndTime = e;
                            }
                        }

                        _unitOfWork.ElogbookRepository<WorkSchedule>().Add(entity);
                        _unitOfWork.Complete();
                    }
                    else
                    {
                        entity = _unitOfWork.ElogbookRepository<WorkSchedule>()
                            .GetQuery(x => x.ID == requestModel.ID && !x.IsDeleted)
                            .FirstOrDefault();

                        if (entity == null) return false;

                        entity.ScheduleName = !string.IsNullOrWhiteSpace(requestModel.ScheduleName) ? requestModel.ScheduleName : entity.ScheduleName;
                        entity.WorkType = requestModel.WorkType > 0 ? requestModel.WorkType : entity.WorkType;
                        entity.StandardWorkDurationMinutes = requestModel.StandardWorkDurationMinutes > 0 ? requestModel.StandardWorkDurationMinutes : entity.StandardWorkDurationMinutes;
                        entity.IsActive = requestModel.IsActive;
                        entity.LastUpdatedUserID = requestModel.CurrentUserID;
                        entity.UpdatedAt = DateTime.Now;

                        if (!string.IsNullOrWhiteSpace(requestModel.StandardStartTime) 
                            && TimeSpan.TryParse(requestModel.StandardStartTime, out TimeSpan ss))
                        {
                            entity.StandardStartTime = ss;
                        }
                        if (!string.IsNullOrWhiteSpace(requestModel.StandardEndTime)
                            && TimeSpan.TryParse(requestModel.StandardEndTime, out TimeSpan se))
                        { 
                            entity.StandardEndTime = se;
                        }

                        _unitOfWork.ElogbookRepository<WorkSchedule>().Update(entity);
                        _unitOfWork.Complete();
                    }

                    // Handle breaks (add new or update existing)
                    // Get all existing breaks for this schedule
                    var existingBreaks = _unitOfWork.ElogbookRepository<WorkScheduleBreak>()
                        .GetQuery(x => x.WorkScheduleID == entity.ID && !x.IsDeleted).ToList();

                    // Get list of break IDs from request
                    var requestBreakIds = requestModel.BreakDays?.Where(b => b != null && b.ID > 0).Select(b => b.ID).ToList() ?? new List<int>();

                    // Delete breaks not in the request (soft delete)
                    var breaksToDelete = existingBreaks.Where(x => !requestBreakIds.Contains(x.ID)).ToList();
                    foreach (var breakToDelete in breaksToDelete)
                    {
                        _unitOfWork.ElogbookRepository<WorkScheduleBreak>().Delete(breakToDelete, requestModel.CurrentUserID);
                    }

                    if (requestModel.BreakDays != null && requestModel.BreakDays.Count > 0)
                    {
                        foreach (var b in requestModel.BreakDays)
                        {
                            if (b == null) continue;

                            WorkScheduleBreak breakEntity = null;
                            if (b.ID > 0)
                            {
                                breakEntity = _unitOfWork.ElogbookRepository<WorkScheduleBreak>()
                                    .GetQuery(x => x.ID == b.ID && !x.IsDeleted).FirstOrDefault();
                            }

                            if (breakEntity == null)
                            {
                                breakEntity = new WorkScheduleBreak
                                {
                                    WorkScheduleID = entity.ID,
                                    BreakName = b.BreakName,
                                    BreakDurationMinutes = b.BreakDurationMinutes,
                                    CreatedUserID = requestModel.CurrentUserID,
                                    LastUpdatedUserID = requestModel.CurrentUserID,
                                    InsertedAt = DateTime.Now,
                                    UpdatedAt = DateTime.Now
                                };

                                if (!string.IsNullOrWhiteSpace(b.BreakStartTime)
                                    && TimeSpan.TryParse(b.BreakStartTime, out TimeSpan bs))
                                {
                                    breakEntity.BreakStartTime = bs;
                                }

                                if (!string.IsNullOrWhiteSpace(b.BreakEndTime)
                                    && TimeSpan.TryParse(b.BreakEndTime, out TimeSpan be))
                                {
                                    breakEntity.BreakEndTime = be;
                                }

                                _unitOfWork.ElogbookRepository<WorkScheduleBreak>().Add(breakEntity);
                            }
                            else
                            {
                                breakEntity.BreakName = !string.IsNullOrWhiteSpace(b.BreakName) ? b.BreakName : breakEntity.BreakName;
                                breakEntity.BreakDurationMinutes = b.BreakDurationMinutes > 0 ? b.BreakDurationMinutes : breakEntity.BreakDurationMinutes;
                                if (!string.IsNullOrWhiteSpace(b.BreakStartTime)
                                    && TimeSpan.TryParse(b.BreakStartTime, out TimeSpan bs2))
                                {
                                    breakEntity.BreakStartTime = bs2;
                                }
                                if (!string.IsNullOrWhiteSpace(b.BreakEndTime)
                                    && TimeSpan.TryParse(b.BreakEndTime, out TimeSpan be2))
                                {
                                    breakEntity.BreakEndTime = be2;
                                }

                                breakEntity.LastUpdatedUserID = requestModel.CurrentUserID;
                                breakEntity.UpdatedAt = DateTime.Now;

                                _unitOfWork.ElogbookRepository<WorkScheduleBreak>().Update(breakEntity);
                            }
                        }
                    }
                    _unitOfWork.Complete();

                    // Handle work days: add missing days, update existing
                    if (requestModel.WorkDays != null && requestModel.WorkDays.Count > 0)
                    {
                        foreach (var d in requestModel.WorkDays)
                        {
                            if (d == null) continue;

                            var exist = _unitOfWork.ElogbookRepository<WorkScheduleDay>()
                                .GetQuery(x => x.WorkScheduleID == entity.ID && x.DayOfWeek == d.DayOfWeek && !x.IsDeleted)
                                .FirstOrDefault();

                            if (exist == null)
                            {
                                var newDay = new WorkScheduleDay
                                {
                                    WorkScheduleID = entity.ID,
                                    DayOfWeek = d.DayOfWeek,
                                    WorkingType = d.WorkingType,
                                    IsWorkingDay = d.IsWorkingDay,
                                    StartTime = d.StartTime,
                                    EndTime = d.EndTime,
                                    WorkDurationMinutes = d.WorkDurationMinutes,
                                    CreatedUserID = requestModel.CurrentUserID,
                                    LastUpdatedUserID = requestModel.CurrentUserID,
                                    InsertedAt = DateTime.Now,
                                    UpdatedAt = DateTime.Now
                                };

                                _unitOfWork.ElogbookRepository<WorkScheduleDay>().Add(newDay);
                                _unitOfWork.Complete();
                                exist = newDay;
                            }
                            else
                            {
                                exist.WorkingType = d.WorkingType;
                                exist.IsWorkingDay = d.IsWorkingDay;
                                exist.StartTime = d.StartTime;
                                exist.EndTime = d.EndTime;
                                exist.WorkDurationMinutes = d.WorkDurationMinutes;
                                exist.LastUpdatedUserID = requestModel.CurrentUserID;
                                exist.UpdatedAt = DateTime.Now;

                                _unitOfWork.ElogbookRepository<WorkScheduleDay>().Update(exist);
                                _unitOfWork.Complete();
                            }

                            // Handle day breaks (add new or update existing, and delete removed ones)
                            // Get all existing day breaks for this work schedule day
                            var existingDayBreaks = _unitOfWork.ElogbookRepository<WorkScheduleDayBreak>()
                                .GetQuery(x => x.WorkScheduleDayID == exist.ID && !x.IsDeleted).ToList();

                            // Get list of day break IDs from request
                            var requestDayBreakIds = d.DayBreaks?.Where(db => db != null && db.ID > 0).Select(db => db.ID).ToList() ?? new List<int>();

                            // Delete day breaks not in the request (soft delete)
                            var dayBreaksToDelete = existingDayBreaks.Where(x => !requestDayBreakIds.Contains(x.ID)).ToList();
                            foreach (var dayBreakToDelete in dayBreaksToDelete)
                            {
                                _unitOfWork.ElogbookRepository<WorkScheduleDayBreak>().Delete(dayBreakToDelete, requestModel.CurrentUserID);
                            }

                            if (d.DayBreaks != null && d.DayBreaks.Count > 0)
                            {
                                foreach (var db in d.DayBreaks)
                                {
                                    if (db == null) continue;

                                    WorkScheduleDayBreak dayBreakEntity = null;
                                    if (db.ID > 0)
                                    {
                                        dayBreakEntity = _unitOfWork.ElogbookRepository<WorkScheduleDayBreak>()
                                            .GetQuery(x => x.ID == db.ID && !x.IsDeleted).FirstOrDefault();
                                    }

                                    if (dayBreakEntity == null)
                                    {
                                        dayBreakEntity = new WorkScheduleDayBreak
                                        {
                                            WorkScheduleDayID = exist.ID,
                                            BreakName = db.BreakName,
                                            BreakDurationMinutes = db.BreakDurationMinutes,
                                            CreatedUserID = requestModel.CurrentUserID,
                                            LastUpdatedUserID = requestModel.CurrentUserID,
                                            InsertedAt = DateTime.Now,
                                            UpdatedAt = DateTime.Now
                                        };

                                        if (!string.IsNullOrWhiteSpace(db.BreakStartTime)
                                            && TimeSpan.TryParse(db.BreakStartTime, out TimeSpan dbs))
                                        {
                                            dayBreakEntity.BreakStartTime = dbs;
                                        }

                                        if (!string.IsNullOrWhiteSpace(db.BreakEndTime)
                                            && TimeSpan.TryParse(db.BreakEndTime, out TimeSpan dbe))
                                        {
                                            dayBreakEntity.BreakEndTime = dbe;
                                        }

                                        _unitOfWork.ElogbookRepository<WorkScheduleDayBreak>().Add(dayBreakEntity);
                                    }
                                    else
                                    {
                                        dayBreakEntity.BreakName = !string.IsNullOrWhiteSpace(db.BreakName) ? db.BreakName : dayBreakEntity.BreakName;
                                        dayBreakEntity.BreakDurationMinutes = db.BreakDurationMinutes > 0 ? db.BreakDurationMinutes : dayBreakEntity.BreakDurationMinutes;
                                        if (!string.IsNullOrWhiteSpace(db.BreakStartTime)
                                            && TimeSpan.TryParse(db.BreakStartTime, out TimeSpan dbs2))
                                        {
                                            dayBreakEntity.BreakStartTime = dbs2;
                                        }
                                        if (!string.IsNullOrWhiteSpace(db.BreakEndTime)
                                            && TimeSpan.TryParse(db.BreakEndTime, out TimeSpan dbe2))
                                        {
                                            dayBreakEntity.BreakEndTime = dbe2;
                                        }

                                        dayBreakEntity.LastUpdatedUserID = requestModel.CurrentUserID;
                                        dayBreakEntity.UpdatedAt = DateTime.Now;

                                        _unitOfWork.ElogbookRepository<WorkScheduleDayBreak>().Update(dayBreakEntity);
                                    }
                                }
                                _unitOfWork.Complete();
                            }
                        }
                    }

                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    LogCustom.LogMessage(TrackingLogLevel.ERROR, "WorkScheduleService",
                        "Thêm mới lịch làm việc không thành công!" + Environment.NewLine + ex.ToString());
                    return false;
                }
            }
        }

        /// <summary>
        /// lấy dữ liệu ban đầu để hiển thị trên màn hình thêm mới lịch làm việc
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public WorkScheduleResponseModel GetInitialData()
        {
            try
            {
                var data = _unitOfWork.ElogbookRepository<WorkSchedule>().GetQuery(x => !x.IsDeleted);
                var resultData = data.Select(x => new WorkScheduleResponseModel
                {
                    ID = x.ID,
                    ScheduleName = x.ScheduleName,
                    WorkType = x.WorkType,
                    StandardStartTime = x.StandardStartTime,
                    StandardEndTime = x.StandardEndTime,
                    StandardWorkDurationMinutes = x.StandardWorkDurationMinutes,
                    IsActive = x.IsActive,
                    BreakDays = x.WorkScheduleBreaks.Where(b => !b.IsDeleted)
                    .Select(b => new WorkScheduleBreakResponseModel
                    {
                        ID = b.ID,
                        BreakName = b.BreakName,
                        BreakStartTime = b.BreakStartTime,
                        BreakEndTime = b.BreakEndTime,
                        BreakDurationMinutes = b.BreakDurationMinutes
                    }).ToList(),
                    WorkDays = x.WorkScheduleDays.Where(d => !d.IsDeleted)
                    .Select(d => new WorkScheduleDayResponseModel
                    {
                        ID = d.ID,
                        DayOfWeek = d.DayOfWeek,
                        WorkingType = d.WorkingType,
                        IsWorkingDay = d.IsWorkingDay,
                        StartTime = d.StartTime,
                        EndTime = d.EndTime,
                        WorkDurationMinutes = d.WorkDurationMinutes,
                        DayBreaks = d.WorkScheduleDayBreaks.Where(db => !db.IsDeleted)
                        .Select(db => new WorkScheduleDayBreakResponseModel
                        {
                            ID = db.ID,
                            BreakName = db.BreakName,
                            BreakStartTime = db.BreakStartTime,
                            BreakEndTime = db.BreakEndTime,
                            BreakDurationMinutes = db.BreakDurationMinutes
                        }).ToList()
                    }).ToList()
                }).FirstOrDefault();

                return resultData;
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "WorkScheduleService", "GetInitialData thất bại: " + ex.ToString());
                return new WorkScheduleResponseModel();
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả các lịch nghỉ lễ
        /// </summary>
        /// <returns></returns>
        public List<HolidayScheduleResponseModel> GetAllHolidaySchedule()
        {
            try
            {
                var list = _unitOfWork.ElogbookRepository<HolidaySchedule>()
                    .GetQuery(x => !x.IsDeleted)
                    .Select(e => new HolidayScheduleResponseModel
                    {
                        ID = e.ID,
                        HolidayName = e.HolidayName,
                        StartDate = e.StartDate,
                        EndDate = e.EndDate,
                        Description = e.Description
                    }).ToList();

                return list;
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "WorkScheduleService", "GetAllHolidaySchedule failed: " + ex.ToString());
                return new List<HolidayScheduleResponseModel>();
            }
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một lịch nghỉ lễ theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public HolidayScheduleResponseModel GetHolidayScheduleById(int id)
        {
            try
            {
                var entity = _unitOfWork.ElogbookRepository<HolidaySchedule>()
                    .GetQuery(x => x.ID == id && !x.IsDeleted)
                    .Select(e => new HolidayScheduleResponseModel
                    {
                        ID = e.ID,
                        HolidayName = e.HolidayName,
                        StartDate = e.StartDate,
                        EndDate = e.EndDate,
                        Description = e.Description
                    }).FirstOrDefault();

                return entity;
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "WorkScheduleService", "GetHolidayScheduleById thất bại: " + ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Thêm mới hoặc chỉnh sửa một lịch nghỉ lễ
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        public bool AddEditHolidaySchedule(HolidayScheduleRequestModel requestModel, int currentUserId)
        {
            try
            {
                if (requestModel == null) return false;

                DateTime stDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.StartDate, false);
                DateTime enDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.EndDate, true);

                if (requestModel.ID <= 0)
                {
                    var entity = new HolidaySchedule
                    {
                        HolidayName = requestModel.HolidayName,
                        StartDate = stDate,
                        EndDate = enDate,
                        Description = requestModel.Description,
                        CreatedUserID = currentUserId,
                        LastUpdatedUserID = currentUserId,
                        InsertedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    _unitOfWork.ElogbookRepository<HolidaySchedule>().Add(entity);
                    return _unitOfWork.Complete() > 0;
                }
                else
                {
                    var entity = _unitOfWork.ElogbookRepository<HolidaySchedule>()
                        .GetQuery(x => x.ID == requestModel.ID && !x.IsDeleted).FirstOrDefault();

                    if (entity == null) return false;

                    entity.HolidayName = !string.IsNullOrWhiteSpace(requestModel.HolidayName) ? requestModel.HolidayName : entity.HolidayName;
                    entity.StartDate = !string.IsNullOrWhiteSpace(requestModel.StartDate) ? stDate : entity.StartDate;
                    entity.EndDate = !string.IsNullOrWhiteSpace(requestModel.EndDate) ? enDate : entity.EndDate;
                    entity.Description = !string.IsNullOrWhiteSpace(requestModel.Description) ? requestModel.Description : entity.Description;
                    entity.LastUpdatedUserID = currentUserId;
                    entity.UpdatedAt = DateTime.Now;

                    _unitOfWork.ElogbookRepository<HolidaySchedule>().Update(entity);
                    return _unitOfWork.Complete() > 0;
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "WorkScheduleService", "AddEditHolidaySchedule thất bại: " + ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Xóa một lịch nghỉ lễ theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        public bool DeleteHolidaySchedule(int id, int currentUserId)
        {
            try
            {
                var entity = _unitOfWork.ElogbookRepository<HolidaySchedule>()
                    .GetQuery(x => x.ID == id && !x.IsDeleted).FirstOrDefault();

                if (entity == null) return false;

                _unitOfWork.ElogbookRepository<HolidaySchedule>().Delete(entity, currentUserId);
                return _unitOfWork.Complete() > 0;
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "WorkScheduleService", "DeleteHolidaySchedule thất bại: " + ex.ToString());
                return false;
            }
        }
    }
}
