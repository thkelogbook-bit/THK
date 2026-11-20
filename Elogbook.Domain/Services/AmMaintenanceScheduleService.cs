using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Extensions;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Elogbook.Data;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;

namespace Elogbook.Domain.Services
{
    /// <summary>
    /// AmMaintenanceScheduleService
    /// </summary>
    public class AmMaintenanceScheduleService : IAmMaintenanceScheduleService
    {
        #region Private fields
        private readonly ElogbookUnitOfWork _unitOfWork;
        #endregion

        #region Contructor
        public AmMaintenanceScheduleService(ElogbookUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Public method
        public bool CheckDuplicateMaintenanceSchedule(int id, string name, bool isCreate, int factoryID)
        {
            // Trường hợp thêm mới
            if (isCreate)
            {
                return _unitOfWork.ElogbookRepository<AmMaintenanceSchedule>()
                    .GetQuery(x => x.MaintenanceScheduleName.Equals(name, StringComparison.OrdinalIgnoreCase) && x.FactoryID == factoryID && !x.IsDeleted).Any();
            }
            // Trường hợp cập nhật
            else
            {
                return _unitOfWork.ElogbookRepository<AmMaintenanceSchedule>()
                    .GetQuery(x => x.ID != id && x.MaintenanceScheduleName.Equals(name, StringComparison.OrdinalIgnoreCase) && x.FactoryID == factoryID && !x.IsDeleted).Any();
            }
        }

        public int CreateAM(int maintenanceScheduleId)
        {
            var amMs = _unitOfWork.ElogbookRepository<AmMaintenanceSchedule>()
                .GetQuery(x => x.ID == maintenanceScheduleId && x.Active == (int)AmEnumUsageStatus.Apply && !x.IsDeleted).FirstOrDefault();

            if (amMs != null)
            {
                // Tạo bảo trì tự quản trước 3 ngày
                CronExpression exp = new CronExpression(amMs.CronJobFormat.Trim());

                string syncDataKey = DateTime.Now.ToString("ddMMyyyy") +
                                    "_" + amMs.FactoryID +
                                    "_" + amMs.OperationPositionID +
                                    "_" + amMs.OperationShiftID;

                DateTime currentDate = DateTime.Now;

                var diffStart = amMs.StartDate.Date - currentDate.Date;

                // Nếu ngày bắt đầu của lịch nhờ hơn hoặc bằng 3 ngày thì tiến hành tạo task
                if (diffStart.Days == 0)
                {
                    DateTimeOffset nextFire = (DateTimeOffset)exp.GetNextValidTimeAfter(DateTime.Now.AddDays(-1));

                    var runNextDate = nextFire.LocalDateTime;

                    var diffOfDates = runNextDate.Date - currentDate.Date;

                    if (runNextDate.Date >= amMs.StartDate.Date && runNextDate.Date <= amMs.EndDate.Date && diffOfDates.Days == 0)
                    {
                        // Tạo bảo trì tự quản
                        var entityAm = new AutonomousMaintenance()
                        {
                            FactoryID = amMs.FactoryID,
                            AmLocationID = amMs.AmLocationID,
                            OperationPositionID = amMs.OperationPositionID,
                            OperationShiftID = amMs.OperationShiftID,
                            AmDeviceID = amMs.AmDeviceID,
                            AmMaintenanceScheduleID = amMs.ID,
                            AmStatusID = (int)AmEnumStatus.Waiting,
                            ActualDate = runNextDate.Date,
                            SyncDataKey = syncDataKey,
                            CreatedUserID = amMs.CreatedUserID,
                            LastUpdatedUserID = amMs.CreatedUserID,
                            AmWorkDetails = amMs.AmMaintenanceWorks.Select(e => new AmWorkDetail()
                            {
                                FactoryID = amMs.FactoryID,
                                AmWorkTypeID = e.AmWorkTypeID,
                                AmWorkID = e.AmWorkID,
                                AmStatusID = (int)AmEnumStatus.Waiting,
                                Order = e.Order,
                                SLA = e.SLA > 0 ? e.SLA : null,
                                Description = e.AmWork.WorkDesc,
                                CreatedUserID = amMs.CreatedUserID,
                                LastUpdatedUserID = amMs.CreatedUserID,
                                SyncDataKey = syncDataKey,
                            }).ToList(),
                        };
                        _unitOfWork.ElogbookRepository<AutonomousMaintenance>().Add(entityAm);
                        _unitOfWork.Complete();
                    }
                }
            }

            return (int)HttpStatusCode.OK;
        }

        public ResponseModel<int> CreateMaintenanceSchedule(AmMaintenanceScheduleAddEditModel requestModel)
        {
            ResponseModel<int> responseModel = new ResponseModel<int>();

            using (var transaction = _unitOfWork.DataContext.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    string syncDataKey = DateTime.Now.ToString("ddMMyyyy") +
                                "_" + requestModel.FactoryID +
                                "_" + requestModel.OperationPositionID +
                                "_" + requestModel.OperationShiftID;

                    // Tạo lịch
                    var entityMs = new AmMaintenanceSchedule
                    {
                        MaintenanceScheduleName = requestModel.MaintenanceScheduleName,
                        FactoryID = requestModel.FactoryID,
                        AmLocationID = requestModel.SiteDescID,
                        AmDeviceID = requestModel.AssetDescID,
                        OperationPositionID = requestModel.OperationPositionID,
                        OperationShiftID = requestModel.OperationShiftID,
                        Active = requestModel.Active > 0 ? requestModel.Active : (int)AmEnumUsageStatus.Apply,
                        CronJobFormat = requestModel.CronJobFormat,
                        CronJobDesc = requestModel.CronJobDesc,
                        NotificationsTime = (int)requestModel.NotificationsTime,
                        StartDate = requestModel.StartDate,
                        EndDate = requestModel.EndDate,
                        SyncDataKey = syncDataKey,
                        AmMaintenanceWorks = requestModel.MaintenanceWorks.Select(e => new AmMaintenanceWork()
                        {
                            AmWorkTypeID = e.WorkTypeID,
                            AmWorkID = e.WorkID,
                            Order = e.Order,
                            SLA = e.SLA > 0 ? e.SLA : null,
                            SyncDataKey = syncDataKey,
                            CreatedUserID = requestModel.CurrentUserID,
                            LastUpdatedUserID = requestModel.CurrentUserID,
                        }).ToList(),
                        CreatedUserID = requestModel.CurrentUserID,
                        LastUpdatedUserID = requestModel.CurrentUserID,
                    };
                    _unitOfWork.ElogbookRepository<AmMaintenanceSchedule>().Add(entityMs);

                    _unitOfWork.Complete();

                    transaction.Commit();

                    responseModel.Data = entityMs.ID;
                    return responseModel;
                }
                catch (Exception ex)
                {
                    LogCustom.LogMessage(TrackingLogLevel.ERROR, "AmMaintenanceScheduleService: CreateAmMaintenanceSchedule", Environment.NewLine + ex.ToString());
                    transaction.Rollback();

                    responseModel.Data = 0;
                    responseModel.ErrorMessages.Add("Đã xảy ra lỗi trong quá trình tạo lịch bảo trì");

                    return responseModel;
                }
            }
        }

