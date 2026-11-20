using Elogbook.Core;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request.SyncAMs;
using Elogbook.Core.Models.Response.MasterData.AMs;
using Elogbook.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;

namespace Elogbook.Domain.Services
{
    /// <summary>
    /// Xử lý đồng bộ dữ liệu mobile app
    /// </summary>
    public class AmMobileSyncDataService : IAmMobileSyncDataService
    {
        public class Result
        {
            public string Message { get; set; }
        }

        private readonly ElogbookUnitOfWork _unitOfWork;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public AmMobileSyncDataService(ElogbookUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region public

        /// <summary>
        /// Đồng bộ dữ liệu tạm dừng bảo trì AM
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel SyncAM(MobileAutonomousMaintenanceSyncDataModel requestModel)
        {
            ResponseBaseModel responseModel = new ResponseBaseModel();
            List<int> lstAutonomousMaintenanceID = new List<int>();
            LogCustom.LogMessage(TrackingLogLevel.DEBUG, "SyncAM", Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel));

            using (var transaction = _unitOfWork.DataContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
            {
                try
                {
                    if (requestModel.MobileAutonomousMaintenanceModels.Count > 0)
                    {
                        foreach (var item in requestModel.MobileAutonomousMaintenanceModels)
                        {
                            var entity = _unitOfWork.ElogbookRepository<AutonomousMaintenance>().GetQuery(x => x.ID == item.ID).FirstOrDefault();
                            if (entity != null)
                            {
                                if (!lstAutonomousMaintenanceID.Contains(entity.ID))
                                    lstAutonomousMaintenanceID.Add(entity.ID);

                                // Nếu mobile xóa AM thì thực hiện xóa các thông tin liên quan của AM
                                if (item.IsDeleted)
                                {
                                    var sqlParams = new SqlParameter[]
                                    {
                                        new SqlParameter("AutonomousMaintenanceID", System.Data.SqlDbType.Int)
                                        {
                                            Value = item.ID,
                                        },
                                        new SqlParameter("CurrentUserID", System.Data.SqlDbType.Int)
                                        {
                                            Value = requestModel.CurrentUserID,
                                        },
                                    };

                                    var result = _unitOfWork.DataContext.Database
                                        .SqlQuery<Result>("AM_DeleteAutonomousMaintenance @AutonomousMaintenanceID, @CurrentUserID", sqlParams).ToList();

                                    if (result.Count > 0 && result[0].Message == "SUCCESS")
                                    {
                                        //ignore error when create success
                                    }
                                    else
                                    {
                                        Exception exception = new Exception($"Đồng bộ dữ liệu AM ID = {item.ID} thất bại. Vui lòng liên hệ quản trị viên để kiểm tra lỗi.");
                                        throw exception;
                                    }
                                }
                                // Xử lý cập nhật dữ liệu
                                else
                                {
                                    // Cập nhập trạng thái cho AM
                                    entity.FactoryID = item.FactoryID;
                                    entity.AmLocationID = item.AmLocationID;
                                    entity.AmDeviceID = item.AmDeviceID;
                                    entity.OperationPositionID = item.OperationPositionID;
                                    entity.OperationShiftID = item.OperationShiftID;
                                    entity.AmMaintenanceScheduleID = item.AmMaintenanceScheduleID;
                                    entity.ActualDate = item.ActualDate;
                                    entity.AmStatusID = item.AmStatusID;
                                    entity.IsAbnormal = item.IsAbnormal;
                                    entity.AmDeviceAbnormalStatusID = item.AmDeviceAbnormalStatusID;
                                    entity.BeforeImages = HandleImages(entity.BeforeImages, item.BeforeImages);
                                    entity.AfterImages = HandleImages(entity.AfterImages, item.AfterImages);
                                    entity.DeviceAbnormalDescription = item.DeviceAbnormalDescription;
                                    entity.DeviceAbnormalAttachments = HandleImages(entity.DeviceAbnormalAttachments, item.DeviceAbnormalAttachments);
                                    entity.AmRequestTicketID = item.AmRequestTicketID;
                                    entity.CompleterUserID = item.CompleterUserID;
                                    entity.CompleterSignatureContent = item.CompleterSignatureContent;
                                    entity.CompleteDate = item.CompleteDate;
                                    entity.IsSendNotify = item.IsSendNotify;
                                    entity.IsAutoComplete = item.IsAutoComplete;
                                    entity.SyncDataKey = item.SyncDataKey;
                                    entity.AppID = item.AppID;
                                    entity.SyncedDate = DateTime.Now;
                                    entity.IsDeleted = item.IsDeleted;
                                    entity.CreatedUserID = item.CreatedUserID;
                                    entity.InsertedAt = item.InsertedAt;
                                    entity.LastUpdatedUserID = item.LastUpdatedUserID;
                                    entity.UpdatedAt = item.UpdatedAt;
                                    entity.IsSyncToOperationDiary = item.IsSyncToOperationDiary;
                                    _unitOfWork.ElogbookRepository<AutonomousMaintenance>().Update(entity);

                                    if (requestModel.AMMobileWorkingProgress != null && requestModel.AMMobileWorkingProgress.Count > 0)
                                    {
                                        SyncAMWorkingProcess(requestModel.AMMobileWorkingProgress, entity.ID, null);
                                    }

                                    if (requestModel.AMMobileStopMaintenance != null && requestModel.AMMobileStopMaintenance.Count > 0)
                                    {
                                        foreach (var stopItem in requestModel.AMMobileStopMaintenance)
                                        {
                                            SyncAMWorkStop(stopItem, entity.ID);
                                        }
                                    }

                                    // Cập nhật AM Work Detail
                                    if (requestModel.AMMobileWorkDetailModels != null && requestModel.AMMobileWorkDetailModels.Count > 0)
                                    {
                                        foreach (var amDetailItem in requestModel.AMMobileWorkDetailModels)
                                        {
                                            var amWorkDetail = _unitOfWork.ElogbookRepository<AmWorkDetail>()
                                                .GetQuery(x => x.ID == amDetailItem.ID
                                                && x.AutonomousMaintenanceID == entity.ID).FirstOrDefault();

                                            if (amWorkDetail == null)
                                            {
                                                Exception exception = new Exception($"AmWorkDetail {amDetailItem.ID} không tồn tại trong db");
                                                throw exception;
                                            }
                                            else
                                            {
                                                // Cập nhập AM detail
                                                amWorkDetail.AutonomousMaintenanceID = entity.ID;
                                                amWorkDetail.FactoryID = amDetailItem.FactoryID;
                                                amWorkDetail.AmWorkTypeID = amDetailItem.AmWorkTypeID;
                                                amWorkDetail.AmWorkID = amDetailItem.AmWorkID;
                                                amWorkDetail.AmStatusID = amDetailItem.AmStatusID;
                                                amWorkDetail.ImplementerID = amDetailItem.ImplementerID;
                                                amWorkDetail.Order = amDetailItem.Order;
                                                amWorkDetail.SLA = amDetailItem.SLA > 0 ? amDetailItem.SLA : null;
                                                amWorkDetail.Description = amDetailItem.Description;
                                                amWorkDetail.StickTime = amDetailItem.StickTime;
                                                amWorkDetail.SyncDataKey = amDetailItem.SyncDataKey;
                                                amWorkDetail.AppID = amDetailItem.AppID;
                                                amWorkDetail.SyncedDate = DateTime.Now;
                                                amWorkDetail.IsDeleted = amDetailItem.IsDeleted;
                                                amWorkDetail.CreatedUserID = amDetailItem.CreatedUserID;
                                                amWorkDetail.InsertedAt = amDetailItem.InsertedAt;
                                                amWorkDetail.LastUpdatedUserID = amDetailItem.LastUpdatedUserID;
                                                amWorkDetail.UpdatedAt = amDetailItem.UpdatedAt;

                                                _unitOfWork.ElogbookRepository<AmWorkDetail>().Update(amWorkDetail);

                                                if (requestModel.AMMobileWorkPending != null && requestModel.AMMobileWorkPending.Count > 0)
                                                {
                                                    foreach (var workPending in requestModel.AMMobileWorkPending)
                                                    {
                                                        SyncAMWorkPending(workPending, entity.ID, amWorkDetail.ID);
                                                    }
                                                }

                                                if (requestModel.AMMobileWorkingProgress != null && requestModel.AMMobileWorkingProgress.Count > 0)
                                                {
                                                    SyncAMWorkingProcess(requestModel.AMMobileWorkingProgress, entity.ID, amWorkDetail.ID);
                                                }
                                            }
                                        }
                                    }

                                    _unitOfWork.Complete();
                                }
                            }
                            else
                            {
                                Exception exception = new Exception($"AM {item.ID} không tồn tại.");
                                throw exception;
                            }
                        }

                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                    responseModel.Content = false;
                    responseModel.Message = "Đồng bộ dữ liệu thất bại. Vui lòng liên hệ quản trị viên để kiểm tra lỗi.\n" + ex.ToString();
                    LogCustom.LogMessage(TrackingLogLevel.ERROR, "AMMobileWorkDetailService",
                    Environment.NewLine + CommonFunction.SerializeObjectToString(requestModel) + Environment.NewLine + ex.ToString());
                    transaction.Rollback();
                    return responseModel;
                }
            }

            AutonomousMaintenanceSyncDataResultModel masterDataModel = new AutonomousMaintenanceSyncDataResultModel();

            if (lstAutonomousMaintenanceID != null && lstAutonomousMaintenanceID.Count > 0)
            {
                masterDataModel.MobileAutonomousMaintenances = _unitOfWork.ElogbookRepository<AutonomousMaintenance>()
                        .GetQuery(x => lstAutonomousMaintenanceID.Contains(x.ID))
                        .Select(
                           e => new MobileAutonomousMaintenance()
                           {
                               ID = e.ID,
                               FactoryID = e.FactoryID,
                               AmLocationID = e.AmLocationID,
                               AmDeviceID = e.AmDeviceID,
                               OperationPositionID = e.OperationPositionID,
                               OperationShiftID = e.OperationShiftID,
                               AmMaintenanceScheduleID = e.AmMaintenanceScheduleID,
                               ActualDate = e.ActualDate,
                               AmStatusID = e.AmStatusID,
                               IsAbnormal = e.IsAbnormal,
                               AmDeviceAbnormalStatusID = e.AmDeviceAbnormalStatusID,
                               BeforeImages = e.BeforeImages,
                               AfterImages = e.AfterImages,
                               DeviceAbnormalDescription = e.DeviceAbnormalDescription,
                               DeviceAbnormalAttachments = e.DeviceAbnormalAttachments,
                               AmRequestTicketID = e.AmRequestTicketID,
                               CompleterUserID = e.CompleterUserID,
                               CompleterSignatureContent = e.CompleterSignatureContent,
                               CompleteDate = e.CompleteDate,
                               IsSendNotify = e.IsSendNotify,
                               IsAutoComplete = e.IsAutoComplete,
                               SyncDataKey = e.SyncDataKey,
                               AppID = e.AppID,
                               SyncedDate = e.SyncedDate,
                               IsDeleted = e.IsDeleted,
                               CreatedUserID = e.CreatedUserID,
                               InsertedAt = e.InsertedAt,
                               LastUpdatedUserID = e.LastUpdatedUserID,
                               UpdatedAt = e.UpdatedAt,
                               IsSyncToOperationDiary = e.IsSyncToOperationDiary,
                           }).ToList();

                foreach (var item in masterDataModel.MobileAutonomousMaintenances)
                {
                    // Chữ kỳ hoàn thành
                    if (!item.IsAutoComplete)
                    {
                        var users = _unitOfWork.ElogbookRepository<User>().GetById(item.CompleterUserID.GetValueOrDefault());
                        if (users != null)
                        {
                            item.CompleterSignatureContent = users.Signature;
                        }
                    }
                }

                // Thông tin chi tiết thực hiện công việc AM
                masterDataModel.MobileAmWorkDetails = _unitOfWork.ElogbookRepository<AmWorkDetail>()
                    .GetQuery(x => lstAutonomousMaintenanceID.Contains(x.AutonomousMaintenanceID))
                    .Select(
                       e => new MobileAmWorkDetail()
                       {
                           ID = e.ID,
                           AutonomousMaintenanceID = e.AutonomousMaintenanceID,
                           FactoryID = e.FactoryID,
                           AmWorkTypeID = e.AmWorkTypeID,
                           AmWorkID = e.AmWorkID,
                           AmStatusID = e.AmStatusID,
                           ImplementerID = e.ImplementerID,
                           Order = e.Order,
                           SLA = e.SLA > 0 ? e.SLA : null,
                           Description = e.Description,
                           StickTime = e.StickTime,
                           SyncDataKey = e.SyncDataKey,
                           AppID = e.AppID,
                           SyncedDate = e.SyncedDate,
                           IsDeleted = e.IsDeleted,
                           CreatedUserID = e.CreatedUserID,
                           InsertedAt = e.InsertedAt,
                           LastUpdatedUserID = e.LastUpdatedUserID,
                           UpdatedAt = e.UpdatedAt,
                       }).ToList();

                // Không thực hiện bảo trì tự quản
                masterDataModel.MobileAmStopMaintenances = _unitOfWork.ElogbookRepository<AmStopMaintenance>()
                    .GetQuery(x => lstAutonomousMaintenanceID.Contains(x.AutonomousMaintenanceID))
                    .Select(
                       e => new MobileAmStopMaintenance()
                       {
                           ID = e.ID,
                           AutonomousMaintenanceID = e.AutonomousMaintenanceID,
                           FactoryID = e.FactoryID,
                           AmLocationID = e.AmLocationID,
                           AmReasonForStopID = e.AmReasonForStopID,
                           ImplementerID = e.ImplementerID,
                           ImplementerSignatureContent = e.ImplementerSignatureContent,
                           Description = e.Description,
                           StopTime = e.StopTime,
                           SyncDataKey = e.SyncDataKey,
                           AppID = e.AppID,
                           SyncedDate = e.SyncedDate,
                           IsDeleted = e.IsDeleted,
                           CreatedUserID = e.CreatedUserID,
                           InsertedAt = e.InsertedAt,
                           LastUpdatedUserID = e.LastUpdatedUserID,
                           UpdatedAt = e.UpdatedAt,
                       }).ToList();

                // Tạm dừng thực hiện AM
                masterDataModel.MobileAmWorkPendings = _unitOfWork.ElogbookRepository<AmWorkPending>()
                    .GetQuery(x => lstAutonomousMaintenanceID.Contains(x.AutonomousMaintenanceID))
                    .Select(
                       e => new MobileAmWorkPending()
                       {
                           ID = e.ID,
                           AutonomousMaintenanceID = e.AutonomousMaintenanceID,
                           AmWorkDetailID = e.AmWorkDetailID,
                           FactoryID = e.FactoryID,
                           AmLocationID = e.AmLocationID,
                           AmReasonForPendingID = e.AmReasonForPendingID,
                           AmReasonForStopID = e.AmReasonForStopID,
                           ImplementerID = e.ImplementerID,
                           Description = e.Description,
                           PendingTime = e.PendingTime,
                           SyncDataKey = e.SyncDataKey,
                           AppID = e.AppID,
                           SyncedDate = e.SyncedDate,
                           IsDeleted = e.IsDeleted,
                           CreatedUserID = e.CreatedUserID,
                           InsertedAt = e.InsertedAt,
                           LastUpdatedUserID = e.LastUpdatedUserID,
                           UpdatedAt = e.UpdatedAt,
                       }).ToList();

                // Tiến trình thực hiện AM
                masterDataModel.MobileAmWorkingProgresss = _unitOfWork.ElogbookRepository<AmWorkingProgress>()
                    .GetQuery(x => lstAutonomousMaintenanceID.Contains(x.AutonomousMaintenanceID))
                    .Select(
                       e => new MobileAmWorkingProgress()
                       {
                           ID = e.ID,
                           AutonomousMaintenanceID = e.AutonomousMaintenanceID,
                           FactoryID = e.FactoryID,
                           AmWorkDetailID = e.AmWorkDetailID,
                           AmWorkTypeID = e.AmWorkTypeID,
                           AmWorkID = e.AmWorkID,
                           ImplementerID = e.ImplementerID,
                           ActualDate = e.ActualDate,
                           ReasonID = e.ReasonID,
                           ReasonType = e.ReasonType,
                           Content = e.Content,
                           Remark = e.Remark,
                           SyncDataKey = e.SyncDataKey,
                           AppID = e.AppID,
                           SyncedDate = e.SyncedDate,
                           IsDeleted = e.IsDeleted,
                           CreatedUserID = e.CreatedUserID,
                           InsertedAt = e.InsertedAt,
                           LastUpdatedUserID = e.LastUpdatedUserID,
                           UpdatedAt = e.UpdatedAt,
                       }).ToList();
            }

            responseModel.Content = masterDataModel;
            responseModel.StatusCode = (int)HttpStatusCode.OK;
            responseModel.Message = "Đồng bộ dữ liệu thành công";

            return responseModel;
        }

        #endregion

        #region privated

        /// <summary>
        /// Cập nhật AM Work pending
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="autonomousMaintenanceID"></param>
        /// <param name="amWorkDetailID"></param>
        private void SyncAMWorkPending(AMMobileWorkPendingModel requestModel, int autonomousMaintenanceID, int amWorkDetailID)
        {
            var entity = _unitOfWork.ElogbookRepository<AmWorkPending>()
                .GetQuery(x => x.AmWorkDetailID == amWorkDetailID
                            && x.AutonomousMaintenanceID == autonomousMaintenanceID).FirstOrDefault();
            if (entity != null)
            {
                entity.AutonomousMaintenanceID = autonomousMaintenanceID;
                entity.AmWorkDetailID = amWorkDetailID;
                entity.FactoryID = requestModel.FactoryID;
                entity.AmLocationID = requestModel.AmLocationID;
                entity.AmReasonForPendingID = requestModel.AmReasonForPendingID;
                entity.AmReasonForStopID = requestModel.AmReasonForStopID;
                entity.ImplementerID = requestModel.ImplementerID;
                entity.Description = requestModel.Description;
                entity.PendingTime = requestModel.PendingTime;
                entity.SyncDataKey = requestModel.SyncDataKey;
                entity.AppID = requestModel.AppID;
                entity.SyncedDate = DateTime.Now;
                entity.IsDeleted = requestModel.IsDeleted;
                entity.CreatedUserID = requestModel.CreatedUserID;
                entity.InsertedAt = requestModel.InsertedAt;
                entity.LastUpdatedUserID = requestModel.LastUpdatedUserID;
                entity.UpdatedAt = requestModel.UpdatedAt;

                _unitOfWork.ElogbookRepository<AmWorkPending>().Update(entity);
            }
            else
            {
                var amWorkPending = new AmWorkPending()
                {
                    AutonomousMaintenanceID = autonomousMaintenanceID,
                    AmWorkDetailID = amWorkDetailID,
                    FactoryID = requestModel.FactoryID,
                    AmLocationID = requestModel.AmLocationID,
                    AmReasonForPendingID = requestModel.AmReasonForPendingID,
                    AmReasonForStopID = requestModel.AmReasonForStopID,
                    ImplementerID = requestModel.ImplementerID,
                    Description = requestModel.Description,
                    PendingTime = requestModel.PendingTime,
                    SyncDataKey = requestModel.SyncDataKey,
                    AppID = requestModel.AppID,
                    SyncedDate = DateTime.Now,
                    IsDeleted = requestModel.IsDeleted,
                    CreatedUserID = requestModel.CreatedUserID,
                    InsertedAt = requestModel.InsertedAt,
                    LastUpdatedUserID = requestModel.LastUpdatedUserID,
                    UpdatedAt = requestModel.UpdatedAt,
                };

                _unitOfWork.ElogbookRepository<AmWorkPending>().Add(amWorkPending);
            }
        }

        /// <summary>
        /// Đồng bộ dữ liệu không thực hiện bảo trì AM
        /// </summary>
        /// <param name="requestModel"></param>
        private void SyncAMWorkStop(AMMobileStopMaintenanceModel requestModel, int autonomousMaintenanceID)
        {
            var entity = _unitOfWork.ElogbookRepository<AmStopMaintenance>()
                .GetQuery(x => x.AutonomousMaintenanceID == autonomousMaintenanceID).FirstOrDefault();
            if (entity != null)
            {
                entity.AutonomousMaintenanceID = autonomousMaintenanceID;
                entity.FactoryID = requestModel.FactoryID;
                entity.AmLocationID = requestModel.AmLocationID;
                entity.AmReasonForStopID = requestModel.AmReasonForStopID;
                entity.ImplementerID = requestModel.ImplementerID;
                entity.ImplementerSignatureContent = requestModel.ImplementerSignatureContent;
                entity.Description = requestModel.Description;
                entity.StopTime = requestModel.StopTime;
                entity.SyncDataKey = requestModel.SyncDataKey;
                entity.AppID = requestModel.AppID;
                entity.SyncedDate = DateTime.Now;
                entity.IsDeleted = requestModel.IsDeleted;
                entity.CreatedUserID = requestModel.CreatedUserID;
                entity.InsertedAt = requestModel.InsertedAt;
                entity.LastUpdatedUserID = requestModel.LastUpdatedUserID;
                entity.UpdatedAt = requestModel.UpdatedAt;
                _unitOfWork.ElogbookRepository<AmStopMaintenance>().Update(entity);
            }
            else
            {
                var amStopMaintenance = new AmStopMaintenance()
                {
                    AutonomousMaintenanceID = autonomousMaintenanceID,
                    FactoryID = requestModel.FactoryID,
                    AmLocationID = requestModel.AmLocationID,
                    AmReasonForStopID = requestModel.AmReasonForStopID,
                    ImplementerID = requestModel.ImplementerID,
                    ImplementerSignatureContent = requestModel.ImplementerSignatureContent,
                    Description = requestModel.Description,
                    StopTime = requestModel.StopTime,
                    SyncDataKey = requestModel.SyncDataKey,
                    AppID = requestModel.AppID,
                    SyncedDate = DateTime.Now,
                    IsDeleted = requestModel.IsDeleted,
                    CreatedUserID = requestModel.CreatedUserID,
                    InsertedAt = requestModel.InsertedAt,
                    LastUpdatedUserID = requestModel.LastUpdatedUserID,
                    UpdatedAt = requestModel.UpdatedAt,
                };

                _unitOfWork.ElogbookRepository<AmStopMaintenance>().Add(amStopMaintenance);
            }
        }

        /// <summary>
        /// Đồng bộ dữ liệu tiến trình thực hiện bảo trì AM
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="autonomousMaintenanceID"></param>
        /// <param name="amWorkDetailID"></param>
        private void SyncAMWorkingProcess(List<AMMobileWorkingProgress> requestModel, int autonomousMaintenanceID, int? amWorkDetailID)
        {
            if (requestModel != null && requestModel.Count > 0)
            {
                var amWorkingProgress = requestModel.Where(x => x.AutonomousMaintenanceID == autonomousMaintenanceID
                                                            && x.AmWorkDetailID == amWorkDetailID).ToList();
                if (amWorkingProgress.Count > 0)
                {
                    foreach (var item in amWorkingProgress)
                    {
                        var updateEntity = _unitOfWork.ElogbookRepository<AmWorkingProgress>()
                                                        .GetQuery(x => x.AutonomousMaintenanceID == autonomousMaintenanceID
                                                            && x.AmWorkDetailID == amWorkDetailID
                                                            && x.ID == item.ID && !x.IsDeleted).FirstOrDefault();
                        if (updateEntity != null)
                        {
                            updateEntity.AutonomousMaintenanceID = autonomousMaintenanceID;
                            updateEntity.FactoryID = item.FactoryID;
                            updateEntity.AmWorkDetailID = item.AmWorkDetailID;
                            updateEntity.AmWorkTypeID = item.AmWorkTypeID;
                            updateEntity.AmWorkID = item.AmWorkID;
                            updateEntity.ImplementerID = item.ImplementerID;
                            updateEntity.ActualDate = item.ActualDate;
                            updateEntity.ReasonID = item.ReasonID;
                            updateEntity.ReasonType = item.ReasonType;
                            updateEntity.Content = item.Content;
                            updateEntity.Remark = item.Remark;
                            updateEntity.SyncDataKey = item.SyncDataKey;
                            updateEntity.AppID = item.AppID;
                            updateEntity.SyncedDate = DateTime.Now;
                            updateEntity.IsDeleted = item.IsDeleted;
                            updateEntity.CreatedUserID = item.CreatedUserID;
                            updateEntity.InsertedAt = item.InsertedAt;
                            updateEntity.LastUpdatedUserID = item.LastUpdatedUserID;
                            updateEntity.UpdatedAt = item.UpdatedAt;

                            _unitOfWork.ElogbookRepository<AmWorkingProgress>().Update(updateEntity);
                        }
                        else
                        {
                            var addEntity = new AmWorkingProgress()
                            {
                                AutonomousMaintenanceID = autonomousMaintenanceID,
                                FactoryID = item.FactoryID,
                                AmWorkDetailID = item.AmWorkDetailID,
                                AmWorkTypeID = item.AmWorkTypeID,
                                AmWorkID = item.AmWorkID,
                                ImplementerID = item.ImplementerID,
                                ActualDate = item.ActualDate,
                                ReasonID = item.ReasonID,
                                ReasonType = item.ReasonType,
                                Content = item.Content,
                                Remark = item.Remark,
                                SyncDataKey = item.SyncDataKey,
                                AppID = item.AppID,
                                SyncedDate = DateTime.Now,
                                IsDeleted = item.IsDeleted,
                                CreatedUserID = item.CreatedUserID,
                                InsertedAt = item.InsertedAt,
                                LastUpdatedUserID = item.LastUpdatedUserID,
                                UpdatedAt = item.UpdatedAt,
                            };

                            _unitOfWork.ElogbookRepository<AmWorkingProgress>().Add(addEntity);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Join 2 chuỗi string
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private string HandleImages(string a, string b)
        {
            if (string.IsNullOrWhiteSpace(a) && string.IsNullOrWhiteSpace(b))
            {
                // Cả a và b đều null hoặc rỗng
                return string.Empty;
            }
            else if (string.IsNullOrWhiteSpace(a))
            {
                // Nếu a null hoặc rỗng, chỉ lấy b
                return b;
            }
            else if (string.IsNullOrWhiteSpace(b))
            {
                // Nếu b null hoặc rỗng, chỉ lấy a
                return a;
            }
            else
            {
                // Cả a và b đều có giá trị, kết hợp và loại bỏ các phần tử trùng lặp
                string[] arrA = a.Split(',');
                string[] arrB = b.Split(',');
                string[] mergedArray = arrA.Concat(arrB).Distinct().ToArray();
                return string.Join(",", mergedArray);
            }
        }

        #endregion
    }
}
