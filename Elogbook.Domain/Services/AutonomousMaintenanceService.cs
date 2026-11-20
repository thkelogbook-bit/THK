using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Extensions;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Elogbook.Core.Models.Response.EmailNotification;
using Elogbook.Core.Models.Response.EmailService;
using Elogbook.Core.Models.Response.User;
using Elogbook.Data;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using Attachment = Elogbook.Data.Attachment;

namespace Elogbook.Domain.Services
{
    /// <summary>
    /// Xử lý dữ liệu
    /// </summary>
    public class AutonomousMaintenanceService : IAutonomousMaintenanceService
    {
        #region Private fields
        private readonly ElogbookUnitOfWork _unitOfWork;
        private readonly ICommonService _commonService;
        private const string MP4_EXTENTION = ".mp4";
        private const string MOV_EXTENTION = ".mov";
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IEmailConfigurationService _emailConfigurationService;
        private readonly ISystemConfigurationService _systemConfigurationService;
        #endregion

        #region Contructor
        public AutonomousMaintenanceService(ElogbookUnitOfWork unitOfWork,
            ICommonService commonService,
            IEmailTemplateService emailTemplateService,
            IEmailConfigurationService emailConfigurationService,
            ISystemConfigurationService systemConfigurationService)
        {
            _unitOfWork = unitOfWork;
            _commonService = commonService;
            _emailTemplateService = emailTemplateService;
            _emailConfigurationService = emailConfigurationService;
            _systemConfigurationService = systemConfigurationService;
        }
        #endregion

        #region Public method
        public int CreateAutonomousMaintenance()
        {
            // Lấy tất cả danh sách lịch đang active
            DateTime currentDate = DateTime.Now;
            var entities = _unitOfWork.ElogbookRepository<AmMaintenanceSchedule>().GetQuery(x => !x.IsDeleted
                                                                                            && x.Active == (int)AmEnumUsageStatus.Apply
                                                                                            && DbFunctions.TruncateTime(x.StartDate) <= currentDate.Date
                                                                                            && DbFunctions.TruncateTime(x.EndDate) >= currentDate.Date
                                                                                            && !string.IsNullOrEmpty(x.CronJobFormat)).ToList();

            foreach (var item in entities)
            {
                using (var transaction = _unitOfWork.DataContext.Database.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        CronExpression exp = new CronExpression(item.CronJobFormat.Trim());

                        List<DateTime> lstDates = new List<DateTime>();

                        for (int i = -1; i < 3; i++)
                        {
                            DateTimeOffset nextFire = (DateTimeOffset)exp.GetNextValidTimeAfter(DateTime.Now.AddDays(i));

                            var nextDate = nextFire.LocalDateTime;

                            var diffOfDates = nextDate.Date - currentDate.Date;

                            if (nextDate.Date >= item.StartDate.Date && nextDate.Date <= item.EndDate.Date
                                && !lstDates.Contains(nextDate.Date) && diffOfDates.Days <= 3)
                            {
                                lstDates.Add(nextDate.Date);
                            }
                        }

                        var amEntities = _unitOfWork.ElogbookRepository<AutonomousMaintenance>().GetQuery(x => !x.IsDeleted
                                && x.AmMaintenanceScheduleID == item.ID
                                && DbFunctions.TruncateTime(x.ActualDate) >= currentDate.Date).ToList();

                        var actualDateExisted = amEntities.Select(x => x.ActualDate).ToList();

                        if (lstDates.Count > 0)
                        {
                            foreach (var actualDate in lstDates)
                            {
                                if (!actualDateExisted.Contains(actualDate))
                                {
                                    string syncDataKey = DateTime.Now.ToString("ddMMyyyy") +
                                               "_" + item.FactoryID +
                                               "_" + item.OperationPositionID +
                                               "_" + item.OperationShiftID;

                                    // Tạo bảo trì tự quản
                                    var entityAm = new AutonomousMaintenance()
                                    {
                                        FactoryID = item.FactoryID,
                                        AmLocationID = item.AmLocationID,
                                        AmDeviceID = item.AmDeviceID,
                                        OperationPositionID = item.OperationPositionID,
                                        OperationShiftID = item.OperationShiftID,
                                        AmMaintenanceScheduleID = item.ID,
                                        ActualDate = actualDate.Date,
                                        AmStatusID = (int)AmEnumStatus.Waiting,
                                        IsAbnormal = false,
                                        AmDeviceAbnormalStatusID = null,
                                        BeforeImages = null,
                                        AfterImages = null,
                                        DeviceAbnormalDescription = null,
                                        DeviceAbnormalAttachments = null,
                                        AmRequestTicketID = null,
                                        CompleterUserID = null,
                                        CompleterSignatureContent = null,
                                        CompleteDate = null,
                                        IsSendNotify = false,
                                        IsAutoComplete = false,
                                        SyncDataKey = syncDataKey,
                                        AppID = null,
                                        SyncedDate = DateTime.Now,
                                        IsDeleted = false,
                                        CreatedUserID = item.CreatedUserID,
                                        InsertedAt = DateTime.Now,
                                        LastUpdatedUserID = item.CreatedUserID,
                                        UpdatedAt = DateTime.Now,
                                        AmWorkDetails = item.AmMaintenanceWorks.Where(x => !x.IsDeleted).Select(e => new AmWorkDetail()
                                        {
                                            FactoryID = item.FactoryID,
                                            AmWorkTypeID = e.AmWorkTypeID,
                                            AmWorkID = e.AmWorkID,
                                            AmStatusID = (int)AmEnumStatus.Waiting,
                                            ImplementerID = null,
                                            Order = e.Order,
                                            SLA = e.SLA,
                                            Description = e.AmWork.WorkDesc,
                                            StickTime = null,
                                            SyncDataKey = syncDataKey,
                                            AppID = null,
                                            SyncedDate = DateTime.Now,
                                            IsDeleted = false,
                                            CreatedUserID = item.CreatedUserID,
                                            InsertedAt = DateTime.Now,
                                            LastUpdatedUserID = item.CreatedUserID,
                                            UpdatedAt = DateTime.Now,
                                        }).ToList(),
                                    };
                                    _unitOfWork.ElogbookRepository<AutonomousMaintenance>().Add(entityAm);

                                    var asset = new PMIS_ASSET_MAINTENANCE()
                                    {
                                        AMID = null,
                                        ASSETID = item.AmDevice.AssetCode,
                                        WORDERID = null,
                                        TASKID = null,
                                        TASKDESC = entityAm.AmDeviceID.ToString(),
                                        TPLSTARTDATE = null,
                                        TPLFINISHDATE = null,
                                        TACSTARTDATE = null,
                                        TACFINISHDATE = null,
                                        WTYPEID = CommonConstant.TYPE_BAO_TRI_TU_QUAN,
                                        IsDeleted = false,
                                        CreatedUserID = null,
                                        InsertedAt = entityAm.InsertedAt,
                                        LastUpdatedUserID = null,
                                        UpdatedAt = entityAm.UpdatedAt,
                                        JobId = null,
                                        Status = entityAm.AmStatusID,
                                        IsSync = null,
                                    };
                                    _unitOfWork.ElogbookRepository<PMIS_ASSET_MAINTENANCE>().Add(asset);
                                }
                            }
                        }

                        _unitOfWork.Complete();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        LogCustom.LogMessage(TrackingLogLevel.ERROR, "AutonomousMaintenanceService: CreateAutonomousMaintenance", Environment.NewLine + ex.ToString());
                        transaction.Rollback();
                    }
                }
            }

            return (int)HttpStatusCode.OK;
        }