        public ResponseBaseModel DeleteMaintenanceSchedule(int id, int userID)
        {
            using (var transaction = _unitOfWork.DataContext.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    var entity = _unitOfWork.ElogbookRepository<AmMaintenanceSchedule>().GetQuery(x => x.ID == id && !x.IsDeleted).FirstOrDefault();
                    if (entity != null)
                    {
                        // Xóa lịch
                        _unitOfWork.ElogbookRepository<AmMaintenanceSchedule>().Delete(entity, userID);

                        // Xóa task theo lịch
                        var lstWork = _unitOfWork.ElogbookRepository<AmMaintenanceWork>()
                            .GetQuery(x => x.AmMaintenanceScheduleID == entity.ID && !x.IsDeleted).ToList();

                        if (lstWork.Count > 0)
                        {
                            foreach (var item in lstWork)
                            {
                                _unitOfWork.ElogbookRepository<AmMaintenanceWork>().Delete(item, userID);
                            }
                        }

                        
                        DateTime currentDate = DateTime.Now;
                        var lstAm = _unitOfWork.ElogbookRepository<AutonomousMaintenance>().GetQuery(x => x.AmMaintenanceScheduleID == entity.ID
                                                                                                        && x.AmStatusID == (int)AmEnumStatus.Waiting
                                                                                                        && DbFunctions.TruncateTime(x.ActualDate) > currentDate.Date
                                                                                                        && !x.IsDeleted).ToList();

                        if (lstAm.Count > 0)
                        {
                            List<int> amIds = lstAm.Select(x => x.ID).Distinct().ToList();

                            foreach (var item in lstAm)
                            {
                                _unitOfWork.ElogbookRepository<AutonomousMaintenance>().Delete(item, userID);
                            }

                            var lstAmWorkDetail = _unitOfWork.ElogbookRepository<AmWorkDetail>().GetQuery(x => amIds.Contains(x.AutonomousMaintenanceID) && !x.IsDeleted).ToList();

                            if (lstAmWorkDetail.Count > 0)
                            {
                                foreach (var item in lstAmWorkDetail)
                                {
                                    _unitOfWork.ElogbookRepository<AmWorkDetail>().Delete(item, userID);
                                }
                            }
                        }

                        if (_unitOfWork.Complete() > 0)
                        {
                            transaction.Commit();
                            return HttpResponse.OK("Xóa lịch bảo trì thành công");
                        }
                        else
                        {
                            transaction.Rollback();
                            return HttpResponse.Error("Xóa lịch bảo trì không thành công");
                        }
                    }

                    return HttpResponse.Error("Lịch bảo trì không tồn tại");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    LogCustom.LogMessage(TrackingLogLevel.ERROR, "AmMaintenanceScheduleService: DeleteMaintenanceSchedule", Environment.NewLine + ex.ToString());
                    return HttpResponse.Error("Đã có lỗi xảy ra trong quá trình xóa" + Environment.NewLine + ex.ToString());
                }
            }
        }

