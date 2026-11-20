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
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;

namespace Elogbook.Domain.Services
{
    public class AmRequestTicketService : IAmRequestTicketService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;
        private readonly ICommonService _commonService;
        private readonly IEmailService _emailService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="commonService"></param>
        /// <param name="emailService"></param>
        public AmRequestTicketService(ElogbookUnitOfWork unitOfWork,
            ICommonService commonService,
            IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _commonService = commonService;
            _emailService = emailService;
        }

        /// <summary>
        /// Kiểm tra trùng phiếu yêu cầu
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="isAddMode"></param>
        /// <returns></returns>
        public bool CheckDuplicateData(AmAddEditRequestTicketModel requestModel, bool isAddMode)
        {
            // Trường hợp thêm mới
            if (isAddMode)
            {
                return _unitOfWork.ElogbookRepository<AmRequestTicket>().GetQuery(x => x.FactoryID == requestModel.FactoryID
                                                                                    && x.RequestTicketNumber.Equals(requestModel.RequestTicketNumber, StringComparison.OrdinalIgnoreCase)
                                                                                    && !x.IsDeleted).Any();
            }
            // Trường hợp cập nhật
            else
            {
                return _unitOfWork.ElogbookRepository<AmRequestTicket>().GetQuery(x => x.FactoryID == requestModel.FactoryID
                                                                                    && x.ID != requestModel.ID
                                                                                    && x.RequestTicketNumber.Equals(requestModel.RequestTicketNumber, System.StringComparison.OrdinalIgnoreCase)
                                                                                    && !x.IsDeleted).Any();
            }
        }