        public ResponseModel<bool> CreateAmStopMaintenance(AmStopMaintenanceAddEditModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            using (var transaction = _unitOfWork.DataContext.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    var amData = _unitOfWork.ElogbookRepository<AutonomousMaintenance>().GetById(requestModel.AutonomousMaintenanceID);

                    if (amData != null)
                    {
                        string syncDataKey = DateTime.Now.ToString("ddMMyyyy") +
                                                            "_" + amData.FactoryID +
                                                            "_" + amData.OperationPositionID +
                                                            "_" + amData.OperationShiftID;

                        string strImplementerSignatureContent = null;

                        var users = _unitOfWork.ElogbookRepository<User>().GetQuery(x => !x.IsDeleted && x.ID == requestModel.ImplementerUserID).FirstOrDefault();

                        // Trường hợp có tồn tại chữ ký trong table User thì ko cần tạo
                        if (users != null && !string.IsNullOrWhiteSpace(users.Signature))
                        {
                            strImplementerSignatureContent = users.Signature;
                        }
                        // Trường hợp chưa tồn tại chữ ký trong table user thì lưu chữ ký
                        else
                        {
                            // Tạo chữ ký và lưu vào Signature
                            if (requestModel.ImplementerSignature != null && !string.IsNullOrWhiteSpace(requestModel.ImplementerSignature.SignatureContent))
                            {
                                strImplementerSignatureContent = requestModel.ImplementerSignature.SignatureContent;
                            }
                        }

                        var amStop = _unitOfWork.ElogbookRepository<AmStopMaintenance>().GetQuery(x => x.AutonomousMaintenanceID == amData.ID).FirstOrDefault();

                        if (amStop != null)
                        {
                            // Cập nhật lý do Không thực hiện bảo trì
                            amStop.FactoryID = amData.FactoryID;
                            amStop.AmLocationID = amData.AmLocationID;
                            amStop.AmReasonForStopID = requestModel.ReasonForStopID;
                            amStop.StopTime = DateTime.Now;
                            amStop.ImplementerID = requestModel.CurrentUserID;
                            amStop.LastUpdatedUserID = requestModel.CurrentUserID;
                            amStop.ImplementerSignatureContent = strImplementerSignatureContent;
                            amStop.UpdatedAt = DateTime.Now;
                            amStop.SyncedDate = DateTime.Now;
                            _unitOfWork.ElogbookRepository<AmStopMaintenance>().Update(amStop);
                        }
                        else
                        {
                            // Tạo dữ liệu dừng bảo trì vào AmStopMaintenance
                            var entity = new AmStopMaintenance()
                            {
                                AutonomousMaintenanceID = amData.ID,
                                FactoryID = amData.FactoryID,
                                AmLocationID = amData.AmLocationID,
                                AmReasonForStopID = requestModel.ReasonForStopID,
                                ImplementerID = requestModel.ImplementerUserID,
                                ImplementerSignatureContent = strImplementerSignatureContent,
                                Description = !string.IsNullOrWhiteSpace(requestModel.Description) ? requestModel.Description : null,
                                StopTime = DateTime.Now,
                                SyncDataKey = syncDataKey,
                                AppID = null,
                                SyncedDate = DateTime.Now,
                                IsDeleted = false,
                                CreatedUserID = requestModel.CurrentUserID,
                                InsertedAt = DateTime.Now,
                                LastUpdatedUserID = requestModel.CurrentUserID,
                                UpdatedAt = DateTime.Now,
                            };

                            _unitOfWork.ElogbookRepository<AmStopMaintenance>().Add(entity);
                        }

                        // tạo tiến trình
                        var amWorkingProgress = new AmWorkingProgress()
                        {
                            AutonomousMaintenanceID = amData.ID,
                            FactoryID = amData.FactoryID,
                            AmWorkDetailID = null,
                            AmWorkTypeID = null,
                            AmWorkID = null,
                            ImplementerID = requestModel.CurrentUserID,
                            ActualDate = amData.ActualDate,
                            ReasonID = requestModel.ReasonForStopID,
                            ReasonType = null,
                            Content = "Không thực hiện",
                            Remark = !string.IsNullOrWhiteSpace(requestModel.Description) ? requestModel.Description : null,
                            SyncDataKey = syncDataKey,
                            AppID = null,
                            SyncedDate = DateTime.Now,
                            IsDeleted = false,
                            CreatedUserID = requestModel.CurrentUserID,
                            InsertedAt = DateTime.Now,
                            LastUpdatedUserID = requestModel.CurrentUserID,
                            UpdatedAt = DateTime.Now,
                        };

                        // Lưu tiến trình
                        _unitOfWork.ElogbookRepository<AmWorkingProgress>().Add(amWorkingProgress);
                        // Cập nhật trạng thái (Không thực hiện) vào AutonomousMaintenance
                        amData.AmStatusID = (int)AmEnumStatus.DoNotWork;
                        DateTime dateUpdate = amData.UpdatedAt;
                        amData.UpdatedAt = DateTime.Now;
                        amData.LastUpdatedUserID = requestModel.CurrentUserID;
                        amData.CompleterUserID = requestModel.ImplementerUserID;
                        amData.CompleterSignatureContent = strImplementerSignatureContent;
                        amData.CompleteDate = DateTime.Now;
                        amData.SyncedDate = DateTime.Now;
                        _unitOfWork.ElogbookRepository<AutonomousMaintenance>().Update(amData);

                        _unitOfWork.Complete();
                        // Đồng bộ dữ liệu vào NKVH
                        SyncDataToOperationDiary(amData, requestModel.CurrentUserID);

                        // Cập nhật những công việc AM có trạng thái Đang thực hiện thành Dừng thực hiện
                        var amWorkDetails = _unitOfWork.ElogbookRepository<AmWorkDetail>()
                            .GetQuery(x => x.AutonomousMaintenanceID == requestModel.AutonomousMaintenanceID && !x.IsDeleted && x.AmStatusID == (int)AmEnumStatus.OnGoing).ToList();

                        if (amWorkDetails.Count > 0)
                        {
                            foreach (var item in amWorkDetails)
                            {
                                var requestStopWorkDetail = new AmWorkingProgressAddModel()
                                {
                                    ActionType = (int)AmEnumStatus.Stop,
                                    AutonomousMaintenanceID = requestModel.AutonomousMaintenanceID,
                                    WorkDetailID = item.ID,
                                    ImplementerID = requestModel.ImplementerUserID,
                                    Content = EnumExtensions.GetEnumDescription(AmEnumStatus.Stop),
                                    ReasonID = requestModel.ReasonForStopID,
                                    ReasonType = (int)AmEnumReasonType.StopWork,
                                    Description = !string.IsNullOrWhiteSpace(requestModel.Description) ? requestModel.Description : null,
                                    PendingDate = DateTime.Now,
                                    CurrentUserID = requestModel.CurrentUserID,
                                };
                                StopTaskAM(requestStopWorkDetail);
                            }
                        }

                        // Lịch sử bảo trì thiết bị
                        AddHistoryData(amData.AmDeviceID, amData.ID, amData.AmStatusID, dateUpdate, amData.UpdatedAt);

                        transaction.Commit();

                        // Trường hợp AM có bất thường thiết bị thì gửi mail thông báo
                        if(amData.IsAbnormal == true && amData.AmDeviceAbnormalStatusID > 1)
                        {
                            CreateMailDeviceAbnormal(amData.ID);
                        }

                        responseModel.Data = true;
                    }
                    else
                    {
                        responseModel.Data = false;
                        responseModel.ErrorMessages.Add("Bảo trì tự quản không tồn tại");
                    }

                    return responseModel;
                }
                catch (Exception ex)
                {
                    LogCustom.LogMessage(TrackingLogLevel.ERROR, "AutonomousMaintenanceService: CreateAmStopMaintenance", Environment.NewLine + ex.ToString());
                    transaction.Rollback();

                    responseModel.Data = false;
                    responseModel.ErrorMessages.Add("Bảo trì tự quản không tồn tại");

                    return responseModel;
                }
            }
        }

        public AutonomousMaintenanceModel GetAutonomousMaintenanceById(int id)
        {
            var entity = _unitOfWork.ElogbookRepository<AutonomousMaintenance>().GetQuery(x => x.ID == id && !x.IsDeleted).Select(
              e => new AutonomousMaintenanceModel()
              {
                  ID = e.ID,
                  MaintenanceSchedule = new ValueLabelCheckedModel() { Value = e.AmMaintenanceSchedule.ID, Label = e.AmMaintenanceSchedule.MaintenanceScheduleName },
                  Factory = new ValueLabelCheckedModel() { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                  OperationPosition = new ValueLabelCheckedModel() { Value = e.OperationPosition.ID, Label = e.OperationPosition.OperationPositionName },
                  OperationShift = new ValueLabelCheckedModel() { Value = e.OperationShift.ID, Label = e.OperationShift.OperationShiftName },
                  Status = new AmStatusInfoModel { ID = e.AmStatus.ID, Status = e.AmStatus.StatusName },
                  Device = new ValueLabelCheckedModel { Value = e.AmDevice.ID, Label = e.AmDevice.AssetDesc },
                  Location = new ValueLabelCheckedModel { Value = e.AmLocation.ID, Label = e.AmLocation.SiteDesc },
                  ActualDate = e.ActualDate,
                  WorkDetails = e.AmWorkDetails.Select(t => new AmWorkDetailModel()
                  {
                      ID = t.ID,
                      Factory = new ValueLabelCheckedModel() { Value = t.Factory.ID, Label = t.Factory.FactoryName },
                      AutonomousMaintenance = new ValueLabelCheckedModel() { Value = t.AutonomousMaintenance.ID, Label = "" },
                      Work = new ValueLabelCheckedModel() { Value = t.AmWork.ID, Label = t.AmWork.WorkName },
                      WorkType = new ValueLabelCheckedModel() { Value = t.AmWorkType.ID, Label = t.AmWorkType.TypeDesc },
                      Status = new AmStatusInfoModel { ID = t.AmStatus.ID, Status = t.AmStatus.StatusName },
                      Order = t.Order,
                      SLA = t.SLA > 0 ? t.SLA : null,
                      Description = t.Description,
                      StickTime = t.StickTime,
                      Implementer = t.ImplementerID != null ? new ValueLabelCheckedModel() { Value = t.Implementer.ID, Label = t.Implementer.FullName } : null,
                      UpdatedAt = t.UpdatedAt,
                  }).ToList(),
                  DeviceAbnormalStatus = e.AmDeviceAbnormalStatusID.HasValue ? new ValueLabelCheckedModel { Value = e.AmDeviceAbnormalStatus.ID, Label = e.AmDeviceAbnormalStatus.Name } : null,
                  DeviceAbnormalDescription = e.DeviceAbnormalDescription,
                  DeviceAbnormalAttachmentIds = e.DeviceAbnormalAttachments,
                  BeforeImageIds = e.BeforeImages,
                  AfterImageIds = e.AfterImages,
                  RequestTicketID = e.AmRequestTicketID,
                  CompleterUserID = e.CompleterUserID,
                  CompleterUser = e.CompleterUserID != null ? new ValueLabelCheckedModel() { Value = e.CompleterUser.ID, Label = e.CompleterUser.FullName } : null,
                  CompleteDate = e.CompleteDate,
                  CompleterSignatureContent = e.CompleterSignatureContent,
                  SyncDataKey = e.SyncDataKey,
                  AppID = e.AppID,
                  SyncedDate = e.SyncedDate,
                  IsAutoComplete = e.IsAutoComplete,
                  InsertedAt = e.InsertedAt,
                  UpdatedAt = e.UpdatedAt,
                  AmRequestTicketID = e.AmRequestTicketID,
                  CreatedUser = e.LastUpdatedUserID != null ? new ValueLabelModel() { Value = e.CreatedUser.ID, Label = e.CreatedUser.FullName } : null,
                  LastUpdatedUser = e.LastUpdatedUserID != null ? new ValueLabelModel() { Value = e.LastUpdatedUser.ID, Label = e.LastUpdatedUser.FullName } : null,
              }).FirstOrDefault();

            if (entity != null)
            {
                // Hình ảnh file đính kèm trước thực hiện bảo trì
                if (!string.IsNullOrWhiteSpace(entity.BeforeImageIds))
                {
                    var attackmentIds = entity.BeforeImageIds.Split(',').Select(int.Parse).ToList();

                    var dataBeforeImages = _unitOfWork.ElogbookRepository<Attachment>().GetQuery(x => !x.IsDeleted && attackmentIds.Contains(x.ID)).ToList();

                    if (dataBeforeImages.Count > 0)
                    {
                        var beforeImages = new List<AttachmentModel>();
                        foreach (var item in dataBeforeImages)
                        {
                            var newImageInfo = new AttachmentModel()
                            {
                                ID = item.ID,
                                FileName = item.FileName,
                                FilePath = _commonService.GetPathFile(item.FilePath),
                                PathThumbnail = _commonService.GetPathFile(item.PathThumbnail),
                                ScreenName = item.ScreenName,
                                FileType = item.FileType,
                                InsertedAt = item.InsertedAt,
                                UpdatedAt = item.UpdatedAt
                            };
                            beforeImages.Add(newImageInfo);
                        }
                        entity.BeforeImages = beforeImages;
                    }
                }

                // Hình ảnh file đính kèm sau thực hiện bảo trì
                if (!string.IsNullOrWhiteSpace(entity.AfterImageIds))
                {
                    var attackmentIds = entity.AfterImageIds.Split(',').Select(int.Parse).ToList();

                    var dataAfterImages = _unitOfWork.ElogbookRepository<Attachment>().GetQuery(x => !x.IsDeleted && attackmentIds.Contains(x.ID)).ToList();

                    if (dataAfterImages.Count > 0)
                    {
                        var afterImages = new List<AttachmentModel>();
                        foreach (var item in dataAfterImages)
                        {
                            var newImageInfo = new AttachmentModel()
                            {
                                ID = item.ID,
                                FileName = item.FileName,
                                FilePath = _commonService.GetPathFile(item.FilePath),
                                PathThumbnail = _commonService.GetPathFile(item.PathThumbnail),
                                ScreenName = item.ScreenName,
                                FileType = item.FileType,
                                InsertedAt = item.InsertedAt,
                                UpdatedAt = item.UpdatedAt
                            };
                            afterImages.Add(newImageInfo);
                        }
                        entity.AfterImages = afterImages;
                    }
                }

                // Hình ảnh bất thường thiết bị
                if (!string.IsNullOrWhiteSpace(entity.DeviceAbnormalAttachmentIds))
                {
                    var attackmentIds = entity.DeviceAbnormalAttachmentIds.Split(',').Select(int.Parse).ToList();

                    var dataDeviceAbnormalAttachments = _unitOfWork.ElogbookRepository<Attachment>().GetQuery(x => !x.IsDeleted && attackmentIds.Contains(x.ID)).ToList();

                    if (dataDeviceAbnormalAttachments.Count > 0)
                    {
                        var attchments = new List<AttachmentModel>();
                        foreach (var item in dataDeviceAbnormalAttachments)
                        {
                            var attchmentInfo = new AttachmentModel()
                            {
                                ID = item.ID,
                                FileName = item.FileName,
                                FilePath = _commonService.GetPathFile(item.FilePath),
                                PathThumbnail = _commonService.GetPathFile(item.PathThumbnail),
                                ScreenName = item.ScreenName,
                                FileType = item.FileType,
                                InsertedAt = item.InsertedAt,
                                UpdatedAt = item.UpdatedAt
                            };
                            attchments.Add(attchmentInfo);
                        }
                        entity.DeviceAbnormalAttachments = attchments;
                    }
                }

                // Lý do không thực hiện bảo trì tự quản
                entity.StopMaintenance = GetAmStopMaintenanceById(entity.ID);

                // Tiến trình
                entity.AmWorkingProgress = GetAmWorkingProgress(entity.ID);

                // Chữ kỳ hoàn thành
                // Kiểm tra nếu AM nhiều hơn một công việc thực hiện thì mới lấy chữ ký từ 
                if (!entity.IsAutoComplete && entity.WorkDetails.Count > 1)
                {
                    var users = _unitOfWork.ElogbookRepository<User>().GetById(entity.CompleterUserID.GetValueOrDefault());
                    if (users != null)
                    {
                        entity.CompleterSignatureContent = users.Signature;
                    }
                }

                //Tính toán StickTime
                if (entity.WorkDetails.Count > 0)
                {
                    foreach (var item in entity.WorkDetails)
                    {
                        if (item.Status.ID == (int)AmEnumStatus.OnGoing)
                        {
                            item.StickTimeInProgress = GetStickTime(item.UpdatedAt, DateTime.Now, item.StickTime);
                        }

                        if (item.Status.ID == (int)AmEnumStatus.Pending)
                        {
                            item.StickTimeInProgress = item.StickTime;
                        }
                    }
                }
            }

            return entity;
        }

        public AmStopMaintenanceModel GetAmStopMaintenanceById(int autonomousMaintenanceID)
        {
            var entity = _unitOfWork.ElogbookRepository<AmStopMaintenance>().GetQuery(x => x.AutonomousMaintenanceID == autonomousMaintenanceID).Select(
                e => new AmStopMaintenanceModel()
                {
                    ID = e.ID,
                    Factory = new ValueLabelCheckedModel() { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                    AutonomousMaintenance = new ValueLabelCheckedModel() { Value = e.AutonomousMaintenance.ID, Label = string.Empty },
                    AmLocation = new ValueLabelCheckedModel() { Value = e.AmLocation.ID, Label = e.AmLocation.SiteDesc },
                    AmReasonForStop = new ValueLabelCheckedModel() { Value = e.AmReasonForStop.ID, Label = e.AmReasonForStop.ReasonDesc },
                    Description = e.Description,
                    StopTime = e.StopTime,
                    ImplementerSignatureContent = e.ImplementerSignatureContent,
                    ImplementerID = e.ImplementerID,
                    Implementer = e.ImplementerID != null ? new ValueLabelCheckedModel() { Value = e.Implementer.ID, Label = e.Implementer.FullName } : null,
                    InsertedAt = e.InsertedAt,
                    AppID = e.AppID,
                    SyncDataKey = e.SyncDataKey,
                    SyncedDate = e.SyncedDate
                }).FirstOrDefault();

            // Chữ ký người thực hiện
            if (entity != null && string.IsNullOrWhiteSpace(entity.ImplementerSignatureContent))
            {
                // Kiểm tra nếu AM nhiều hơn một công việc thực hiện thì mới lấy chữ ký từ 
                var amWorkDetails = _unitOfWork.ElogbookRepository<AmWorkDetail>().GetQuery(x => !x.IsDeleted && x.AutonomousMaintenanceID == autonomousMaintenanceID).ToList();
                if (amWorkDetails.Count > 1)
                {
                    var users = _unitOfWork.ElogbookRepository<User>().GetQuery(x => !x.IsDeleted && x.ID == entity.ImplementerID).FirstOrDefault();
                    if (users != null)
                    {
                        entity.ImplementerSignatureContent = users.Signature;
                    }
                }
            }

            return entity;
        }

        public ResponseBaseModel GetAutonomousMaintenances(AutonomousMaintenanceSearchModel requestModel)
        {
            DateTime frDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.StartDate, false);
            DateTime toDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.EndDate, false);

            // Tất cả theo điều kiện
            var entities = _unitOfWork.ElogbookRepository<AutonomousMaintenance>().GetQuery(x => !x.IsDeleted);

            // Nhà máy
            if (requestModel.FactoryID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.FactoryID == requestModel.FactoryID);
            }

            // Cương vị
            if (requestModel.OperationPositionID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.OperationPositionID == requestModel.OperationPositionID);
            }

            // Khu vực
            if (requestModel.LocationID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.AmLocationID == requestModel.LocationID);
            }

            // Tên lịch công tác
            if (!string.IsNullOrWhiteSpace(requestModel.MaintenanceScheduleName))
            {
                entities = entities.Where(x => x.AmMaintenanceSchedule.MaintenanceScheduleName.Contains(requestModel.MaintenanceScheduleName));
            }

            // Trạng thái
            if (requestModel.StatusID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.AmStatusID == requestModel.StatusID);
            }

            // Thiết bị id
            if (requestModel.DeviceID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.AmDeviceID == requestModel.DeviceID);
            }

            // Tên thiết bị
            if (!string.IsNullOrWhiteSpace(requestModel.DeviceName))
            {
                entities = entities.Where(x => x.AmDevice.AssetDesc.Contains(requestModel.DeviceName));
            }

            // Trạng thái bất thường thiết bị
            if (requestModel.AmDeviceAbnormalStatusID != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.AmDeviceAbnormalStatusID == requestModel.AmDeviceAbnormalStatusID);
            }

            //Ngày bắt đầu
            if (!string.IsNullOrWhiteSpace(requestModel.StartDate))
            {
                entities = entities.Where(x => DbFunctions.TruncateTime(x.ActualDate) >= frDate.Date);
            }

            //Ngày kết thúc
            if (!string.IsNullOrWhiteSpace(requestModel.EndDate))
            {
                entities = entities.Where(x => DbFunctions.TruncateTime(x.ActualDate) <= toDate.Date);
            }

            // Chỉ hiển thị những dữ liệu ngày hiện tại trở lại đối với trường hợp tìm kiếm tất cả
            DateTime currentDate = DateTime.Now;
            entities = entities.Where(x => DbFunctions.TruncateTime(x.ActualDate) <= currentDate.Date);

            var page = entities.Select(
                e => new AutonomousMaintenanceModel()
                {
                    ID = e.ID,
                    Factory = new ValueLabelCheckedModel()
                    {
                        Value = e.Factory.ID,
                        Label = e.Factory.FactoryName
                    },
                    MaintenanceSchedule = new ValueLabelCheckedModel()
                    {
                        Value = e.AmMaintenanceSchedule.ID,
                        Label = e.AmMaintenanceSchedule.MaintenanceScheduleName
                    },
                    OperationPosition = new ValueLabelCheckedModel()
                    {
                        Value = e.OperationPosition.ID,
                        Label = e.OperationPosition.OperationPositionName
                    },
                    OperationShift = new ValueLabelCheckedModel()
                    {
                        Value = e.OperationShift.ID,
                        Label = e.OperationShift.OperationShiftName
                    },
                    Status = new AmStatusInfoModel
                    {
                        ID = e.AmStatus.ID,
                        Status = e.AmStatus.StatusName
                    },
                    Device = new ValueLabelCheckedModel
                    {
                        Value = e.AmDevice.ID,
                        Label = e.AmDevice.AssetDesc
                    },
                    Location = new ValueLabelCheckedModel
                    {
                        Value = e.AmLocation.ID,
                        Label = e.AmLocation.SiteDesc
                    },
                    DeviceAbnormalStatus = e.AmDeviceAbnormalStatusID.HasValue ? new ValueLabelCheckedModel
                    {
                        Value = e.AmDeviceAbnormalStatus.ID,
                        Label = e.AmDeviceAbnormalStatus.Name
                    } : new ValueLabelCheckedModel { Value = 0, Label = string.Empty },
                    WorkDetails = e.AmWorkDetails.Select(t => new AmWorkDetailModel()
                    {
                        ID = t.ID,
                        Factory = new ValueLabelCheckedModel() { Value = t.Factory.ID, Label = t.Factory.FactoryName },
                        AutonomousMaintenance = new ValueLabelCheckedModel() { Value = t.AutonomousMaintenance.ID, Label = "" },
                        Work = new ValueLabelCheckedModel() { Value = t.AmWork.ID, Label = t.AmWork.WorkName },
                        WorkType = new ValueLabelCheckedModel() { Value = t.AmWorkType.ID, Label = t.AmWorkType.TypeDesc },
                        Status = new AmStatusInfoModel { ID = t.AmStatus.ID, Status = t.AmStatus.StatusName },
                        Order = t.Order,
                        SLA = t.SLA > 0 ? t.SLA : null,
                        Description = t.Description,
                        StickTime = t.StickTime,
                        Implementer = t.ImplementerID != null ? new ValueLabelCheckedModel() { Value = t.Implementer.ID, Label = t.Implementer.FullName } : null,
                        UpdatedAt = t.UpdatedAt,
                    }).ToList(),
                    ActualDate = e.ActualDate,
                    InsertedAt = e.InsertedAt,
                    DeviceID = e.AmDeviceID,
                    DeviceName = e.AmDevice.AssetDesc,
                    OperationShiftID = e.OperationShiftID,
                    OperationShiftName = e.OperationShift.OperationShiftName,
                    AmRequestTicketID = e.AmRequestTicketID,
                })
                .OrderByDescending(t => t.ActualDate)
                .ThenBy(t => t.DeviceName)
                .ThenBy(t => t.OperationShiftName)
                .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            return HttpResponse.OkList(page);
        }

        /// <summary>
        /// Thực hiện công việc (Bắt đầu, tạm dừng, dừng, tiếp tục, hoàn thành)
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseModel<AmWorkDetailModel> WorkProcessing(AmWorkingProgressAddModel requestModel)
        {
            ResponseModel<AmWorkDetailModel> responseModel = new ResponseModel<AmWorkDetailModel>();

            var amWorkDetail = _unitOfWork.ElogbookRepository<AmWorkDetail>().GetById(requestModel.WorkDetailID);
            if (amWorkDetail != null)
            {
                // Xử lý khoảng thời gian làm việc
                string stickTime = "00:00";
                if (requestModel.ActionType == (int)AmEnumStatus.Pending
                    || requestModel.ActionType == (int)AmEnumStatus.Stop
                    || requestModel.ActionType == (int)AmEnumStatus.Completed)
                {
                    stickTime = GetStickTime(amWorkDetail.UpdatedAt, DateTime.Now, amWorkDetail.StickTime);
                }

                if (requestModel.ActionType == (int)AmEnumStatus.Continue)
                {
                    stickTime = amWorkDetail.StickTime;
                }

                // Cập nhập trạng thái cho công việc
                amWorkDetail.AmStatusID = requestModel.ActionType;
                if (requestModel.ActionType == (int)AmEnumStatus.Continue)
                {
                    amWorkDetail.AmStatusID = (int)AmEnumStatus.OnGoing;
                }
                amWorkDetail.LastUpdatedUserID = requestModel.CurrentUserID;
                amWorkDetail.ImplementerID = requestModel.CurrentUserID;
                amWorkDetail.StickTime = stickTime;
                amWorkDetail.UpdatedAt = DateTime.Now;
                amWorkDetail.SyncedDate = DateTime.Now;
                _unitOfWork.ElogbookRepository<AmWorkDetail>().Update(amWorkDetail);

                // Trường hợp tạm dừng hoặc dừng thực hiện công việc thì lưu vào table AmWorkPending
                if (requestModel.ActionType == (int)AmEnumStatus.Pending || requestModel.ActionType == (int)AmEnumStatus.Stop)
                {
                    var amWorkPending = _unitOfWork.ElogbookRepository<AmWorkPending>().GetQuery(x => x.AmWorkDetailID == requestModel.WorkDetailID).FirstOrDefault();
                    if (amWorkPending != null)
                    {
                        if (requestModel.ReasonType == 1)
                        {
                            amWorkPending.AmReasonForPendingID = requestModel.ReasonID;
                        }
                        if (requestModel.ReasonType == 2)
                        {
                            amWorkPending.AmReasonForStopID = requestModel.ReasonID;
                        }
                        amWorkPending.Description = !string.IsNullOrWhiteSpace(requestModel.Description) ? requestModel.Description : null;
                        amWorkPending.PendingTime = DateTime.Now;
                        amWorkPending.ImplementerID = requestModel.ImplementerID > 0 ? requestModel.ImplementerID : requestModel.CurrentUserID;
                        amWorkPending.UpdatedAt = DateTime.Now;
                        amWorkPending.SyncedDate = DateTime.Now;

                        _unitOfWork.ElogbookRepository<AmWorkPending>().Update(amWorkPending);
                    }
                    else
                    {
                        var amWorkPendingAdd = new AmWorkPending()
                        {
                            AutonomousMaintenanceID = requestModel.AutonomousMaintenanceID,
                            AmWorkDetailID = requestModel.WorkDetailID,
                            FactoryID = amWorkDetail.FactoryID,
                            AmLocationID = amWorkDetail.AutonomousMaintenance.AmLocationID,
                            ImplementerID = requestModel.ImplementerID > 0 ? requestModel.ImplementerID : requestModel.CurrentUserID,
                            Description = !string.IsNullOrWhiteSpace(requestModel.Description) ? requestModel.Description : null,
                            PendingTime = requestModel.PendingDate != null ? (DateTime)requestModel.PendingDate : DateTime.Now,
                            SyncDataKey = null,
                            AppID = null,
                            SyncedDate = DateTime.Now,
                            IsDeleted = false,
                            CreatedUserID = requestModel.CurrentUserID,
                            InsertedAt = DateTime.Now,
                            LastUpdatedUserID = requestModel.CurrentUserID,
                            UpdatedAt = DateTime.Now,
                        };
                        if (requestModel.ReasonType == 1)
                        {
                            amWorkPendingAdd.AmReasonForPendingID = requestModel.ReasonID;
                        }
                        if (requestModel.ReasonType == 2)
                        {
                            amWorkPendingAdd.AmReasonForStopID = requestModel.ReasonID;
                        }
                        _unitOfWork.ElogbookRepository<AmWorkPending>().Add(amWorkPendingAdd);
                    }
                }

                // Tạo tiến trình
                var amWorkingProgress = new AmWorkingProgress()
                {
                    AutonomousMaintenanceID = requestModel.AutonomousMaintenanceID,
                    FactoryID = amWorkDetail.FactoryID,
                    AmWorkDetailID = requestModel.WorkDetailID,
                    AmWorkTypeID = amWorkDetail.AmWorkTypeID,
                    AmWorkID = amWorkDetail.AmWorkID,
                    ImplementerID = requestModel.CurrentUserID,
                    ActualDate = amWorkDetail.AutonomousMaintenance.ActualDate,
                    ReasonID = requestModel.ReasonID.HasValue ? requestModel.ReasonID : null,
                    ReasonType = requestModel.ReasonType.HasValue ? requestModel.ReasonType : null,
                    Content = !string.IsNullOrWhiteSpace(requestModel.Content) ? requestModel.Content : null,
                    Remark = !string.IsNullOrWhiteSpace(requestModel.Description) ? requestModel.Description : null,
                    SyncDataKey = null,
                    AppID = null,
                    SyncedDate = DateTime.Now,
                    IsDeleted = false,
                    CreatedUserID = requestModel.CurrentUserID,
                    InsertedAt = DateTime.Now,
                    LastUpdatedUserID = requestModel.CurrentUserID,
                    UpdatedAt = DateTime.Now,
                };
                _unitOfWork.ElogbookRepository<AmWorkingProgress>().Add(amWorkingProgress);

                // Đổi trạng thái "đang thực hiện" cho bảo trì tự quản khi "bắt đầu" thực hiện bất kỳ 1 công việc nào
                var am = _unitOfWork.ElogbookRepository<AutonomousMaintenance>().GetById(amWorkDetail.AutonomousMaintenanceID);
                if (am != null && am.AmStatusID == (int)AmEnumStatus.Waiting && requestModel.ActionType == (int)AmEnumStatus.OnGoing)
                {
                    am.AmStatusID = (int)AmEnumStatus.OnGoing;
                    am.UpdatedAt = DateTime.Now;
                    am.LastUpdatedUserID = requestModel.CurrentUserID;
                    am.SyncedDate = DateTime.Now;
                    _unitOfWork.ElogbookRepository<AutonomousMaintenance>().Update(am);
                }

                // Trường hợp có chỉ có 1 task AM và thực hiện thao tác "Dừng thực hiện"
                if (am != null && am.AmStatusID == (int)AmEnumStatus.Waiting && requestModel.ActionType == (int)AmEnumStatus.Stop)
                {
                    var lstAmDetail = _unitOfWork.ElogbookRepository<AmWorkDetail>().GetQuery(x => x.AutonomousMaintenanceID == am.ID).ToList();

                    if (lstAmDetail.Count == 1)
                    {
                        var amStop = _unitOfWork.ElogbookRepository<AmStopMaintenance>().GetQuery(x => x.AutonomousMaintenanceID == am.ID).FirstOrDefault();

                        if (amStop != null)
                        {
                            // Cập nhật lý do Không thực hiện bảo trì
                            amStop.FactoryID = am.FactoryID;
                            amStop.AmLocationID = am.AmLocationID;
                            amStop.AmReasonForStopID = requestModel.ReasonID;
                            amStop.StopTime = DateTime.Now;
                            amStop.ImplementerID = requestModel.CurrentUserID;
                            amStop.Description = requestModel.Description;
                            amStop.LastUpdatedUserID = requestModel.CurrentUserID;
                            amStop.UpdatedAt = DateTime.Now;
                            amStop.SyncedDate = DateTime.Now;

                            _unitOfWork.ElogbookRepository<AmStopMaintenance>().Update(amStop);
                        }
                        else
                        {
                            // Thêm lý do Không thực hiện bảo trì
                            var entity = new AmStopMaintenance()
                            {
                                AutonomousMaintenanceID = am.ID,
                                FactoryID = am.FactoryID,
                                AmLocationID = am.AmLocationID,
                                AmReasonForStopID = requestModel.ReasonID,
                                ImplementerID = requestModel.CurrentUserID,
                                ImplementerSignatureContent = null,
                                Description = requestModel.Description,
                                StopTime = DateTime.Now,
                                SyncDataKey = null,
                                AppID = null,
                                SyncedDate = DateTime.Now,
                                IsDeleted = false,
                                CreatedUserID = requestModel.CurrentUserID,
                                InsertedAt = DateTime.Now,
                                LastUpdatedUserID = requestModel.CurrentUserID,
                                UpdatedAt = DateTime.Now,
                            };
                            _unitOfWork.ElogbookRepository<AmStopMaintenance>().Add(entity);
                        }

                        // Cập nhật trạng thái "Không thực hiện" cho AutonomousMaintenance
                        am.AmStatusID = (int)AmEnumStatus.DoNotWork;
                        am.UpdatedAt = DateTime.Now;
                        am.CompleteDate = DateTime.Now;
                        am.CompleterUserID = requestModel.CurrentUserID;
                        am.LastUpdatedUserID = requestModel.CurrentUserID;
                        am.SyncedDate = DateTime.Now;
                        _unitOfWork.ElogbookRepository<AutonomousMaintenance>().Update(am);
                    }
                }

                var success = _unitOfWork.Complete() > 0;

                if (success)
                {
                    // Đồng bộ dữ liệu vào NKVH
                    SyncDataToOperationDiary(am, requestModel.CurrentUserID);
                    responseModel.Data = GetAmWorkDetailById(amWorkDetail.ID);

                    if(am != null)
                    {
                        var lstAmDetail = _unitOfWork.ElogbookRepository<AmWorkDetail>().GetQuery(x => x.AutonomousMaintenanceID == am.ID).ToList();
                        if (lstAmDetail.Count == 1)
                        {
                            // Trường hợp AM có bất thường thiết bị thì gửi mail thông báo
                            CreateMailDeviceAbnormal(am.ID);
                        }
                    }
                }
            }

            return responseModel;
        }

        public AmWorkDetailModel GetAmWorkDetailById(int id)
        {
            var entity = _unitOfWork.ElogbookRepository<AmWorkDetail>().GetQuery(x => x.ID == id && !x.IsDeleted).Select(
              e => new AmWorkDetailModel()
              {
                  ID = e.ID,
                  Factory = new ValueLabelCheckedModel() { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                  AutonomousMaintenance = new ValueLabelCheckedModel() { Value = e.AutonomousMaintenance.ID, Label = string.Empty },
                  Work = new ValueLabelCheckedModel() { Value = e.AmWork.ID, Label = e.AmWork.WorkName },
                  WorkType = new ValueLabelCheckedModel() { Value = e.AmWorkType.ID, Label = e.AmWorkType.TypeDesc },
                  Status = new AmStatusInfoModel { ID = e.AmStatus.ID, Status = e.AmStatus.StatusName },
                  Order = e.Order,
                  SLA = e.SLA > 0 ? e.SLA : null,
                  Description = e.Description,
                  SyncedDate = e.SyncedDate,
                  SyncDataKey = e.SyncDataKey,
                  StickTime = e.StickTime,
                  AppID = e.AppID,
                  Implementer = e.ImplementerID != null ? new ValueLabelCheckedModel() { Value = e.Implementer.ID, Label = e.Implementer.FullName } : null,
                  UpdatedAt = e.UpdatedAt,
              }).FirstOrDefault();

            return entity;
        }

        public AmWorkPendingModel GetAmWorkPendingById(int amWorkDetailID)
        {
            var entity = _unitOfWork.ElogbookRepository<AmWorkPending>().GetQuery(x => x.AmWorkDetailID == amWorkDetailID && !x.IsDeleted).Select(
              e => new AmWorkPendingModel()
              {
                  ID = e.ID,
                  Factory = new ValueLabelCheckedModel() { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                  AutonomousMaintenance = new ValueLabelCheckedModel() { Value = e.AutonomousMaintenance.ID, Label = string.Empty },
                  AmWorkDetail = new ValueLabelCheckedModel() { Value = e.AmWorkDetail.ID, Label = null },
                  Location = new ValueLabelCheckedModel() { Value = e.AmLocation.ID, Label = e.AmLocation.SiteDesc },
                  ReasonForStop = e.AmReasonForStopID != null ? new ValueLabelCheckedModel() { Value = e.AmReasonForStop.ID, Label = e.AmReasonForStop.ReasonDesc } : null,
                  ReasonForPending = e.AmReasonForPendingID != null ? new ValueLabelCheckedModel() { Value = e.AmReasonForPending.ID, Label = e.AmReasonForPending.ReasonDesc } : null,
                  Description = e.Description,
                  PendingTime = e.PendingTime,
                  Implementer = e.ImplementerID != null ? new ValueLabelCheckedModel() { Value = e.Implementer.ID, Label = e.Implementer.FullName } : null,
                  InsertedAt = e.InsertedAt,
              }).OrderByDescending(x => x.InsertedAt).FirstOrDefault();

            return entity;
        }

        public List<AmWorkingProgressModel> GetAmWorkingProgress(int autonomousMaintenanceID)
        {
            var entity = _unitOfWork.ElogbookRepository<AmWorkingProgress>().GetQuery(x => x.AutonomousMaintenanceID == autonomousMaintenanceID).Select(
                e => new AmWorkingProgressModel()
                {
                    ID = e.ID,
                    Factory = new ValueLabelCheckedModel() { Value = e.Factory.ID, Label = e.Factory.FactoryName },
                    AutonomousMaintenance = new ValueLabelCheckedModel() { Value = e.AutonomousMaintenance.ID, Label = string.Empty },
                    AmWorkDetail = e.AmWorkDetailID.HasValue ? new ValueLabelCheckedModel() { Value = e.AmWorkDetail.ID, Label = null } : null,
                    WorkType = e.AmWorkTypeID.HasValue ? new ValueLabelCheckedModel() { Value = e.AmWorkType.ID, Label = e.AmWorkType.TypeDesc } : null,
                    Work = e.AmWorkID.HasValue ? new ValueLabelCheckedModel { Value = e.AmWork.ID, Label = e.AmWork.WorkName } : null,
                    ImplementerUser = e.ImplementerID != null ? new ValueLabelCheckedModel() { Value = e.Implementer.ID, Label = e.Implementer.FullName } : null,
                    ReasonID = e.ReasonID,
                    ReasonType = e.ReasonType,
                    ActualDate = e.ActualDate,
                    Content = e.Content,
                    Remark = e.Remark,
                    UpdatedAt = e.UpdatedAt,
                    InsertedAt = e.InsertedAt
                }).ToList();

            if (entity.Count > 0)
            {
                foreach (var item in entity)
                {
                    // Trường hợp tạm dừng
                    if (item.ReasonType == (int)AmEnumReasonType.StopAM)
                    {
                        var reasonData = _unitOfWork.ElogbookRepository<AmReasonForPending>().GetQuery(x => x.ID == item.ReasonID && !x.IsDeleted).Select(e => new ValueLabelCheckedModel()
                        {
                            Value = e.ID,
                            Label = e.ReasonDesc
                        }).FirstOrDefault();

                        item.Reason = reasonData;
                    }

                    // Trường hợp dừng
                    if (item.ReasonType == (int)AmEnumReasonType.StopWork || item.ReasonType == null)
                    {
                        var reasonData = _unitOfWork.ElogbookRepository<AmReasonForStop>().GetQuery(x => x.ID == item.ReasonID && !x.IsDeleted).Select(e => new ValueLabelCheckedModel()
                        {
                            Value = e.ID,
                            Label = e.ReasonDesc
                        }).FirstOrDefault();

                        item.Reason = reasonData;
                    }
                }
            }

            return entity;
        }

        public ResponseModel<bool> UpdateDeviceAbnormal(AmDeviceAbnormalUpdateModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            var entity = _unitOfWork.ElogbookRepository<AutonomousMaintenance>().GetQuery(x => x.ID == requestModel.AutonomousMaintenanceID).FirstOrDefault();

            if (entity != null)
            {
                string imageExisted = entity.DeviceAbnormalAttachments;

                entity.AmDeviceAbnormalStatusID = requestModel.DeviceAbnormalStatusID.HasValue ? requestModel.DeviceAbnormalStatusID : entity.AmDeviceAbnormalStatusID;
                entity.DeviceAbnormalDescription = !string.IsNullOrWhiteSpace(requestModel.DeviceAbnormalDescription) ? requestModel.DeviceAbnormalDescription : entity.DeviceAbnormalDescription;
                entity.DeviceAbnormalAttachments = requestModel.DeviceAbnormalAttachments;
                entity.AmRequestTicketID = requestModel.RequestTicketID > 0 ? requestModel.RequestTicketID : entity.AmRequestTicketID;
                entity.IsAbnormal = requestModel.IsAbnormal;
                entity.LastUpdatedUserID = requestModel.CurrentUserID;
                DateTime dateUpdate = entity.UpdatedAt;
                entity.UpdatedAt = DateTime.Now;
                entity.SyncedDate = DateTime.Now;

                // Trường hợp có hình ảnh trước đó thì xóa đi rùi thêm cái mới vào
                if (!string.IsNullOrWhiteSpace(imageExisted))
                {
                    string[] lstImages = imageExisted.Split(',');

                    foreach (var idImage in lstImages)
                    {
                        int idAtt = int.Parse(idImage);

                        // Xóa file dữ liệu
                        var attachment = _unitOfWork.ElogbookRepository<Attachment>().GetQuery(x => x.ID == idAtt && !x.IsDeleted).FirstOrDefault();
                        if (attachment != null)
                        {
                            // kiểm tra file đã tồn tại chưa và xoá
                            var filePath = Path.Combine(AppSettings.SERVER_DIR, attachment.FilePath);
                            if (File.Exists(filePath))
                            {
                                File.Delete(filePath);
                            }
                            _unitOfWork.ElogbookRepository<Attachment>().Delete(attachment, requestModel.CurrentUserID);
                        }
                    }
                }

                _unitOfWork.ElogbookRepository<AutonomousMaintenance>().Update(entity);

                var success = _unitOfWork.Complete() > 0;

                AddHistoryData(entity.AmDeviceID, entity.ID, entity.AmStatusID, dateUpdate, entity.UpdatedAt);

                responseModel.Data = success;
            }
            else
            {
                responseModel.ErrorMessages.Add("Đối tượng cập nhật không tồn tại");
                responseModel.Data = false;
            }

            return responseModel;
        }

        public ResponseModel<bool> SignatureCompleteAm(AmSignatureCompleteModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            var entity = _unitOfWork.ElogbookRepository<AutonomousMaintenance>().GetQuery(x => x.ID == requestModel.AutonomousMaintenanceID).FirstOrDefault();

            if (entity != null)
            {
                // Cập nhật table AutonomousMaintenance
                entity.BeforeImages = requestModel.BeforeImages;
                entity.AfterImages = requestModel.AfterImages;
                entity.AmStatusID = (int)AmEnumStatus.Completed;
                entity.CompleterUserID = requestModel.CompleterUserID;
                entity.CompleteDate = DateTime.Now;
                entity.AmDeviceAbnormalStatusID = entity.AmDeviceAbnormalStatusID.HasValue ? entity.AmDeviceAbnormalStatusID : (int)AmEnumDeAbStatus.No;
                entity.LastUpdatedUserID = requestModel.CurrentUserID;
                DateTime dateUpdate = entity.UpdatedAt;
                entity.UpdatedAt = DateTime.Now;
                entity.SyncedDate = DateTime.Now;

                var user = _unitOfWork.ElogbookRepository<User>().GetQuery(x => !x.IsDeleted && x.ID == requestModel.CompleterUserID).FirstOrDefault();

                // Trường hợp có tồn tại chữ ký trong table User thì ko cần tạo
                if (user != null && !string.IsNullOrWhiteSpace(user.Signature))
                {
                    entity.CompleterSignatureContent = user.Signature;
                }
                // Trường hợp chưa tồn tại chữ ký trong table user thì lưu chữ ký vào table Signature
                else
                {
                    if (requestModel.CompleterSignature != null && !string.IsNullOrWhiteSpace(requestModel.CompleterSignature.SignatureContent))
                    {
                        entity.CompleterSignatureContent = requestModel.CompleterSignature.SignatureContent;
                    }
                }

                _unitOfWork.ElogbookRepository<AutonomousMaintenance>().Update(entity);
                var success = _unitOfWork.Complete() > 0;
                // Đồng bộ dữ liệu vào NKVH
                SyncDataToOperationDiary(entity, requestModel.CurrentUserID);

                responseModel.Data = success;

                if (success)
                {
                    CreateMailDeviceAbnormal(entity.ID);
                }
                AddHistoryData(entity.AmDeviceID, entity.ID, entity.AmStatusID, dateUpdate, entity.UpdatedAt);
            }
            else
            {
                responseModel.ErrorMessages.Add("Đối tượng cập nhật không tồn tại");
                responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                responseModel.Data = false;
            }

            return responseModel;
        }

        private List<UserModel> GetRelatedUsers(string permissionKey)
        {
            var roleIDs = _unitOfWork.ElogbookRepository<UserRole>().GetQuery(x => x.Permissions.Contains(permissionKey)).Select(x => x.ID).ToList();
            var userIDs = _unitOfWork.ElogbookRepository<RoleInUser>().GetQuery(x => roleIDs.Contains(x.UserRoleID)).Select(x => x.UserID).ToList();

            List<UserModel> userModels = _unitOfWork.ElogbookRepository<User>()
                .GetQuery(x => userIDs.Contains(x.ID) && !x.IsDeleted).Select(
                e => new UserModel()
                {
                    ID = e.ID,
                    FullName = e.FullName,
                    Email = e.Email,
                    UserName = e.UserName
                }).ToList();

            return userModels;
        }

        public UploadFileModel UploadFile(FileInfoModel fileInfoModel)
        {
            var rootPath = fileInfoModel.RootPath;
            var fileName = fileInfoModel.FileName;
            UploadFileModel attachmentModel = new UploadFileModel();
            using (var transaction = _unitOfWork.DataContext.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    if (MOV_EXTENTION.Equals(fileInfoModel.FileExtension, StringComparison.OrdinalIgnoreCase))
                    {
                        File.Move(fileInfoModel.LocalFileName, Path.ChangeExtension(Path.Combine(fileInfoModel.RootPath, fileInfoModel.FileName), MP4_EXTENTION));
                        fileInfoModel.FileType = "video/mp4";
                        fileInfoModel.FileExtension = ".mp4";
                        fileInfoModel.FileName = fileInfoModel.FileName.Replace(MOV_EXTENTION, MP4_EXTENTION);
                        fileName = fileInfoModel.FileName;
                    }
                    else
                    {
                        File.Move(fileInfoModel.LocalFileName, Path.Combine(fileInfoModel.RootPath, fileInfoModel.FileName));
                    }

                    File.Delete(fileInfoModel.LocalFileName);

                    string fileThumbnail = "thumbnail_" + fileInfoModel.FileName;
                    string pathThumbnail = string.Empty;
                    List<string> imageExtensions = new List<string>() { ".png", ".jpg", ".jpeg" };

                    if (imageExtensions.Contains(fileInfoModel.FileExtension.ToLower()))
                    {
                        File.Copy(Path.Combine(rootPath, fileInfoModel.FileName), Path.Combine(rootPath, fileThumbnail), true);

                        Image image = Image.FromFile(Path.Combine(rootPath, fileThumbnail));
                        Image thumb = image.GetThumbnailImage(120, 120, () => false, IntPtr.Zero);
                        thumb.Save(Path.ChangeExtension(Path.Combine(rootPath, fileThumbnail), ".png"));
                        pathThumbnail = Path.Combine(CommonConstant.AUTONOMOUS_MAINTENANCE, fileThumbnail.Split('.')[0] + ".png");
                        image.Dispose();
                        thumb.Dispose();
                    }

                    var attachment = new Attachment
                    {
                        FileName = fileInfoModel.FileName,
                        FilePath = Path.Combine(CommonConstant.AUTONOMOUS_MAINTENANCE, fileInfoModel.FileName),
                        ScreenName = CommonConstant.AUTONOMOUS_MAINTENANCE,
                        FileType = fileInfoModel.FileType,
                        PathThumbnail = pathThumbnail,
                        CreatedUserID = fileInfoModel.CurrentUserID,
                        LastUpdatedUserID = fileInfoModel.CurrentUserID
                    };

                    _unitOfWork.ElogbookRepository<Attachment>().Add(attachment);
                    var success = _unitOfWork.Complete() > 0;

                    if (success)
                    {
                        attachmentModel = new UploadFileModel
                        {
                            ID = attachment.ID,
                            FilePath = AppSettings.SERVER_DIR + attachment.FilePath,
                            PathThumbnail = AppSettings.SERVER_DIR + attachment.PathThumbnail,
                            IsError = false,
                            FileType = attachment.FileType
                        };
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    LogCustom.LogMessage(TrackingLogLevel.ERROR, "AutonomousMaintenanceService",
                    Environment.NewLine + CommonFunction.SerializeObjectToString(fileInfoModel) + Environment.NewLine + ex.ToString());

                    if (Directory.Exists(Path.Combine(rootPath, fileName)))
                    {
                        File.Delete(Path.Combine(rootPath, fileName));
                    }

                    transaction.Rollback();
                }
            }

            return attachmentModel;
        }

        public int SendMailNotificationAm()
        {
            var system = _systemConfigurationService.GetAppVersion();
            DateTime currentDate = DateTime.Now;

            // Lấy danh sách bảo trì tự quản chưa được thực hiện
            var amData = _unitOfWork.ElogbookRepository<AutonomousMaintenance>().GetQuery(x => !x.IsDeleted
                                                                                            && x.AmStatusID == (int)AmEnumStatus.Waiting
                                                                                            && DbFunctions.TruncateTime(x.ActualDate) >= currentDate.Date
                                                                                            && !x.IsSendNotify).ToList();
            if (amData.Count > 0)
            {
                // Danh sách lịch
                var amScheduleIds = amData.Select(x => x.AmMaintenanceScheduleID).Distinct().ToList();
                var amScheduleData = _unitOfWork.ElogbookRepository<AmMaintenanceSchedule>()
                    .GetQuery(x => !x.IsDeleted && amScheduleIds.Contains(x.ID) && x.NotificationsTime > -1).ToList();

                // Danh sách ca thực hiện
                var opShiftIds = amData.Select(x => x.OperationShiftID).Distinct().ToList();
                var opShiftData = _unitOfWork.ElogbookRepository<OperationShift>()
                    .GetQuery(x => !x.IsDeleted && opShiftIds.Contains(x.ID)).ToList();

                // Lấy mẫu mail
                var mailTemplate = _unitOfWork.ElogbookRepository<EmailTemplate>().GetQuery(x => x.ID == 15 && !x.IsDeleted).Select(
                    e => new EmailTemplateResponseModel()
                    {
                        EmailSubject = e.EmailSubject,
                        EmailContent = e.EmailContent,
                    }).FirstOrDefault();

                if (mailTemplate == null)
                {
                    return 0;
                }

                // Loop danh sách bảo trì tự quản chưa thực hiện
                foreach (var item in amData)
                {
                    try
                    {
                        var amSchedule = amScheduleData.Find(x => x.ID == item.AmMaintenanceScheduleID);
                        if (amSchedule != null)
                        {
                            var opShift = opShiftData.Find(x => x.ID == item.OperationShiftID);

                            // Lấy danh sách ID đã login vào cương vị trước đó
                            var trackingLogin = _unitOfWork.ElogbookRepository<TrackingLoginOperation>()
                                .GetQuery(x => !x.IsDeleted && x.OperationPositionID == amSchedule.OperationPositionID).ToList();

                            if (trackingLogin.Count > 0)
                            {
                                var lstUserID = trackingLogin.Select(x => x.CreatedUserID).ToList();
                                var userLogin = _unitOfWork.ElogbookRepository<User>().GetQuery(x => lstUserID.Contains(x.ID)).ToList();
                                if (opShift != null && userLogin.Count > 0)
                                {
                                    // Ngày thực hiện bảo trì
                                    DateTime actualDate = item.ActualDate;

                                    // Giờ thực hiện bảo trì
                                    int h = (int)opShift.StartTime;

                                    // Ngày giờ thực hiện bảo trì
                                    DateTime workDate = actualDate.AddHours(h);

                                    var diffOfDates = workDate.Subtract(DateTime.Now);

                                    if ((int)diffOfDates.TotalMinutes == amSchedule.NotificationsTime + 2)
                                    {
                                        EmailTransmissionModel transmissionsModel = new EmailTransmissionModel()
                                        {
                                            Subject = mailTemplate.EmailSubject,
                                            FatoryEmails = amSchedule.Factory.Email,
                                            FatoryName = amSchedule.Factory.FactoryName,
                                            MessageBody = UpdateContentEmail(mailTemplate.EmailContent, amSchedule, opShift, item),
                                        };

                                        SendEmail(transmissionsModel, userLogin);

                                        // Add nội dung thông báo
                                        if (userLogin.Count > 0)
                                        {
                                            foreach (var userInfo in userLogin)
                                            {
                                                if (userInfo.ID > 0)
                                                {
                                                    string titleNotice = transmissionsModel.Subject.Replace(CommonConstant.MAIL_MERGE_TITLE + " - ", string.Empty);
                                                    titleNotice = titleNotice.Replace(system.AppTitle + " - ", string.Empty);

                                                    string body = transmissionsModel.MessageBody.Replace("[Account User]", userInfo.FullName);
                                                    if (system != null)
                                                    {
                                                        body = body.Replace("[He thong Elogbook]", system.AppTitle);
                                                    }

                                                    var entityAlertNotification = new AlertNotification()
                                                    {
                                                        NoticeTitle = titleNotice + " - " + amSchedule.MaintenanceScheduleName,
                                                        NoticeContent = body,
                                                        UserID = userInfo.ID,
                                                        Status = (int)EnumNotification.Unread,
                                                        IsImportant = false,
                                                        InsertedAt = DateTime.Now,
                                                        UpdatedAt = DateTime.Now
                                                    };

                                                    _unitOfWork.ElogbookRepository<AlertNotification>().Add(entityAlertNotification);
                                                }
                                            }
                                        }

                                        // Update AM
                                        item.IsSendNotify = true;
                                        item.UpdatedAt = DateTime.Now;
                                        _unitOfWork.ElogbookRepository<AutonomousMaintenance>().Update(item);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogCustom.LogMessage(TrackingLogLevel.INFO, "Autonomous Maintenance: SendEmailNotificationAm", "Eror send mail notification: "
                                + Environment.NewLine + ex.Message);
                    }
                }
                _unitOfWork.Complete();
            }
            return 0;
        }

        /// <summary>
        /// lịch sử vận hành sửa chữa
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public ResponseBaseModel FindHistory(int deviceId)
        {
            var device = _unitOfWork.ElogbookRepository<AmDevice>().GetById(deviceId);
            var entities = _unitOfWork.ElogbookRepository<PMIS_ASSET_MAINTENANCE>().GetQuery(x => !x.IsDeleted && x.ASSETID == device.AssetCode);

            List<AmHistoryModel> amHistories = new List<AmHistoryModel>();
            foreach (var item in entities)
            {
                if (item.WTYPEID == CommonConstant.TYPE_BAO_TRI_TU_QUAN)
                {
                    if (int.TryParse(item.TASKDESC, out int amId))
                    {
                        string status = "";
                        if (item.Status.HasValue)
                        {
                            var amStatus = _unitOfWork.ElogbookRepository<AmStatus>().GetById(item.Status.Value);
                            status = amStatus.StatusName;
                        }

                        amHistories.Add(new AmHistoryModel()
                        {
                            Type = CommonConstant.BAO_TRI_TU_QUAN,
                            Info = new ValueLabelModel() { Value = amId, Label = device.AssetDesc },
                            TimeStart = item.InsertedAt.ToString("dd/MM/yyy hh:mm:ss"),
                            TimeEnd = item.UpdatedAt.ToString("dd/MM/yyy hh:mm:ss"),
                            Description = status
                        });
                    }
                }
                else
                {
                    amHistories.Add(new AmHistoryModel()
                    {
                        Type = CommonConstant.SUA_CHUA_CONG_VIEC,
                        Info = new ValueLabelModel() { Label = item.WORDERID },
                        TimeStart = "Kế hoạch: " + item.TPLSTARTDATE?.ToString("dd/MM/yyy hh:mm:ss") + Environment.NewLine + "Thực tế: " + item.TPLFINISHDATE?.ToString("dd/MM/yyy hh:mm:ss"),
                        TimeEnd = "Kế hoạch: " + item.TACSTARTDATE?.ToString("dd/MM/yyy hh:mm:ss") + Environment.NewLine + "Thực tế: " + item.TACFINISHDATE?.ToString("dd/MM/yyy hh:mm:ss"),
                        Description = item.TASKDESC
                    });
                }
            }

            // Return and paging data
            var page = amHistories.ToPagedList();

            return HttpResponse.OkList(page);
        }

        public int AutoCompleteAm()
        {
            DateTime currentDate = DateTime.Now;

            var amEntities = _unitOfWork.ElogbookRepository<AutonomousMaintenance>()
                .GetQuery(x => !x.IsDeleted && (x.AmStatusID == (int)AmEnumStatus.Waiting || x.AmStatusID == (int)AmEnumStatus.OnGoing)
                               && DbFunctions.TruncateTime(x.ActualDate) <= currentDate.Date).ToList();

            List<int> statusIds = new List<int>()
            {
                (int)AmEnumStatus.Waiting,
                (int)AmEnumStatus.OnGoing,
                (int)AmEnumStatus.Pending,
                (int)AmEnumStatus.Continue
            };

            if (amEntities.Count > 0)
            {
                foreach (var item in amEntities)
                {
                    var isExist = _unitOfWork.ElogbookRepository<AmWorkDetail>().GetQuery(x => !x.IsDeleted
                                                                                                && x.AutonomousMaintenanceID == item.ID
                                                                                                && statusIds.Contains(x.AmStatusID)).Any();

                    if (!isExist)
                    {
                        var lstDates = _unitOfWork.ElogbookRepository<AmWorkDetail>().GetQuery(x => !x.IsDeleted
                                                                                                   && x.AutonomousMaintenanceID == item.ID
                                                                                                   && !statusIds.Contains(x.AmStatusID)).Select(x => x.UpdatedAt).ToList();

                        currentDate = currentDate.AddHours(-1);

                        DateTime maxDate = lstDates.Max();

                        if (maxDate <= currentDate)
                        {
                            // thực hiện update trạng thái cho table AutonomousMaintenance
                            item.IsAutoComplete = true;
                            item.CompleteDate = DateTime.Now;
                            item.AmStatusID = (int)AmEnumStatus.Completed;
                            item.AmDeviceAbnormalStatusID = item.AmDeviceAbnormalStatusID.HasValue ? item.AmDeviceAbnormalStatusID : (int)AmEnumDeAbStatus.No;
                            item.UpdatedAt = DateTime.Now;
                            item.SyncedDate = DateTime.Now;
                            _unitOfWork.ElogbookRepository<AutonomousMaintenance>().Update(item);
                            _unitOfWork.Complete();
                            // Đồng bộ dữ liệu vào NKVH
                            SyncDataToOperationDiary(item, null);

                            CreateMailDeviceAbnormal(item.ID);
                        }
                    }
                }
            }

            return 200;
        }

        public ResponseBaseModel SaveEvidenceAm(SaveImageInfoModel requestModel)
        {
            var entity = _unitOfWork.ElogbookRepository<AutonomousMaintenance>().GetQuery(x => x.ID == requestModel.AmID).FirstOrDefault();

            if (entity != null)
            {
                entity.BeforeImages = !string.IsNullOrWhiteSpace(requestModel.BeforeImageAttIds) ? requestModel.BeforeImageAttIds : entity.BeforeImages;
                entity.AfterImages = !string.IsNullOrWhiteSpace(requestModel.AfterImageAttIds) ? requestModel.AfterImageAttIds : entity.AfterImages;
                entity.LastUpdatedUserID = requestModel.CurrentUserID;
                entity.SyncedDate = DateTime.Now;

                _unitOfWork.ElogbookRepository<AutonomousMaintenance>().Update(entity);
                var success = _unitOfWork.Complete() > 0;
                if (success)
                {
                    return HttpResponse.OK("Lưu thành công");
                }
                else
                {
                    return HttpResponse.Error("Lưu thất bại");
                }

            }
            return HttpResponse.Error("ID bảo trì tự quản không tồn tại");
        }
        #endregion

        #region Private Method
        private void StopTaskAM(AmWorkingProgressAddModel requestModel)
        {
            var amWorkDetail = _unitOfWork.ElogbookRepository<AmWorkDetail>().GetQuery(x => x.ID == requestModel.WorkDetailID).FirstOrDefault();
            if (amWorkDetail != null)
            {
                // Cập nhập trạng thái cho công việc
                amWorkDetail.AmStatusID = requestModel.ActionType;
                amWorkDetail.LastUpdatedUserID = requestModel.CurrentUserID;
                amWorkDetail.ImplementerID = requestModel.CurrentUserID;
                amWorkDetail.UpdatedAt = DateTime.Now;
                amWorkDetail.SyncedDate = DateTime.Now;
                _unitOfWork.ElogbookRepository<AmWorkDetail>().Update(amWorkDetail);

                // Trường hợp dừng thực hiện công việc thì lưu vào table AmWorkPending
                if (requestModel.ActionType == (int)AmEnumStatus.Stop)
                {
                    var amWorkPending = _unitOfWork.ElogbookRepository<AmWorkPending>().GetQuery(x => x.AmWorkDetailID == requestModel.WorkDetailID).FirstOrDefault();
                    if (amWorkPending != null)
                    {
                        if (requestModel.ReasonType == 1)
                        {
                            amWorkPending.AmReasonForPendingID = requestModel.ReasonID;
                        }
                        if (requestModel.ReasonType == 2)
                        {
                            amWorkPending.AmReasonForStopID = requestModel.ReasonID;
                        }
                        amWorkPending.Description = !string.IsNullOrWhiteSpace(requestModel.Description) ? requestModel.Description : null;
                        amWorkPending.PendingTime = DateTime.Now;
                        amWorkPending.ImplementerID = requestModel.ImplementerID > 0 ? requestModel.ImplementerID : null;
                        amWorkPending.UpdatedAt = DateTime.Now;
                        amWorkPending.SyncedDate = DateTime.Now;
                        _unitOfWork.ElogbookRepository<AmWorkPending>().Update(amWorkPending);
                    }
                    else
                    {
                        var amWorkPendingAdd = new AmWorkPending()
                        {
                            AutonomousMaintenanceID = requestModel.AutonomousMaintenanceID,
                            FactoryID = amWorkDetail.FactoryID,
                            AmWorkDetailID = requestModel.WorkDetailID,
                            AmLocationID = amWorkDetail.AutonomousMaintenance.AmLocationID,
                            ImplementerID = requestModel.ImplementerID > 0 ? requestModel.ImplementerID : null,
                            Description = !string.IsNullOrWhiteSpace(requestModel.Description) ? requestModel.Description : null,
                            PendingTime = requestModel.PendingDate != null ? (DateTime)requestModel.PendingDate : DateTime.Now,
                            SyncDataKey = null,
                            AppID = null,
                            SyncedDate = DateTime.Now,
                            IsDeleted = false,
                            CreatedUserID = requestModel.CurrentUserID,
                            InsertedAt = DateTime.Now,
                            LastUpdatedUserID = requestModel.CurrentUserID,
                            UpdatedAt = DateTime.Now,
                        };
                        if (requestModel.ReasonType == 1)
                        {
                            amWorkPendingAdd.AmReasonForPendingID = requestModel.ReasonID;
                        }
                        if (requestModel.ReasonType == 2)
                        {
                            amWorkPendingAdd.AmReasonForStopID = requestModel.ReasonID;
                        }
                        _unitOfWork.ElogbookRepository<AmWorkPending>().Add(amWorkPendingAdd);
                    }
                }

                // Tạo tiến trình
                var amWorkingProgress = new AmWorkingProgress()
                {
                    AutonomousMaintenanceID = requestModel.AutonomousMaintenanceID,
                    FactoryID = amWorkDetail.FactoryID,
                    AmWorkDetailID = requestModel.WorkDetailID,
                    AmWorkTypeID = amWorkDetail.AmWorkTypeID,
                    AmWorkID = amWorkDetail.AmWorkID,
                    ImplementerID = requestModel.CurrentUserID,
                    ActualDate = amWorkDetail.AutonomousMaintenance.ActualDate,
                    ReasonID = requestModel.ReasonID.HasValue ? requestModel.ReasonID : null,
                    ReasonType = requestModel.ReasonType.HasValue ? requestModel.ReasonType : null,
                    Content = !string.IsNullOrWhiteSpace(requestModel.Content) ? requestModel.Content : null,
                    Remark = !string.IsNullOrWhiteSpace(requestModel.Description) ? requestModel.Description : null,
                    SyncDataKey = null,
                    AppID = null,
                    SyncedDate = DateTime.Now,
                    IsDeleted = false,
                    CreatedUserID = requestModel.CurrentUserID,
                    InsertedAt = DateTime.Now,
                    LastUpdatedUserID = requestModel.CurrentUserID,
                    UpdatedAt = DateTime.Now,
                };
                _unitOfWork.ElogbookRepository<AmWorkingProgress>().Add(amWorkingProgress);

                _unitOfWork.Complete();
            }
        }

        private void AddHistoryData(int deviceId, int amId, int statusId, DateTime timeStart, DateTime timeEnd)
        {
            var device = _unitOfWork.ElogbookRepository<AmDevice>().GetById(deviceId);

            var asset = new PMIS_ASSET_MAINTENANCE()
            {
                ASSETID = device.AssetCode,
                TASKDESC = amId.ToString(),
                Status = statusId,
                WTYPEID = CommonConstant.TYPE_BAO_TRI_TU_QUAN,
                InsertedAt = timeStart,
                UpdatedAt = timeEnd
            };

            _unitOfWork.ElogbookRepository<PMIS_ASSET_MAINTENANCE>().Add(asset);
            _unitOfWork.Complete();
        }

        private string GetStickTime(DateTime dateBef, DateTime dateAft, string stickTime)
        {
            int minutesOld = 0;
            int secondsOld = 0;

            if (!string.IsNullOrWhiteSpace(stickTime))
            {
                string[] stickTimeOlds = stickTime.Split(':');
                int.TryParse(stickTimeOlds[0], out minutesOld);
                int.TryParse(stickTimeOlds[1], out secondsOld);
            }

            var diffOfDates = dateAft.Subtract(dateBef);

            int minutesTemp = diffOfDates.Minutes + minutesOld;
            int secondsTemp = diffOfDates.Seconds + secondsOld;

            if (secondsTemp > 59)
            {
                secondsTemp -= 59;
                minutesTemp++;
            }

            string minutesNew = minutesTemp.ToString().Length < 2 ? "0" + minutesTemp.ToString() : minutesTemp.ToString();
            string secondsNew = secondsTemp.ToString().Length < 2 ? "0" + secondsTemp.ToString() : secondsTemp.ToString();

            return minutesNew + ":" + secondsNew;
        }

        private void SendEmail(EmailTransmissionModel item, List<User> users)
        {
            var dataEmailSetting = GetEmailConfiguration();

            var smtpSend = new SmtpClient
            {
                Host = dataEmailSetting.EmailServer,
                Port = dataEmailSetting.Port,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(dataEmailSetting.UserName, dataEmailSetting.PassWord),
                EnableSsl = true,
                Timeout = 20000
            };
            var system = _systemConfigurationService.GetAppVersion();
            //replace subject
            item.Subject = item.Subject.Replace("[He thong Elogbook]", system?.AppTitle);
            var bodyRoot = item.MessageBody;

            if (system != null)
            {
                bodyRoot = bodyRoot.Replace("[He thong Elogbook]", system.AppTitle);
            }

            foreach (var us in users)
            {
                if (!string.IsNullOrWhiteSpace(us.Email))
                {
                    var body = bodyRoot.Replace("[Account User]", us.FullName);

                    // Send mail
                    using (var mailMessage = new MailMessage())
                    {
                        try
                        {
                            mailMessage.Sender = new MailAddress(dataEmailSetting.EmailAddress);
                            mailMessage.From = new MailAddress(dataEmailSetting.EmailAddress);

                            //Subject & Body
                            mailMessage.Subject = item.Subject;
                            mailMessage.Body = body;
                            mailMessage.BodyEncoding = Encoding.UTF8;
                            mailMessage.IsBodyHtml = true;
                            mailMessage.To.Add(us.Email);
                            smtpSend.Send(mailMessage);
                        }
                        catch (Exception ex)
                        {
                            LogCustom.LogMessage(TrackingLogLevel.INFO, "Autonomous Maintenance: SendEmailNotificationAm", "Eror send mail notification: "
                                + Environment.NewLine + ex.Message);
                        }
                    }
                }
            }

            if(!string.IsNullOrWhiteSpace(item.FatoryEmails))
            {
                string[] listEmail = item.FatoryEmails.Split(';');
                for (int i = CommonConstant.ZERO; i < listEmail.Length; i++)
                {
                    string tempEmail = listEmail[i].Trim();
                    if (!string.IsNullOrWhiteSpace(tempEmail))
                    {
                        var body = bodyRoot.Replace("[Account User]", item.FatoryName);

                        using (var mailMessage = new MailMessage())
                        {
                            try
                            {
                                mailMessage.Sender = new MailAddress(dataEmailSetting.EmailAddress);
                                mailMessage.From = new MailAddress(dataEmailSetting.EmailAddress);

                                //Subject & Body
                                mailMessage.Subject = item.Subject;
                                mailMessage.Body = body;
                                mailMessage.BodyEncoding = Encoding.UTF8;
                                mailMessage.IsBodyHtml = true;
                                mailMessage.To.Add(tempEmail);
                                smtpSend.Send(mailMessage);
                            }
                            catch (Exception ex)
                            {
                                LogCustom.LogMessage(TrackingLogLevel.INFO, "Autonomous Maintenance: SendEmailNotificationAm", "Eror send mail notification: "
                                    + Environment.NewLine + ex.Message);
                            }
                        }
                    }
                }
            }
        }

        private string UpdateContentEmail(string emailContent, AmMaintenanceSchedule amSchedule, OperationShift opShift, AutonomousMaintenance am)
        {
            string strCalendarName = amSchedule.MaintenanceScheduleName;
            string strActureDate = am.ActualDate.Date.ToString("dd/MM/yyyy");
            string strLocation = am.AmLocation.SiteDesc;
            string strDevice = am.AmDevice.AssetDesc;
            string strOpPosition = am.OperationPosition.OperationPositionName;
            string strOpShift = opShift.OperationShiftName;

            StringBuilder sbWork = new StringBuilder();
            var rowIndex = 0;
            foreach (var item in am.AmWorkDetails)
            {
                rowIndex++;
                if (rowIndex > 1)
                {
                    sbWork.Append("  + ");
                }
                else
                {
                    sbWork.Append(" + ");
                }

                sbWork.Append(item.AmWorkType.TypeDesc);
                sbWork.Append(" - ");
                sbWork.Append(item.AmWork.WorkName);
                sbWork.Append(",<br />");
            }
            string strWork = sbWork.ToString();

            emailContent = emailContent.Replace(CommonConstant.MAIL_MERGE_TEN_LICH_BAO_TRI, strCalendarName);
            emailContent = emailContent.Replace("[Ngay cua hoat dong AM]", strActureDate);
            emailContent = emailContent.Replace(CommonConstant.MAIL_MERGE_KHU_VUC, strLocation);
            emailContent = emailContent.Replace("[Thiet bi]", strDevice);
            emailContent = emailContent.Replace(CommonConstant.MAIL_MERGE_CUONG_VI, strOpPosition);
            emailContent = emailContent.Replace(CommonConstant.MAIL_MERGE_CA, strOpShift);
            emailContent = emailContent.Replace("+ [Loai cong viec AM] - [Cong viec AM],", strWork);
            emailContent = emailContent.Replace(CommonConstant.MAIL_MERGE_LINK_HE_THONG, AppSettings.DOMAIN);

            return emailContent;
        }

        private EmailConfigurationResponseModel GetEmailConfiguration()
        {
            var data = _unitOfWork.ElogbookRepository<EmailConfiguration>().GetQuery().Select(
                        e => new EmailConfigurationResponseModel()
                        {
                            ID = e.ID,
                            EmailAddress = e.EmailAddress,
                            EmailServer = e.EmailServer,
                            Port = e.Port,
                            EmailType = e.EmailType,
                            EmailTypeInfo = new ValueLabelModel { Value = e.EmailType, Label = "SMTP" },
                            UserName = e.UserName,
                            PassWord = e.PassWord,
                        }).FirstOrDefault();
            if (data != null)
            {
                data.PassWord = EncryptDecrypt.Decrypt(data.PassWord);
            }

            return data;
        }

        private void SyncDataToOperationDiary(AutonomousMaintenance entity, int? userID)
        {
            try
            {
                if (!entity.IsSyncToOperationDiary)
                {
                    // Lấy thông tin NKVH để đồng bộ dữ liệu từ AM
                    var operationDiary = _unitOfWork.ElogbookRepository<OperationDiary>().GetQuery(x => x.OperationShiftID == entity.OperationShiftID
                                                                                            && x.FactoryID == entity.FactoryID
                                                                                            && x.OperationPositionID == entity.OperationPositionID
                                                                                            && DbFunctions.TruncateTime(x.CreatedDate) == entity.ActualDate.Date
                                                                                            && !x.IsDeleted)
                        .Select(
                        e => new OperationDiaryModel()
                        {
                            ID = e.ID,
                            CreateDate = e.CreatedDate,
                            IsReceived = e.IsReceived,
                            IsDelivered = e.IsDelivered,
                            SyncDataKey = e.SyncDataKey,
                            OperationDiaryPositionShifts = e.OperationDiaryPositionInShift.Where(x => !x.IsDeleted).Select(
                                                t => new OperationDiaryPositionShiftModel()
                                                {
                                                    PositionInShiftID = t.PositionInShiftID ?? 0,
                                                    AssignedUser = new ValueLabelModel { Value = t.AssignedUser.ID, Label = t.AssignedUser.FullName },
                                                    Position = t.Position,
                                                }).OrderBy(z => z.Position).ToList(),
                        }).FirstOrDefault();

                    // Nếu NKVH chưa thực hiện giao/nhận ca thì mới đồng bộ dữ liệu
                    if (operationDiary != null && !operationDiary.IsDelivered && !operationDiary.IsReceived)
                    {
                        // Đồng bộ thông tin Tình hình vận hành
                        if (entity.AmStatusID == (int)AmEnumStatus.Completed)
                        {
                            var amWorkDetail = _unitOfWork.ElogbookRepository<AmWorkDetail>()
                                .GetQuery(x => x.AutonomousMaintenanceID == entity.ID && !x.IsDeleted && x.ImplementerID != null)
                                .OrderBy(x => x.Order)
                                .FirstOrDefault();

                            var userInfo = GetLeaderShiftInfo(operationDiary);
                            string description = $"Đã thực hiện hoạt động AM trên thiết bị \"{entity.AmDevice.AssetDesc}\" hoàn thành vào lúc: ";

                            if (entity.CompleteDate.HasValue)
                            {
                                description += entity.CompleteDate.GetValueOrDefault().ToString("dd/MM/yyyy HH:mm:ss");
                            }

                            OperationSituation operationSituation = new OperationSituation
                            {
                                AppID = string.Empty,
                                OperationDiaryID = operationDiary.ID,
                                AssignedFromID = userInfo?.Value,
                                AssignedFromName = userInfo != null ? userInfo.Label : string.Empty,
                                Description = description,
                                SyncDataKey = operationDiary.SyncDataKey,
                                IsDeleted = false,
                                CreatedUserID = userID,
                                InsertedAt = DateTime.Now,
                                LastUpdatedUserID = userID,
                                UpdatedAt = DateTime.Now,
                                SyncedDate = DateTime.Now,
                                FactoryID = entity.FactoryID,
                                Attachments = string.Empty,
                            };

                            if (amWorkDetail != null)
                            {
                                var amWorkingProgress = _unitOfWork.ElogbookRepository<AmWorkingProgress>()
                                .GetQuery(x => x.AutonomousMaintenanceID == entity.ID && !x.IsDeleted && x.AmWorkDetailID == amWorkDetail.ID)
                                .OrderBy(x => x.InsertedAt)
                                .FirstOrDefault();

                                if (amWorkingProgress != null)
                                {
                                    double.TryParse($"{amWorkingProgress.InsertedAt.Hour:00}.{amWorkingProgress.InsertedAt.Minute:00}", out double hour);
                                    operationSituation.Hour = hour;
                                }

                                if (amWorkDetail.Implementer != null)
                                {
                                    operationSituation.AssignedToID = amWorkDetail.Implementer.ID;
                                    operationSituation.AssignedToName = amWorkDetail.Implementer.FullName;
                                }
                            }
                            _unitOfWork.ElogbookRepository<OperationSituation>().Add(operationSituation);

                            if (entity.IsAbnormal.GetValueOrDefault())
                            {
                                SyncAbnormalIncident(entity, operationDiary, userID);
                            }
                        }
                        // Đồng bộ thông tin Bất thường sự cố
                        else if (entity.AmStatusID == (int)AmEnumStatus.DoNotWork && entity.IsAbnormal.GetValueOrDefault())
                        {
                            SyncAbnormalIncident(entity, operationDiary, userID);
                        }
                        else
                        {
                            // Nếu không thuộc 2 trường hợp trên thì không đồng bộ dữ liệu vào NKVH
                            return;
                        }

                        if (_unitOfWork.Complete() > 0)
                        {
                            var autonomousMaintenance = _unitOfWork.ElogbookRepository<AutonomousMaintenance>().GetById(entity.ID);
                            if (autonomousMaintenance != null)
                            {
                                autonomousMaintenance.IsSyncToOperationDiary = true;
                                autonomousMaintenance.SyncedDate = DateTime.Now;
                                autonomousMaintenance.UpdatedAt = DateTime.Now;
                                _unitOfWork.ElogbookRepository<AutonomousMaintenance>().Update(autonomousMaintenance);
                                _unitOfWork.Complete();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "SyncDataToOperationDiary",
                    Environment.NewLine + $"AutonomousMaintenanceID: {entity.ID}" + Environment.NewLine + ex.ToString());
            }
        }

        private ValueLabelModel GetLeaderShiftInfo(OperationDiaryModel operationDiary)
        {
            var shifts = _unitOfWork.ElogbookRepository<PositionInShift>().GetQuery().ToList();
            var position = _unitOfWork.ElogbookRepository<OperationPosition>().GetQuery(x => x.JobTitle == (int)JobTitle.ShiftLeader).ToList();

            // Danh sách id của những cương vị trong ca được khai báo trong ca ghi NKVH
            List<int> lstPositionInShiftID = operationDiary.OperationDiaryPositionShifts.Select(e => e.PositionInShiftID).ToList();
            // Danh sách cương vị trong ca
            var positionInShifts = shifts.Where(x => lstPositionInShiftID.Contains(x.ID)).ToList();
            // Id cương vị trong ca
            var positionInShiftID = 0;
            // Xác định danh sách cương vị trong ca có tồn tại Trưởng ca
            foreach (var positionInShift in positionInShifts)
            {
                // Lấy cương vị Trưởng ca
                OperationPosition operationPosition = null;
                operationPosition = position.Find(x => x.ID == positionInShift.OperationPositionID);

                // Nếu có cương vị Trưởng ca
                if (operationPosition != null)
                {
                    positionInShiftID = positionInShift.ID;
                    break;
                }
            }

            // Thông tin Trưởng ca NKVH
            var userInfo = operationDiary.OperationDiaryPositionShifts
                .Where(x => x.PositionInShiftID == positionInShiftID)
                .Select(e => new ValueLabelModel { Value = e.AssignedUser.Value, Label = e.AssignedUser.Label }).FirstOrDefault();

            return userInfo;
        }

        private void SyncAbnormalIncident(AutonomousMaintenance entity, OperationDiaryModel operationDiary, int? userID)
        {
            var abnormalIncident = _unitOfWork.ElogbookRepository<AbnormalIncident>().GetQuery(x => x.OperationDiaryID == operationDiary.ID && !x.IsDeleted).FirstOrDefault();
            // Cập nhật bất thường sự cố
            if (abnormalIncident != null && abnormalIncident.ID > 0)
            {
                abnormalIncident.Description =
                    abnormalIncident.Description
                    + "\n"
                    + (!string.IsNullOrWhiteSpace(entity.DeviceAbnormalDescription) ? $"{entity.AmDevice.AssetDesc} - {entity.DeviceAbnormalDescription}" : entity.AmDevice.AssetDesc);
                abnormalIncident.LastUpdatedUserID = userID;
                abnormalIncident.SyncedDate = DateTime.Now;
                abnormalIncident.UpdatedAt = DateTime.Now;
                abnormalIncident.FactoryID = entity.FactoryID;
                _unitOfWork.ElogbookRepository<AbnormalIncident>().Update(abnormalIncident);
            }
            // Thêm mới bất thường sự cố
            else
            {
                var abnormalIncidentNew = new AbnormalIncident
                {
                    SyncDataKey = operationDiary.SyncDataKey,
                    OperationDiaryID = operationDiary.ID,
                    Description = !string.IsNullOrWhiteSpace(entity.DeviceAbnormalDescription) ? $"{entity.AmDevice.AssetDesc} - {entity.DeviceAbnormalDescription}" : entity.AmDevice.AssetDesc,
                    CreatedUserID = userID,
                    LastUpdatedUserID = userID,
                    Attachments = string.Empty,
                    SyncedDate = DateTime.Now,
                    InsertedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    FactoryID = entity.FactoryID,
                };
                _unitOfWork.ElogbookRepository<AbnormalIncident>().Add(abnormalIncidentNew);
            }
        }

        private void CreateMailDeviceAbnormal(int amId)
        {
            var am = _unitOfWork.ElogbookRepository<AutonomousMaintenance>().GetById(amId);

            if (am.IsAbnormal == true && am.AmDeviceAbnormalStatusID > 1)
            {
                //[Notification-16]
                //get system data
                string module = EnumExtensions.GetEnumDescription(EnumModuleNotification.AM);
                string notification = EnumExtensions.GetEnumDescription(EnumNotificationType.AmNotificationDevice);
                int? moduleId = _unitOfWork.ElogbookRepository<SystemData>().GetQuery(x => x.Description == module && x.DataType == 2).Select(x => x.ID).FirstOrDefault();
                int? notificationId = _unitOfWork.ElogbookRepository<SystemData>().GetQuery(x => x.Description == notification && x.DataType == 1).Select(x => x.ID).FirstOrDefault();

                // get mail template
                if (moduleId > 0 && notificationId > 0)
                {
                    var mailTemplate = _emailTemplateService.GetTemplateByModuleAndNotification(moduleId.Value, notificationId.Value);
                    if (mailTemplate != null)
                    {
                        var mailConfig = _emailConfigurationService.GetEmailConfiguration();

                        //create Email Transmissions
                        AddEmailTransmissionsModel transmissionsModel = new AddEmailTransmissionsModel()
                        {
                            EmailReplyToAddr = mailConfig.EmailAddress,
                            Subject = mailTemplate.EmailSubject,
                            MessageBody = mailTemplate.EmailContent,
                            IsBodyHtml = true,
                            DetailFailure = "",
                        };

                        List<AddEmailTransmissionRecipientsModel> recipientsModels = new List<AddEmailTransmissionRecipientsModel>
                                            {
                                                new AddEmailTransmissionRecipientsModel
                                                {
                                                    RecipientEmailAddr = am.CompleterUser?.Email,
                                                    RecipientEmailName = am.CompleterUser?.FullName,
                                                    UserID = am.CompleterUser.ID > 0 ? am.CompleterUser.ID : 0,
                                                    RecipientType = 3
                                                }
                                            };

                        List<string> listPermission = new List<string> { "VIEW_QLBTTQ", "PROCESS_QLBTTQ", "NO_PROCESS_QLBTTQ", "VIEW_REPORT_TTAM" };
                        foreach (var permission in listPermission)
                        {
                            var users = GetRelatedUsers(permission);
                            if (users.Any())
                            {
                                foreach (var user in users)
                                {
                                    var isExistUser = recipientsModels.Find(x => x.UserID == user.ID);
                                    if (!string.IsNullOrWhiteSpace(user.Email) && isExistUser == null)
                                    {
                                        recipientsModels.Add(new AddEmailTransmissionRecipientsModel
                                        {
                                            RecipientEmailAddr = user.Email,
                                            RecipientEmailName = user.FullName,
                                            UserID = user.ID,
                                            RecipientType = 3
                                        });
                                    }
                                }
                            }
                        }
                        if (am.FactoryID > 0)
                        {
                            var factory = _unitOfWork.ElogbookRepository<Factory>().GetById(am.FactoryID);
                            if (factory != null && !string.IsNullOrWhiteSpace(factory.Email))
                            {
                                transmissionsModel.FatoryEmails = factory.Email;
                                transmissionsModel.FatoryName = factory.FactoryName;
                            }
                        }
                        _commonService.AddEmailTransmissions(transmissionsModel, recipientsModels, am.LastUpdatedUserID.Value, 0, 0, amId);
                    }
                }
            }
        }
        #endregion
    }
}