        public AmMaintenanceScheduleModel GetMaintenanceScheduleById(int id)
        {
            var entity = _unitOfWork.ElogbookRepository<AmMaintenanceSchedule>().GetQuery(x => x.ID == id && !x.IsDeleted).Select(
              e => new AmMaintenanceScheduleModel()
              {
                  ID = e.ID,
                  MaintenanceScheduleName = e.MaintenanceScheduleName,
                  Factory = new ValueLabelCheckedModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                  SiteDesc = new ValueLabelCheckedModel { Value = e.AmLocation.ID, Label = e.AmLocation.SiteDesc },
                  AssetDesc = new ValueLabelCheckedModel { Value = e.AmDevice.ID, Label = e.AmDevice.AssetDesc },
                  OperationPosition = new ValueLabelCheckedModel { Value = e.OperationPosition.ID, Label = e.OperationPosition.OperationPositionName },
                  OperationShift = new ValueLabelCheckedModel { Value = e.OperationShift.ID, Label = e.OperationShift.OperationShiftName },
                  CreateUser = new ValueLabelCheckedModel { Value = e.CreatedUser.ID, Label = e.CreatedUser.FullName },
                  CronJobFormat = e.CronJobFormat,
                  NotificationsTime = e.NotificationsTime,
                  CronJobDesc = e.CronJobDesc,
                  Active = e.Active,
                  StartDate = e.StartDate,
                  EndDate = e.EndDate,
              }).FirstOrDefault();

            if (entity != null)
            {
                if (entity.EndDate.Date == DateTime.MaxValue.Date)
                {
                    entity.EndDateStr = null;
                }
                else
                {
                    entity.EndDateStr = entity.EndDate.ToString("dd/MM/yyyy");
                }
                var amMaintenanceWorks = _unitOfWork.ElogbookRepository<AmMaintenanceWork>()
                    .GetQuery(x => x.AmMaintenanceScheduleID == entity.ID && !x.IsDeleted)
                    .Select(e => new AmMaintenanceWorkModel()
                    {
                        ID = e.ID,
                        Order = e.Order,
                        SLA = e.SLA > 0 ? e.SLA : null,
                        MaintenanceSchedule = new ValueLabelCheckedModel { Value = e.AmMaintenanceSchedule.ID, Label = e.AmMaintenanceSchedule.MaintenanceScheduleName },
                        Work = new ValueLabelCheckedModel { Value = e.AmWork.ID, Label = e.AmWork.WorkName },
                        WorkType = new ValueLabelCheckedModel { Value = e.AmWorkType.ID, Label = e.AmWorkType.TypeDesc },
                        CreateUser = new ValueLabelCheckedModel { Value = e.CreatedUser.ID, Label = e.CreatedUser.FullName },
                    })
                    .ToList();
                entity.MaintenanceWorks = amMaintenanceWorks;
            }

            return entity;
        }