        /// <summary>
        /// Thêm mới phiếu yêu cầu
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public AmRequestTicketModel Add(AmAddEditRequestTicketModel requestModel)
        {
            using (var transaction = _unitOfWork.DataContext.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    var entity = new AmRequestTicket
                    {
                        //ID: Mã phiếu yêu cầu tự động tăng
                        FactoryID = requestModel.FactoryID,
                        AmLocationId = requestModel.LocationId > CommonConstant.ZERO ? requestModel.LocationId : null,
                        AmDeviceId = requestModel.DeviceId > CommonConstant.ZERO ? requestModel.DeviceId : null,
                        AmEmergencyId = requestModel.EmergencyId,
                        OrganizationId = requestModel.OrganizationId > CommonConstant.ZERO ? requestModel.OrganizationId : null,
                        RequestTicketNumber = requestModel.RequestTicketNumber,
                        RequestContent = requestModel.RequestContent,
                        Detail = requestModel.Detail,
                        Note = requestModel.Note,
                        AmStatusID = requestModel.IsSend ? (int)AmEnumRequestTicket.InProgress : (int)AmEnumRequestTicket.UnSend,
                        IsDeleted = false,
                        CreatedUserID = requestModel.CurrentUserID,
                        InsertedAt = DateTime.Now,
                        LastUpdatedUserID = requestModel.CurrentUserID,
                        UpdatedAt = DateTime.Now,
                        AutonomousMaintenanceID = requestModel.AutonomousMaintenanceID > CommonConstant.ZERO ? requestModel.AutonomousMaintenanceID : null,
                        Attachments = requestModel.Attachments,
                        RegistrationTicketID = null,
                        RegistrationTicketType = null,
                    };

                    if (requestModel.IsSend)
                    {
                        entity.InsertedAtBySend = entity.InsertedAt;
                    }

                    _unitOfWork.ElogbookRepository<AmRequestTicket>().Add(entity);

                    if (_unitOfWork.Complete() > CommonConstant.ZERO)
                    {
                        // Cập nhật trạng thái bất thường thiết bị cho Bảo trì tự quản(AutonomousMaintenance)
                        if (entity.AutonomousMaintenanceID.GetValueOrDefault() > CommonConstant.ZERO)
                        {
                            var amEntity = _unitOfWork.ElogbookRepository<AutonomousMaintenance>()
                            .GetQuery(x => x.ID == requestModel.AutonomousMaintenanceID && !x.IsDeleted).FirstOrDefault();

                            // Trường hợp có tồn tại bảo trì tự quản
                            if (amEntity != null)
                            {
                                amEntity.AmDeviceAbnormalStatusID = (int)AmEnumDeAbStatus.Yes_Created;
                                amEntity.UpdatedAt = DateTime.Now;
                                amEntity.AmRequestTicketID = entity.ID;
                                amEntity.LastUpdatedUserID = requestModel.CurrentUserID;
                                _unitOfWork.ElogbookRepository<AutonomousMaintenance>().Update(amEntity);
                                _unitOfWork.Complete();
                            }
                        }

                        AddTicketStatusHistory(entity.ID, requestModel.IsSend ? "Gửi yêu cầu" : "Lưu yêu cầu", requestModel.CurrentUserID, entity.Note);

                        transaction.Commit();

                        if (entity.AmStatusID == (int)AmEnumRequestTicket.InProgress)
                        {
                            //[Notification-18]
                            var processEmailRequestModel = new ProcessEmailRequestModel
                            {
                                ModuleNotification = EnumModuleNotification.PYC,
                                FactoryID = requestModel.FactoryID,
                                NotificationTypeValue = (int)NotificationEvent.PYC_APPROVED,
                                CurrentUserID = requestModel.CurrentUserID,
                                AmRequestTicketID = entity.ID
                            };
                            _emailService.ProcessEmail(processEmailRequestModel);
                        }
                        return GetById(entity.ID);
                    }
                    transaction.Rollback();
                }
                catch (Exception ex)
                {
                    LogCustom.LogMessage(TrackingLogLevel.ERROR, CommonConstant.ADD_TICKET, Environment.NewLine + ex.ToString());
                    transaction.Rollback();
                }
                return new AmRequestTicketModel();
            }
        }

        /// <summary>
        /// Trả về phiếu yêu cầu theo mã số
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AmRequestTicketModel GetById(int id)
        {
            var entity = _unitOfWork.ElogbookRepository<AmRequestTicket>().GetQuery(x => x.ID == id && !x.IsDeleted).Select(
                    e => new AmRequestTicketModel()
                    {
                        ID = e.ID,
                        Factory = new ComboboxDataModel
                        {
                            Value = e.Factory != null ? e.Factory.ID : CommonConstant.ZERO,
                            Label = e.Factory != null ? e.Factory.FactoryName : string.Empty,
                        },
                        AmLocation = new ComboboxDataModel
                        {
                            Value = e.AmLocation != null ? e.AmLocation.ID : CommonConstant.ZERO,
                            Label = e.AmLocation != null ? e.AmLocation.SiteDesc : string.Empty,
                        },
                        AmEmergency = new ComboboxDataModel
                        {
                            Value = e.AmEmergency != null ? e.AmEmergency.ID : CommonConstant.ZERO,
                            Label = e.AmEmergency != null ? e.AmEmergency.Name : string.Empty,
                        },
                        AmDevice = new ComboboxDataModel
                        {
                            Value = e.AmDevice != null ? e.AmDevice.ID : CommonConstant.ZERO,
                            Label = e.AmDevice != null ? e.AmDevice.AssetDesc : string.Empty,
                        },
                        Organization = new ComboboxDataModel
                        {
                            Value = e.Organization != null ? e.Organization.ID : CommonConstant.ZERO,
                            Label = e.Organization != null ? e.Organization.Name : string.Empty,
                        },
                        RequestTicketNumber = e.RequestTicketNumber,
                        RegistrationTicketType = e.RegistrationTicketType,
                        RequestContent = e.RequestContent,
                        Detail = e.Detail,
                        Note = e.Note,
                        Status = new AmStatusInfoModel { ID = e.RequestTicketStatus.ID, Status = e.RequestTicketStatus.StatusName },
                        PositionStatus = e.RequestTicketStatus.Position,
                        AttachmentIds = e.Attachments,
                        InsertedAtBySend = e.InsertedAtBySend,
                        AutonomousMaintenanceID = e.AutonomousMaintenanceID
                    }).FirstOrDefault();

            if (entity != null)
            {
                if (!string.IsNullOrEmpty(entity.AttachmentIds))
                {
                    entity.Attachments = GetAttachedFile(entity.AttachmentIds);
                }

                if (entity.RegistrationTicketID.HasValue)
                {
                    var registration = _unitOfWork.ElogbookRepository<RegistrationTicket>().GetById(entity.RegistrationTicketID.Value);
                    entity.RegistrationName = registration?.TicketNumber;
                }
            }

            return entity;
        }

        private List<AttachmentModel> GetAttachedFile(string attachments)
        {
            List<AttachmentModel> results = new List<AttachmentModel>();
            var attackments = attachments.Split(',');
            if (attackments.Any())
            {
                foreach (var attachment in attackments)
                {
                    if (int.TryParse(attachment, out int idItem))
                    {
                        var att = _unitOfWork.ElogbookRepository<Attachment>().GetQuery(x => !x.IsDeleted && x.ID == idItem).FirstOrDefault();
                        if (att != null)
                        {
                            results.Add(new AttachmentModel
                            {
                                ID = att.ID,
                                FileName = att.FileName,
                                FilePath = _commonService.GetPathFile(att.FilePath),
                                PathThumbnail = _commonService.GetPathFile(att.PathThumbnail),
                                ScreenName = att.ScreenName,
                                FileType = att.FileType,
                            });
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Trả về tất cả phiếu yêu cầu theo điều kiện tìm kiếm
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel Find(AmSearchRequestTicketModel requestModel)
        {
            var entities = _unitOfWork.ElogbookRepository<AmRequestTicket>().GetQuery(x => !x.IsDeleted);

            switch (requestModel.Type)
            {
                case 2: // trưởng ca
                    entities = entities.Where(x => x.AmStatusID != (int)AmEnumRequestTicket.UnSend);
                    break;
                case 3: // đơn vị sửa chữa
                    entities = entities.Where(x => x.AmStatusID == (int)AmEnumRequestTicket.Registered || x.AmStatusID == (int)AmEnumRequestTicket.Approved);
                    break;
                case 1: // vận hành viên
                default:
                    break;
            }

            //Nhà máy
            if (requestModel.FactoryID.GetValueOrDefault() > CommonConstant.ZERO)
            {
                entities = entities.Where(x => x.FactoryID == requestModel.FactoryID);
            }

            //Khu vực
            if (requestModel.LocationId.GetValueOrDefault() > CommonConstant.ZERO)
            {
                entities = entities.Where(x => x.AmLocationId == requestModel.LocationId);
            }

            //Thiết bị
            if (requestModel.DeviceId.GetValueOrDefault() > CommonConstant.ZERO)
            {
                entities = entities.Where(x => x.AmDeviceId == requestModel.DeviceId);
            }

            //Đơn vị
            if (requestModel.OrganizationId.GetValueOrDefault() > CommonConstant.ZERO)
            {
                entities = entities.Where(x => x.OrganizationId == requestModel.OrganizationId);
            }

            //Mức độ khẩn cấp
            if (requestModel.AmEmergencyId.GetValueOrDefault() > CommonConstant.ZERO)
            {
                entities = entities.Where(x => x.AmEmergencyId == requestModel.AmEmergencyId);
            }

            DateTime frDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.DateFrom, false);
            DateTime toDate = DateTimeConvert.ConvertDateTimeFromString(requestModel.DateTo, true);

            //Ngày tạo (từ)
            if (!string.IsNullOrWhiteSpace(requestModel.DateFrom))
            {
                entities = entities.Where(x => DbFunctions.TruncateTime(x.InsertedAt) >= frDate.Date);
            }

            //Ngày tạo (đến)
            if (!string.IsNullOrWhiteSpace(requestModel.DateTo))
            {
                entities = entities.Where(x => DbFunctions.TruncateTime(x.InsertedAt) <= toDate.Date);
            }

            //Trạng thái sử dụng
            if (requestModel.StatusID.GetValueOrDefault() > CommonConstant.ZERO)
            {
                entities = entities.Where(x => x.AmStatusID == requestModel.StatusID);
            }

            //Nội dung / số phiếu
            if (!string.IsNullOrWhiteSpace(requestModel.ContentTicketNumber))
            {
                entities = entities.Where(x => x.RequestContent.Contains(requestModel.ContentTicketNumber) || x.RequestTicketNumber.Contains(requestModel.ContentTicketNumber));
            }

            List<AmRequestTicketModel> rootData = null;

            if(requestModel.Type == 3)
            {
                // Return and paging data
                var page = entities.Select(
                    e => new AmRequestTicketModel()
                    {
                        ID = e.ID,
                        Factory = new ComboboxDataModel
                        {
                            Value = e.Factory != null ? e.Factory.ID : CommonConstant.ZERO,
                            Label = e.Factory != null ? e.Factory.FactoryName : string.Empty,
                        },
                        AmLocation = new ComboboxDataModel
                        {
                            Value = e.AmLocation != null ? e.AmLocation.ID : CommonConstant.ZERO,
                            Label = e.AmLocation != null ? e.AmLocation.SiteDesc : string.Empty,
                        },
                        AmEmergency = new ComboboxDataModel
                        {
                            Value = e.AmEmergency != null ? e.AmEmergency.ID : CommonConstant.ZERO,
                            Label = e.AmEmergency != null ? e.AmEmergency.Name : string.Empty,
                        },
                        AmDevice = new ComboboxDataModel
                        {
                            Value = e.AmDevice != null ? e.AmDevice.ID : CommonConstant.ZERO,
                            Label = e.AmDevice != null ? e.AmDevice.AssetDesc : string.Empty,
                        },
                        Organization = new ComboboxDataModel
                        {
                            Value = e.Organization != null ? e.Organization.ID : CommonConstant.ZERO,
                            Label = e.Organization != null ? e.Organization.Name : string.Empty,
                        },
                        RequestTicketNumber = e.RequestTicketNumber,
                        RequestContent = e.RequestContent,
                        Detail = e.Detail,
                        Note = e.Note,
                        Status = new AmStatusInfoModel
                        {
                            ID = e.RequestTicketStatus.ID,
                            Status = ((requestModel.Type == 3 && e.RequestTicketStatus.ID == (int)AmEnumRequestTicket.Approved) ? CommonConstant.PYC_WAITING_STATUS : e.RequestTicketStatus.StatusName)
                        },
                        PositionStatus = e.RequestTicketStatus.Position,
                        InsertedAt = e.InsertedAt,
                        CreatedUser = new ValueLabelModel { Value = e.CreatedUser.ID, Label = e.CreatedUser.FullName },
                        UpdatedAt = e.UpdatedAt,
                        LastUpdatedUser = new ValueLabelModel { Value = e.LastUpdatedUser.ID, Label = e.LastUpdatedUser.FullName },
                        RegistrationTicketID = e.RegistrationTicketID,
                        RegistrationTicketType = e.RegistrationTicketType,
                    })
                    .OrderBy(x => x.PositionStatus)
                    .ThenByDescending(x=>x.InsertedAt)
                    .ThenByDescending(x=>x.RequestTicketNumber)
                    .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

                rootData = page.ToList();

                if (rootData.Count > 0)
                {
                    //Thông tin phiếu đăng ký công tác
                    foreach (var item in rootData)
                    {
                        if (item.RegistrationTicketID.HasValue)
                        {
                            var registration = _unitOfWork.ElogbookRepository<RegistrationTicket>().GetById(item.RegistrationTicketID.Value);
                            item.RegistrationName = registration?.TicketNumber;
                        }
                    }
                }
                return HttpResponse.OkList(rootData, requestModel.PageNumber, requestModel.PageSize, page.TotalRecords, page.TotalPages);
            }
            else
            {
                // Return and paging data
                var page = entities.Select(
                    e => new AmRequestTicketModel()
                    {
                        ID = e.ID,
                        Factory = new ComboboxDataModel
                        {
                            Value = e.Factory != null ? e.Factory.ID : CommonConstant.ZERO,
                            Label = e.Factory != null ? e.Factory.FactoryName : string.Empty,
                        },
                        AmLocation = new ComboboxDataModel
                        {
                            Value = e.AmLocation != null ? e.AmLocation.ID : CommonConstant.ZERO,
                            Label = e.AmLocation != null ? e.AmLocation.SiteDesc : string.Empty,
                        },
                        AmEmergency = new ComboboxDataModel
                        {
                            Value = e.AmEmergency != null ? e.AmEmergency.ID : CommonConstant.ZERO,
                            Label = e.AmEmergency != null ? e.AmEmergency.Name : string.Empty,
                        },
                        AmDevice = new ComboboxDataModel
                        {
                            Value = e.AmDevice != null ? e.AmDevice.ID : CommonConstant.ZERO,
                            Label = e.AmDevice != null ? e.AmDevice.AssetDesc : string.Empty,
                        },
                        Organization = new ComboboxDataModel
                        {
                            Value = e.Organization != null ? e.Organization.ID : CommonConstant.ZERO,
                            Label = e.Organization != null ? e.Organization.Name : string.Empty,
                        },
                        RequestTicketNumber = e.RequestTicketNumber,
                        RequestContent = e.RequestContent,
                        Detail = e.Detail,
                        Note = e.Note,
                        Status = new AmStatusInfoModel
                        {
                            ID = e.RequestTicketStatus.ID,
                            Status = ((requestModel.Type == 3 && e.RequestTicketStatus.ID == (int)AmEnumRequestTicket.Approved) ? CommonConstant.PYC_WAITING_STATUS : e.RequestTicketStatus.StatusName)
                        },
                        PositionStatus = e.RequestTicketStatus.Position,
                        InsertedAt = e.InsertedAt,
                        CreatedUser = new ValueLabelModel { Value = e.CreatedUser.ID, Label = e.CreatedUser.FullName },
                        UpdatedAt = e.UpdatedAt,
                        LastUpdatedUser = new ValueLabelModel { Value = e.LastUpdatedUser.ID, Label = e.LastUpdatedUser.FullName },
                        RegistrationTicketID = e.RegistrationTicketID,
                        RegistrationTicketType = e.RegistrationTicketType,
                    }).OrderByDescending(x => x.InsertedAt)
                    .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

                rootData = page.ToList();

                if (rootData.Count > 0)
                {
                    //Thông tin phiếu đăng ký công tác
                    foreach (var item in rootData)
                    {
                        if (item.RegistrationTicketID.HasValue)
                        {
                            var registration = _unitOfWork.ElogbookRepository<RegistrationTicket>().GetById(item.RegistrationTicketID.Value);
                            item.RegistrationName = registration?.TicketNumber;
                        }
                    }
                }
                return HttpResponse.OkList(rootData, requestModel.PageNumber, requestModel.PageSize, page.TotalRecords, page.TotalPages);
            }
        }

        /// <summary>
        /// Trả về tất cả phiếu yêu cầu theo điều kiện tìm kiếm (mobile)
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ResponseBaseModel FindMobile(AmSearchRequestTicketMobileModel requestModel)
        {
            var entities = _unitOfWork.ElogbookRepository<AmRequestTicket>().GetQuery(x => !x.IsDeleted);

            //Nhà máy
            if (requestModel.FactoryID > CommonConstant.ZERO)
            {
                entities = entities.Where(x => x.FactoryID == requestModel.FactoryID);
            }

            //Khu vực
            if (requestModel.LocationId.GetValueOrDefault() > CommonConstant.ZERO)
            {
                entities = entities.Where(x => x.AmLocationId == requestModel.LocationId);
            }

            //Thiết bị
            if (requestModel.DeviceId.GetValueOrDefault() > CommonConstant.ZERO)
            {
                entities = entities.Where(x => x.AmDeviceId == requestModel.DeviceId);
            }

            //Ngày tạo (từ)
            if (requestModel.DateFrom.HasValue)
            {
                entities = entities.Where(x => x.InsertedAt >= requestModel.DateFrom);
            }

            //Ngày tạo (đến)
            if (requestModel.DateTo.HasValue)
            {
                entities = entities.Where(x => x.InsertedAt <= requestModel.DateTo);
            }

            //Mức độ khẩn cấp
            if (requestModel.AmEmergencyId.GetValueOrDefault() > CommonConstant.ZERO)
            {
                entities = entities.Where(x => x.AmEmergencyId == requestModel.AmEmergencyId);
            }

            //Từ khóa
            if (!string.IsNullOrWhiteSpace(requestModel.SearchKey))
            {
                entities = entities.Where(x => x.RequestTicketNumber.Contains(requestModel.SearchKey)
                || x.CreatedUser.FullName.Contains(requestModel.SearchKey)
                || (x.AmStatusID == (int)AmEnumRequestTicket.Approved && x.LastUpdatedUser.FullName.Contains(requestModel.SearchKey))
                || (x.OrganizationId.HasValue && x.Organization.Name.Contains(requestModel.SearchKey)));
            }

            switch (requestModel.Tab)
            {
                case 1:
                    entities = entities.Where(x => x.AmStatusID == (int)AmEnumRequestTicket.InProgress);
                    break;
                case 2:
                    entities = entities.Where(x => x.AmStatusID == (int)AmEnumRequestTicket.Reject);
                    break;
                case 3:
                    entities = entities.Where(x => x.AmStatusID == (int)AmEnumRequestTicket.Approved);
                    break;
                case 4:
                    entities = entities.Where(x => x.AmStatusID == (int)AmEnumRequestTicket.Registered);
                    break;
                case 5:
                    entities = entities.Where(x => x.AmStatusID == (int)AmEnumRequestTicket.UnSend);
                    break;
                case 6:
                    entities = entities.Where(x => x.AmStatusID == (int)AmEnumRequestTicket.Stop);
                    break;
            }

            // Return and paging data
            var page = entities.Select(
                e => new AmRequestTicketModel()
                {
                    ID = e.ID,
                    Factory = new ComboboxDataModel
                    {
                        Value = e.Factory != null ? e.Factory.ID : CommonConstant.ZERO,
                        Label = e.Factory != null ? e.Factory.FactoryName : string.Empty,
                    },
                    AmLocation = new ComboboxDataModel
                    {
                        Value = e.AmLocation != null ? e.AmLocation.ID : CommonConstant.ZERO,
                        Label = e.AmLocation != null ? e.AmLocation.SiteDesc : string.Empty,
                    },
                    AmEmergency = new ComboboxDataModel
                    {
                        Value = e.AmEmergency != null ? e.AmEmergency.ID : CommonConstant.ZERO,
                        Label = e.AmEmergency != null ? e.AmEmergency.Name : string.Empty,
                    },
                    AmDevice = new ComboboxDataModel
                    {
                        Value = e.AmDevice != null ? e.AmDevice.ID : CommonConstant.ZERO,
                        Label = e.AmDevice != null ? e.AmDevice.AssetDesc : string.Empty,
                    },
                    Organization = new ComboboxDataModel
                    {
                        Value = e.Organization != null ? e.Organization.ID : CommonConstant.ZERO,
                        Label = e.Organization != null ? e.Organization.Name : string.Empty,
                    },
                    RequestTicketNumber = e.RequestTicketNumber,
                    RequestContent = e.RequestContent,
                    Detail = e.Detail,
                    Note = e.Note,
                    InsertedAt = e.InsertedAt,
                    CreatedUser = new ValueLabelModel() { Value = e.CreatedUserID, Label = e.CreatedUser.FullName },
                    UpdatedAt = e.UpdatedAt,
                    LastUpdatedUser = new ValueLabelModel() { Value = e.LastUpdatedUserID, Label = e.LastUpdatedUser.FullName },
                    InsertedAtBySend = e.InsertedAtBySend,
                    RegistrationTicketID = e.RegistrationTicketID,
                    RegistrationTicketType = e.RegistrationTicketType,
                    AutonomousMaintenanceID = e.AutonomousMaintenanceID
                }).OrderByDescending(x => x.InsertedAt)
                .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            var rootData = page.ToList();
            if (rootData.Count > 0)
            {
                //Thông tin phiếu đăng ký công tác
                foreach (var item in rootData)
                {
                    if (item.RegistrationTicketID.HasValue)
                    {
                        var registration = _unitOfWork.ElogbookRepository<RegistrationTicket>().GetById(item.RegistrationTicketID.Value);
                        item.RegistrationName = registration?.TicketNumber;
                    }
                }
            }

            return HttpResponse.OkList(rootData, requestModel.PageNumber, requestModel.PageSize, page.TotalRecords, page.TotalPages);
        }

        /// <summary>
        /// Trả về tất cả số lượng phiếu yêu cầu theo điều kiện của mỗi tab (mobile)
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public CountStatusRequestTicketMobileModel FindMobileCount(AmSearchRequestTicketMobileModel requestModel)
        {
            var entities = _unitOfWork.ElogbookRepository<AmRequestTicket>().GetQuery(x => !x.IsDeleted);

            //Nhà máy
            if (requestModel.FactoryID > CommonConstant.ZERO)
            {
                entities = entities.Where(x => x.FactoryID == requestModel.FactoryID);
            }

            //Khu vực
            if (requestModel.LocationId.GetValueOrDefault() > CommonConstant.ZERO)
            {
                entities = entities.Where(x => x.AmLocationId == requestModel.LocationId);
            }

            //Thiết bị
            if (requestModel.DeviceId.GetValueOrDefault() > CommonConstant.ZERO)
            {
                entities = entities.Where(x => x.AmDeviceId == requestModel.DeviceId);
            }

            //Ngày tạo (từ)
            if (requestModel.DateFrom.HasValue)
            {
                entities = entities.Where(x => x.InsertedAt >= requestModel.DateFrom);
            }

            //Ngày tạo (đến)
            if (requestModel.DateTo.HasValue)
            {
                entities = entities.Where(x => x.InsertedAt <= requestModel.DateTo);
            }

            //Mức độ khẩn cấp
            if (requestModel.AmEmergencyId.GetValueOrDefault() > CommonConstant.ZERO)
            {
                entities = entities.Where(x => x.AmEmergencyId == requestModel.AmEmergencyId);
            }

            //Từ khóa
            if (!string.IsNullOrWhiteSpace(requestModel.SearchKey))
            {
                entities = entities.Where(x => x.RequestTicketNumber.Contains(requestModel.SearchKey)
                || x.CreatedUser.FullName.Contains(requestModel.SearchKey)
                || (x.AmStatusID == (int)AmEnumRequestTicket.Approved && x.LastUpdatedUser.FullName.Contains(requestModel.SearchKey))
                || (x.OrganizationId.HasValue && x.Organization.Name.Contains(requestModel.SearchKey)));
            }

            CountStatusRequestTicketMobileModel countData = new CountStatusRequestTicketMobileModel
            {
                InProgress = entities.Count(x => x.AmStatusID == (int)AmEnumRequestTicket.InProgress),
                Approved = entities.Count(x => x.AmStatusID == (int)AmEnumRequestTicket.Approved),
                Reject = entities.Count(x => x.AmStatusID == (int)AmEnumRequestTicket.Reject),
                Registered = entities.Count(x => x.AmStatusID == (int)AmEnumRequestTicket.Registered)
            };

            return countData;
        }

        /// <summary>
        /// Cập nhập phiếu yêu cầu
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public AmRequestTicketModel Update(AmAddEditRequestTicketModel requestModel)
        {
            using (var transaction = _unitOfWork.DataContext.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    var entity = _unitOfWork.ElogbookRepository<AmRequestTicket>().GetById(requestModel.ID);
                    if (entity != null)
                    {
                        bool IsChangeStatus = entity.AmStatusID != requestModel.StatusID;
                        if (entity.AmStatusID == (int)AmEnumRequestTicket.UnSend && requestModel.IsSend)
                        {
                            entity.InsertedAtBySend = DateTime.Now;
                        }
                        entity.AmDeviceId = requestModel.DeviceId > CommonConstant.ZERO ? requestModel.DeviceId : null;
                        entity.FactoryID = requestModel.FactoryID;
                        entity.AmLocationId = requestModel.LocationId > CommonConstant.ZERO ? requestModel.LocationId : null;
                        entity.AmEmergencyId = requestModel.EmergencyId;
                        entity.OrganizationId = requestModel.OrganizationId > CommonConstant.ZERO ? requestModel.OrganizationId : null;
                        entity.RequestTicketNumber = requestModel.RequestTicketNumber;
                        entity.RequestContent = requestModel.RequestContent;
                        entity.Detail = requestModel.Detail;
                        entity.Note = requestModel.Note;
                        entity.AmStatusID = requestModel.IsSend ? (int)AmEnumRequestTicket.InProgress : entity.AmStatusID;
                        entity.CreatedUserID = requestModel.CurrentUserID;
                        entity.LastUpdatedUserID = requestModel.CurrentUserID;
                        entity.Attachments = requestModel.Attachments;

                        _unitOfWork.ElogbookRepository<AmRequestTicket>().Update(entity);
                        if (_unitOfWork.Complete() > CommonConstant.ZERO)
                        {
                            // Cập nhật trạng thái bất thường thiết bị cho Bảo trì tự quản(AutonomousMaintenance)
                            if (entity.AutonomousMaintenanceID.GetValueOrDefault() > CommonConstant.ZERO)
                            {
                                var amEntity = _unitOfWork.ElogbookRepository<AutonomousMaintenance>()
                                .GetQuery(x => x.ID == requestModel.AutonomousMaintenanceID && !x.IsDeleted).FirstOrDefault();

                                // Trường hợp có tồn tại bảo trì tự quản
                                if (amEntity != null)
                                {
                                    // Trường hợp bấm bấm nút [Gửi yêu cầu] thì đổi trạng thái sang [Có bất thường - Đang xử lý]
                                    amEntity.AmDeviceAbnormalStatusID = requestModel.IsSend ? (int)AmEnumDeAbStatus.Yes_Created : amEntity.AmDeviceAbnormalStatusID;
                                    amEntity.UpdatedAt = DateTime.Now;
                                    amEntity.LastUpdatedUserID = requestModel.CurrentUserID;
                                    _unitOfWork.ElogbookRepository<AutonomousMaintenance>().Update(amEntity);
                                    _unitOfWork.Complete();
                                }
                            }

                            AddTicketStatusHistory(requestModel.ID, requestModel.IsSend ? "Gửi yêu cầu" : "Cập nhật yêu cầu", requestModel.CurrentUserID, requestModel.Note);

                            // update chỉ có 2status là chưa gửi phiếu và đang xử lý
                            if (IsChangeStatus && entity.AmStatusID == (int)AmEnumRequestTicket.InProgress)
                            {
                                //[Notification-18]
                                var processEmailRequestModel = new ProcessEmailRequestModel
                                {
                                    ModuleNotification = EnumModuleNotification.PYC,
                                    FactoryID = requestModel.FactoryID,
                                    NotificationTypeValue = (int)NotificationEvent.PYC_APPROVED,
                                    CurrentUserID = requestModel.CurrentUserID,
                                    AmRequestTicketID = entity.ID
                                };
                                _emailService.ProcessEmail(processEmailRequestModel);
                            }

                            transaction.Commit();
                            return GetById(entity.ID);
                        }
                        transaction.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    LogCustom.LogMessage(TrackingLogLevel.ERROR, "AmRequestTicketService: Update AmEnumRequestTicket", Environment.NewLine + ex.ToString());
                    transaction.Rollback();
                }
                return new AmRequestTicketModel();
            }
        }

        /// <summary>
        /// Cập nhật trạng thái phiếu
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool UpdateStatus(ChangeStatusRequestModel request)
        {
            bool result = false;
            var entity = _unitOfWork.ElogbookRepository<AmRequestTicket>().GetById(request.ID);
            if (entity != null)
            {
                if (entity.AmStatusID == (int)AmEnumRequestTicket.UnSend && request.Status == (int)AmEnumRequestTicket.InProgress)
                {
                    entity.InsertedAtBySend = DateTime.Now;
                }

                entity.AmStatusID = request.Status <= CommonConstant.ZERO ? (int)AmEnumRequestTicket.InProgress : request.Status;
                entity.RegistrationTicketID = request.RegistrationTicketID;
                entity.RegistrationTicketType = request.RegistrationTicketType > 0 ? request.RegistrationTicketType: entity.RegistrationTicketType;
                entity.LastUpdatedUserID = request.CurrentUserID;

                _unitOfWork.ElogbookRepository<AmRequestTicket>().Update(entity);

                string actionContent = string.Empty;
                int? statusID = null;
                if (entity.AmStatusID == (int)AmEnumRequestTicket.Reject)
                {
                    statusID = (int)AmEnumDeAbStatus.Yes_Created;
                    actionContent = CommonConstant.MAIL_MERGE_REJECT_STATUS;
                }

                if (entity.AmStatusID == (int)AmEnumRequestTicket.Stop)
                {
                    statusID = (int)AmEnumDeAbStatus.Yes_Stop;
                    actionContent = CommonConstant.MAIL_MERGE_STOP_STATUS;
                }

                if (entity.AmStatusID == (int)AmEnumRequestTicket.Approved)
                {
                    statusID = (int)AmEnumDeAbStatus.Yes_Approved;
                    actionContent = "Duyệt yêu cầu";
                }

                // Cập nhật trạng thái bất thường thiết bị cho Bảo trì tự quản(AutonomousMaintenance)
                if (entity.AmStatusID == (int)AmEnumRequestTicket.Reject
                    || entity.AmStatusID == (int)AmEnumRequestTicket.Approved
                    || entity.AmStatusID == (int)AmEnumRequestTicket.Stop)
                {
                    var amEntity = _unitOfWork.ElogbookRepository<AutonomousMaintenance>().GetById(entity.AutonomousMaintenanceID.GetValueOrDefault());
                    if (amEntity != null)
                    {
                        amEntity.AmDeviceAbnormalStatusID = statusID > CommonConstant.ZERO ? statusID : amEntity.AmDeviceAbnormalStatusID;
                        amEntity.UpdatedAt = DateTime.Now;
                        amEntity.LastUpdatedUserID = request.CurrentUserID;
                        _unitOfWork.ElogbookRepository<AutonomousMaintenance>().Update(amEntity);
                    }
                }

                result = _unitOfWork.Complete() > CommonConstant.ZERO;
                if (result)
                {
                    if (!string.IsNullOrWhiteSpace(actionContent))
                    {
                        AddTicketStatusHistory(request.ID, actionContent, request.CurrentUserID, request.Note);
                    }

                    //Chỉ gửi mail khi bị Từ chối , Được duyệt, Dừng xử lý
                    if (entity.AmStatusID == (int)AmEnumRequestTicket.Reject
                        || entity.AmStatusID == (int)AmEnumRequestTicket.Approved
                        || entity.AmStatusID == (int)AmEnumRequestTicket.Stop)
                    {
                        //[Notification-19-20-21]
                        var processEmailRequestModel = new ProcessEmailRequestModel
                        {
                            ModuleNotification = EnumModuleNotification.PYC,
                            AmRequestTicketID = entity.ID,
                            TicketStatus = entity.AmStatusID,
                            CurrentUserID = request.CurrentUserID
                        };
                        _emailService.ProcessEmail(processEmailRequestModel);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Xóa phiếu yêu cầu
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        public bool Delete(int id, int currentUserId)
        {
            var entity = _unitOfWork.ElogbookRepository<AmRequestTicket>().GetById(id);
            if (entity != null)
            {
                _unitOfWork.ElogbookRepository<AmRequestTicket>().Delete(entity, currentUserId);

                var autonomousMaintenance = _unitOfWork.ElogbookRepository<AutonomousMaintenance>().GetQuery(x => x.AmRequestTicketID == id);
                foreach (var item in autonomousMaintenance)
                {
                    item.AmRequestTicketID = null;
                    _unitOfWork.ElogbookRepository<AutonomousMaintenance>().Update(item);
                }

                var registrationTicket = _unitOfWork.ElogbookRepository<RegistrationTicket>().GetQuery(x => x.RequestTicketID == id);
                foreach (var item in registrationTicket)
                {
                    item.RequestTicketID = null;
                    _unitOfWork.ElogbookRepository<RegistrationTicket>().Update(item);
                }

                var workTicket = _unitOfWork.ElogbookRepository<WorkTicket>().GetQuery(x => x.RequestTicketID == id);
                foreach (var item in workTicket)
                {
                    item.RequestTicketID = null;
                    _unitOfWork.ElogbookRepository<WorkTicket>().Update(item);
                }

                _unitOfWork.Complete();

                AddTicketStatusHistory(id, "Xóa yêu cầu", currentUserId, "Xóa phiếu");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Đếm số phiếu yêu cầu
        /// trạng thái đang xử lý với PYC trưởng ca
        /// trạng thái đã duyệt với PYC đơn vị sửa chữa
        /// </summary>
        /// <returns></returns>
        public CountNumberTicketStatusModel GetCountNumberTicket()
        {
            var entities = _unitOfWork.ElogbookRepository<AmRequestTicket>().GetQuery(x => !x.IsDeleted);
            return new CountNumberTicketStatusModel()
            {
                Total_Shift_Leader = entities.Count(x => x.AmStatusID == (int)AmEnumRequestTicket.InProgress),
                Total_DVSC = entities.Count(x => x.AmStatusID == (int)AmEnumRequestTicket.Approved)
            };
        }

        /// <summary>
        /// lấy data tạo phiếu đăng ký Phiếu Điện/TCNH
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="isCTD"></param>
        /// <returns></returns>
        public DataRegistrationTicketModel GetDataRegistrationTicket(int Id, bool isCTD)
        {
            var entity = _unitOfWork.ElogbookRepository<AmRequestTicket>().GetById(Id);
            if (entity != null)
            {
                var result = new DataRegistrationTicketModel
                {
                    RequestTicketID = Id,
                    FactoryID = entity.FactoryID,
                    ReceiveUnitID = entity.OrganizationId,
                    WorkPlacement = entity.AmLocation != null ? entity.AmLocation.SiteDesc + " - " + entity.AmDevice.AssetDesc : string.Empty,
                    WorkContent = entity.RequestContent + Environment.NewLine + entity.Detail
                };

                if (isCTD)
                {
                    result.RegulationNumber = entity.RequestTicketNumber;
                }

                return result;
            }

            return null;
        }

        /// <summary>
        /// thêm lịch sử thao tác của phiếu yêu cầu
        /// </summary>
        /// <param name="id"></param>
        /// <param name="statusId"></param>
        /// <param name="userID"></param>
        /// <param name="comment"></param>
        private void AddTicketStatusHistory(int id, string actionText, int userID, string comment)
        {
            var ticketStatusHistory = new AmRequestTicketHistory()
            {
                RequestTicketId = id,
                Comment = comment,
                CreatedUserID = userID,
                LastUpdatedUserID = userID,
                Action = actionText
            };

            _unitOfWork.ElogbookRepository<AmRequestTicketHistory>().Add(ticketStatusHistory);
            _unitOfWork.Complete();
        }

        /// <summary>
        /// lấy data lịch sử phiếu yêu cầu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseBaseModel GetHistory(int id)
        {
            var entities = _unitOfWork.ElogbookRepository<AmRequestTicketHistory>().GetQuery(x => x.RequestTicketId == id && !x.IsDeleted).ToList();
            List<RequestTicketHistoryModel> historys = new List<RequestTicketHistoryModel>();
            foreach (var item in entities)
            {
                historys.Add(new RequestTicketHistoryModel()
                {
                    Time = item.InsertedAt,
                    Author = item.CreatedUser.FullName,
                    Action = item.Action,
                    Comment = item.Comment
                });
            }

            // Return and paging data
            var page = historys.OrderBy(t => t.Time).ToList().ToPagedList();

            return HttpResponse.OkList(page);
        }

        /// <summary>
        /// Lấy data thông tin liên quan phiếu yêu cầu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseBaseModel GetRelateInfo(int id)
        {
            List<RelateInfoModel> relates = new List<RelateInfoModel>();

            /// Bảo trì tự quản
            var entity = _unitOfWork.ElogbookRepository<AmRequestTicket>().GetQuery(x => x.ID == id && x.AutonomousMaintenanceID.HasValue).FirstOrDefault();
            if (entity != null)
            {
                var am = _unitOfWork.ElogbookRepository<AutonomousMaintenance>().GetById(entity.AutonomousMaintenanceID.Value);
                if (am != null)
                {
                    relates.Add(new RelateInfoModel()
                    {
                        Type = CommonConstant.BAO_TRI_TU_QUAN,
                        Info = new ValueLabelModel() { Label = am.AmDevice.AssetDesc, Value = am.ID },
                        InsertedAt = am.ActualDate,
                        Status = am.AmStatus.StatusName,
                        DeviceInfo = new ValueLabelModel() { Label = am.AmDevice.AssetDesc, Value = am.ID },
                        MaintenanceId = am.ID,
                        RouterLink = $"{CommonConstant.AM_ROUTERLINK}{am.ID}",
                    });
                }
            }

            // phiếu đăng ký
            var registrationTickets = _unitOfWork.ElogbookRepository<RegistrationTicket>().GetQuery(x => x.RequestTicketID == id);
            foreach (var registration in registrationTickets)
            {
                relates.Add(new RelateInfoModel()
                {
                    Type = CommonConstant.PDK,
                    Info = new ValueLabelModel() { Label = registration.TicketNumber, Value = registration.ID },
                    InsertedAt = registration.InsertedAt,
                    Status = registration.TicketStatus.StatusName,
                    RegistrationId = registration.ID,
                    RouterLink = registration.RegistrationTicketType == CommonConstant.TYPE_PCTD ? $"{CommonConstant.PDKD_ROUTERLINK}{registration.ID}" : $"{CommonConstant.PDKTCNH_ROUTERLINK}{registration.ID}",
                });
            }

            var workTickets = _unitOfWork.ElogbookRepository<WorkTicket>().GetQuery(x => x.RequestTicketID == id);
            foreach (var item in workTickets)
            {
                relates.Add(new RelateInfoModel()
                {
                    Type = CommonConstant.PCT,
                    Info = new ValueLabelModel() { Label = item.WorkTicketNumber, Value = item.ID },
                    InsertedAt = item.InsertedAt,
                    Status = item.TicketStatus.StatusName,
                    WorkticketId = item.ID,
                    RouterLink = item.ScreenType == CommonConstant.TYPE_PCTD ? $"{CommonConstant.PCTD_ROUTERLINK}{item.ID}" : $"{CommonConstant.PCTTCNH_ROUTERLINK}{item.ID}",
                });
            }

            // Return and paging data
            var page = relates.OrderBy(t => t.Type).ToList().ToPagedList();

            return HttpResponse.OkList(page);
        }
    }
}