        public ResponseBaseModel GetMaintenanceSchedules(AmMaintenanceScheduleSearchModel requestModel)
        {
            DateTime stDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.StartDate, false);
            DateTime edDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.EndDate, false);

            // Tất cả theo điều kiện
            var entities = _unitOfWork.ElogbookRepository<AmMaintenanceSchedule>().GetQuery(x => !x.IsDeleted);

            // Nhà máy
            if (requestModel.FactoryID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.FactoryID == requestModel.FactoryID);
            }

            // Thiết bị
            if (requestModel.AssetDescID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.AmDeviceID == requestModel.AssetDescID);
            }

            // Tên thiết bị
            if (!string.IsNullOrWhiteSpace(requestModel.DeviceName))
            {
                entities = entities.Where(x => x.AmDevice.AssetDesc.Contains(requestModel.DeviceName));
            }

            // Khu vực
            if (requestModel.SiteDescID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.AmLocationID == requestModel.SiteDescID);
            }

            // Tên lịch bảo trì
            if (!string.IsNullOrWhiteSpace(requestModel.MaintenanceScheduleName))
            {
                entities = entities.Where(x => x.MaintenanceScheduleName.Contains(requestModel.MaintenanceScheduleName));
            }

            // Trạng thái
            if (requestModel.Active != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.Active == requestModel.Active);
            }

            //Ngày bắt đầu
            if (!string.IsNullOrWhiteSpace(requestModel.StartDate))
            {
                entities = entities.Where(x => (DbFunctions.TruncateTime(x.StartDate) >= stDate.Date
                                            || DbFunctions.TruncateTime(x.EndDate) >= stDate.Date));
            }

            //Ngày kết thúc
            if (!string.IsNullOrWhiteSpace(requestModel.EndDate))
            {
                entities = entities.Where(x => (DbFunctions.TruncateTime(x.StartDate) <= edDate.Date
                || DbFunctions.TruncateTime(x.EndDate) <= edDate.Date));
            }

            // Loại công việc
            if (requestModel.WorkTypeID != CommonConstant.ALL_KEY)
            {
                var amworkTypeDatas = _unitOfWork.ElogbookRepository<AmMaintenanceWork>()
                    .GetQuery(x => x.AmWorkTypeID == requestModel.WorkTypeID && !x.IsDeleted).ToList();

                var idList = amworkTypeDatas.Select(x => x.AmMaintenanceScheduleID).Distinct().ToList();
                if (idList.Count > 0)
                {
                    entities = entities.Where(x => idList.Contains(x.ID));
                }
                else
                {
                    return HttpResponse.OkList(new List<AmMaintenanceScheduleModel>(), requestModel.PageNumber, requestModel.PageSize);
                }
            }

            var page = entities.Select(
                e => new AmMaintenanceScheduleModel()
                {
                    ID = e.ID,
                    MaintenanceScheduleName = e.MaintenanceScheduleName,
                    Factory = new ValueLabelCheckedModel { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                    SiteDesc = new ValueLabelCheckedModel { Value = e.AmLocation.ID, Label = e.AmLocation.SiteDesc },
                    AssetDesc = new ValueLabelCheckedModel { Value = e.AmDevice.ID, Label = e.AmDevice.AssetDesc },
                    OperationPosition = new ValueLabelCheckedModel { Value = e.OperationPosition.ID, Label = e.OperationPosition.OperationPositionName },
                    OperationShift = new ValueLabelCheckedModel { Value = e.OperationShift.ID, Label = e.OperationShift.OperationShiftName },
                    CreateUser = new ValueLabelCheckedModel { Value = e.CreatedUser.ID, Label = e.CreatedUser.FullName },
                    CronJobFormat = e.CronJobFormat,
                    CronJobDesc = e.CronJobDesc,
                    Active = e.Active,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate
                })
                .OrderByDescending(t => t.ID).ToList();

            var resultData = page.ToList();

            if (resultData.Count > 0)
            {
                foreach (var item in resultData)
                {
                    item.IsUseData = _unitOfWork.ElogbookRepository<AutonomousMaintenance>()
                        .GetQuery(x => x.AmMaintenanceScheduleID == item.ID && x.AmStatusID != (int)AmEnumStatus.Waiting && !x.IsDeleted).Any();
                }
            }

            return HttpResponse.OkList(resultData.ToPagedList(requestModel.PageNumber, requestModel.PageSize));
        }

        public bool UpdateMaintenanceSchedule(AmMaintenanceScheduleAddEditModel requestModel)
        {
            using (var transaction = _unitOfWork.DataContext.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    var entity = _unitOfWork.ElogbookRepository<AmMaintenanceSchedule>().GetQuery(x => x.ID == requestModel.ID && !x.IsDeleted).FirstOrDefault();
                    if (entity != null)
                    {
                        // Cập nhật dữ liệu table AmMaintenanceSchedule
                        entity.MaintenanceScheduleName = !string.IsNullOrWhiteSpace(requestModel.MaintenanceScheduleName) ? requestModel.MaintenanceScheduleName : entity.MaintenanceScheduleName;
                        entity.FactoryID = requestModel.FactoryID > 0 ? requestModel.FactoryID : entity.FactoryID;
                        entity.AmDeviceID = requestModel.AssetDescID > 0 ? requestModel.AssetDescID : entity.AmDeviceID;
                        entity.OperationPositionID = requestModel.OperationPositionID > 0 ? requestModel.OperationPositionID : entity.OperationPositionID;
                        entity.OperationShiftID = requestModel.OperationShiftID > 0 ? requestModel.OperationShiftID : entity.OperationShiftID;
                        entity.StartDate = requestModel.StartDate > DateTime.MinValue ? requestModel.StartDate : entity.StartDate;
                        entity.EndDate = requestModel.EndDate;
                        entity.Active = requestModel.Active > 0 ? requestModel.Active : entity.Active;
                        entity.NotificationsTime = requestModel.NotificationsTime.HasValue ? (int)requestModel.NotificationsTime : entity.NotificationsTime;
                        entity.CronJobFormat = !string.IsNullOrWhiteSpace(requestModel.CronJobFormat) ? requestModel.CronJobFormat : entity.CronJobFormat;
                        entity.CronJobDesc = !string.IsNullOrWhiteSpace(requestModel.CronJobDesc) ? requestModel.CronJobDesc : entity.CronJobDesc;
                        entity.LastUpdatedUserID = requestModel.CurrentUserID;
                        entity.UpdatedAt = DateTime.Now;

                        _unitOfWork.ElogbookRepository<AmMaintenanceSchedule>().Update(entity);

                        string syncDataKey = DateTime.Now.ToString("ddMMyyyy") +
                                                            "_" + entity.FactoryID +
                                                            "_" + entity.OperationPositionID +
                                                            "_" + entity.OperationShiftID;

                        // Cập nhật dữ liệu table AmMaintenanceWork
                        if (requestModel.MaintenanceWorks != null && requestModel.MaintenanceWorks.Count > 0)
                        {
                            foreach (var item in requestModel.MaintenanceWorks)
                            {
                                // Thêm mới
                                if (item.Action == (int)EnumAction.Insert)
                                {
                                    var entityInsert = new AmMaintenanceWork()
                                    {
                                        AmWorkTypeID = item.WorkTypeID,
                                        AmWorkID = item.WorkID,
                                        Order = item.Order,
                                        SLA = item.SLA > 0 ? item.SLA : null,
                                        CreatedUserID = requestModel.CurrentUserID,
                                        LastUpdatedUserID = requestModel.CurrentUserID,
                                        AmMaintenanceScheduleID = requestModel.ID,
                                        SyncDataKey = syncDataKey
                                    };
                                    _unitOfWork.ElogbookRepository<AmMaintenanceWork>().Add(entityInsert);
                                }

                                // Cập nhật
                                if (item.Action == (int)EnumAction.Update)
                                {
                                    var entityUpdate = _unitOfWork.ElogbookRepository<AmMaintenanceWork>()
                                        .GetQuery(x => x.ID == item.ID && x.AmMaintenanceScheduleID == requestModel.ID && !x.IsDeleted).FirstOrDefault();
                                    if (entityUpdate != null)
                                    {
                                        entityUpdate.AmWorkTypeID = item.WorkTypeID > 0 ? item.WorkTypeID : entityUpdate.AmWorkTypeID;
                                        entityUpdate.AmWorkID = item.WorkID > 0 ? item.WorkID : entityUpdate.AmWorkID;
                                        entityUpdate.Order = item.Order > 0 ? item.Order : entityUpdate.Order;
                                        entityUpdate.SLA = item.SLA > 0 ? item.SLA : entityUpdate.SLA;
                                        entityUpdate.LastUpdatedUserID = requestModel.CurrentUserID;

                                        _unitOfWork.ElogbookRepository<AmMaintenanceWork>().Update(entityUpdate);
                                    }
                                }

                                // Xóa
                                if (item.Action == (int)EnumAction.Delete)
                                {
                                    var entityDelete = _unitOfWork.ElogbookRepository<AmMaintenanceWork>()
                                        .GetQuery(x => x.ID == item.ID && x.AmMaintenanceScheduleID == requestModel.ID && !x.IsDeleted).FirstOrDefault();
                                    if (entityDelete != null)
                                    {
                                        _unitOfWork.ElogbookRepository<AmMaintenanceWork>().Delete(entityDelete, requestModel.CurrentUserID);
                                    }
                                }
                            }
                        }

                        // Cập nhật dữ liệu table AutonomousMaintenance
                        // Trường hợp bảo trì tự quản thuộc đối tượng lịch cập nhật thì tiến hành cập nhật
                        // Chỉ cập nhật cho những bảo trì đã được tạo mà chưa được bắt đầu
                        DateTime currentDate = DateTime.Now;

                        var lstAm = _unitOfWork.ElogbookRepository<AutonomousMaintenance>().GetQuery(x => x.AmMaintenanceScheduleID == entity.ID  
                                                                                                        && x.AmStatusID == (int)AmEnumStatus.Waiting 
                                                                                                        && DbFunctions.TruncateTime(x.ActualDate) > currentDate.Date 
                                                                                                        && !x.IsDeleted).ToList();
                        if (lstAm.Count > 0)
                        {
                            List<int> amIds = lstAm.Select(x => x.ID).Distinct().ToList();

                            foreach (var am in lstAm)
                            {
                                am.FactoryID = entity.FactoryID;
                                am.AmLocationID = entity.AmLocationID;
                                am.OperationPositionID = entity.OperationPositionID;
                                am.OperationShiftID = entity.OperationShiftID;
                                am.AmDeviceID = entity.AmDeviceID;
                                am.AmMaintenanceScheduleID = entity.ID;
                                am.LastUpdatedUserID = entity.CreatedUserID;

                                _unitOfWork.ElogbookRepository<AutonomousMaintenance>().Update(am);
                            }

                            // Cập nhật dữ liệu table AmWorkDetail
                            // Xóa hết những task Am hiện tại
                            _unitOfWork.ElogbookRepository<AmWorkDetail>().Delete(x => amIds.Contains(x.AutonomousMaintenanceID), 0, true);

                            // Tạo lại task Am
                            foreach (var amId in amIds)
                            {
                                foreach (var taskAm in requestModel.MaintenanceWorks)
                                {
                                    if (taskAm.Action == (int)EnumAction.Insert || taskAm.Action == (int)EnumAction.Update)
                                    {
                                        var amWorkDesc = _unitOfWork.ElogbookRepository<AmWork>()
                                        .GetQuery(x => x.ID == taskAm.WorkID && !x.IsDeleted)
                                        .Select(x => x.WorkDesc).FirstOrDefault();

                                        var amDetail = new AmWorkDetail()
                                        {
                                            AutonomousMaintenanceID = amId,
                                            AmWorkTypeID = taskAm.WorkTypeID,
                                            FactoryID = entity.FactoryID,
                                            AmWorkID = taskAm.WorkID,
                                            AmStatusID = (int)AmEnumStatus.Waiting,
                                            Order = taskAm.Order,
                                            SLA = taskAm.SLA > 0 ? taskAm.SLA : null,
                                            Description = amWorkDesc,
                                            CreatedUserID = entity.CreatedUserID,
                                            LastUpdatedUserID = entity.CreatedUserID,
                                            SyncDataKey = syncDataKey,
                                        };
                                        _unitOfWork.ElogbookRepository<AmWorkDetail>().Add(amDetail);
                                    }
                                }
                            }
                        }

                        if (_unitOfWork.Complete() > 0)
                        {
                            transaction.Commit();
                            return true;
                        }
                        else
                        {
                            transaction.Rollback();
                            return false;
                        }
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    LogCustom.LogMessage(TrackingLogLevel.ERROR, "AmMaintenanceScheduleService:UpdateMaintenanceSchedule ", Environment.NewLine + ex.ToString());
                    transaction.Rollback();
                    return false;
                }
            }

        }
        #endregion
    }
}
