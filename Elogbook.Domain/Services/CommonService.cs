using Core.System.Runtime.Managers;
using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Extensions;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.BaseCommon;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Elogbook.Core.Models.Response.MasterData.AMs;
using Elogbook.Core.Models.Response.OperationShift;
using Elogbook.Core.Models.Response.SystemConfiguration;
using Elogbook.Core.Models.Response.User;
using Elogbook.Data;
using EnumsNET;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Elogbook.Domain.Services
{
    public class CommonService : ICommonService
    {
        #region Properties field

        private readonly ElogbookUnitOfWork _unitOfWork;
        private static readonly object _lockRegisterTicket = new object();
        private static readonly object _lockApproveTicket = new object();
        private static readonly object _lockRequestTicket = new object();
        private static readonly object _lockRequestTicketFactory = new object();

        #endregion

        #region Contructor

        public CommonService(ElogbookUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Thêm danh sách mail chờ xử lý
        /// </summary>
        /// <param name="transmissionsModel"></param>
        /// <param name="recipientsModels"></param>
        /// <param name="currentUserID"></param>
        /// <param name="registrationId"></param>
        /// <param name="workticketId"></param>
        /// <param name="amId"></param>
        /// <param name="requestTicketId"></param>
        public void AddEmailTransmissions(AddEmailTransmissionsModel transmissionsModel,
            List<AddEmailTransmissionRecipientsModel> recipientsModels,
            int currentUserID,
            int registrationId = 0,
            int workticketId = 0,
            int amId = 0, int
            requestTicketId = 0)
        {
            if (recipientsModels != null && recipientsModels.Count > 0 && transmissionsModel != null)
            {
                var timeUpdate = DateTime.Now;
                int? userId = null;
                if (currentUserID > 0)
                {
                    userId = currentUserID;
                }
                var entity = new EmailTransmissions()
                {
                    EmailReplyToAddr = transmissionsModel.EmailReplyToAddr ?? "",
                    Subject = transmissionsModel.Subject,
                    MessageBody = transmissionsModel.MessageBody,
                    IsBodyHtml = transmissionsModel.IsBodyHtml,
                    DetailFailure = "",
                    CreatedUserID = userId,
                    LastUpdatedUserID = userId,
                    InsertedAt = timeUpdate,
                    UpdatedAt = timeUpdate,
                    RegistrationTicketId = registrationId,
                    WorkTicketId = workticketId,
                    AutonomousMaintenanceId = amId,
                    AmRequestTicketId = requestTicketId,
                };
                _unitOfWork.ElogbookRepository<EmailTransmissions>().Add(entity);

                // Add mail của nhà máy
                if (!string.IsNullOrWhiteSpace(transmissionsModel.FatoryEmails))
                {
                    string[] listEmail = transmissionsModel.FatoryEmails.Split(';');

                    for (int i = CommonConstant.ZERO; i < listEmail.Length; i++)
                    {
                        string tempEmail = listEmail[i].Trim();
                        if (!string.IsNullOrWhiteSpace(tempEmail))
                        {
                            recipientsModels.Add(new AddEmailTransmissionRecipientsModel
                            {
                                RecipientEmailAddr = tempEmail,
                                RecipientEmailName = transmissionsModel.FatoryName,
                                RecipientType = 3
                            });
                        }
                    }
                }

                if (_unitOfWork.Complete() > 0)
                {
                    recipientsModels = recipientsModels.GroupBy(x => x.RecipientEmailAddr).Select(s => s.First()).ToList();
                    foreach (var item in recipientsModels)
                    {
                        if (!string.IsNullOrEmpty(item.RecipientEmailAddr))
                        {
                            // Add nội dung mail
                            var entityRecipients = new EmailTransmissionRecipients()
                            {
                                RecipientEmailAddr = item.RecipientEmailAddr,
                                EmailTransmissionID = entity.ID,
                                RecipientEmailName = item.RecipientEmailName,
                                RecipientType = item.RecipientType,
                                CreatedUserID = userId,
                                LastUpdatedUserID = userId,
                                InsertedAt = timeUpdate,
                                UpdatedAt = timeUpdate
                            };
                            _unitOfWork.ElogbookRepository<EmailTransmissionRecipients>().Add(entityRecipients);
                        }

                        if (item.UserID > 0)
                        {
                            var entityAlertNotification = ReplaceContentSubjectNotice(entity, item.RecipientEmailName, item.UserID);

                            _unitOfWork.ElogbookRepository<AlertNotification>().Add(entityAlertNotification);
                        }
                    }

                    _unitOfWork.Complete();
                    BackgroundJob.Enqueue<IEmailService>(x => x.SendEmailConfiguration(entity.ID));
                }
            }
        }

        private AlertNotification ReplaceContentSubjectNotice(EmailTransmissions email, string fullName, int userID)
        {
            var system = _unitOfWork.ElogbookRepository<AppVersion>().GetQuery()
                .Select(e => new AppVersionModel() { ID = e.ID, AppTitle = e.AppTitle, }).FirstOrDefault();

            //replace subject
            string titleNotice = email.Subject.Replace(CommonConstant.MAIL_MERGE_TITLE + " - ", string.Empty);
            string contentNotice = string.Empty;

            //replace body
            if (email.RegistrationTicketId.GetValueOrDefault() > CommonConstant.ZERO)
            {
                var registration = _unitOfWork.ElogbookRepository<RegistrationTicket>().GetById(email.RegistrationTicketId.Value);
                if (registration != null)
                {
                    var body = email.MessageBody;

                    if (system != null)
                    {
                        body = body.Replace(CommonConstant.MAIL_MERGE_TITLE, system.AppTitle);
                    }

                    body = body.Replace(CommonConstant.MAIL_MERGE_ACCOUNT_USER, fullName);
                    body = body.Replace(CommonConstant.MAIL_MERGE_DON_VI_CONG_TAC_DANG_KY, registration.RegistrationUnitView);
                    body = body.Replace(CommonConstant.MAIL_MERGE_NOI_DUNG_CONG_VIEC, registration.WorkContent);
                    body = body.Replace(CommonConstant.MAIL_MERGE_DIA_DIEM, registration.RegistrationTicketType == CommonConstant.TYPE_PCTD ? registration.WorkPlacement : registration.LocationDevice);
                    body = body.Replace(CommonConstant.MAIL_MERGE_ACCOUNT_TAO, registration.CreatedUser?.FullName);
                    body = body.Replace(CommonConstant.MAIL_MERGE_ACCOUNT_THU_HOI, registration.LastUpdatedUser?.FullName);
                    body = body.Replace(CommonConstant.MAIL_MERGE_THOI_GIAN_THU_HOI, registration.UpdatedAt.ToString(CommonConstant.FORMAT_DATE_DDMMYYYYHHMMSS));

                    if (registration.StatusValue == (int)EnumTicketStatus.Approved)
                    {
                        var workTicket = _unitOfWork.ElogbookRepository<WorkTicket>()
                            .GetQuery(x => x.RegistrationTicketID == registration.ID && !x.IsDeleted).FirstOrDefault();

                        if (workTicket != null)
                        {
                            body = body.Replace(CommonConstant.MAIL_MERGE_ACCOUNT_CAP, workTicket.CreatedUser?.FullName);
                            body = body.Replace(CommonConstant.MAIL_MERGE_THOI_GIAN_BAT_DAU, workTicket.StartDate.ToString(CommonConstant.FORMAT_DATE_DDMMYYYYHHMMSS));
                            body = body.Replace(CommonConstant.MAIL_MERGE_THOI_GIAN_KET_THUC, workTicket.EndDate.ToString(CommonConstant.FORMAT_DATE_DDMMYYYYHHMMSS));
                            titleNotice = titleNotice + " - " + workTicket.WorkTicketNumber;
                            body = body.Replace(CommonConstant.MAIL_MERGE_SO_PHIEU_CONG_TAC, workTicket.WorkTicketNumber);

                            if (workTicket.ScreenType == (int)TicketTypePCT.DIEN)
                            {
                                body = body.Replace(CommonConstant.MAIL_MERGE_LOAI_PHIEU, workTicket.TicketStatus?.StatusName);
                            }

                            if (workTicket.ScreenType == (int)TicketTypePCT.COTHUYNHIETHOA)
                            {
                                var wtFeatureIds = _unitOfWork.ElogbookRepository<WorkTicketFeatureLink>()
                                    .GetQuery(x => !x.IsDeleted && x.WorkTicketID == workTicket.ID).Select(x => x.FeatureID).ToList();

                                var featureName = _unitOfWork.ElogbookRepository<WorkTicketFeatures>()
                                    .GetQuery(x => !x.IsDeleted && wtFeatureIds.Contains(x.ID)).Select(x => x.FeatureName).ToList();

                                body = body.Replace(CommonConstant.MAIL_MERGE_LOAI_PHIEU, string.Join(",", featureName));
                            }
                        }
                        else
                        {
                            titleNotice = titleNotice + " - " + registration.TicketNumber;
                            body = body.Replace(CommonConstant.MAIL_MERGE_THOI_GIAN_BAT_DAU, registration.StartDate.ToString(CommonConstant.FORMAT_DATE_DDMMYYYYHHMMSS));
                            body = body.Replace(CommonConstant.MAIL_MERGE_THOI_GIAN_KET_THUC, registration.EndDate.ToString(CommonConstant.FORMAT_DATE_DDMMYYYYHHMMSS));
                            body = body.Replace(CommonConstant.MAIL_MERGE_SO_PHIEU_CONG_TAC, registration.TicketNumber);
                        }
                    }
                    else
                    {
                        titleNotice = titleNotice + " - " + registration.TicketNumber;
                        body = body.Replace(CommonConstant.MAIL_MERGE_THOI_GIAN_BAT_DAU, registration.StartDate.ToString(CommonConstant.FORMAT_DATE_DDMMYYYYHHMMSS));
                        body = body.Replace(CommonConstant.MAIL_MERGE_THOI_GIAN_KET_THUC, registration.EndDate.ToString(CommonConstant.FORMAT_DATE_DDMMYYYYHHMMSS));
                        body = body.Replace(CommonConstant.MAIL_MERGE_SO_PHIEU_CONG_TAC, registration.TicketNumber);
                    }

                    if (body.Contains(CommonConstant.MAIL_MERGE_LY_DO_TU_CHOI))
                    {
                        var his = _unitOfWork.ElogbookRepository<TicketStatusHistory>()
                            .GetQuery(x => x.RegistrationTicketID == email.RegistrationTicketId.Value
                                        && x.Description.Equals(CommonConstant.MAIL_MERGE_REJECT)).FirstOrDefault();
                        if (his != null)
                        {
                            body = body.Replace(CommonConstant.MAIL_MERGE_LY_DO_TU_CHOI, his.Reason);
                            if (his.CreatedUser != null)
                            {
                                body = body.Replace(CommonConstant.MAIL_MERGE_ACCOUNT_TU_CHOI, his.CreatedUser.FullName);
                            }
                        }
                    }

                    body = body.Replace(CommonConstant.MAIL_MERGE_LINK_HE_THONG, AppSettings.DOMAIN);

                    contentNotice = body;
                }
            }

            if (email.WorkTicketId.GetValueOrDefault() > CommonConstant.ZERO)
            {
                var workTicket = _unitOfWork.ElogbookRepository<WorkTicket>().GetById(email.WorkTicketId.Value);
                if (workTicket != null)
                {
                    titleNotice = titleNotice + " - " + workTicket.WorkTicketNumber;
                    var body = email.MessageBody;
                    if (system != null)
                    {
                        body = body.Replace(CommonConstant.MAIL_MERGE_TITLE, system.AppTitle);
                    }
                    body = body.Replace(CommonConstant.MAIL_MERGE_ACCOUNT_USER, fullName);
                    body = body.Replace(CommonConstant.MAIL_MERGE_DON_VI_CONG_TAC_DANG_KY, workTicket.Factory?.FactoryName);
                    body = body.Replace(CommonConstant.MAIL_MERGE_NOI_DUNG_CONG_VIEC, workTicket.WorkContent);
                    body = body.Replace(CommonConstant.MAIL_MERGE_DIA_DIEM, workTicket.WorkPlacement);
                    body = body.Replace(CommonConstant.MAIL_MERGE_THOI_GIAN_BAT_DAU, workTicket.StartDate.ToString(CommonConstant.FORMAT_DATE_DDMMYYYYHHMMSS));
                    body = body.Replace(CommonConstant.MAIL_MERGE_THOI_GIAN_KET_THUC, workTicket.EndDate.ToString(CommonConstant.FORMAT_DATE_DDMMYYYYHHMMSS));
                    body = body.Replace(CommonConstant.MAIL_MERGE_SO_PHIEU_CONG_TAC, workTicket.WorkTicketNumber);
                    body = body.Replace(CommonConstant.MAIL_MERGE_ACCOUNT_CAP, workTicket.CreatedUser?.FullName);
                    // Thông tin cho phép
                    var allowedInfo = _unitOfWork.ElogbookRepository<AllowedInfo>()
                        .GetQuery(x => x.WorkTicketID == workTicket.ID && !x.IsDeleted).FirstOrDefault();
                    if (allowedInfo != null)
                    {
                        var acceptorUserEntity = GetLeadershipById(email.WorkTicketId.Value, (int)LeadershipUserRoles.Acceptor);
                        body = body.Replace(CommonConstant.MAIL_MERGE_ACCOUNT_CHO_PHEP, acceptorUserEntity?.FullName);
                        body = body.Replace(CommonConstant.MAIL_MERGE_THOI_GIAN_CHO_PHEP, allowedInfo.AllowedTime.ToString(CommonConstant.FORMAT_DATE_DDMMYYYYHHMMSS));
                    }
                    var workFinish = _unitOfWork.ElogbookRepository<WorkFinish>().GetQuery(x => x.WorkTicketID == workTicket.ID && !x.IsDeleted).FirstOrDefault();

                    if (workFinish != null)
                    {
                        if (workTicket.ScreenType == (int)TicketTypePCT.DIEN && workFinish.CheckCompletedTime != null)
                        {
                            body = body.Replace(CommonConstant.MAIL_MERGE_THOI_GIAN_HOAN_THANH, workFinish.CheckCompletedTime.Value.ToString(CommonConstant.FORMAT_DATE_DDMMYYYYHHMMSS));
                        }
                        else if (workTicket.ScreenType == (int)TicketTypePCT.COTHUYNHIETHOA && workFinish.CheckReceivedTime != null)
                        {
                            body = body.Replace(CommonConstant.MAIL_MERGE_THOI_GIAN_HOAN_THANH, workFinish.CheckReceivedTime.Value.ToString(CommonConstant.FORMAT_DATE_DDMMYYYYHHMMSS));
                        }
                        else
                        {
                            body = body.Replace(CommonConstant.MAIL_MERGE_THOI_GIAN_HOAN_THANH, "");
                        }
                    }
                    else
                    {
                        body = body.Replace(CommonConstant.MAIL_MERGE_THOI_GIAN_HOAN_THANH, "");
                    }

                    if (workTicket.ScreenType == (int)TicketTypePCT.DIEN)
                    {
                        body = body.Replace(CommonConstant.MAIL_MERGE_LOAI_PHIEU, workTicket.TicketStatus?.StatusName);
                    }

                    if (workTicket.ScreenType == (int)TicketTypePCT.COTHUYNHIETHOA)
                    {
                        var wtFeatureIds = _unitOfWork.ElogbookRepository<WorkTicketFeatureLink>()
                            .GetQuery(x => !x.IsDeleted && x.WorkTicketID == workTicket.ID).Select(x => x.FeatureID).ToList();

                        var featureName = _unitOfWork.ElogbookRepository<WorkTicketFeatures>()
                            .GetQuery(x => !x.IsDeleted && wtFeatureIds.Contains(x.ID)).Select(x => x.FeatureName).ToList();

                        body = body.Replace(CommonConstant.MAIL_MERGE_LOAI_PHIEU, string.Join(",", featureName));
                    }

                    body = body.Replace(CommonConstant.MAIL_MERGE_LINK_HE_THONG, AppSettings.DOMAIN);
                    contentNotice = body;
                }
            }

            if (email.AutonomousMaintenanceId.GetValueOrDefault() > CommonConstant.ZERO)
            {
                var am = _unitOfWork.ElogbookRepository<AutonomousMaintenance>().GetById(email.AutonomousMaintenanceId.Value);

                if (am != null)
                {
                    titleNotice = titleNotice + " - " + am.AmMaintenanceSchedule.MaintenanceScheduleName;
                    var body = email.MessageBody;
                    if (system != null)
                    {
                        body = body.Replace(CommonConstant.MAIL_MERGE_TITLE, system.AppTitle);
                    }

                    body = body.Replace(CommonConstant.MAIL_MERGE_ACCOUNT_USER, fullName);
                    body = body.Replace(CommonConstant.MAIL_MERGE_KHU_VUC, am.AmLocation.SiteDesc);
                    body = body.Replace(CommonConstant.MAIL_MERGE_THIET_BI, am.AmDevice.AssetDesc);
                    body = body.Replace(CommonConstant.MAIL_MERGE_ACCOUNT_THUC_HIEN_AM, am.LastUpdatedUser?.FullName);
                    body = body.Replace(CommonConstant.MAIL_MERGE_NGAY_CUA_HOAT_DONG_AM, am.UpdatedAt.ToString(CommonConstant.FORMAT_DATE_DDMMYYYYHHMM));
                    body = body.Replace(CommonConstant.MAIL_MERGE_TEN_LICH_BAO_TRI, am.AmMaintenanceSchedule.MaintenanceScheduleName);
                    body = body.Replace(CommonConstant.MAIL_MERGE_CUONG_VI, am.OperationPosition.OperationPositionName);
                    body = body.Replace(CommonConstant.MAIL_MERGE_CA, am.OperationShift.OperationShiftName);
                    body = body.Replace(CommonConstant.MAIL_MERGE_NOI_DUNG_BAT_THUONG_THIET_BI, am.DeviceAbnormalDescription);
                    body = body.Replace(CommonConstant.MAIL_MERGE_LINK_HE_THONG, AppSettings.DOMAIN);

                    contentNotice = body;
                }
            }

            if (email.AmRequestTicketId.GetValueOrDefault() > CommonConstant.ZERO)
            {
                var requestTicket = _unitOfWork.ElogbookRepository<AmRequestTicket>().GetById(email.AmRequestTicketId.Value);
                if (requestTicket != null)
                {
                    titleNotice = titleNotice + " - " + requestTicket.RequestTicketNumber;

                    var amEmergency = _unitOfWork.ElogbookRepository<AmEmergency>()
                            .GetQuery(x => x.ID == requestTicket.AmEmergencyId && !x.IsDeleted).FirstOrDefault();
                    string emergencyName = string.Empty;

                    if (amEmergency != null)
                    {
                        emergencyName = amEmergency.Name;
                    }

                    var body = email.MessageBody;
                    if (system != null)
                    {
                        body = body.Replace(CommonConstant.MAIL_MERGE_TITLE, system.AppTitle);
                    }
                    body = body.Replace(CommonConstant.MAIL_MERGE_ACCOUNT_USER, fullName);
                    body = body.Replace(CommonConstant.MAIL_MERGE_SO_PHIEU_YEU_CAU, requestTicket.RequestTicketNumber);
                    body = body.Replace(CommonConstant.MAIL_MERGE_MUC_DO_KHAN_CAP, emergencyName);
                    body = body.Replace(CommonConstant.MAIL_MERGE_NOI_DUNG_CONG_VIEC, requestTicket.RequestContent);
                    body = body.Replace(CommonConstant.MAIL_MERGE_NHA_MAY, requestTicket.Factory?.FactoryName);
                    body = body.Replace(CommonConstant.MAIL_MERGE_ACCOUNT_TAO, requestTicket.CreatedUser.FullName);
                    body = body.Replace(CommonConstant.MAIL_MERGE_ACCOUNT_TU_CHOI, requestTicket.LastUpdatedUser?.FullName);
                    body = body.Replace(CommonConstant.MAIL_MERGE_ACCOUNT_PHE_DUYET, requestTicket.LastUpdatedUser?.FullName);

                    // Từ chối phiếu yêu cầu
                    if (body.Contains(CommonConstant.MAIL_MERGE_LY_DO_TU_CHOI))
                    {
                        var his = _unitOfWork.ElogbookRepository<AmRequestTicketHistory>()
                            .GetQuery(x => x.RequestTicketId == email.AmRequestTicketId.Value && !x.IsDeleted && x.Action.Equals(CommonConstant.MAIL_MERGE_REJECT_STATUS))
                            .OrderByDescending(x => x.InsertedAt).ToList();

                        if (his.Count > 0)
                        {
                            var comment = his.Select(x => x.Comment).First();
                            body = body.Replace(CommonConstant.MAIL_MERGE_LY_DO_TU_CHOI, comment);
                        }
                    }

                    // Dừng xử lý phiếu yêu cầu
                    if (body.Contains(CommonConstant.MAIL_MERGE_LY_DO_DUNG_XU_LY))
                    {
                        var his = _unitOfWork.ElogbookRepository<AmRequestTicketHistory>().GetQuery(x =>
                        x.RequestTicketId == email.AmRequestTicketId.Value && !x.IsDeleted).Where(x => x.Action.Equals(CommonConstant.MAIL_MERGE_STOP_STATUS)).FirstOrDefault();
                        if (his != null)
                        {
                            body = body.Replace(CommonConstant.MAIL_MERGE_LY_DO_DUNG_XU_LY, his.Comment);
                        }
                    }

                    body = body.Replace(CommonConstant.MAIL_MERGE_LINK_HE_THONG, AppSettings.DOMAIN);

                    contentNotice = body;
                }
            }

            var entityAlertNotification = new AlertNotification()
            {
                NoticeTitle = titleNotice,
                NoticeContent = contentNotice,
                UserID = userID,
                Status = (int)EnumNotification.Unread,
                IsImportant = false,
                CreatedUserID = email.CreatedUserID,
                LastUpdatedUserID = email.CreatedUserID,
                InsertedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            return entityAlertNotification;
        }

        private LeadershipUserModel GetLeadershipById(int workTicketID, int roles)
        {
            var entity = _unitOfWork.ElogbookRepository<LeadershipUser>()
                            .GetQuery(x => x.WorkTicketID == workTicketID && x.Roles == roles && !x.IsDeleted)
                            .Select(e => new LeadershipUserModel()
                            {
                                ID = e.ID,
                                WorkTicketID = e.WorkTicketID,
                                UserName = e.UserName,
                                IsExternal = e.IsExternal ?? false,
                                FullName = e.FullName,
                                Roles = e.Roles,
                                ElectricRankStr = e.ElectricRank,
                                ElectricRankCard = e.ElectricRankCard,
                                LaborRankStr = e.LaborRank,
                                LaborCard = e.LaborCard,
                                Signature = e.Signature,
                                PositionName = e.PositionName,
                            }).FirstOrDefault();

            return entity;
        }

        /// <summary>
        /// delete attachment file
        /// </summary>
        /// <param name="attachments"></param>
        /// <param name="attachmentIDs"></param>
        /// <param name="userID"></param>
        public void DeleteAttachFileUnuse(string attachments, List<int> attachmentIDs, int userID)
        {
            if (!string.IsNullOrWhiteSpace(attachments) && attachmentIDs != null && attachmentIDs.Count > 0)
            {
                List<string> fileExtensions = new List<string>() { "png", "jpg", "jpeg" };

                var ids = attachments.Split(',');
                foreach (var item in ids)
                {
                    if (int.TryParse(item, out int id) && !attachmentIDs.Contains(id))
                    {
                        var entityFile = _unitOfWork.ElogbookRepository<Attachment>().GetById((id));
                        if (entityFile != null)
                        {
                            // kiểm tra file đã tồn tại chưa và xoá
                            var filePath = Path.Combine(AppSettings.SERVER_DIR, entityFile.FilePath);
                            if (File.Exists(filePath))
                            {
                                File.Delete(filePath);
                            }

                            // kiểm tra file Thumbnail đã tồn tại chưa và xoá
                            var fileExtension = Path.GetExtension(entityFile.FileName.Trim());
                            if (fileExtensions.Contains(fileExtension))
                            {
                                var filePathThumbnail = Path.Combine(AppSettings.SERVER_DIR, entityFile.PathThumbnail);
                                if (File.Exists(filePathThumbnail))
                                {
                                    File.Delete(filePathThumbnail);
                                }
                            }

                            _unitOfWork.ElogbookRepository<Attachment>().Delete(entityFile, userID);
                            _unitOfWork.Complete();
                        }
                    }
                }
            }
        }

        public SystemDataModel GetSystemData(string[] dataTypes, int companyID, int productionPlantID, int userID)
        {
            SystemDataModel systemDataModel = new SystemDataModel();

            if (dataTypes != null && dataTypes.Length > 0)
            {
                for (int i = 0; i < dataTypes.Length; i++)
                {
                    var dataTypeValue = dataTypes[i];
                    if (dataTypeValue != null)
                        dataTypeValue = dataTypeValue.Trim().ToLower();

                    switch (dataTypeValue)
                    {
                        // Nhà máy
                        case "factory":
                            systemDataModel.Factories = GetFactories(-1);
                            break;
                        // Phân xưởng
                        case "productionplant":
                            break;
                        // Danh sách người dùng
                        case "users":
                            systemDataModel.Users = GetUsersInfo("");
                            break;
                        // Trạng thái truy cập
                        case "userstatus":
                            systemDataModel.UserStatus = CommonListValueLabel.GetUserStatus();
                            break;
                        // Loại tài khoản
                        case "usertype":
                            systemDataModel.UserTypes = CommonListValueLabel.GetUserTypes();
                            break;
                        // Vai trò
                        case "userrole":
                            var userRoles = new List<ValueLabelModel>
                            {
                                new ValueLabelModel { Value = CommonConstant.ALL_KEY, Label = CommonConstant.ALL_VALUE },
                            };
                            userRoles.AddRange(GetUserRoles(userID));
                            systemDataModel.UserRoles = userRoles;
                            break;
                        // Nguồn của tài khoản người dùng
                        case "usersource":
                            systemDataModel.UserSources = CommonListValueLabel.GetUserSources();
                            break;
                        // Trạng thái sử dụng
                        case "accessstatus":
                            systemDataModel.AccessStatus = CommonListValueLabel.GetAccessStatus();
                            break;
                        // Quyền thao tác
                        case "datapermission":
                            systemDataModel.DataPermission = CommonListValueLabel.GetDataPermission();
                            break;
                        // Tổ máy
                        case "powerunit":
                            systemDataModel.PowerUnit = GetPowerUnits();
                            break;
                        // Sử dụng thẻ RFID/QR Code
                        case "rfidqr":
                            systemDataModel.RFIDQR = CommonListValueLabel.GetRFIDQR();
                            break;
                        // Chọn ngày
                        case "datetype":
                            systemDataModel.DateType = CommonListValueLabel.SearchDateType();
                            break;
                        // Ca vận hành
                        case "operationshift":
                            systemDataModel.OperationShift = GetOperationShifts();
                            break;
                        // Kíp vận hành
                        case "operationcrew":
                            systemDataModel.OperationCrew = GetOperationCrews();
                            break;
                        // Mẫu phương thức giao ca
                        case "shifttemplate":
                            systemDataModel.DeliveryShiftMethod = GetDeliveryShiftMethodTemplate();
                            break;
                        // Các loại phiếu công tác
                        case "tickettype":
                            systemDataModel.TicketTypes = CommonListValueLabel.GetTicketTypes();
                            break;
                        // Trạng thái phiếu
                        case "ticketstatus":
                            var ticketStatuss = new List<ValueLabelModel>
                            {
                                new ValueLabelModel { Value = CommonConstant.ALL_KEY, Label = CommonConstant.ALL_VALUE },
                            };
                            ticketStatuss.AddRange(GetTicketStatus());
                            systemDataModel.TicketStatus = ticketStatuss;
                            break;
                        // Cương vị
                        case "operationposition":
                            var operationpositions = new List<ValueLabelModel>
                            {
                                new ValueLabelModel { Value = CommonConstant.ALL_KEY, Label = CommonConstant.ALL_VALUE },
                            };
                            operationpositions.AddRange(GetOperationPositions());
                            systemDataModel.OperationPositions = operationpositions;
                            break;
                        // Chu kỳ
                        case "cycle":
                            var cycles = new List<ValueLabelModel>
                            {
                                new ValueLabelModel { Value = CommonConstant.ALL_KEY, Label = CommonConstant.ALL_VALUE },
                            };
                            cycles.AddRange(GetCycles());
                            systemDataModel.Cycles = cycles;
                            break;
                        // Nhóm thông số
                        case "parametergroup":
                            var paramerterGroups = new List<ValueLabelModel>
                            {
                                new ValueLabelModel { Value = CommonConstant.ALL_KEY, Label = CommonConstant.ALL_VALUE },
                            };
                            paramerterGroups.AddRange(GetParameterGroups());
                            systemDataModel.ParameterGroups = paramerterGroups;
                            break;
                        // Công ty
                        case "company":
                            var companies = new List<ValueLabelModel>
                            {
                                new ValueLabelModel { Value = CommonConstant.ALL_KEY, Label = CommonConstant.ALL_VALUE },
                            };
                            companies.AddRange(GetCompanies());
                            systemDataModel.Companies = companies;
                            break;
                        // Bậc an toàn điện
                        case "electricrank":
                            systemDataModel.Electricranks = CommonListValueLabel.GetElectricRanks();
                            var electricranks = new List<ValueStringLabelModel>
                            {
                                new ValueStringLabelModel { Value = "", Label = CommonConstant.ALL_VALUE },
                            };
                            electricranks.AddRange(GetElectricRanks());
                            systemDataModel.ElectricranksStr = electricranks;
                            break;
                        // Bậc thợ
                        case "laborrank":
                            systemDataModel.LaborRanks = CommonListValueLabel.GetLaborRanks();
                            var laborranks = new List<ValueStringLabelModel>
                            {
                                new ValueStringLabelModel { Value = "", Label = CommonConstant.ALL_VALUE },
                            };
                            laborranks.AddRange(GetLaborRanks());
                            systemDataModel.LaborRanksStr = laborranks;
                            break;
                        // Lỗi thiết bị
                        case "deviceerror":
                            systemDataModel.DeviceErrors = CommonListValueLabel.GetDeviceErrors();
                            break;
                        // Tình trạng xử lý
                        case "processingstatus":
                            systemDataModel.ProcessingStatus = CommonListValueLabel.GetProcessingStatus();
                            break;
                        // Tính năng phiếu công tác (thủy/cơ/nhiệt/hóa)
                        case "workticketfeatures":
                            var workTicketFeatures = new List<ValueLabelModel>
                            {
                                new ValueLabelModel { Value = CommonConstant.ALL_KEY, Label = CommonConstant.ALL_VALUE },
                            };
                            workTicketFeatures.AddRange(GetWorkTicketFeatures());
                            systemDataModel.WorkTicketFeatures = workTicketFeatures;
                            break;
                        // Vị trí đăng ký làm việc (thủy /cơ /nhiệt /hóa)
                        case "worklocation":
                            var workLocation = new List<ValueLabelModel>
                            {
                                new ValueLabelModel { Value = CommonConstant.ALL_KEY, Label = CommonConstant.ALL_VALUE },
                            };
                            workLocation.AddRange(GetWorkLocation());
                            systemDataModel.WorkLocation = workLocation;
                            break;
                        // Sự kiện gửi email
                        case "emailnotification":
                            var emailnotifications = new List<ValueLabelModel>
                            {
                                new ValueLabelModel { Value = CommonConstant.ALL_KEY, Label = CommonConstant.ALL_VALUE },
                            };
                            emailnotifications.AddRange(GetSystemData((int)SystemDataType.EmailNotificationEvent));
                            systemDataModel.EmailNotifications = emailnotifications;
                            break;
                        // Các chức năng có sự kiện gửi email
                        case "modulename":
                            var modulenames = new List<ValueLabelModel>
                            {
                                new ValueLabelModel { Value = CommonConstant.ALL_KEY, Label = CommonConstant.ALL_VALUE },
                            };
                            modulenames.AddRange(GetSystemData((int)SystemDataType.ModuleName));
                            systemDataModel.ModuleNames = modulenames;
                            break;
                    }
                }
            }

            return systemDataModel;
        }

        public List<ValueLabelModel> GetSystemDataByFilter(string dataType, string filterDescription, int companyID, int productionPlantID)
        {
            List<ValueLabelModel> listData = new List<ValueLabelModel>();
            switch (dataType)
            {
                // Nhà máy
                case "factory":
                    listData = GetFactories(productionPlantID, filterDescription, false);
                    break;
                // Phân xưởng
                case "productionplant":
                    listData = GetProductionPlants(CommonConstant.ALL_KEY, filterDescription);
                    break;
                // Danh sách người dùng
                case "users":
                    listData = GetUsers(filterDescription);
                    break;
            }

            return listData;
        }

        public List<ValueLabelModel> GetProductionPlantByFactoryID(int factoryID)
        {
            var productionPlants = new List<ValueLabelModel>
                            {
                                new ValueLabelModel { Value = CommonConstant.ALL_KEY, Label = CommonConstant.ALL_VALUE },
                            };

            var listData = GetProductionPlants(factoryID);

            if (listData != null && listData.Count > 0)
                productionPlants.AddRange(listData);

            return productionPlants;
        }

        public List<ProductionPlantInfoModel> GetProductionPlantByCompanyID(int companyID)
        {
            var productionPlants = new List<ProductionPlantInfoModel>
                            {
                                new ProductionPlantInfoModel { Value = CommonConstant.ALL_KEY, Label = CommonConstant.ALL_VALUE },
                            };

            var listData = _unitOfWork.ElogbookRepository<Organization>().GetQuery(x => !x.IsDeleted
                                                                                    && x.Active == (int)AccessStatus.Active
                                                                                    && "Phân xưởng".Equals(x.Level, StringComparison.OrdinalIgnoreCase)
                                                                                    && (x.ParentID == companyID))
                .Select(
                    e => new ProductionPlantInfoModel()
                    {
                        Value = e.ID,
                        Label = e.Name,
                        Isoperate = e.ProductionPlantType == "Vận hành",
                    }).OrderBy(t => t.Label).ToList();

            if (listData.Count > 0)
                productionPlants.AddRange(listData);

            return productionPlants;
        }

        public MasterDataModel GetMasterData(string deviceID, int factoryID, int operationPositionID, bool force)
        {
            try
            {
                LogCustom.LogMessage(TrackingLogLevel.DEBUG, $"{GetType()}, GetMasterData(), deviceID: {deviceID}, factoryID: {factoryID}, operationPositionID: {operationPositionID}, force: {force}");
                DateTime dateTimeSync = DateTimeConvert.ConvertDateTimeFromString(DateTime.Now.AddDays(-AppSettings.DATE_SYNC_DATA_MOBILE).ToString("dd/MM/yyyy").Replace("-", "/"), true);
                MasterDataModel masterDataModel = new MasterDataModel();
                DateTime syncDate = DateTime.MinValue;
                DateTime syncDataModule = DateTime.MinValue;

                // Lấy thời gian sync dữ liệu gần nhất, nếu chưa sync lần nào thì trả về tất cả dữ liệu
                var trackingSyncMobileData = _unitOfWork.ElogbookRepository<TrackingSyncMobileData>().GetQuery(x => !x.IsDeleted && x.DeviceID.Equals(deviceID, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (trackingSyncMobileData != null && !force)
                {
                    syncDate = trackingSyncMobileData.SyncDate;
                    syncDataModule = trackingSyncMobileData.SyncDataModule;
                }

                if (factoryID <= 0)
                {
                    #region Loại Phiếu

                    masterDataModel.WorkTicketFeatures = _unitOfWork.ElogbookRepository<WorkTicketFeatures>().GetQuery(x => x.UpdatedAt > syncDate)
                        .Select(
                            e => new WorkTicketFeaturesModel()
                            {
                                ID = e.ID,
                                Label = e.FeatureName
                            }).ToList();

                    #endregion

                    #region Nhân viên

                    masterDataModel.Users = _unitOfWork.ElogbookRepository<User>().GetQuery(x => x.UpdatedAt > syncDate)
                        .Select(
                            e => new MobileUserInfo()
                            {
                                ID = e.ID,
                                FullName = e.FullName,
                                UserName = e.UserName,
                                Password = e.Password,
                                PositionName = e.PositionName,
                                DateOfBirth = e.DateOfBirth,
                                NationalID = e.NationalID,
                                ElectricRankStr = e.ElectricRank,
                                ElectricRankCard = e.ElectricRankCard,
                                Signature = e.Signature,
                                CompanyID = e.CompanyID,
                                ProductionPlantID = e.ProductionPlantID,
                                TeamID = e.TeamID,
                                GroupID = e.GroupID,
                                FactoryID = e.FactoryID,
                                AccountType = e.AccountType,
                                SourceType = e.SourceType,
                                Status = e.Status,
                                IsDeleted = e.IsDeleted,
                                CreatedUserID = e.CreatedUserID,
                                InsertedAt = e.InsertedAt,
                                LastUpdatedUserID = e.LastUpdatedUserID,
                                UpdatedAt = e.UpdatedAt,
                                SigningMethod = new ValueLabelModel { Value = e.SigningMethod.ID, Label = e.SigningMethod.SigningMethodName },
                                SigningMethodID = e.SigningMethod.ID
                            }).OrderBy(t => t.ID).ToList();

                    masterDataModel.RoleInUsers = _unitOfWork.ElogbookRepository<RoleInUser>().GetQuery(x => x.UpdatedAt > syncDate)
                                .Select(
                                    e => new MobileRoleInUsersInfo()
                                    {
                                        ID = e.ID,
                                        UserRoleID = e.UserRoleID,
                                        UserID = e.UserID,
                                        IsDeleted = e.IsDeleted,
                                        CreatedUserID = e.CreatedUserID,
                                        InsertedAt = e.InsertedAt,
                                        LastUpdatedUserID = e.LastUpdatedUserID,
                                        UpdatedAt = e.UpdatedAt
                                    }).OrderBy(t => t.ID).ToList();

                    #endregion

                    #region Vai trò

                    masterDataModel.UserRole = _unitOfWork.ElogbookRepository<UserRole>().GetQuery(x => x.UpdatedAt > syncDate)
                                    .Select(
                                        e => new MobileUserRoleInfo()
                                        {
                                            ID = e.ID,
                                            RoleName = e.RoleName,
                                            Description = e.Description,
                                            DataPermission = e.DataPermission,
                                            Permissions = e.Permissions,
                                            Active = e.Active,
                                            IsAdmin = e.IsAdmin,
                                            IsDeleted = e.IsDeleted,
                                            CreatedUserID = e.CreatedUserID,
                                            InsertedAt = e.InsertedAt,
                                            LastUpdatedUserID = e.LastUpdatedUserID,
                                            UpdatedAt = e.UpdatedAt
                                        }).OrderBy(t => t.ID).ToList();

                    masterDataModel.Modules = _unitOfWork.ElogbookRepository<Modules>().GetQuery(x => x.UpdatedAt > syncDate)
                                    .Select(
                                        e => new MobileModulesInfo()
                                        {
                                            ID = e.ID,
                                            ModuleValue = e.ModuleValue,
                                            ModuleName = e.ModuleName,
                                            SystemType = e.SystemType,
                                            IsDeleted = e.IsDeleted,
                                            CreatedUserID = e.CreatedUserID,
                                            InsertedAt = e.InsertedAt,
                                            LastUpdatedUserID = e.LastUpdatedUserID,
                                            UpdatedAt = e.UpdatedAt
                                        }).OrderBy(t => t.ID).ToList();

                    masterDataModel.UserPermission = _unitOfWork.ElogbookRepository<UserPermission>().GetQuery(x => x.UpdatedAt > syncDate)
                                    .Select(
                                        e => new MobileUserPermissionInfo()
                                        {
                                            ID = e.ID,
                                            PermissionKey = e.PermissionKey,
                                            PermissionName = e.PermissionName,
                                            Active = e.Active,
                                            ModuleValue = e.ModuleValue,
                                            SystemType = e.SystemType,
                                            IsDeleted = e.IsDeleted,
                                            CreatedUserID = e.CreatedUserID,
                                            InsertedAt = e.InsertedAt,
                                            LastUpdatedUserID = e.LastUpdatedUserID,
                                            UpdatedAt = e.UpdatedAt
                                        }).OrderBy(t => t.ID).ToList();

                    #endregion

                    #region Nhà máy

                    masterDataModel.Factory = _unitOfWork.ElogbookRepository<Factory>().GetQuery(x => x.UpdatedAt > syncDate)
                               .Select(
                                   e => new MobileFactoryInfo()
                                   {
                                       ID = e.ID,
                                       FactoryName = e.FactoryName,
                                       CompanyID = e.CompanyID,
                                       ProductionPlantID = e.ProductionPlantID,
                                       Status = e.Status,
                                       IsUseOperationParameterGroup = e.IsUseOperationParameterGroup,
                                       IsFilterFactory = e.IsFilterFactory,
                                       IsDeleted = e.IsDeleted,
                                       CreatedUserID = e.CreatedUserID,
                                       InsertedAt = e.InsertedAt,
                                       LastUpdatedUserID = e.LastUpdatedUserID,
                                       UpdatedAt = e.UpdatedAt,
                                   }).OrderBy(t => t.ID).ToList();

                    #endregion

                    #region Tổ chức

                    masterDataModel.Organization = _unitOfWork.ElogbookRepository<Organization>().GetQuery(x => x.UpdatedAt > syncDate)
                                .Select(
                                    e => new MobileOrganizationInfo()
                                    {
                                        ID = e.ID,
                                        ParentID = e.ParentID,
                                        Name = e.Name,
                                        ShortName = e.ShortName,
                                        Level = e.Level,
                                        CompanyType = e.CompanyType,
                                        ProductionPlantType = e.ProductionPlantType,
                                        Active = e.Active,
                                        IsDeleted = e.IsDeleted,
                                        CreatedUserID = e.CreatedUserID,
                                        InsertedAt = e.InsertedAt,
                                        LastUpdatedUserID = e.LastUpdatedUserID,
                                        UpdatedAt = e.UpdatedAt
                                    }).OrderBy(t => t.ID).ToList();

                    #endregion

                    #region Ca vận hành

                    masterDataModel.PositionShift = _unitOfWork.ElogbookRepository<OperationShift>()
                                .GetQuery(x => x.UpdatedAt > syncDate)
                                .Select(
                                    e => new MobileOperationShiftInfo()
                                    {
                                        ID = e.ID,
                                        OperationShiftName = e.OperationShiftName,
                                        Position = e.Position,
                                        FactoryID = e.FactoryID,
                                        StartTime = e.StartTime,
                                        EndTime = e.EndTime,
                                        Active = e.Active,
                                        IsDeleted = e.IsDeleted,
                                        CreatedUserID = e.CreatedUserID,
                                        InsertedAt = e.InsertedAt,
                                        LastUpdatedUserID = e.LastUpdatedUserID,
                                        UpdatedAt = e.UpdatedAt
                                    }).OrderBy(t => t.ID).ToList();

                    masterDataModel.UsedTimes = _unitOfWork.ElogbookRepository<UsedTimes>().GetQuery(x => x.UpdatedAt > syncDate)
                                .Select(
                                    e => new MobileUsedTimesInfo()
                                    {
                                        ID = e.ID,
                                        ActiveTime = e.ActiveTime,
                                        InactiveTime = e.InactiveTime,
                                        IsDeleted = e.IsDeleted,
                                        CreatedUserID = e.CreatedUserID,
                                        InsertedAt = e.InsertedAt,
                                        LastUpdatedUserID = e.LastUpdatedUserID,
                                        UpdatedAt = e.UpdatedAt
                                    }).OrderBy(t => t.ID).ToList();

                    #endregion

                    #region Chu kỳ

                    masterDataModel.CycleManagement = _unitOfWork.ElogbookRepository<CycleManagement>().GetQuery(x => x.UpdatedAt > syncDate)
                                .Select(
                                    e => new MobileCycleManagementInfo()
                                    {
                                        ID = e.ID,
                                        CycleName = e.CycleName,
                                        Position = e.Position,
                                        TimeLine = e.TimeLine,
                                        OpenInputBefore = e.OpenInputBefore,
                                        BlockInputAfter = e.BlockInputAfter,
                                        Active = e.Active,
                                        IsDeleted = e.IsDeleted,
                                        CreatedUserID = e.CreatedUserID,
                                        InsertedAt = e.InsertedAt,
                                        LastUpdatedUserID = e.LastUpdatedUserID,
                                        UpdatedAt = e.UpdatedAt
                                    }).OrderBy(t => t.ID).ToList();

                    masterDataModel.CycleFactory = _unitOfWork.ElogbookRepository<CycleFactory>()
                                .GetQuery(x => x.UpdatedAt > syncDate)
                                .Select(
                                    e => new MobileCycleFactoryInfo()
                                    {
                                        ID = e.ID,
                                        CycleID = e.CycleID,
                                        FactoryID = e.FactoryID,
                                        IsDeleted = e.IsDeleted,
                                        CreatedUserID = e.CreatedUserID,
                                        InsertedAt = e.InsertedAt,
                                        LastUpdatedUserID = e.LastUpdatedUserID,
                                        UpdatedAt = e.UpdatedAt
                                    }).OrderBy(t => t.ID).ToList();

                    masterDataModel.TimeLineCycle = _unitOfWork.ElogbookRepository<TimeLineCycle>().GetQuery(x => x.UpdatedAt > syncDate)
                                .Select(
                                    e => new MobileTimeLineCycleInfo()
                                    {
                                        ID = e.ID,
                                        Hour = e.Hour,
                                        CycleID = e.CycleID,
                                        Active = e.Active,
                                        IsDeleted = e.IsDeleted,
                                        CreatedUserID = e.CreatedUserID,
                                        InsertedAt = e.InsertedAt,
                                        LastUpdatedUserID = e.LastUpdatedUserID,
                                        UpdatedAt = e.UpdatedAt
                                    }).OrderBy(t => t.ID).ToList();

                    #endregion

                    #region Cương vị

                    masterDataModel.OperationPosition = _unitOfWork.ElogbookRepository<OperationPosition>()
                                .GetQuery(x => x.UpdatedAt > syncDate)
                                .Select(
                                    e => new MobileOperationPositionInfo()
                                    {
                                        ID = e.ID,
                                        OperationPositionName = e.OperationPositionName,
                                        FactoryID = e.FactoryID,
                                        Active = e.Active,
                                        Quantity = e.Quantity,
                                        AuthenticationType = e.AuthenticationType,
                                        IsUsingOnlyOneTablet = e.IsUsingOnlyOneTablet,
                                        AllowWriteDiaryPortal = e.AllowWriteDiaryPortal,
                                        AllowWriteDiaryMobile = e.AllowWriteDiaryMobile,
                                        AllowWriteParameterPortal = e.AllowWriteParameterPortal,
                                        AllowWriteParameterMobile = e.AllowWriteParameterMobile,
                                        JobTitle = e.JobTitle,
                                        TemplateDeliveryShiftMethod = e.TemplateDeliveryShiftMethod,
                                        Position = e.Position,
                                        IsConcurrentlyInShift = e.IsConcurrentlyInShift,
                                        IsDeleted = e.IsDeleted,
                                        CreatedUserID = e.CreatedUserID,
                                        InsertedAt = e.InsertedAt,
                                        LastUpdatedUserID = e.LastUpdatedUserID,
                                        UpdatedAt = e.UpdatedAt,
                                        ShiftReceivingTemplate = e.ShiftReceivingTemplate,
                                    }).OrderBy(t => t.ID).ToList();

                    #endregion

                    #region Cương vị trong ca

                    masterDataModel.PositionInShift = _unitOfWork.ElogbookRepository<PositionInShift>()
                                .GetQuery(x => x.UpdatedAt > syncDate)
                                .Select(
                                    e => new MobilePositionInShiftInfo()
                                    {
                                        ID = e.ID,
                                        ParentID = e.ParentID,
                                        OperationPositionID = e.OperationPositionID,
                                        IsSystem = e.IsSystem,
                                        Active = e.Active,
                                        Quantity = e.Quantity,
                                        Position = e.Position,
                                        IsDeleted = e.IsDeleted,
                                        CreatedUserID = e.CreatedUserID,
                                        InsertedAt = e.InsertedAt,
                                        LastUpdatedUserID = e.LastUpdatedUserID,
                                        UpdatedAt = e.UpdatedAt
                                    }).OrderBy(t => t.ID).ToList();

                    #endregion

                    #region Tổ máy

                    masterDataModel.PowerUnit = _unitOfWork.ElogbookRepository<PowerUnit>().GetQuery(x => x.UpdatedAt > syncDate)
                                .Select(
                                    e => new MobilePowerUnitInfo()
                                    {
                                        ID = e.ID,
                                        PowerUnitName = e.PowerUnitName,
                                        FactoryID = e.FactoryID,
                                        Active = e.Active,
                                        IsDeleted = e.IsDeleted,
                                        CreatedUserID = e.CreatedUserID,
                                        InsertedAt = e.InsertedAt,
                                        LastUpdatedUserID = e.LastUpdatedUserID,
                                        UpdatedAt = e.UpdatedAt
                                    }).OrderBy(t => t.ID).ToList();

                    #endregion

                    #region Thông số

                    masterDataModel.AlarmSettings = _unitOfWork.ElogbookRepository<AlarmSetting>().GetQuery(x => x.UpdatedAt > syncDate)
                                .Select(
                                    e => new MobileAlarmSettingsInfo()
                                    {
                                        ID = e.ID,
                                        SettingName = e.SettingName,
                                        SettingType = e.SettingType,
                                        ColorCode = e.ColorCode,
                                        IsDeleted = e.IsDeleted,
                                        CreatedUserID = e.CreatedUserID,
                                        InsertedAt = e.InsertedAt,
                                        LastUpdatedUserID = e.LastUpdatedUserID,
                                        UpdatedAt = e.UpdatedAt
                                    }).OrderBy(t => t.ID).ToList();

                    #endregion

                    #region Nhóm thông số

                    masterDataModel.ParameterGroup = _unitOfWork.ElogbookRepository<ParameterGroup>()
                                .GetQuery(x => x.UpdatedAt > syncDate)
                                .Select(
                                    e => new MobileParameterGroupInfo()
                                    {
                                        ID = e.ID,
                                        ParameterGroupName = e.ParameterGroupName,
                                        FactoryID = e.FactoryID,
                                        OperationPositionID = e.OperationPositionID,
                                        CycleID = e.CycleID,
                                        Position = e.Position,
                                        Status = e.Status,
                                        IsDeleted = e.IsDeleted,
                                        CreatedUserID = e.CreatedUserID,
                                        InsertedAt = e.InsertedAt,
                                        LastUpdatedUserID = e.LastUpdatedUserID,
                                        UpdatedAt = e.UpdatedAt
                                    }).OrderBy(t => t.ID).ToList();

                    #endregion

                    #region Kíp vận hành

                    masterDataModel.OperationCrew = _unitOfWork.ElogbookRepository<OperationCrew>()
                                .GetQuery(x => x.UpdatedAt > syncDate)
                                .Select(
                                    e => new MobileOperationCrewInfo()
                                    {
                                        ID = e.ID,
                                        OperationCrewName = e.OperationCrewName,
                                        FactoryID = e.FactoryID,
                                        Active = e.Active,
                                        IsDeleted = e.IsDeleted,
                                        CreatedUserID = e.CreatedUserID,
                                        InsertedAt = e.InsertedAt,
                                        LastUpdatedUserID = e.LastUpdatedUserID,
                                        UpdatedAt = e.UpdatedAt
                                    }).OrderBy(t => t.ID).ToList();

                    #endregion

                    #region Phiếu công tác

                    masterDataModel.WorkTicket = _unitOfWork.ElogbookRepository<WorkTicket>()
                                .GetQuery(x => x.UpdatedAt > syncDate)
                                .Select(
                                    e => new MobileWorkTicketInfo()
                                    {
                                        ID = e.ID,
                                        ScreenType = e.ScreenType,
                                        WorkTicketNumber = e.WorkTicketNumber,
                                        WorkTicketStatus = new ValueLabelModel { Value = e.TicketStatus.ID, Label = e.TicketStatus.StatusName },
                                        IsDeleted = e.IsDeleted,
                                        CreatedUserID = e.CreatedUserID,
                                        InsertedAt = e.InsertedAt,
                                        LastUpdatedUserID = e.LastUpdatedUserID,
                                        UpdatedAt = e.UpdatedAt,
                                        FactoryID = e.FactoryID,
                                    }).OrderBy(t => t.ID).ToList();

                    if (masterDataModel.WorkTicket != null && masterDataModel.WorkTicket.Count > 0)
                    {
                        foreach (var item in masterDataModel.WorkTicket)
                        {
                            var leaderShipEntity = _unitOfWork.ElogbookRepository<LeadershipUser>().GetQuery(x => x.WorkTicketID == item.ID
                                                                                                                    && !x.IsDeleted
                                                                                                                    && x.Roles == (int)LeadershipUserRoles.Commander).FirstOrDefault();
                            if (leaderShipEntity != null)
                                item.LeaderName = leaderShipEntity.FullName;
                        }
                    }

                    #endregion

                    #region Thông tin version app

                    masterDataModel.AppVersionInfo = _unitOfWork.ElogbookRepository<AppVersion>()
                                .GetQuery()
                                .Select(
                                    e => new AppVersionModel()
                                    {
                                        ID = e.ID,
                                        Version = e.Version,
                                        ForceUpdate = e.ForceUpdate,
                                        IsDeleted = e.IsDeleted,
                                        InsertedAt = e.InsertedAt,
                                        UpdatedAt = e.UpdatedAt
                                    }).OrderBy(t => t.ID).FirstOrDefault();

                    #endregion

                    LogCustom.LogMessage(TrackingLogLevel.DEBUG, $"{GetType()}, GetMasterData(), Update sync info");
                    masterDataModel.ConfigurationInfo = new MobileConfigurationInfo
                    {
                        ServerPath = AppSettings.SERVER_DIR,
                        SyncOfflineTime = AppSettings.SYNC_OFFLINE_TIME,
                        DeleteOffline = AppSettings.DELETE_OFFLINE,
                        IsExtendTime = AppSettings.NKVH_IS_EXTENDTIME,
                        IsShowShiftReceiving = AppSettings.IS_SHOW_SHIFT_RECEIVING,
                        IsShowNotification = AppSettings.IS_SHOW_NOTIFICATION,
                    };

                    //Kiểm tra tồn tại dữ liệu trước khi thêm mới/cập nhật
                    var existRecord = _unitOfWork.ElogbookRepository<TrackingSyncMobileData>().GetQuery(x => !x.IsDeleted && x.DeviceID.Equals(deviceID, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    if (existRecord != null)
                    {
                        existRecord.SyncDate = DateTime.Now;
                        _unitOfWork.ElogbookRepository<TrackingSyncMobileData>().Update(existRecord);
                    }
                    else
                    {
                        var newEntity = new TrackingSyncMobileData
                        {
                            SyncDataModule = System.Data.SqlTypes.SqlDateTime.MinValue.Value,
                            SyncDate = DateTime.Now,
                            DeviceID = deviceID,
                        };
                        _unitOfWork.ElogbookRepository<TrackingSyncMobileData>().Add(newEntity);
                    }
                }

                // Lấy thông tin NKVH, TSVH theo Nhà máy, Cương vị
                if (factoryID > 0)
                {
                    #region Thông tin phân bố nhân sự cho NKVH

                    // Vị trí phân công nhân sự
                    masterDataModel.AssignmentSlot = _unitOfWork.ElogbookRepository<AssignmentSlot>()
                                .GetQuery(x => x.FactoryID == factoryID
                                && x.UpdatedAt > syncDate)
                                .Select(
                                    e => new MobileAssignmentSlotInfo()
                                    {
                                        ID = e.ID,
                                        FactoryID = e.FactoryID,
                                        OperationPositionID = e.OperationPositionID,
                                        WorkDate = e.WorkDate,
                                        OperationShiftID = e.OperationShiftID,
                                        Active = e.Active,
                                        IsDeleted = e.IsDeleted,
                                        CreatedUserID = e.CreatedUserID,
                                        InsertedAt = e.InsertedAt,
                                        LastUpdatedUserID = e.LastUpdatedUserID,
                                        UpdatedAt = e.UpdatedAt
                                    }).OrderBy(t => t.ID).ToList();

                    List<int> listIdAssignmentSlot = new List<int>();
                    if (masterDataModel.AssignmentSlot != null && masterDataModel.AssignmentSlot.Count > 0)
                    {
                        listIdAssignmentSlot = masterDataModel.AssignmentSlot.Select(x => x.ID).ToList();

                        if (listIdAssignmentSlot != null && listIdAssignmentSlot.Count > 0)
                        {
                            // Thông tin nhân sự được phân công
                            masterDataModel.AssignmentEmployee = _unitOfWork.ElogbookRepository<AssignmentEmployee>()
                                .GetQuery(x => listIdAssignmentSlot.Contains(x.AssignmentSlotID))
                                .Select(
                                    e => new MobileAssignmentEmployeeInfo()
                                    {
                                        ID = e.ID,
                                        AssignmentSlotID = e.AssignmentSlotID,
                                        UserID = e.UserID,
                                        Active = e.Active,
                                        IsDeleted = e.IsDeleted,
                                        CreatedUserID = e.CreatedUserID,
                                        InsertedAt = e.InsertedAt,
                                        LastUpdatedUserID = e.LastUpdatedUserID,
                                        UpdatedAt = e.UpdatedAt
                                    }).OrderBy(t => t.ID).ToList();
                        }
                    }

                    #endregion

                    #region Nhật ký vận hành

                    LogCustom.LogMessage(TrackingLogLevel.DEBUG, $"{GetType()}, GetMasterData(), Nhật ký vận hành");
                    // Nhật ký vận hành
                    masterDataModel.OperationDiary = _unitOfWork.ElogbookRepository<OperationDiary>()
                                .GetQuery(x => x.FactoryID == factoryID
                                && x.OperationPositionID == operationPositionID
                                && x.SyncedDate > syncDataModule
                                && DbFunctions.TruncateTime(x.InsertedAt) > dateTimeSync.Date)
                                .Select(
                                    e => new MobileOperationDiaryInfo()
                                    {
                                        ID = e.ID,
                                        AppID = e.AppID,
                                        SyncDataKey = e.SyncDataKey,
                                        FactoryID = e.FactoryID,
                                        CreatedDate = e.CreatedDate,
                                        OperationPositionID = e.OperationPositionID,
                                        OperationShiftID = e.OperationShiftID,
                                        OperationCrewID = e.OperationCrewID,
                                        Comments = e.Comments,
                                        IsReceived = e.IsReceived,
                                        IsDelivered = e.IsDelivered,
                                        IsCanEdit = e.IsCanEdit,
                                        TimeDelivery = e.TimeDelivery,
                                        IsDeleted = e.IsDeleted,
                                        CreatedUserID = e.CreatedUserID,
                                        InsertedAt = e.InsertedAt,
                                        LastUpdatedUserID = e.LastUpdatedUserID,
                                        UpdatedAt = e.UpdatedAt,
                                        Position = e.OperationShift != null ? e.OperationShift.Position : 0,
                                        ShiftReceiving = e.ShiftReceiving,
                                    }).OrderBy(t => t.ID).ToList();

                    List<int> listIdOperationDiary = new List<int>();
                    if (masterDataModel.OperationDiary != null && masterDataModel.OperationDiary.Count > 0)
                        listIdOperationDiary = masterDataModel.OperationDiary.Select(x => x.ID).ToList();

                    if (listIdOperationDiary != null && listIdOperationDiary.Count > 0)
                    {
                        // Cương vị trong ca
                        masterDataModel.OperationDiaryPositionShift = _unitOfWork.ElogbookRepository<OperationDiaryPositionShift>()
                            .GetQuery(x => x.SyncedDate > syncDataModule && listIdOperationDiary.Contains(x.OperationDiaryID))
                            .Select(
                                e => new MobileOperationDiaryPositionShiftInfo()
                                {
                                    AppID = e.AppID,
                                    SyncDataKey = e.SyncDataKey,
                                    ID = e.ID,
                                    OperationDiaryID = e.OperationDiaryID,
                                    PositionInShiftID = e.PositionInShiftID,
                                    Label = e.Label,
                                    AssignedUserID = e.AssignedUserID ?? 0,
                                    IsDefault = e.IsDefault,
                                    Position = e.Position,
                                    IsDeleted = e.IsDeleted,
                                    CreatedUserID = e.CreatedUserID,
                                    InsertedAt = e.InsertedAt,
                                    LastUpdatedUserID = e.LastUpdatedUserID,
                                    UpdatedAt = e.UpdatedAt
                                }).OrderBy(t => t.ID).ToList();

                        // 1.Tình hình vận hành
                        masterDataModel.OperationSituation = _unitOfWork.ElogbookRepository<OperationSituation>()
                            .GetQuery(x => x.SyncedDate > syncDataModule && listIdOperationDiary.Contains(x.OperationDiaryID))
                                        .Select(
                                            e => new MobileOperationSituationInfo()
                                            {
                                                AppID = e.AppID,
                                                SyncDataKey = e.SyncDataKey,
                                                ID = e.ID,
                                                OperationDiaryID = e.OperationDiaryID,
                                                Hour = e.Hour,
                                                AssignedFromID = e.AssignedFromID,
                                                AssignedFromName = e.AssignedFromName,
                                                AssignedToID = e.AssignedToID,
                                                AssignedToName = e.AssignedToName,
                                                Description = e.Description,
                                                Attachments = e.Attachments,
                                                IsDeleted = e.IsDeleted,
                                                CreatedUserID = e.CreatedUserID,
                                                InsertedAt = e.InsertedAt,
                                                LastUpdatedUserID = e.LastUpdatedUserID,
                                                UpdatedAt = e.UpdatedAt
                                            }).OrderBy(t => t.ID).ToList();

                        // Công tác đang thực hiện
                        //masterDataModel.ActiveWork = _unitOfWork.ElogbookRepository<ActiveWork>()
                        //    .GetQuery(x => x.SyncedDate > syncDataModule && listIdOperationDiary.Contains(x.OperationDiaryID))
                        //                .Select(
                        //                    e => new MobileActiveWorkInfo()
                        //                    {
                        //                        AppID = e.AppID,
                        //                        SyncDataKey = e.SyncDataKey,
                        //                        ID = e.ID,
                        //                        OperationDiaryID = e.OperationDiaryID,
                        //                        StartHour = e.StartHour,
                        //                        EndHour = e.EndHour,
                        //                        Description = e.Description,
                        //                        Attachments = e.Attachments,
                        //                        IsDeleted = e.IsDeleted,
                        //                        CreatedUserID = e.CreatedUserID,
                        //                        InsertedAt = e.InsertedAt,
                        //                        LastUpdatedUserID = e.LastUpdatedUserID,
                        //                        UpdatedAt = e.UpdatedAt
                        //                    }).OrderBy(t => t.ID).ToList();

                        // 2.Bất thường sự cố
                        masterDataModel.AbnormalIncident = _unitOfWork.ElogbookRepository<AbnormalIncident>()
                            .GetQuery(x => x.SyncedDate > syncDataModule && listIdOperationDiary.Contains(x.OperationDiaryID))
                                        .Select(
                                            e => new MobileAbnormalIncidentInfo()
                                            {
                                                AppID = e.AppID,
                                                SyncDataKey = e.SyncDataKey,
                                                ID = e.ID,
                                                OperationDiaryID = e.OperationDiaryID,
                                                Description = e.Description,
                                                Attachments = e.Attachments,
                                                IsDeleted = e.IsDeleted,
                                                CreatedUserID = e.CreatedUserID,
                                                InsertedAt = e.InsertedAt,
                                                LastUpdatedUserID = e.LastUpdatedUserID,
                                                UpdatedAt = e.UpdatedAt,
                                            }).OrderBy(t => t.ID).ToList();

                        // 3.Phương thức giao ca
                        masterDataModel.DeliveryShiftMethod = _unitOfWork.ElogbookRepository<DeliveryShiftMethod>()
                            .GetQuery(x => x.SyncedDate > syncDataModule && listIdOperationDiary.Contains(x.OperationDiaryID))
                                        .Select(
                                            e => new MobileDeliveryShiftMethodInfo()
                                            {
                                                AppID = e.AppID,
                                                SyncDataKey = e.SyncDataKey,
                                                ID = e.ID,
                                                OperationDiaryID = e.OperationDiaryID,
                                                Description = e.Description,
                                                Attachments = e.Attachments,
                                                IsDeleted = e.IsDeleted,
                                                CreatedUserID = e.CreatedUserID,
                                                InsertedAt = e.InsertedAt,
                                                LastUpdatedUserID = e.LastUpdatedUserID,
                                                UpdatedAt = e.UpdatedAt
                                            }).OrderBy(t => t.ID).ToList();

                        // 4.Tồn tại trong ca
                        masterDataModel.ExistInShift = _unitOfWork.ElogbookRepository<ExistInShift>().GetQuery(x => (x.OperationDiary.FactoryID == factoryID && x.OperationDiary.OperationPositionID == operationPositionID &&
                                                                                                (x.StatusID != (int)IssueOfShiftStatus.DONE
                                || (DbFunctions.TruncateTime(x.InsertedAt) > dateTimeSync.Date && x.SyncedDate > syncDataModule))))
                                        .Select(
                                            e => new MobileExistInShiftInfo()
                                            {
                                                AppID = e.AppID,
                                                ID = e.ID,
                                                OperationDiaryID = e.OperationDiaryID,
                                                RegistrationTicketID = e.RegistrationTicketID,
                                                RegistrationTicketName = e.RegistrationTicketName,
                                                TeamLeader = e.TeamLeader,
                                                Area = e.Area,
                                                Content = e.Content,
                                                Attachments = e.Attachments,
                                                StatusID = e.StatusID,
                                                IsDeleteIssueOfShift = e.IsDeleteIssueOfShift,
                                                CompletedDateTime = e.CompletedDateTime,
                                                SyncDataKey = e.SyncDataKey,
                                                IsDeleted = e.IsDeleted,
                                                CreatedUserID = e.CreatedUserID,
                                                InsertedAt = e.InsertedAt,
                                                LastUpdatedUserID = e.LastUpdatedUserID,
                                                UpdatedAt = e.UpdatedAt,
                                                FactoryID = e.OperationDiary.FactoryID,
                                                OperationPositionID = e.OperationDiary.OperationPositionID,
                                            }).OrderBy(t => t.ID).ToList();

                        List<int> listIdExistInShift = new List<int>();
                        if (masterDataModel.ExistInShift != null && masterDataModel.ExistInShift.Count > 0)
                            listIdExistInShift = masterDataModel.ExistInShift.Select(x => x.ID).ToList();

                        // 5.Giao/nhận ca
                        masterDataModel.DeliveryReceiveShift = _unitOfWork.ElogbookRepository<DeliveryReceiveShift>()
                            .GetQuery(x => x.SyncedDate > syncDataModule && listIdOperationDiary.Contains(x.OperationDiaryID))
                                        .Select(
                                            e => new MobileDeliveryReceiveShiftInfo()
                                            {
                                                AppID = e.AppID,
                                                SyncDataKey = e.SyncDataKey,
                                                ID = e.ID,
                                                OperationDiaryID = e.OperationDiaryID,
                                                OperationDiaryPositionShiftID = e.OperationDiaryPositionShiftID,
                                                SupervisorID = e.SupervisorID,
                                                IsDeliveryShift = e.IsDeliveryShift,
                                                IsDeleted = e.IsDeleted,
                                                CreatedUserID = e.CreatedUserID,
                                                InsertedAt = e.InsertedAt,
                                                LastUpdatedUserID = e.LastUpdatedUserID,
                                                UpdatedAt = e.UpdatedAt
                                            }).OrderBy(t => t.ID).ToList();

                        if (listIdExistInShift != null && listIdExistInShift.Count > 0)
                        {
                            // 6.Tồn tại trong ca
                            masterDataModel.IssueOfShiftDetail = _unitOfWork.ElogbookRepository<IssueOfShiftDetail>()
                                    .GetQuery(x => listIdExistInShift.Contains((int)x.ExistInShiftID))
                                    .Select(
                                        e => new MobileIssueOfShiftDetailInfo()
                                        {
                                            ID = e.ID,
                                            Content = e.Content,
                                            Attachments = e.Attachments,
                                            IsComplete = e.IsComplete,
                                            ExistInShiftID = e.ExistInShiftID,
                                            IsDeleted = e.IsDeleted,
                                            CreatedUserID = e.CreatedUserID,
                                            InsertedAt = e.InsertedAt,
                                            LastUpdatedUserID = e.LastUpdatedUserID,
                                            UpdatedAt = e.UpdatedAt
                                        }).OrderBy(t => t.ID).ToList();
                        }
                        LogCustom.LogMessage(TrackingLogLevel.DEBUG, $"{GetType()}, GetMasterData(), Hoàn thành NKVH");
                    }

                    #endregion

                    #region Thông số vận hành

                    masterDataModel.Parameter = _unitOfWork.ElogbookRepository<Parameter>()
                                    .GetQuery(x => x.FactoryID == factoryID
                                    && x.OperationPositionID == operationPositionID && x.SyncedDate > syncDataModule)
                                    .Select(
                                        e => new MobileParameterInfo()
                                        {
                                            ID = e.ID,
                                            IsGroup = e.IsGroup,
                                            FactoryID = e.FactoryID,
                                            OperationPositionID = e.OperationPositionID,
                                            CycleID = e.CycleID,
                                            ParameterGroupID = e.ParameterGroupID,
                                            RFIDCode = e.RFIDCode,
                                            QRCode = e.QRCode,
                                            ParameterName = e.ParameterName,
                                            KKSCode = e.KKSCode,
                                            UnitMeasure = e.UnitMeasure,
                                            Setting = e.Setting,
                                            FromRange = e.FromRange,
                                            ToRange = e.ToRange,
                                            IsNumberType = e.IsNumberType,
                                            QLKTCode = e.QLKTCode,
                                            Status = e.Status,
                                            Position = e.Position,
                                            IsDeleted = e.IsDeleted,
                                            CreatedUserID = e.CreatedUserID,
                                            InsertedAt = e.InsertedAt,
                                            LastUpdatedUserID = e.LastUpdatedUserID,
                                            UpdatedAt = e.UpdatedAt
                                        }).OrderBy(t => t.ID).ToList();

                    List<int> listParameterID = new List<int>();
                    if (masterDataModel.Parameter != null && masterDataModel.Parameter.Count > 0)
                        listParameterID = masterDataModel.Parameter.Select(x => x.ID).ToList();

                    masterDataModel.AlertDetail = _unitOfWork.ElogbookRepository<AlertDetail>().GetQuery(x => x.SyncedDate > syncDataModule && listParameterID.Contains(x.ParameterID != null ? (int)x.ParameterID : 0))
                                    .Select(
                                        e => new MobileAlertDetailInfo()
                                        {
                                            ID = e.ID,
                                            ParameterID = e.ParameterID,
                                            Alert = e.Alert,
                                            Operator = e.Operator,
                                            Value = e.Value,
                                            Color = e.Color,
                                            IsDeleted = e.IsDeleted,
                                            CreatedUserID = e.CreatedUserID,
                                            InsertedAt = e.InsertedAt,
                                            LastUpdatedUserID = e.LastUpdatedUserID,
                                            UpdatedAt = e.UpdatedAt
                                        }).OrderBy(t => t.ID).ToList();

                    // Các mốc thời gian ghi thông số vận hành
                    masterDataModel.OperationTimeLine = _unitOfWork.ElogbookRepository<OperationTimeLine>()
                                    .GetQuery(x => x.FactoryID == factoryID && x.OperationPositionID == operationPositionID
                                    && x.SyncedDate > syncDataModule && DbFunctions.TruncateTime(x.InsertedAt) > dateTimeSync.Date)
                                    .Select(
                                        e => new MobileOperationTimeLineInfo()
                                        {
                                            ID = e.ID,
                                            AppID = e.AppID,
                                            TimeLineSyncDataKey = e.TimeLineSyncDataKey,
                                            TimeLineName = e.TimeLineName,
                                            FactoryID = e.FactoryID,
                                            OperationPositionID = e.OperationPositionID,
                                            CycleID = e.CycleID,
                                            TotalParametersDefault = e.TotalParametersDefault,
                                            TotalParametersChecked = e.TotalParametersChecked,
                                            TotalParametersSynchronized = e.TotalParametersSynchronized,
                                            IsDeleted = e.IsDeleted,
                                            CreatedUserID = e.CreatedUserID,
                                            InsertedAt = e.InsertedAt,
                                            LastUpdatedUserID = e.LastUpdatedUserID,
                                            UpdatedAt = e.UpdatedAt
                                        }).OrderBy(t => t.ID).ToList();

                    List<int> listID = new List<int>();
                    if (masterDataModel.OperationTimeLine != null && masterDataModel.OperationTimeLine.Count > 0)
                        listID = masterDataModel.OperationTimeLine.Select(x => x.ID).ToList();

                    //Dữ liệu ghi thông số vận hành
                    if (listID.Count > 0)
                    {
                        masterDataModel.OperationParameters = _unitOfWork.ElogbookRepository<OperationParameters>()
                            .GetQuery(x => x.FactoryID == factoryID && x.OperationPositionID == operationPositionID && x.SyncedDate > syncDataModule && listID.Contains(x.OperationTimeLineID != null ? (int)x.OperationTimeLineID : 0))
                            .Select(
                                e => new MobileOperationParametersInfo()
                                {
                                    ID = e.ID,
                                    AppID = e.AppID,
                                    SyncDataKey = e.SyncDataKey,
                                    ParameterName = e.ParameterName,
                                    ParameterID = e.ParameterID,
                                    ParameterGroupID = e.ParameterGroupID,
                                    OperationTimeLineID = e.OperationTimeLineID,
                                    FactoryID = e.FactoryID,
                                    OperationPositionID = e.OperationPositionID,
                                    CycleID = e.CycleID,
                                    CurrentValue = e.CurrentValue,
                                    KKSCode = e.KKSCode,
                                    UnitMeasure = e.UnitMeasure,
                                    Setting = e.Setting,
                                    FromRange = e.FromRange,
                                    ToRange = e.ToRange,
                                    IsNumberType = e.IsNumberType,
                                    PreviousValue = e.PreviousValue,
                                    ErrorCode = e.ErrorCode,
                                    IsChecked = e.IsChecked,
                                    IsDeleted = e.IsDeleted,
                                    CreatedUserID = e.CreatedUserID,
                                    InsertedAt = e.InsertedAt,
                                    LastUpdatedUserID = e.LastUpdatedUserID,
                                    UpdatedAt = e.UpdatedAt,
                                    CheckedDate = e.CheckedDate,
                                }).OrderBy(t => t.ID).ToList();
                    }

                    #endregion

                    #region Danh sách tệp đính kèm

                    masterDataModel.Attachments = _unitOfWork.ElogbookRepository<Attachment>().GetQuery(x => x.SyncedDate > syncDataModule)
                                        .Select(
                                            e => new MobileAttachmentInfo()
                                            {
                                                ID = e.ID,
                                                FilePath = e.FilePath,
                                                PathThumbnail = e.PathThumbnail,
                                                FileName = e.FileName,
                                                Description = e.Description,
                                                FileType = e.FileType,
                                                ScreenName = e.ScreenName,
                                                IsDeleted = e.IsDeleted,
                                                CreatedUserID = e.CreatedUserID,
                                                InsertedAt = e.InsertedAt,
                                                LastUpdatedUserID = e.LastUpdatedUserID,
                                                UpdatedAt = e.UpdatedAt
                                            }).OrderBy(t => t.ID).ToList();

                    #endregion

                    #region AM

                    // Danh mục - Trạng thái dùng chung
                    masterDataModel.MobileAmStatus = _unitOfWork.ElogbookRepository<AmStatus>().GetQuery(x => x.UpdatedAt > syncDate)
                       .Select(
                           e => new MobileAmStatus()
                           {
                               ID = e.ID,
                               StatusName = e.StatusName,
                               IsDeleted = e.IsDeleted,
                               CreatedUserID = e.CreatedUserID,
                               InsertedAt = e.InsertedAt,
                               LastUpdatedUserID = e.LastUpdatedUserID,
                               UpdatedAt = e.UpdatedAt,
                           }).ToList();

                    // Danh mục - Thiết bị AM
                    masterDataModel.MobileAmDevices = _unitOfWork.ElogbookRepository<AmDevice>().GetQuery(x => x.UpdatedAt > syncDate)
                       .Select(
                           e => new MobileAmDevice()
                           {
                               ID = e.ID,
                               AmLocationID = e.AmLocationID,
                               AssetParent = e.AssetParent,
                               AssetDesc = e.AssetDesc,
                               AssetCode = e.AssetCode,
                               AmDeviceStatusID = e.AmDeviceStatusID,
                               Active = e.Active,
                               AmDeviceTypeID = e.AmDeviceTypeID,
                               AmManufacturerID = e.AmManufacturerID,
                               AmSupplierID = e.AmSupplierID,
                               AmCountryManufactureID = e.AmCountryManufactureID,
                               OrganizationID = e.OrganizationID,
                               SerialNum = e.SerialNum,
                               AssetType = e.AssetType,
                               Meter = e.Meter,
                               Kss = e.Kss,
                               Khvh = e.Khvh,
                               Note = e.Note,
                               Description = e.Description,
                               Drawing = e.Drawing,
                               AmDeviceCategoryID = e.AmDeviceCategoryID,
                               Images = e.Images,
                               InstallDate = e.InstallDate,
                               OperatingDate = e.OperatingDate,
                               IsDeleted = e.IsDeleted,
                               CreatedUserID = e.CreatedUserID,
                               InsertedAt = e.InsertedAt,
                               LastUpdatedUserID = e.LastUpdatedUserID,
                               UpdatedAt = e.UpdatedAt,
                               PMISId = e.PMISId,
                           }).ToList();

                    // Danh mục - Loại thiết bị
                    masterDataModel.MobileAmDeviceTypes = _unitOfWork.ElogbookRepository<AmDeviceType>().GetQuery(x => x.UpdatedAt > syncDate)
                       .Select(
                           e => new MobileAmDeviceType()
                           {
                               ID = e.ID,
                               TypeCode = e.TypeCode,
                               TypeDesc = e.TypeDesc,
                               Active = e.Active,
                               IsDeleted = e.IsDeleted,
                               CreatedUserID = e.CreatedUserID,
                               InsertedAt = e.InsertedAt,
                               LastUpdatedUserID = e.LastUpdatedUserID,
                               UpdatedAt = e.UpdatedAt,
                               PMISId = e.PMISId,
                               PMISType = e.PMISType,
                           }).ToList();

                    // Danh mục - Địa điểm/Khu vực
                    masterDataModel.MobileAmLocations = _unitOfWork.ElogbookRepository<AmLocation>().GetQuery(x => x.UpdatedAt > syncDate)
                       .Select(
                           e => new MobileAmLocation()
                           {
                               ID = e.ID,
                               OrganizationID = e.OrganizationID,
                               SiteParent = e.SiteParent,
                               SiteDesc = e.SiteDesc,
                               SiteCode = e.SiteCode,
                               Active = e.Active,
                               IsDeleted = e.IsDeleted,
                               CreatedUserID = e.CreatedUserID,
                               InsertedAt = e.InsertedAt,
                               LastUpdatedUserID = e.LastUpdatedUserID,
                               UpdatedAt = e.UpdatedAt,
                               PMISId = e.PMISId,
                           }).ToList();

                    // Danh sách phân loại thiết bị AM
                    masterDataModel.MobileAmCategories = _unitOfWork.ElogbookRepository<AmDeviceCategory>().GetQuery(x => x.UpdatedAt > syncDate)
                       .Select(
                           e => new MobileAmCategory()
                           {
                               ID = e.ID,
                               OrgID = e.OrgID,
                               CategoryParent = e.CategoryParent,
                               CategoryCode = e.CategoryCode,
                               CategoryName = e.CategoryName,
                               IsDeleted = e.IsDeleted,
                               CreatedUserID = e.CreatedUserID,
                               InsertedAt = e.InsertedAt,
                               LastUpdatedUserID = e.LastUpdatedUserID,
                               UpdatedAt = e.UpdatedAt,
                               PMISId = e.PMISId,
                           }).ToList();

                    // Danh sách Danh mục Nước sản xuất
                    masterDataModel.MobileAmCountryManufactures = _unitOfWork.ElogbookRepository<AmCountryManufacture>().GetQuery(x => x.UpdatedAt > syncDate)
                       .Select(
                           e => new MobileAmCountryManufacture()
                           {
                               ID = e.ID,
                               CountryCode = e.CountryCode,
                               CountryName = e.CountryName,
                               IsDeleted = e.IsDeleted,
                               CreatedUserID = e.CreatedUserID,
                               InsertedAt = e.InsertedAt,
                               LastUpdatedUserID = e.LastUpdatedUserID,
                               UpdatedAt = e.UpdatedAt,
                               PMISId = e.PMISId,
                           }).ToList();

                    // Danh sách status của thiết bị
                    masterDataModel.MobileAmDeviceStatus = _unitOfWork.ElogbookRepository<AmDeviceStatus>().GetQuery(x => x.UpdatedAt > syncDate)
                       .Select(
                           e => new MobileAmDeviceStatus()
                           {
                               ID = e.ID,
                               StatusDesc = e.StatusDesc,
                               IsDeleted = e.IsDeleted,
                               CreatedUserID = e.CreatedUserID,
                               InsertedAt = e.InsertedAt,
                               LastUpdatedUserID = e.LastUpdatedUserID,
                               UpdatedAt = e.UpdatedAt,
                           }).ToList();

                    // Danh mục Nhà chế tạo AM
                    masterDataModel.MobileAmManufacturers = _unitOfWork.ElogbookRepository<AmManufacturer>().GetQuery(x => x.UpdatedAt > syncDate)
                       .Select(
                           e => new MobileAmManufacturer()
                           {
                               ID = e.ID,
                               ManufacturerCode = e.ManufacturerCode,
                               ManufacturerName = e.ManufacturerName,
                               IsDeleted = e.IsDeleted,
                               CreatedUserID = e.CreatedUserID,
                               InsertedAt = e.InsertedAt,
                               LastUpdatedUserID = e.LastUpdatedUserID,
                               UpdatedAt = e.UpdatedAt,
                               PMISId = e.PMISId,
                           }).ToList();

                    // Danh mục Nhà cung cấp
                    masterDataModel.MobileAmSuppliers = _unitOfWork.ElogbookRepository<AmSupplier>().GetQuery(x => x.UpdatedAt > syncDate)
                       .Select(
                           e => new MobileAmSupplier()
                           {
                               ID = e.ID,
                               SupplierCode = e.SupplierCode,
                               SupplierName = e.SupplierName,
                               IsDeleted = e.IsDeleted,
                               CreatedUserID = e.CreatedUserID,
                               InsertedAt = e.InsertedAt,
                               LastUpdatedUserID = e.LastUpdatedUserID,
                               UpdatedAt = e.UpdatedAt,
                               PMISId = e.PMISId,
                           }).ToList();

                    // Danh mục - Trạng thái bất thường thiết bị
                    masterDataModel.MobileAmDeviceAbnormalStatus = _unitOfWork.ElogbookRepository<AmDeviceAbnormalStatus>().GetQuery(x => x.UpdatedAt > syncDate)
                       .Select(
                           e => new MobileAmDeviceAbnormalStatus()
                           {
                               ID = e.ID,
                               Name = e.Name,
                               IsDeleted = e.IsDeleted,
                               CreatedUserID = e.CreatedUserID,
                               InsertedAt = e.InsertedAt,
                               LastUpdatedUserID = e.LastUpdatedUserID,
                               UpdatedAt = e.UpdatedAt,
                           }).ToList();

                    // Danh mục - Mức độ khẩn cấp
                    masterDataModel.MobileAmEmergencys = _unitOfWork.ElogbookRepository<AmEmergency>().GetQuery(x => x.FactoryID == factoryID
                                && x.UpdatedAt > syncDate)
                       .Select(
                           e => new MobileAmEmergency()
                           {
                               ID = e.ID,
                               FactoryID = e.FactoryID,
                               Name = e.Name,
                               Active = e.Active,
                               IsDeleted = e.IsDeleted,
                               CreatedUserID = e.CreatedUserID,
                               InsertedAt = e.InsertedAt,
                               LastUpdatedUserID = e.LastUpdatedUserID,
                               UpdatedAt = e.UpdatedAt,
                           }).ToList();

                    // Danh mục - Nguyên nhân dừng thực hiện (Bảo trì tự quản hoặc thực hiện công viêc)
                    masterDataModel.MobileAmReasonForStops = _unitOfWork.ElogbookRepository<AmReasonForStop>().GetQuery(x => x.FactoryID == factoryID
                                && x.UpdatedAt > syncDate)
                       .Select(
                           e => new MobileAmReasonForStop()
                           {
                               ID = e.ID,
                               FactoryID = e.FactoryID,
                               Active = e.Active,
                               ReasonDesc = e.ReasonDesc,
                               ReasonType = e.ReasonType,
                               IsDeleted = e.IsDeleted,
                               CreatedUserID = e.CreatedUserID,
                               InsertedAt = e.InsertedAt,
                               LastUpdatedUserID = e.LastUpdatedUserID,
                               UpdatedAt = e.UpdatedAt,
                           }).ToList();

                    // Danh mục - Nguyên nhân tạm dừng công việc
                    masterDataModel.MobileAmReasonForPendings = _unitOfWork.ElogbookRepository<AmReasonForPending>().GetQuery(x => x.FactoryID == factoryID
                                && x.UpdatedAt > syncDate)
                       .Select(
                           e => new MobileAmReasonForPending()
                           {
                               ID = e.ID,
                               FactoryID = e.FactoryID,
                               ReasonDesc = e.ReasonDesc,
                               Active = e.Active,
                               IsDeleted = e.IsDeleted,
                               CreatedUserID = e.CreatedUserID,
                               InsertedAt = e.InsertedAt,
                               LastUpdatedUserID = e.LastUpdatedUserID,
                               UpdatedAt = e.UpdatedAt,
                           }).ToList();

                    // Danh muc - Loại công việc
                    masterDataModel.MobileAmWorkTypes = _unitOfWork.ElogbookRepository<AmWorkType>().GetQuery(x => x.FactoryID == factoryID
                                && x.UpdatedAt > syncDate)
                       .Select(
                           e => new MobileAmWorkType()
                           {
                               ID = e.ID,
                               FactoryID = e.FactoryID,
                               Active = e.Active,
                               TypeDesc = e.TypeDesc,
                               IsDeleted = e.IsDeleted,
                               CreatedUserID = e.CreatedUserID,
                               InsertedAt = e.InsertedAt,
                               LastUpdatedUserID = e.LastUpdatedUserID,
                               UpdatedAt = e.UpdatedAt,
                           }).ToList();

                    // Danh muc - Công việc
                    masterDataModel.MobileAmWorks = _unitOfWork.ElogbookRepository<AmWork>().GetQuery(x => x.FactoryID == factoryID
                                && x.UpdatedAt > syncDate)
                       .Select(
                           e => new MobileAmWork()
                           {
                               ID = e.ID,
                               FactoryID = e.FactoryID,
                               AmWorkTypeID = e.AmWorkTypeID,
                               Active = e.Active,
                               WorkName = e.WorkName,
                               WorkDesc = e.WorkDesc,
                               IsDeleted = e.IsDeleted,
                               CreatedUserID = e.CreatedUserID,
                               InsertedAt = e.InsertedAt,
                               LastUpdatedUserID = e.LastUpdatedUserID,
                               UpdatedAt = e.UpdatedAt,
                           }).ToList();

                    // Thông tin Bảo trì tự quản AM
                    LogCustom.LogMessage(TrackingLogLevel.DEBUG, $"{GetType()}, GetMasterData(), Bảo trì tự quản - AutonomousMaintenance AM");
                    masterDataModel.MobileAutonomousMaintenances = _unitOfWork.ElogbookRepository<AutonomousMaintenance>()
                        .GetQuery(x => x.FactoryID == factoryID
                        && x.OperationPositionID == operationPositionID
                        && (x.SyncedDate > syncDataModule || DbFunctions.TruncateTime(x.ActualDate) > dateTimeSync.Date)
                        && (DbFunctions.TruncateTime(x.InsertedAt) > dateTimeSync.Date)
                            || DbFunctions.TruncateTime(x.ActualDate) > dateTimeSync.Date)
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
                               CompleteDate = e.CompleteDate,
                               CompleterSignatureContent = e.CompleterSignatureContent,
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

                    List<int> listIdAutonomousMaintenance = new List<int>();
                    if (masterDataModel.MobileAutonomousMaintenances != null && masterDataModel.MobileAutonomousMaintenances.Count > 0)
                        listIdAutonomousMaintenance = masterDataModel.MobileAutonomousMaintenances.Select(x => x.ID).ToList();

                    if (listIdAutonomousMaintenance != null && listIdAutonomousMaintenance.Count > 0)
                    {
                        // Thông tin chi tiết thực hiện công việc AM
                        masterDataModel.MobileAmWorkDetails = _unitOfWork.ElogbookRepository<AmWorkDetail>()
                            .GetQuery(x => x.SyncedDate > syncDataModule && listIdAutonomousMaintenance.Contains(x.AutonomousMaintenanceID))
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
                            .GetQuery(x => x.SyncedDate > syncDataModule && listIdAutonomousMaintenance.Contains(x.AutonomousMaintenanceID))
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
                            .GetQuery(x => x.SyncedDate > syncDataModule && listIdAutonomousMaintenance.Contains(x.AutonomousMaintenanceID))
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
                            .GetQuery(x => x.SyncedDate > syncDataModule && listIdAutonomousMaintenance.Contains(x.AutonomousMaintenanceID))
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

                    #endregion

                    LogCustom.LogMessage(TrackingLogLevel.DEBUG, $"{GetType()}, GetMasterData(), Update sync info");
                    //Kiểm tra tồn tại dữ liệu trước khi thêm mới/cập nhật
                    var existRecord = _unitOfWork.ElogbookRepository<TrackingSyncMobileData>().GetQuery(x => !x.IsDeleted && x.DeviceID.Equals(deviceID, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    if (existRecord != null)
                    {
                        existRecord.SyncDataModule = DateTime.Now;
                        _unitOfWork.ElogbookRepository<TrackingSyncMobileData>().Update(existRecord);
                    }
                    else
                    {
                        var newEntity = new TrackingSyncMobileData
                        {
                            SyncDate = System.Data.SqlTypes.SqlDateTime.MinValue.Value,
                            SyncDataModule = DateTime.Now,
                            DeviceID = deviceID,
                        };
                        _unitOfWork.ElogbookRepository<TrackingSyncMobileData>().Add(newEntity);
                    }
                }

                #region Thêm mới/Cập nhật tracking sync dữ liệu

                _unitOfWork.Complete();

                #endregion

                return masterDataModel;
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.Common.ToString(), ex.ToString());
                throw;
            }
        }

        public List<UserInfo> GetUsersInfo(string fullName)
        {
            return _unitOfWork.ElogbookRepository<User>().GetQuery(x => !x.IsDeleted
                                                                    && x.FullName.Contains(fullName))
                    .OrderByDescending(t => t.InsertedAt)
                    .Select(
                        e => new UserInfo()
                        {
                            ID = e.ID,
                            FullName = e.FullName,
                            ProductionPlant = new ValueLabelModel { Value = e.ProductionPlant.ID, Label = e.ProductionPlant.Name },
                        }).ToList();
        }

        public List<PositionInShiftInfo> GetPositionShiftInfo(int operationPositionID)
        {
            //var positionInShiftInfos = new List<PositionInShiftInfo>();

            //var entities = _unitOfWork.ElogbookRepository<PositionInShift>()
            //    .GetQuery(x => x.ParentID == operationPositionID
            //                && !x.IsDeleted
            //                && x.Active == (int)AccessStatus.Active)
            //    .OrderBy(x => x.Position)
            //    .ToList();

            //var operationPosition = _unitOfWork.ElogbookRepository<OperationPosition>()
            //    .GetQuery(x => x.ID == operationPositionID)
            //    .FirstOrDefault();

            //bool isShiftLeaderCustom =
            //    operationPosition != null &&
            //    operationPosition.JobTitle == (int)JobTitle.ShiftLeader &&
            //    AppSettings.IS_REQUIRED_TEAMLEADER_IN_SHIFTLEADER;

            //foreach (var item in entities)
            //{
            //    // Với cương vị Trưởng ca và có custom thì gán cứng số lượng là 2 để tạo vị trí thực hiện trong ca vận hành gồm 1 Trưởng ca, 1 Trưởng kíp
            //    if (isShiftLeaderCustom && item.OperationPositionID == operationPositionID)
            //    {
            //        item.Quantity = 2;
            //    }

            //    for (int i = 0; i < item.Quantity; i++)
            //    {
            //        string positionInShiftName;
            //        var isRequired = false;

            //        if (isShiftLeaderCustom && item.OperationPositionID == operationPositionID)
            //        {
            //            positionInShiftName = i == 0
            //                ? "Trưởng ca"
            //                : "Trưởng kíp";
            //            isRequired = true;
            //        }
            //        else
            //        {
            //            positionInShiftName = item.Quantity > 1
            //                ? $"{item.OperationPosition.OperationPositionName} ({i + 1})"
            //                : item.OperationPosition.OperationPositionName;
            //        }

            //        positionInShiftInfos.Add(new PositionInShiftInfo()
            //        {
            //            ID = item.ID,
            //            PositionInShiftName = positionInShiftName,
            //            Position = item.Position,
            //            Guid = Guid.NewGuid(),
            //            IsCustomRequired = isRequired,
            //        });
            //    }
            //}

            //return positionInShiftInfos;
            var positionInShiftInfos = new List<PositionInShiftInfo>();
            var entities = _unitOfWork.ElogbookRepository<PositionInShift>().GetQuery(x => x.ParentID == operationPositionID && !x.IsDeleted
                            && x.Active == (int)AccessStatus.Active).OrderBy(x => x.Position).ToList();

            foreach (var item in entities)
            {
                for (int i = 0; i < item.Quantity; i++)
                {
                    positionInShiftInfos.Add(new PositionInShiftInfo()
                    {
                        ID = item.ID,
                        PositionInShiftName = item.Quantity > 1 ? item.OperationPosition.OperationPositionName + $" ({i + 1})" : item.OperationPosition.OperationPositionName,
                        Position = item.Position,
                        Guid = Guid.NewGuid(),
                    });
                }
            }

            return positionInShiftInfos;
        }

        public List<PositionInShiftInfo> GetPositionShiftAndEmployeeInfo(PositionInShiftAndEmployeeModel requestModel)
        {
            var sqlParams = new SqlParameter[]
            {
                new SqlParameter("FactoryID", System.Data.SqlDbType.Int)
                {
                    Value = requestModel.FactoryID,
                },
                new SqlParameter("OperationPositionID", System.Data.SqlDbType.Int)
                {
                    Value = requestModel.OperationPositionID,
                },
                new SqlParameter("OperationShiftID", System.Data.SqlDbType.Int)
                {
                    Value = requestModel.OperationShiftID,
                },
                new SqlParameter("WorkDate", System.Data.SqlDbType.Date)
                {
                    Value = requestModel.WorkDate,
                },
                new SqlParameter("IS_REQUIRED_TEAMLEADER_IN_SHIFTLEADER", System.Data.SqlDbType.Bit)
                {
                    Value = AppSettings.IS_REQUIRED_TEAMLEADER_IN_SHIFTLEADER,
                }
            };

            var positionInShiftInfos = _unitOfWork.DataContext.Database
                .SqlQuery<PositionInShiftInfo>("NKVH_GetPositionShiftAndEmployee @FactoryID, @OperationPositionID, @OperationShiftID, @WorkDate, " +
                "@IS_REQUIRED_TEAMLEADER_IN_SHIFTLEADER", sqlParams).ToList();

            return positionInShiftInfos;
        }

        public List<TicketInforModel> GetTickets(int factoryID, string ticketNumber)
        {
            if (!string.IsNullOrEmpty(ticketNumber)) ticketNumber = ticketNumber.Trim();
            else ticketNumber = string.Empty;

            return _unitOfWork.ElogbookRepository<WorkTicket>().GetQuery(x => !x.IsDeleted && x.FactoryID == factoryID
                                                                                    && x.WorkTicketNumber.Contains(ticketNumber))
               .Select(
                   e => new TicketInforModel()
                   {
                       Value = e.ID,
                       Label = e.WorkTicketNumber,
                       Commander = e.LeadershipUsers.Where(x => x.WorkTicketID == e.ID && x.Roles == (int)LeadershipUserRoles.Commander && !x.IsDeleted)
                                            .Select(z => z.FullName).FirstOrDefault(),
                   }).OrderBy(t => t.Label).ToList();

        }

        public List<TicketNumberInfo> GetMobileTickets(int factoryID, string ticketNumber)
        {
            if (!string.IsNullOrEmpty(ticketNumber)) ticketNumber = ticketNumber.Trim();
            else ticketNumber = string.Empty;

            var entites = _unitOfWork.ElogbookRepository<WorkTicket>().GetQuery(x => !x.IsDeleted && x.FactoryID == factoryID
                                                                                   && x.WorkTicketNumber.Contains(ticketNumber))
               .Select(
                   e => new TicketNumberInfo()
                   {
                       Value = e.ID,
                       Label = e.WorkTicketNumber,
                       LeaderName = string.Empty,
                   }).OrderBy(t => t.Label).ToList();
            if (entites.Count > 0)
            {
                foreach (var item in entites)
                {
                    var leaderShipEntity = _unitOfWork.ElogbookRepository<LeadershipUser>().GetQuery(x => x.WorkTicketID == item.Value
                                                                                                            && !x.IsDeleted
                                                                                                            && x.Roles == (int)LeadershipUserRoles.Commander).FirstOrDefault();
                    if (leaderShipEntity != null) item.LeaderName = leaderShipEntity.FullName;
                }
            }

            return entites;
        }

        public UserInfo GetUserSignature(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username)) username = string.Empty;
            else username = username.Trim();

            if (string.IsNullOrWhiteSpace(password)) password = string.Empty;
            else password = password.Trim();

            var userInfo = _unitOfWork.ElogbookRepository<User>().GetQuery(x => x.UserName.Equals(username)
                                                                                        && !x.IsDeleted
                                                                                        && x.Status == (int)AccessStatus.Active).FirstOrDefault();

            var returnUser = new UserInfo();
            if (userInfo != null)
            {
                if (RuntimeManager.ValidatePassword(password, userInfo.Password))
                {
                    if (!string.IsNullOrWhiteSpace(userInfo.Signature))
                    {
                        return GetInternalUserById(username);
                    }
                    else
                    {
                        returnUser.ErrorMessage = "Chữ ký không tồn tại trong hệ thống.";
                    }
                }
                else
                {
                    returnUser.ErrorMessage = "Tài khoản hoặc mật khẩu chưa chính xác.";
                }
            }
            else
            {
                returnUser.ErrorMessage = "Tài khoản hoặc mật khẩu chưa chính xác.";
            }

            return returnUser;
        }

        public List<ValueLabelModel> GetPowerUnits(int factoryID, string filterDescription = "")
        {
            return _unitOfWork.ElogbookRepository<PowerUnit>().GetQuery(x => x.FactoryID == factoryID && x.PowerUnitName.Contains(filterDescription) && !x.IsDeleted && x.Active == (int)AccessStatus.Active)
                .Select(
                    e => new ValueLabelModel()
                    {
                        Value = e.ID,
                        Label = e.PowerUnitName
                    }).OrderBy(t => t.Label).ToList();
        }

        public List<ValueLabelModel> GetCycles(int factoryID, string filterDescription = "")
        {
            //Danh sách Chu kỳ của Nhà máy
            List<int> cycleIDs = _unitOfWork.ElogbookRepository<CycleFactory>().GetQuery(x => x.FactoryID == factoryID
                                                                                          && !x.IsDeleted).Select(e => e.CycleID).ToList();

            return _unitOfWork.ElogbookRepository<CycleManagement>().GetQuery(x => cycleIDs.Contains(x.ID) && x.CycleName.Contains(filterDescription) && !x.IsDeleted && x.Active == (int)AccessStatus.Active)
                            .Select(
                                e => new ValueLabelModel()
                                {
                                    Value = e.ID,
                                    Label = e.CycleName
                                }).OrderBy(t => t.Value).ToList();
        }

        public List<CyclesModel> GetOperationParameterCycles(int factoryID, int operationPositionID)
        {
            var lstCycles = new List<CyclesModel>();

            if (operationPositionID > 0)
            {
                var parameterEntites = _unitOfWork.ElogbookRepository<Parameter>().GetQuery(x => x.OperationPositionID == operationPositionID
                                                                             && x.FactoryID == factoryID
                                                                             && x.Status == (int)AccessStatus.Active
                                                                             && !x.IsDeleted).ToList();

                var lstCycleID = parameterEntites.Select(x => x.CycleID).Distinct().ToList();

                var cycles = _unitOfWork.ElogbookRepository<CycleManagement>().GetQuery(x => lstCycleID.Contains(x.ID) && !x.IsDeleted).ToList();

                foreach (var cycle in cycles)
                {
                    CyclesModel cyclesResponse = new CyclesModel()
                    {
                        ID = cycle.ID,
                        CycleName = cycle.CycleName,
                        OpenInputBeforeStartTime = cycle.OpenInputBefore,
                        LockInputBeforeEndTime = cycle.BlockInputAfter,
                    };
                    lstCycles.Add(cyclesResponse);
                }

                return lstCycles;
            }
            else
            {
                var cycles = _unitOfWork.ElogbookRepository<CycleManagement>().GetQuery(x => x.Active == (int)AccessStatus.Active && !x.IsDeleted);

                if (factoryID > 0)
                {
                    cycles = cycles.Where(x => x.CycleFactories.Any(y => y.FactoryID == factoryID));
                }

                var data = cycles.Select(t => new CyclesModel()
                {
                    ID = t.ID,
                    CycleName = t.CycleName,
                    OpenInputBeforeStartTime = t.OpenInputBefore,
                    LockInputBeforeEndTime = t.BlockInputAfter,
                }).ToList();

                return data;
            }
        }

        public List<MobileOperationPositionInfo> GetOperationPositionsByFactoryIDMobile(int factoryID, string filterDescription = "")
        {
            return _unitOfWork.ElogbookRepository<OperationPosition>().GetQuery(x => x.FactoryID == factoryID && !x.IsDeleted && x.Active == (int)AccessStatus.Active && x.OperationPositionName.Contains(filterDescription))
                    .Select(
                        e => new MobileOperationPositionInfo()
                        {
                            ID = e.ID,
                            OperationPositionName = e.OperationPositionName,
                            FactoryID = e.FactoryID,
                            Active = e.Active,
                            Quantity = e.Quantity,
                            AuthenticationType = e.AuthenticationType,
                            IsUsingOnlyOneTablet = e.IsUsingOnlyOneTablet,
                            AllowWriteDiaryPortal = e.AllowWriteDiaryPortal,
                            AllowWriteDiaryMobile = e.AllowWriteDiaryMobile,
                            AllowWriteParameterPortal = e.AllowWriteParameterPortal,
                            AllowWriteParameterMobile = e.AllowWriteParameterMobile,
                            JobTitle = e.JobTitle,
                            TemplateDeliveryShiftMethod = e.TemplateDeliveryShiftMethod,
                            Position = e.Position,
                            IsConcurrentlyInShift = e.IsConcurrentlyInShift,
                            IsDeleted = e.IsDeleted,
                            CreatedUserID = e.CreatedUserID,
                            InsertedAt = e.InsertedAt,
                            LastUpdatedUserID = e.LastUpdatedUserID,
                            UpdatedAt = e.UpdatedAt,
                            ShiftReceivingTemplate = e.ShiftReceivingTemplate,
                        }).OrderBy(t => t.ID).ToList();
        }

        public List<ValueLabelModel> GetOrganizations(int organizationID, string level, string filterDescription = "")
        {
            var organizations = _unitOfWork.ElogbookRepository<Organization>().GetQuery(x => !x.IsDeleted
                                                                                    && x.Active == (int)AccessStatus.Active
                                                                                    && x.Name.Contains(filterDescription));

            var list = new List<ValueLabelModel>
                            {
                                new ValueLabelModel { Value = CommonConstant.ALL_KEY, Label = CommonConstant.ALL_VALUE },
                            };
            if (string.IsNullOrWhiteSpace(level)) level = string.Empty;
            level = level.ToLower().Trim();
            switch (level)
            {
                case "công ty":
                    list.AddRange(organizations.Where(x => level.Equals(x.Level, StringComparison.OrdinalIgnoreCase))
                            .Select(
                                e => new ValueLabelModel()
                                {
                                    Value = e.ID,
                                    Label = e.Name
                                }).OrderBy(t => t.Value).ToList());
                    break;
                case "phân xưởng":
                case "tổ":
                case "đội/nhóm":
                    list.AddRange(organizations.Where(x => level.Equals(x.Level, StringComparison.OrdinalIgnoreCase) && (x.ParentID == organizationID))
                            .Select(
                                e => new ValueLabelModel()
                                {
                                    Value = e.ID,
                                    Label = e.Name
                                }).OrderBy(t => t.Value).ToList());
                    break;
                default:
                    break;
            }

            return list;
        }

        public List<ValueLabelModel> GetFactories(int productionPlantID, string filterDescription = "", bool isHasAll = true)
        {
            var factories = _unitOfWork.ElogbookRepository<Factory>().GetQuery(x => (x.ProductionPlantID == productionPlantID || productionPlantID == CommonConstant.ALL_KEY)
                                                                                    && x.FactoryName.Contains(filterDescription) && !x.IsDeleted)
                    .Select(
                        e => new ValueLabelModel()
                        {
                            Value = e.ID,
                            Label = e.FactoryName
                        }).OrderBy(t => t.Label).ToList();

            if (isHasAll)
            {
                if (factories.Count == 1)
                {
                    return factories;
                }
                else
                {
                    var list = new List<ValueLabelModel>
                            {
                                new ValueLabelModel { Value = CommonConstant.ALL_KEY, Label = CommonConstant.ALL_VALUE },
                            };
                    list.AddRange(factories);

                    return list;
                }
            }
            else
            {
                return factories;
            }
        }

        public List<ValueLabelModel> GetFactoriesByUserRight(int userID, string filterDescription, bool isActive)
        {
            // Nếu tài khoản admin thì trả về danh sách tất cả nhà máy
            if (_unitOfWork.ElogbookRepository<RoleInUser>().GetQuery(x => x.UserID == userID && x.UserRole.IsAdmin).Any())
            {
                var entities = _unitOfWork.ElogbookRepository<Factory>().GetQuery(x => x.FactoryName.Contains(filterDescription) && !x.IsDeleted);
                if (isActive)
                {
                    entities = entities.Where(x => x.Status == (int)AccessStatus.Active);
                }
                var data = entities.Select(
                                e => new ValueLabelModel()
                                {
                                    Value = e.ID,
                                    Label = e.FactoryName,
                                    IsUseOperationParameterGroup = e.IsUseOperationParameterGroup,
                                }).OrderBy(t => t.Label).ToList();

                if (data.Count == 1)
                {
                    return data;
                }
                else
                {
                    var list = new List<ValueLabelModel>
                            {
                                new ValueLabelModel { Value = CommonConstant.ALL_KEY, Label = CommonConstant.ALL_VALUE },
                            };
                    list.AddRange(data);

                    return list;
                }
            }

            if (string.IsNullOrWhiteSpace(filterDescription)) filterDescription = string.Empty;
            else filterDescription = filterDescription.Trim();

            // Danh sách vai trò của người dùng
            List<int> listUserRoleID = _unitOfWork.ElogbookRepository<RoleInUser>().GetQuery(x => x.UserID == userID && !x.IsDeleted).Select(e => e.UserRoleID).ToList();
            // Thông tin người dùng
            var userInfo = _unitOfWork.ElogbookRepository<User>().GetQuery(x => x.ID == userID && !x.IsDeleted).FirstOrDefault();
            var factoryID = CommonConstant.ALL_KEY;

            if (userInfo != null && userInfo.ID > 0)
            {
                var userRole = _unitOfWork.ElogbookRepository<UserRole>().GetQuery(x => listUserRoleID.Contains(x.ID) &&
                                                                                            x.DataPermission == (int)DataPermission.Unlimited
                                                                                            && !x.IsDeleted).FirstOrDefault();
                // Nếu tất cả các vai trò đều bị giới hạn thì lấy FactoryID
                if (userRole == null)
                {
                    factoryID = userInfo.FactoryID.GetValueOrDefault(0);
                }
            }

            var factories = _unitOfWork.ElogbookRepository<Factory>().GetQuery(x => x.FactoryName.Contains(filterDescription)
                                                                                    && (factoryID == CommonConstant.ALL_KEY || x.ID == factoryID)
                                                                                    && !x.IsDeleted)
                .Select(
                    e => new ValueLabelModel()
                    {
                        Value = e.ID,
                        Label = e.FactoryName,
                        IsUseOperationParameterGroup = e.IsUseOperationParameterGroup,
                    }).OrderBy(t => t.Label).ToList();

            if (factoryID > 0)
            {
                return factories;
            }
            else
            {
                if (factories.Count == 1)
                {
                    return factories;
                }
                else
                {
                    var list = new List<ValueLabelModel>
                            {
                                new ValueLabelModel { Value = CommonConstant.ALL_KEY, Label = CommonConstant.ALL_VALUE },
                            };
                    list.AddRange(factories);

                    return list;
                }
            }
        }

        public List<ComboboxOperationPositionModel> GetOperationPositionsByFactoryID(int factoryID, bool isHasAll = true, string filterDescription = "")
        {
            var positions = _unitOfWork.ElogbookRepository<OperationPosition>().GetQuery(x => !x.IsDeleted && x.Active == (int)AccessStatus.Active
                                                                                    && x.FactoryID == factoryID
                                                                                    && x.OperationPositionName.Contains(filterDescription))
                            .Select(
                                e => new ComboboxOperationPositionModel()
                                {
                                    Value = new ValueModel { ID = e.ID, Quantity = e.Quantity },
                                    Label = e.OperationPositionName,
                                    Position = e.Position,
                                    IsConcurrentlyInShift = e.IsConcurrentlyInShift,
                                    Quantity = e.Quantity
                                }).OrderBy(t => t.Position).ToList();

            if (isHasAll)
            {
                var list = new List<ComboboxOperationPositionModel>
                            {
                                new ComboboxOperationPositionModel { Value = new ValueModel{ ID = CommonConstant.ALL_KEY, Quantity = 0 } , Label = CommonConstant.ALL_VALUE },
                            };
                list.AddRange(positions);

                return list;
            }
            else
            {
                return positions;
            }
        }

        /// <summary>
        /// Danh sách cương vị trong nhà máy ngoại trừ cương vị hiện tại
        /// </summary>
        /// <param name="factoryID"></param>
        /// <param name="currentPositionID"></param>
        /// <returns></returns>
        public List<ComboboxOperationPositionModel> GetPositionsWithoutCurrentPosition(int factoryID, int currentPositionID)
        {
            // Filter data by condition
            var idPositions = _unitOfWork.ElogbookRepository<PositionInShift>().GetQuery(x => x.ParentID == currentPositionID
                                                                                && !x.IsDeleted).Select(x => x.OperationPositionID).ToList();

            return _unitOfWork.ElogbookRepository<OperationPosition>().GetQuery(x => !x.IsDeleted && x.Active == (int)AccessStatus.Active
                                                                                    && x.FactoryID == factoryID && !idPositions.Contains(x.ID))
                            .Select(
                                e => new ComboboxOperationPositionModel()
                                {
                                    Value = new ValueModel { ID = e.ID, Quantity = e.Quantity },
                                    Label = e.OperationPositionName,
                                    Position = e.Position,
                                }).OrderBy(t => t.Position).ToList();
        }

        public List<OperationShiftBaseModel> GetOperationShiftsByFactoryID(int factoryID, string hour = "", bool isMobile = false, bool isHasAll = true, string filterDescription = "")
        {
            double.TryParse(hour.Replace(":", "."), out double outHour);
            //Danh sách ca vận hành cho mobile cho phép lấy tất cả ca vận hành
            if (isMobile && !AppSettings.ALLOW_GET_SHIFT_BY_CURRENT_TIME)
            {
                //Fix issue display operation shift by position
                return _unitOfWork.ElogbookRepository<OperationShift>().GetQuery(x => !x.IsDeleted && x.Active == (int)AccessStatus.Active
                                                                                    && x.FactoryID == factoryID
                                                                                    && x.OperationShiftName.Contains(filterDescription))
                            .Select(
                                e => new OperationShiftBaseModel()
                                {
                                    Value = e.ID,
                                    Label = e.OperationShiftName,
                                    Position = e.Position,
                                    InsertedAt = e.InsertedAt,
                                    EndTime = e.EndTime,
                                    StartTime = e.StartTime,
                                }).OrderBy(t => t.Position)
                                .ThenBy(t => t.Label)
                                .ThenByDescending(t => t.InsertedAt)
                                .ToList();
            }

            //Theo giờ hiện tại
            if (outHour > 0)
            {
                var currentShift = GetShiftByCurrentTime(factoryID, outHour);
                if (currentShift != null)
                {
                    return new List<OperationShiftBaseModel>
                            {
                                new OperationShiftBaseModel { Value = currentShift.Value.Value, Label = currentShift.Label},
                            };
                }
                else
                {
                    return new List<OperationShiftBaseModel>();
                }
            }

            //Danh sách ca vận hành cho web
            var shifts = _unitOfWork.ElogbookRepository<OperationShift>().GetQuery(x => !x.IsDeleted && x.Active == (int)AccessStatus.Active
                                                                                    && x.FactoryID == factoryID
                                                                                    && x.OperationShiftName.Contains(filterDescription))
                            .Select(
                                e => new OperationShiftBaseModel()
                                {
                                    Value = e.ID,
                                    Label = e.OperationShiftName,
                                    Position = e.Position,
                                    InsertedAt = e.InsertedAt,
                                    EndTime = e.EndTime,
                                    StartTime = e.StartTime,
                                }).OrderBy(t => t.Position)
                                .ThenBy(t => t.Label)
                                .ThenByDescending(t => t.InsertedAt)
                                .ToList();

            if (isHasAll)
            {
                var list = new List<OperationShiftBaseModel>
                            {
                                new OperationShiftBaseModel { Value = CommonConstant.ALL_KEY, Label = CommonConstant.ALL_VALUE },
                            };
                list.AddRange(shifts);

                return list;
            }
            else
            {
                return shifts;
            }
        }

        public List<ValueLabelModel> GetOperationCrewsByFactoryID(int factoryID, bool isHasAll = true, string filterDescription = "")
        {
            var crews = _unitOfWork.ElogbookRepository<OperationCrew>().GetQuery(x => !x.IsDeleted && x.Active == (int)AccessStatus.Active
                                                                                    && x.FactoryID == factoryID
                                                                                    && x.OperationCrewName.Contains(filterDescription))
                            .Select(
                                e => new ValueLabelModel()
                                {
                                    Value = e.ID,
                                    Label = e.OperationCrewName
                                }).OrderBy(t => t.Value).ToList();

            if (isHasAll)
            {
                var list = new List<ValueLabelModel>
                            {
                                new ValueLabelModel { Value = CommonConstant.ALL_KEY, Label = CommonConstant.ALL_VALUE },
                            };
                list.AddRange(crews);

                return list;
            }
            else
            {
                return crews;
            }
        }

        public List<ValueLabelModel> GetParameterGroupByOperationPositionID(int operationPositionID, bool isHasAll = true, string filterDescription = "")
        {
            var crews = _unitOfWork.ElogbookRepository<ParameterGroup>().GetQuery(x => !x.IsDeleted && x.Status == (int)AccessStatus.Active
                                                                                        && x.OperationPositionID == operationPositionID
                                                                                        && x.ParameterGroupName.Contains(filterDescription))
                                .Select(
                                    e => new ValueLabelModel()
                                    {
                                        Value = e.ID,
                                        Label = e.ParameterGroupName
                                    }).OrderBy(t => t.Value).ToList();

            if (isHasAll)
            {
                var list = new List<ValueLabelModel>
                            {
                                new ValueLabelModel { Value = CommonConstant.ALL_KEY, Label = CommonConstant.ALL_VALUE },
                            };
                list.AddRange(crews);

                return list;
            }
            else
            {
                return crews;
            }
        }

        public List<ValueLabelModel> GetParameterGroupByFactoryIDAndPositionID(int factoryID, int operationPositionID)
        {
            if (factoryID <= 0) factoryID = CommonConstant.ALL_KEY;
            if (operationPositionID <= 0) operationPositionID = CommonConstant.ALL_KEY;

            var parameterGroups = _unitOfWork.ElogbookRepository<ParameterGroup>().GetQuery(x => !x.IsDeleted && x.Status == (int)AccessStatus.Active
                                                                                        && (x.FactoryID == factoryID || factoryID == CommonConstant.ALL_KEY)
                                                                                        && (x.OperationPositionID == operationPositionID || operationPositionID == CommonConstant.ALL_KEY))
                                .Select(
                                    e => new ValueLabelModel()
                                    {
                                        Value = e.ID,
                                        Label = e.ParameterGroupName
                                    }).OrderBy(t => t.Value).ToList();

            var list = new List<ValueLabelModel>
                            {
                                new ValueLabelModel { Value = CommonConstant.ALL_KEY, Label = CommonConstant.ALL_VALUE },
                            };
            list.AddRange(parameterGroups);

            return list;
        }

        public CategoryModel GetCategoryByFactoryID(int factoryID, string filterDescription = "")
        {
            CategoryModel categoryModel = new CategoryModel
            {
                Users = _unitOfWork.ElogbookRepository<User>().GetQuery(x => !x.IsDeleted && x.Status == (int)AccessStatus.Active
                                                                                        && x.FactoryID == factoryID
                                                                                        && x.FullName.Contains(filterDescription))
                                .Select(
                                    e => new UserInfo()
                                    {
                                        ID = e.ID,
                                        FullName = e.FullName,
                                        UserName = e.UserName,
                                        DateOfBirth = e.DateOfBirth,
                                        NationalID = e.NationalID,
                                    }).OrderBy(t => t.FullName).ToList()
            };

            return categoryModel;
        }

        public bool CheckExistData(string tableName, int id)
        {
            switch (tableName)
            {
                case "factory":
                    return _unitOfWork.ElogbookRepository<Factory>().GetQuery(x => !x.IsDeleted && x.Status == (int)AccessStatus.Active && x.ID == id).Any();
                case "operationposition":
                    return _unitOfWork.ElogbookRepository<OperationPosition>().GetQuery(x => !x.IsDeleted && x.Active == (int)AccessStatus.Active && x.ID == id).Any();
                case "operationshift":
                    return _unitOfWork.ElogbookRepository<OperationShift>().GetQuery(x => !x.IsDeleted && x.Active == (int)AccessStatus.Active && x.ID == id).Any();
                case "operationcrew":
                    return _unitOfWork.ElogbookRepository<OperationCrew>().GetQuery(x => !x.IsDeleted && x.Active == (int)AccessStatus.Active && x.ID == id).Any();
                case "positioninshift":
                    return _unitOfWork.ElogbookRepository<PositionInShift>().GetQuery(x => !x.IsDeleted && x.Active == (int)AccessStatus.Active && x.ID == id).Any();
                case "user":
                    return _unitOfWork.ElogbookRepository<User>().GetQuery(x => !x.IsDeleted && x.Status == (int)AccessStatus.Active && x.ID == id).Any();
                default:
                    return true;
            }
        }

        /// <summary>
        /// Lấy đường dẫn đầy đủ của file.
        /// Nếu moduleValue được cung cấp và fileName chưa bắt đầu bằng moduleValue,
        /// sẽ tự động thêm moduleValue vào trước fileName.
        /// </summary>
        public string GetPathFile(string fileName, string moduleValue = null)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return string.Empty;
            }

            // Nếu có moduleValue và fileName chưa chứa prefix moduleValue → thêm vào
            if (!string.IsNullOrWhiteSpace(moduleValue)
                && !fileName.StartsWith(moduleValue + "\\", StringComparison.OrdinalIgnoreCase)
                && !fileName.StartsWith(moduleValue + "/", StringComparison.OrdinalIgnoreCase))
            {
                fileName = moduleValue + "\\" + fileName;
            }

            return AppSettings.SERVER_DIR + fileName;
        }

        /// <summary>
        /// Lấy danh sách setting cảnh báo
        /// </summary>
        public AlarmSettingModel GetAlarmSettings()
        {
            AlarmSettingModel alarmSettingModel = new AlarmSettingModel();

            var entities = _unitOfWork.ElogbookRepository<AlarmSetting>().GetQuery(x => !x.IsDeleted).ToList();

            if (entities.Count > 0)
            {
                List<string> Alarms = new List<string>();
                List<string> Operators = new List<string>();
                List<string> Colors = new List<string>();
                foreach (var item in entities)
                {
                    switch (item.SettingType)
                    {
                        case 1:
                            Alarms.Add(item.SettingName);
                            break;
                        case 2:
                            Operators.Add(item.SettingName);
                            break;
                        case 3:
                            Colors.Add(item.SettingName);
                            break;
                    }
                }
                alarmSettingModel.Alarms = Alarms;
                alarmSettingModel.Operators = Operators;
                alarmSettingModel.Colors = Colors;
            }

            return alarmSettingModel;
        }

        public SignatureInfo GetSignatureById(int id)
        {
            var entity = _unitOfWork.ElogbookRepository<Signature>().GetQuery(x => x.ID == id && !x.IsDeleted)
                .Select(e => new SignatureInfo()
                {
                    ID = e.ID,
                    SignatureContentByte = e.SignatureContentByte,
                    SignatureContent = e.SignatureContent,
                    Type = e.Type
                })
                .FirstOrDefault();
            if (entity != null) return entity;

            return null;
        }

        public SignatureInfo CreateSignature(SignatureModel model)
        {
            var signature = new Signature
            {
                Type = model.Type,
                SignatureContentByte = model.SignatureContent != null ? Encoding.UTF8.GetBytes(model.SignatureContent) : null,
                CreatedUserID = model.CurrentUserID,
                LastUpdatedUserID = model.CurrentUserID,
            };

            _unitOfWork.ElogbookRepository<Signature>().Add(signature);

            var saveSuccess = _unitOfWork.Complete() > 0;
            if (saveSuccess)
                return GetSignatureById(signature.ID);
            else
                return null;
        }

        public UserInfo GetInternalUserById(string userName)
        {
            return _unitOfWork.ElogbookRepository<User>().GetQuery(x => !x.IsDeleted
                                                                    && x.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase))
                    .Select(
                        e => new UserInfo()
                        {
                            ID = e.ID,
                            FullName = e.FullName,
                            UserName = e.UserName,
                            DateOfBirth = e.DateOfBirth,
                            NationalID = e.NationalID,
                            ElectricRankStr = e.ElectricRank,
                            ElectricRankCard = e.ElectricRankCard,
                            LaborRankStr = e.LaborRank,
                            LaborCard = e.LaborCard,
                            Signature = e.Signature,
                            CompanyID = e.CompanyID,
                            ProductionPlantID = e.ProductionPlantID,
                            TeamID = e.TeamID,
                            GroupID = e.GroupID,
                            FactoryID = e.FactoryID,
                            AccountType = e.AccountType,
                            SourceType = e.SourceType,
                            Status = e.Status,
                            IsExternal = false,
                            PositionName = e.PositionName,
                            PhoneNumber = e.PhoneNumber,
                            SigningMethod = new ValueLabelModel { Value = e.SigningMethod.ID, Label = e.SigningMethod.SigningMethodName },
                            SigningMethodID = e.SigningMethod.ID,
                            ProductionPlant = e.ProductionPlant == null ? null : new ValueLabelModel { Value = e.ProductionPlant.ID, Label = e.ProductionPlant.Name },
                        }).FirstOrDefault();
        }

        public UserInfo GetExternalUserById(string userName)
        {
            return _unitOfWork.ElogbookRepository<GuestUser>().GetQuery(x => !x.IsDeleted
                                                                    && x.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase))
                    .Select(
                        e => new UserInfo()
                        {
                            ID = e.ID,
                            FullName = e.FullName,
                            UserName = e.UserName,
                            DateOfBirth = e.DateOfBirth,
                            NationalID = e.NationalID,
                            ElectricRankStr = e.ElectricRank,
                            ElectricRankCard = e.ElectricRankCard,
                            LaborRankStr = e.LaborRank,
                            LaborCard = e.LaborCard,
                            IsExternal = true
                        }).FirstOrDefault();
        }

        public UserModel GetExternalUserById(int ID)
        {
            var userInfo = _unitOfWork.ElogbookRepository<GuestUser>().GetQuery(x => !x.IsDeleted
                                                                    && x.ID == ID)
                    .Select(
                        e => new UserModel()
                        {
                            ID = e.ID,
                            FullName = e.FullName,
                            UserName = e.UserName,
                            DateOfBirth = e.DateOfBirth,
                            NationalID = e.NationalID,
                            ElectricRankStr = e.ElectricRank,
                            ElectricRankCard = e.ElectricRankCard,
                            NationalAttachmentID = e.NationalAttachmentID != null ? e.NationalAttachmentID.Value : 0,
                            ElectricRankAttachmentID = e.ElectricRankAttachmentID != null ? e.ElectricRankAttachmentID.Value : 0,
                            LaborCard = e.LaborCard,
                            LaborRankStr = e.LaborRank,
                            LaborCardAttachmentID = e.LaborCardAttachmentID != null ? e.LaborCardAttachmentID.Value : 0,
                        }).FirstOrDefault();

            if (userInfo != null)
            {
                // Hình ảnh số CMND/CCCD
                if (userInfo.NationalAttachmentID > 0)
                {
                    var attachment = _unitOfWork.ElogbookRepository<Attachment>().GetQuery(x => !x.IsDeleted && x.ID == userInfo.NationalAttachmentID).FirstOrDefault();
                    if (attachment != null)
                    {
                        userInfo.NationalAttachment = new AttachmentModel
                        {
                            ID = attachment.ID,
                            FileName = attachment.FileName,
                            FilePath = GetPathFile(attachment.FilePath),
                            PathThumbnail = GetPathFile(attachment.PathThumbnail),
                            ScreenName = attachment.ScreenName,
                            FileType = attachment.FileType,
                        };
                    }
                }

                // Hình ảnh thẻ an toàn điện
                if (userInfo.ElectricRankAttachmentID > 0)
                {
                    var attachment = _unitOfWork.ElogbookRepository<Attachment>().GetQuery(x => !x.IsDeleted && x.ID == userInfo.ElectricRankAttachmentID).FirstOrDefault();
                    if (attachment != null)
                    {
                        userInfo.ElectricRankAttachment = new AttachmentModel
                        {
                            ID = attachment.ID,
                            FileName = attachment.FileName,
                            FilePath = GetPathFile(attachment.FilePath),
                            PathThumbnail = GetPathFile(attachment.PathThumbnail),
                            ScreenName = attachment.ScreenName,
                            FileType = attachment.FileType,
                        };
                    }
                }

                // Hình ảnh thẻ an toàn lao động
                if (userInfo.LaborCardAttachmentID > 0)
                {
                    var attachment = _unitOfWork.ElogbookRepository<Attachment>().GetQuery(x => !x.IsDeleted && x.ID == userInfo.LaborCardAttachmentID).FirstOrDefault();
                    if (attachment != null)
                    {
                        userInfo.LaborCardAttachment = new AttachmentModel
                        {
                            ID = attachment.ID,
                            FileName = attachment.FileName,
                            FilePath = GetPathFile(attachment.FilePath),
                            PathThumbnail = GetPathFile(attachment.PathThumbnail),
                            ScreenName = attachment.ScreenName,
                            FileType = attachment.FileType,
                        };
                    }
                }
            }
            return userInfo;
        }

        public UserInfo GetUserById(string userName, bool isExternal)
        {
            if (isExternal)
                return GetExternalUserById(userName);
            else
                return GetInternalUserById(userName);
        }

        /// <summary>
        /// Tìm đơn vị đăng ký và đơn vị cấp phiếu
        /// </summary>
        /// <param name="isUnitRegistration"></param>
        /// <returns></returns>
        public List<ValueLabelModel> GetUnitRegistrationOrUnitApprove(bool isUnitRegistration)
        {
            var organizations = _unitOfWork.ElogbookRepository<Organization>().GetQuery(x => !x.IsDeleted
                                                                                    && x.Active == (int)AccessStatus.Active);

            var list = new List<ValueLabelModel>
            {
                new ValueLabelModel { Value = CommonConstant.ALL_KEY, Label = CommonConstant.ALL_VALUE },
            };

            if (isUnitRegistration)
            {
                var data = organizations.Where(x => x.ParentID != null && x.Level.Equals("Phân xưởng", StringComparison.OrdinalIgnoreCase)).ToList();

                foreach (var item in data)
                {
                    var organization = organizations.FirstOrDefault(x => x.ID == item.ParentID && string.IsNullOrEmpty(x.CompanyType));
                    if (organization != null)
                    {
                        list.Add(new ValueLabelModel()
                        {
                            Value = item.ID,
                            Label = item.Name
                        });
                    }
                }
            }
            else
            {
                var data = organizations.Where(x => x.ParentID != null && x.Level.Equals("Phân xưởng", StringComparison.OrdinalIgnoreCase) && x.ProductionPlantType.Equals("Vận hành", StringComparison.OrdinalIgnoreCase)).ToList();

                foreach (var item in data)
                {
                    var organization = organizations.FirstOrDefault(x => x.ID == item.ParentID && string.IsNullOrEmpty(x.CompanyType));
                    if (organization != null)
                    {
                        list.Add(new ValueLabelModel()
                        {
                            Value = item.ID,
                            Label = item.Name
                        });
                    }
                }
            }

            return list.OrderBy(x => x.Value).ToList();
        }

        public List<InternalExternalUserModel> GetInternalExternalUser(int factoryID, string searchText, string electricRank, int userType, string ticketType)
        {
            if (string.IsNullOrWhiteSpace(searchText)) searchText = string.Empty;
            if (userType == 0) userType = CommonConstant.ALL_KEY;

            List<InternalExternalUserModel> users = new List<InternalExternalUserModel>();

            // Người dùng nội bộ
            if (userType == -1 || userType == 1)
            {
                var entities = _unitOfWork.ElogbookRepository<User>().GetQuery(x => x.Status == (int)AccessStatus.Active && !x.IsDeleted);

                if (!string.IsNullOrWhiteSpace(ticketType) && "PCTTCNH".Equals(ticketType, StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrWhiteSpace(searchText))
                    {
                        entities = entities.Where(x => (x.FullName.Contains(searchText) || x.LaborCard.Contains(searchText)));
                    }
                    if (!string.IsNullOrWhiteSpace(electricRank))
                    {
                        entities = entities.Where(x => x.LaborRank == electricRank);
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(searchText))
                    {
                        entities = entities.Where(x => (x.FullName.Contains(searchText) || x.ElectricRankCard.Contains(searchText)));
                    }
                    if (!string.IsNullOrWhiteSpace(electricRank))
                    {
                        entities = entities.Where(x => x.ElectricRank == electricRank);
                    }
                }

                if (factoryID > 0)
                {
                    entities = entities.Where(x => x.FactoryID == factoryID);
                }

                var listInternal = entities.Select(
                        e => new InternalExternalUserModel()
                        {
                            ID = e.ID,
                            FullName = e.FullName,
                            TempDateOfBirth = e.DateOfBirth,
                            ElectricRankValueStr = e.ElectricRank,
                            ElectricRankCard = e.ElectricRankCard,
                            LaborRankValueStr = e.LaborRank,
                            LaborCard = e.LaborCard,
                            UserType = "Nội bộ",
                            Unit = new ValueLabelModel { Value = e.TeamID, Label = e.Team.Name },
                            UserName = e.UserName,
                            IsExternal = false,
                            PositionName = e.PositionName,
                        }).ToList();

                if (listInternal != null)
                    users.AddRange(listInternal);
            }

            // Người ngoài đơn vị
            if (userType == -1 || userType == 2)
            {
                var listExternal = _unitOfWork.ElogbookRepository<GuestUser>().GetQuery(x => !x.IsDeleted);
                if (!string.IsNullOrWhiteSpace(ticketType) && "PCTTCNH".Equals(ticketType, StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrWhiteSpace(searchText))
                    {
                        listExternal = listExternal.Where(x => (x.FullName.Contains(searchText) || x.LaborCard.Contains(searchText)));
                    }
                    if (!string.IsNullOrWhiteSpace(electricRank))
                    {
                        listExternal = listExternal.Where(x => x.LaborRank == electricRank);
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(searchText))
                    {
                        listExternal = listExternal.Where(x => (x.FullName.Contains(searchText) || x.ElectricRankCard.Contains(searchText)));
                    }

                }

                var data = listExternal.Select(
                e => new InternalExternalUserModel()
                {
                    ID = e.ID,
                    FullName = e.FullName,
                    TempDateOfBirth = e.DateOfBirth,
                    ElectricRankValueStr = e.ElectricRank,
                    ElectricRankCard = e.ElectricRankCard,
                    LaborRankValueStr = e.LaborRank,
                    LaborCard = e.LaborCard,
                    UserType = "Bên ngoài đơn vị",
                    Unit = new ValueLabelModel { },
                    UserName = e.UserName,
                    IsExternal = true,
                    PositionName = string.Empty,
                }).ToList();

                if (listExternal != null)
                    users.AddRange(data);
            }

            foreach (var user in users)
            {
                //Lưu ý: Đối với những người đã ký Vào vị trí làm việc nhưng chưa ký Rời khỏi vị trí làm việc ở phần Công tác hàng ngày trong một phiếu công tác khác đang thực hiện
                //vào thời điểm đăng ký thì vẫn hiển thị nhưng không cho phép chọn. Hiển thị tooltip: “Người dùng này đang thực hiện phiếu công tác khác.”
                if (ticketType == "PCTTCNH")
                {
                    var workerInformation = _unitOfWork.ElogbookRepository<WorkerInformation>().GetQuery(x => !x.IsDeleted
                                                                                            && x.UserName.Equals(user.UserName)
                                                                                            && x.CheckInSignature > 0
                                                                                            && x.WorkTicket.TicketStatus.ID != 9 //Ðã hoàn thành
                                                                                            && !x.WorkTicket.IsDeleted
                                                                                            && x.CheckOutSignature == null).FirstOrDefault();
                    if (workerInformation != null)
                    {
                        user.IsInUse = true;
                    }
                }

                if (!user.IsExternal)
                {
                    var userRolesInfo = GetUserRight(user.ID);
                    if (userRolesInfo != null && userRolesInfo.UsersRight != null && userRolesInfo.UsersRight.Count > 0)
                    {
                        // Xác định người dùng được Quyền cho phép công tác điện
                        if (userRolesInfo.UsersRight.Exists(x => x.PermissionKey == "ALLOW_CTĐ_MOBILE"))
                        {
                            user.IsAllowCTD = true;
                        }

                        // Xác định người dùng được Quyền cho phép công tác Thuỷ-cơ-nhiệt-hoá
                        if (userRolesInfo.UsersRight.Exists(x => x.PermissionKey == "ALLOW_CTTCNH_MOBILE"))
                        {
                            user.IsAllowCTTCNH = true;
                        }
                    }
                }
            }

            return users;
        }

        public bool CheckUserHasRight(int userID, string permissionKey)
        {
            var userRolesInfo = GetUserRight(userID);
            if (userRolesInfo != null
                && userRolesInfo.UsersRight != null
                && userRolesInfo.UsersRight.Count > 0
                && userRolesInfo.UsersRight.Exists(x => x.PermissionKey == permissionKey))
            {
                return true;
            }

            return false;
        }

        public ResponseBaseModel GetInternalExternalUserMobile(GetInternalExternalUserModel request)
        {
            request.PermissionKey = new List<string>();

            request.PageSize = (request.PageSize < 0 || request.PageSize > CommonConstant.DEFAULT_PAGE_SIZE) ? CommonConstant.DEFAULT_PAGE_SIZE : request.PageSize;

            if (request.UserType == 0)
            {
                request.UserType = CommonConstant.ALL_KEY;
            }

            if ("-1".Equals(request.ElectricRank, StringComparison.OrdinalIgnoreCase))
            {
                request.ElectricRank = string.Empty;
            }

            if ("-1".Equals(request.LaborRank, StringComparison.OrdinalIgnoreCase))
            {
                request.LaborRank = string.Empty;
            }

            List<InternalExternalUserModel> users = new List<InternalExternalUserModel>();
            if (request.UserType == -1 || request.UserType == 1)
            {
                //Flag IsFilterFactory mặc định là true, yêu cầu filter theo nhà máy.
                //Đối với công ty nào không cần filter theo nhà máy thì cập nhật flag IsFilterFactory = false ở database
                var factory = _unitOfWork.ElogbookRepository<Factory>().GetById(request.FactoryID);

                var entities = _unitOfWork.ElogbookRepository<User>().GetQuery(x => x.Status == (int)AccessStatus.Active && !x.IsDeleted);
                if (!request.IsFormRegistration)
                {
                    switch (request.TicketType)
                    {
                        case "PCTĐ":
                            if (request.PermissionKey == null)
                            {
                                request.PermissionKey = new List<string>();
                            }

                            break;

                        case "TCNH":
                            if (request.PermissionKey == null)
                            {
                                request.PermissionKey = new List<string>();
                            }

                            break;

                        case "NKVH":
                            entities = entities.Where(x => x.ProductionPlant.ProductionPlantType.Equals("Vận hành"));
                            // Nếu công ty không ràng buộc VHV thuộc nhà máy, thì chỉ cần kiểm tra VHV có nhà máy
                            if (factory != null && !factory.IsFilterFactory)
                            {
                                entities = entities.Where(x => x.FactoryID > 0);
                            }
                            // Nếu công ty có nhiều nhà máy, cần ràng buộc VHV theo nhà máy thì cần tìm kiếm VHV theo nhà máy đó
                            else
                            {
                                if (request.FactoryID > 0)
                                {
                                    entities = entities.Where(x => x.FactoryID == request.FactoryID);
                                }
                            }

                            if (request.PermissionKey == null)
                            {
                                request.PermissionKey = new List<string>();
                            }

                            if (!request.PermissionKey.Contains("ADD_NKVH"))
                            {
                                request.PermissionKey.Add("ADD_NKVH");
                            }
                            break;

                        case "PTT":
                            entities = entities.Where(x => x.ProductionPlantID == request.ProductionPlantID);
                            break;
                    }
                }

                switch (request.TicketType)
                {
                    case "PCTĐ":
                    case "TCNH":
                    case "PTT":
                        // Đối với phiếu thì cấu hình để hiển thị nhân viên EPS
                        if (factory != null && !factory.IsFilterFactory)
                        {
                            entities = entities.Where(x => x.FactoryID > 0 || x.FactoryID == null);
                        }
                        else
                        {
                            if (request.FactoryID > 0)
                            {
                                entities = entities.Where(x => x.FactoryID == request.FactoryID || x.FactoryID == null);
                            }
                        }
                        break;

                    default:
                        // Nếu công ty không ràng buộc VHV thuộc nhà máy, thì chỉ cần kiểm tra VHV có nhà máy
                        if (factory != null && !factory.IsFilterFactory)
                        {
                            entities = entities.Where(x => x.FactoryID > 0);
                        }
                        // Nếu công ty có nhiều nhà máy, cần ràng buộc VHV theo nhà máy thì cần tìm kiếm VHV theo nhà máy đó
                        else
                        {
                            if (request.FactoryID > 0)
                            {
                                entities = entities.Where(x => x.FactoryID == request.FactoryID);
                            }
                        }
                        break;
                }

                // Filter theo Công ty
                if (request.CompanyID > 0)
                {
                    entities = entities.Where(x => x.CompanyID == request.CompanyID);
                }

                // Filter theo quyền
                if (request.PermissionKey != null && request.PermissionKey.Count > 0)
                {
                    foreach (var role in request.PermissionKey)
                    {
                        entities = entities.Where(x => x.RoleInUsers.Any(y => y.UserRole.Permissions.Contains(role) || x.RoleInUsers.Any(z => z.UserRole.IsAdmin)));
                    }
                }

                if (!string.IsNullOrWhiteSpace(request.ElectricRank) && request.TicketType != "NKVH")
                {
                    // Thẻ an toàn lao động
                    if (request.TicketType.Equals("TCNH"))
                    {
                        entities = entities.Where(x => x.LaborCard.Contains(request.ElectricRank));
                    }
                    // Thẻ an toàn điện
                    else
                    {
                        entities = entities.Where(x => x.ElectricRankCard.Contains(request.ElectricRank));
                    }
                }

                // Tìm kiếm theo Tên, thẻ ATĐ, CMND
                if (!string.IsNullOrWhiteSpace(request.SearchText))
                {
                    entities = entities.Where(x => x.FullName.Contains(request.SearchText) || x.ElectricRankCard.Contains(request.SearchText) || x.NationalID.Contains(request.SearchText) || x.LaborCard.Contains(request.SearchText));
                }

                var listInternal = entities.Select(
                        e => new InternalExternalUserModel()
                        {
                            ID = e.ID,
                            FullName = e.FullName,
                            TempDateOfBirth = e.DateOfBirth,
                            ElectricRankValueStr = e.ElectricRank,
                            ElectricRankCard = e.ElectricRankCard,
                            LaborRankValueStr = e.LaborRank,
                            LaborCard = e.LaborCard,
                            UserType = "Nội bộ",
                            Unit = new ValueLabelModel { Value = e.TeamID, Label = e.Team.Name },
                            UserName = e.UserName,
                            IsExternal = false,
                            PositionName = e.PositionName,
                            NationalID = e.NationalID,
                            IsAllowCTD = true,
                            IsAllowCTTCNH = true,
                            ProductionPlant = e.ProductionPlant == null ? null : new ValueLabelModel { Value = e.ProductionPlantID, Label = e.ProductionPlant.Name }
                        }).ToList();

                if (listInternal != null)
                {
                    users.AddRange(listInternal);
                }
            }

            if (request.UserType == -1 || request.UserType == 2)
            {
                var listExternal = _unitOfWork.ElogbookRepository<GuestUser>().GetQuery(x => !x.IsDeleted);

                if (!string.IsNullOrWhiteSpace(request.ElectricRank))
                {
                    // Thẻ an toàn lao động
                    if (request.TicketType.Equals("TCNH"))
                    {
                        listExternal = listExternal.Where(x => x.LaborCard.Contains(request.ElectricRank));
                    }
                    // Thẻ an toàn điện
                    else
                    {
                        listExternal = listExternal.Where(x => x.ElectricRankCard.Contains(request.ElectricRank));
                    }
                }

                // Tìm kiếm theo tên nhân viên
                if (!string.IsNullOrWhiteSpace(request.SearchText))
                {
                    listExternal = listExternal.Where(x => (x.FullName.Contains(request.SearchText) || x.LaborCard.Contains(request.SearchText)));
                }

                var data = listExternal.Select(
                e => new InternalExternalUserModel()
                {
                    ID = e.ID,
                    FullName = e.FullName,
                    TempDateOfBirth = e.DateOfBirth,
                    ElectricRankValueStr = e.ElectricRank,
                    ElectricRankCard = e.ElectricRankCard,
                    LaborRankValueStr = e.LaborRank,
                    LaborCard = e.LaborCard,
                    UserType = "Bên ngoài đơn vị",
                    Unit = new ValueLabelModel { },
                    UserName = e.UserName,
                    IsExternal = true,
                    PositionName = string.Empty,
                    NationalID = e.NationalID,
                    IsAllowCTD = true,
                    IsAllowCTTCNH = true,
                }).ToList();

                if (listExternal != null)
                {
                    users.AddRange(data);
                }
            }

            IPagedList<InternalExternalUserModel> dataEntities = users.OrderBy(t => t.FullName).ToList().ToPagedList(request.PageNumber, request.PageSize);
            List<InternalExternalUserModel> results = new List<InternalExternalUserModel>();
            foreach (var user in dataEntities)
            {
                //Lưu ý: Đối với những người đã ký Vào vị trí làm việc nhưng chưa ký Rời khỏi vị trí làm việc ở phần Công tác hàng ngày trong một phiếu công tác khác đang thực hiện
                //vào thời điểm đăng ký thì vẫn hiển thị nhưng không cho phép chọn. Hiển thị tooltip: “Người dùng này đang thực hiện phiếu công tác khác.”
                if (request.TicketType == "TCNH")
                {
                    var workerInformation = _unitOfWork.ElogbookRepository<WorkerInformation>().GetQuery(x => !x.IsDeleted
                                                                                            && x.UserName.Equals(user.UserName)
                                                                                            && x.CheckInSignature > 0
                                                                                            && x.WorkTicket.TicketStatus.ID != 9 //Ðã hoàn thành
                                                                                            && !x.WorkTicket.IsDeleted
                                                                                            && x.CheckOutSignature == null).FirstOrDefault();
                    if (workerInformation != null)
                    {
                        user.IsInUse = true;
                    }
                }
                results.Add(user);
            }

            return HttpResponse.OkList(results, request.PageNumber, request.PageSize, dataEntities.TotalRecords, dataEntities.TotalPages);
        }

        public List<InternalExternalUserModel> FilterUser(FilterUserModel filterUserModel)
        {
            List<InternalExternalUserModel> lstUser = new List<InternalExternalUserModel>();

            if (filterUserModel.UserType == -1 || filterUserModel.UserType == 1)
            {
                var entities = _unitOfWork.ElogbookRepository<User>().GetQuery(x => x.Status == (int)AccessStatus.Active && !x.IsDeleted);

                var factory = _unitOfWork.ElogbookRepository<Factory>().GetById(filterUserModel.FactoryID);
                if (factory != null && !factory.IsFilterFactory)
                {
                    //Mông Dương thì không ràng buộc người dùng theo nhà máy
                    entities = entities.Where(x => x.FactoryID > 0);
                }
                else
                {
                    // Filter theo nhà máy
                    if (filterUserModel.FactoryID > 0)
                    {
                        entities = entities.Where(x => x.FactoryID == filterUserModel.FactoryID);
                    }
                }

                // Filter theo quyền (OR): user có ít nhất 1 trong các quyền được yêu cầu
                if (filterUserModel.PermissionKey != null && filterUserModel.PermissionKey.Count > 0)
                {
                    var permissionKeys = filterUserModel.PermissionKey;
                    entities = entities.Where(x => x.RoleInUsers.Any(y => permissionKeys.Any(p => y.UserRole.Permissions.Contains(p))));
                }

                // từ khoá
                if (!string.IsNullOrWhiteSpace(filterUserModel.SearchKey))
                {
                    entities = entities.Where(x => x.FullName.Contains(filterUserModel.SearchKey));
                }

                var listInternal = entities.Select(
                        e => new InternalExternalUserModel()
                        {
                            ID = e.ID,
                            FullName = e.FullName,
                            TempDateOfBirth = e.DateOfBirth,
                            ElectricRankValueStr = e.ElectricRank,
                            ElectricRankCard = e.ElectricRankCard,
                            LaborRankValueStr = e.LaborRank,
                            LaborCard = e.LaborCard,
                            UserType = "Nội bộ",
                            Unit = new ValueLabelModel { Value = e.TeamID, Label = e.Team.Name },
                            UserName = e.UserName,
                            IsExternal = false,
                            PositionName = e.PositionName,
                            NationalID = e.NationalID
                        }).ToList();

                lstUser.AddRange(listInternal);
            }

            if (filterUserModel.UserType == -1 || filterUserModel.UserType == 2)
            {
                var listExternal = _unitOfWork.ElogbookRepository<GuestUser>().GetQuery(x => !x.IsDeleted && x.FullName.Contains(filterUserModel.SearchKey));

                var data = listExternal.Select(
                e => new InternalExternalUserModel()
                {
                    ID = e.ID,
                    FullName = e.FullName,
                    TempDateOfBirth = e.DateOfBirth,
                    ElectricRankValueStr = e.ElectricRank,
                    ElectricRankCard = e.ElectricRankCard,
                    LaborRankValueStr = e.LaborRank,
                    LaborCard = e.LaborCard,
                    UserType = "Bên ngoài đơn vị",
                    Unit = new ValueLabelModel { },
                    UserName = e.UserName,
                    IsExternal = true,
                    PositionName = string.Empty,
                    NationalID = e.NationalID
                }).ToList();

                if (listExternal != null)
                    lstUser.AddRange(data);
            }

            return lstUser;
        }

        public UserRolesInfo GetUserRight(int userID)
        {
            var listKey = new List<string>();
            var roles = _unitOfWork.ElogbookRepository<RoleInUser>().GetQuery(x => x.UserID == userID && !x.IsDeleted).ToList();

            var _isLimited = true;
            var _isAdmin = false;
            foreach (var role in roles)
            {
                var userRole = _unitOfWork.ElogbookRepository<UserRole>().GetQuery(x => x.ID == role.UserRoleID && !x.IsDeleted).FirstOrDefault();
                if (userRole != null)
                {
                    if (userRole.IsAdmin)
                    {
                        _isAdmin = userRole.IsAdmin;
                    }
                    // Nếu tài khoản có quyền admin thì trả về full quyền
                    if (userRole.IsAdmin)
                    {
                        return new UserRolesInfo
                        {
                            UsersRight = _unitOfWork.ElogbookRepository<UserPermission>().GetQuery().Select(
                                            t => new UserRightModel()
                                            {
                                                PermissionKey = t.PermissionKey,
                                                PermissionName = t.PermissionName,
                                            }).ToList(),
                            IsLimited = false,
                            IsAdmin = _isAdmin,
                        };
                    }

                    if (userRole.DataPermission == (int)DataPermission.Unlimited && _isLimited)
                        _isLimited = false;

                    if (!string.IsNullOrWhiteSpace(userRole.Permissions))
                    {
                        var listRight = userRole.Permissions.Split(';');
                        for (int i = 0; i < listRight.Length; i++)
                        {
                            if (!listKey.Contains(listRight[i]))
                            {
                                listKey.Add(listRight[i]);
                            }
                        }
                    }
                }
            }

            var listUserRight = new List<UserRightModel>();
            if (listKey.Count > 0)
            {
                listUserRight = _unitOfWork.ElogbookRepository<UserPermission>().GetQuery(x => listKey.Contains(x.PermissionKey)).Select(
                                            t => new UserRightModel()
                                            {
                                                PermissionKey = t.PermissionKey,
                                                PermissionName = t.PermissionName,
                                            }).ToList();
            }

            return new UserRolesInfo
            {
                UsersRight = listUserRight,
                IsLimited = _isLimited,
                IsAdmin = _isAdmin,
            };
        }

        public List<SignatureInfo> GetSignatures(string signatureIds)
        {
            List<SignatureInfo> signatureInfos = new List<SignatureInfo>();
            if (!string.IsNullOrWhiteSpace(signatureIds))
            {
                List<string> result = signatureIds.Split(',').ToList();
                var ids = result.Select(int.Parse).ToList();
                var entity = _unitOfWork.ElogbookRepository<Signature>().GetQuery(x => ids.Contains(x.ID) && !x.IsDeleted).ToList();
                foreach (var item in entity)
                {
                    // Tổng số thông số của mỗi chu kỳ
                    SignatureInfo sign = new SignatureInfo()
                    {
                        ID = item.ID,
                        SignatureContentByte = item.SignatureContentByte,
                        SignatureContent = item.SignatureContent,
                        Type = item.Type
                    };
                    signatureInfos.Add(sign);
                }
            }

            return signatureInfos;
        }

        public List<ValueLabelModel> GetTicketStatus(bool isRegistrationTicket, string ticketType)
        {
            List<int> isList = new List<int>() { 1, 2, 3, 4, 5 };
            if (!isRegistrationTicket)
            {
                isList = new List<int>() { 5, 6, 7, 8, 9 };
            }

            return _unitOfWork.ElogbookRepository<Data.TicketStatus>().GetQuery(x => isList.Contains(x.ID) && !x.IsDeleted && x.Active == (int)AccessStatus.Active)
                            .Select(
                                e => new ValueLabelModel()
                                {
                                    Value = e.ID,
                                    Label = e.StatusName
                                }).OrderBy(t => t.Value).ToList();
        }

        public List<ValueLabelModel> GetTicketManipulationStatus(bool isRegistrationTicket)
        {
            List<int> isList = new List<int>() { 1, 2, 3, 4, 5 };
            if (!isRegistrationTicket)
            {
                isList = new List<int>() { 6, 7, 8, 9 };
            }

            return _unitOfWork.ElogbookRepository<Data.TicketManipulationStatus>().GetQuery(x => isList.Contains(x.ID) && !x.IsDeleted && x.Active == (int)AccessStatus.Active)
                            .Select(
                                e => new ValueLabelModel()
                                {
                                    Value = e.ID,
                                    Label = e.StatusName
                                }).OrderBy(t => t.Value).ToList();
        }

        /// <summary>
        /// Tìm mã số đơn vị đăng ký
        /// </summary>
        /// <param name="iD"></param>
        /// <returns></returns>
        public string GetRegistrationTicketNumber(int iD, string ticketType)
        {
            lock (_lockRegisterTicket)
            {
                var user = _unitOfWork.ElogbookRepository<User>().GetById(iD);
                var ticketName = string.Empty;
                if (user != null)
                {
                    var currentDate = DateTime.Now;
                    var lstNumber = _unitOfWork.ElogbookRepository<ExclusiveData>()
                    .GetQuery(x => x.InsertedAt.Year == currentDate.Year && x.TicketType.Equals(ticketType)).Select(x => x.TicketNumber).ToList();

                    var availableMin = 0;
                    var isMin = false;

                    if (lstNumber.Count > 0)
                    {
                        lstNumber.Sort();
                        foreach (var x in lstNumber)
                        {
                            availableMin += 1;
                            if (availableMin < x)
                            {
                                isMin = true;
                                break;
                            }
                        }

                        if (!isMin)
                        {
                            availableMin += 1;
                        }
                    }
                    else
                    {
                        availableMin += 1;
                    }
                    ticketName = $"{availableMin}/ĐK-{user.ProductionPlant?.ShortName}";

                    var data = new ExclusiveData()
                    {
                        TicketNumber = availableMin,
                        TicketType = ticketType,
                        InsertedAt = currentDate,
                        UpdatedAt = currentDate,
                    };
                    _unitOfWork.ElogbookRepository<ExclusiveData>().Add(data);
                    _unitOfWork.Complete();
                }

                return ticketName;
            }
        }

        /// <summary>
        /// Tìm mã số đơn vị đăng ký theo tháng năm
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="ticketType"></param>
        /// <param name="registrationTicketID"></param>
        /// <returns></returns>
        public string GetWorkTicketNumber(int userID, string ticketType, int registrationTicketID)
        {
            lock (_lockApproveTicket)
            {
                var workTicket = _unitOfWork.ElogbookRepository<WorkTicket>().GetQuery(x => x.RegistrationTicketID == registrationTicketID).FirstOrDefault();
                if (workTicket != null
                    && !string.IsNullOrWhiteSpace(workTicket.WorkTicketNumber)
                    && workTicket.MajorRepairID.GetValueOrDefault() > 0)
                {
                    return workTicket.WorkTicketNumber;
                }

                // get dữ liệu ở bảng tạm
                var user = _unitOfWork.ElogbookRepository<User>().GetById(userID);

                if (user != null)
                {
                    var currentDate = DateTime.Now;
                    var lstNumber = _unitOfWork.ElogbookRepository<ExclusiveData>()
                    .GetQuery(x => x.InsertedAt.Year == currentDate.Year && x.TicketType.Equals(ticketType)).Select(x => x.TicketNumber).ToList();

                    var availableMin = 0;
                    var isMin = false;
                    var ticketName = string.Empty;
                    if (lstNumber.Count > 0)
                    {
                        lstNumber.Sort();
                        foreach (var x in lstNumber)
                        {
                            availableMin += 1;
                            if (availableMin < x)
                            {
                                isMin = true;
                                break;
                            }
                        }

                        if (!isMin)
                        {
                            availableMin += 1;
                        }
                    }
                    else
                    {
                        availableMin += 1;
                    }

                    var ticketFormat = string.Empty;
                    switch (ticketType)
                    {
                        case "QLPTCNH":
                            if (!string.IsNullOrWhiteSpace(user.Factory?.PCTTCNHFormat))
                            {
                                ticketFormat = user.Factory.PCTTCNHFormat;
                            }
                            else
                            {
                                ticketName = $"{availableMin}/{currentDate.Year}/{user.Company?.ShortName}-{user.ProductionPlant?.ShortName}";
                            }
                            break;
                        case "QLPCTD":
                            if (!string.IsNullOrWhiteSpace(user.Factory?.PCTDFormat))
                            {
                                ticketFormat = user.Factory.PCTDFormat;
                            }
                            else
                            {
                                ticketName = $"{availableMin}/{currentDate.Year}/{user.ProductionPlant?.ShortName}-{user.Company?.ShortName}";
                            }
                            break;
                        default:
                            ticketName = string.Empty;
                            break;
                    }

                    if (!string.IsNullOrWhiteSpace(ticketFormat) && !string.IsNullOrWhiteSpace(user.Factory?.PCTDFormat))
                    {
                        var ticketFormats = ticketFormat.Split(',');
                        StringBuilder ticketNumber = new StringBuilder();
                        foreach (var item in ticketFormats)
                        {
                            switch (item)
                            {
                                case "1":
                                    ticketNumber.Append($"{availableMin}");
                                    break;
                                case "2":
                                    ticketNumber.Append($"{currentDate.Year}");
                                    break;
                                case "3":
                                    ticketNumber.Append($"{user.Factory?.ShortName}");
                                    break;
                                case "4":
                                    ticketNumber.Append($"{user.ProductionPlant?.ShortName}");
                                    break;
                                case "5":
                                    ticketNumber.Append($"{user.Company?.ShortName}");
                                    break;
                                case "6":
                                    ticketNumber.Append(CommonConstant.SLASH);
                                    break;
                                case "7":
                                    ticketNumber.Append(CommonConstant.HYPHEN);
                                    break;
                                default:
                                    break;
                            }
                        }

                        ticketName = ticketNumber.ToString();
                    }

                    var data = new ExclusiveData()
                    {
                        TicketNumber = availableMin,
                        TicketType = ticketType,
                        InsertedAt = currentDate,
                        UpdatedAt = currentDate,
                    };
                    _unitOfWork.ElogbookRepository<ExclusiveData>().Add(data);
                    _unitOfWork.Complete();

                    return ticketName;
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Tìm tên đơn vị đăng ký
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetRegistrationUnitView(int id)
        {
            var user = _unitOfWork.ElogbookRepository<User>().GetById(id);

            //gán content đơn vị công tác đăng ký
            string registerUnitView;
            try
            {
                if (user.Company != null && !string.IsNullOrEmpty(user.Company?.CompanyType))
                {
                    registerUnitView = user.Company?.Name;
                }
                else
                {
                    if (!string.IsNullOrEmpty(user.ProductionPlant?.Name) && !string.IsNullOrEmpty(user.Team?.Name))
                    {
                        registerUnitView = $"{user.ProductionPlant?.Name} > {user.Team?.Name}";
                    }
                    else if (!string.IsNullOrEmpty(user.ProductionPlant?.Name) && string.IsNullOrEmpty(user.Team?.Name))
                    {
                        registerUnitView = $"{user.ProductionPlant?.Name}";
                    }
                    else if (string.IsNullOrEmpty(user.ProductionPlant?.Name) && !string.IsNullOrEmpty(user.Team?.Name))
                    {
                        registerUnitView = $"{user.Team?.Name}";
                    }
                    else
                    {
                        registerUnitView = string.Empty;
                    }
                }
            }
            catch
            {
                registerUnitView = string.Empty;
            }

            return registerUnitView;
        }

        /// <summary>
        /// Tìm mã số đơn vị cấp phiếu của phiếu thao tác
        /// </summary>
        /// <param name="iD"></param>
        /// <returns></returns>
        public string GetManipulationUnitApprover(int iD)
        {
            var user = _unitOfWork.ElogbookRepository<Data.User>().GetById(iD);

            //gán content đơn vị cấp phiếu
            string registerUnitView;
            try
            {
                registerUnitView = $"{user.Company?.Name} > {user.ProductionPlant?.Name}";
            }
            catch
            {
                registerUnitView = string.Empty;
            }

            return registerUnitView;
        }

        /// <summary>
        /// Tìm đơn vị đơn vị cấp phiếu (chỉ lấy phân xưởng vận hành)
        /// </summary>
        /// <returns></returns>
        public List<ValueLabelModel> GetUnitApprove()
        {
            var organizations = _unitOfWork.ElogbookRepository<Organization>().GetQuery(x => !x.IsDeleted
                                                                                    && x.Active == (int)AccessStatus.Active);

            var list = new List<ValueLabelModel>
            {
                new ValueLabelModel { Value = CommonConstant.ALL_KEY, Label = CommonConstant.ALL_VALUE },
            };

            var data = organizations.Where(x => x.ParentID != null && x.Level.Equals("phân xưởng", StringComparison.OrdinalIgnoreCase) && x.ProductionPlantType.Equals("Vận hành", StringComparison.OrdinalIgnoreCase)).ToList();

            foreach (var item in data)
            {
                list.Add(new ValueLabelModel()
                {
                    Value = item.ID,
                    Label = item.Name
                });
            }

            return list.OrderBy(x => x.Value).ToList();
        }

        /// <summary>
        /// Tìm số phiếu thao tác
        /// </summary>
        /// <param name="idManipulationTicket"></param>
        /// <returns></returns>
        public string GetTicketNumberManipulation(int idManipulationTicket)
        {
            var manipulationTicket = _unitOfWork.ElogbookRepository<RegistrationManipulationTicket>().GetById(idManipulationTicket);

            //gán content đơn vị công tác đăng ký
            string ticketNumber = string.Empty;
            try
            {
                var year = DateTime.Now.Year;
                string ticketType = manipulationTicket.TicketTypeValue == 1 ? "KH" : "ĐX";

                var entities = _unitOfWork.ElogbookRepository<ManipulationTicket>().GetQuery(x => x.InsertedAt.Year == year).ToList();
                int max = 0;
                for (int i = 0; i < entities.Count; i++)
                {
                    ManipulationTicket item = entities[i];
                    try
                    {
                        string numStr = item.TicketNumber.Substring(0, item.TicketNumber.IndexOf('/'));
                        if (int.TryParse(numStr, out int num))
                        {
                            max = (max <= num) ? num : max;
                        }
                    }
                    catch
                    {
                        //Bỏ qua lỗi để chạy tiếp
                    }
                }

                ticketNumber = $"{(max + 1).ToString().PadLeft(3, '0')}/{year}/{ticketType}/{manipulationTicket.ApproveUnit.ShortName}";
                if (manipulationTicket.Factory != null && !string.IsNullOrWhiteSpace(manipulationTicket.Factory.PTTFormat))
                {
                    var ticketFormats = manipulationTicket.Factory.PTTFormat.Split(',');
                    StringBuilder sbTicketNumber = new StringBuilder();
                    foreach (var item in ticketFormats)
                    {
                        switch (item)
                        {
                            case "1":
                                sbTicketNumber.Append($"{(max + 1).ToString().PadLeft(3, '0')}");
                                break;
                            case "2":
                                sbTicketNumber.Append($"{year}");
                                break;
                            case "3":
                                sbTicketNumber.Append($"{ticketType}");
                                break;
                            case "4":
                                sbTicketNumber.Append($"{manipulationTicket.Factory.ShortName}");
                                break;
                            case "5":
                                sbTicketNumber.Append($"{manipulationTicket.ApproveUnit.ShortName}");
                                break;
                            case "6":
                                sbTicketNumber.Append(CommonConstant.SLASH);
                                break;
                            case "7":
                                sbTicketNumber.Append(CommonConstant.HYPHEN);
                                break;
                            default:
                                break;
                        }
                    }

                    ticketNumber = sbTicketNumber.ToString();
                }
            }
            catch
            {
                ticketNumber = string.Empty;
            }

            return ticketNumber;
        }

        /// <summary>
        /// Danh sách nhà máy
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public List<ValueLabelModel> GetFactoriesActive()
        {
            return _unitOfWork.ElogbookRepository<Factory>().GetQuery(x => x.Status == (int)AccessStatus.Active && !x.IsDeleted)
                            .Select(
                                e => new ValueLabelModel()
                                {
                                    Value = e.ID,
                                    Label = e.FactoryName
                                }).OrderBy(t => t.Label).ToList();
        }

        /// <summary>
        /// Lấy danh sách đơn vị công tác theo công ty
        /// </summary>
        /// <param name="companyID"></param>
        /// <returns></returns>
        public List<ValueLabelModel> GetWorkUnitByCompany(int companyID)
        {
            var organizations = _unitOfWork.ElogbookRepository<Organization>()
                .GetQuery(x => x.ParentID == companyID
                && x.Level.Equals("Phân xưởng", StringComparison.OrdinalIgnoreCase)
                && x.ProductionPlantType.Equals("Vận hành", StringComparison.OrdinalIgnoreCase)
                && !x.IsDeleted && x.Active == (int)AccessStatus.Active).ToList();

            var list = new List<ValueLabelModel>
            {
                new ValueLabelModel { Value = CommonConstant.ALL_KEY, Label = CommonConstant.ALL_VALUE },
            };

            foreach (var item in organizations)
            {
                list.Add(new ValueLabelModel()
                {
                    Value = item.ID,
                    Label = item.Name
                });
            }

            return list.OrderBy(x => x.Value).ToList();
        }

        /// <summary>
        /// Phương thức ký cấp/khóa phiếu công tác
        /// </summary>
        /// <returns></returns>
        public List<ValueLabelModel> GetSigningMethods()
        {
            return _unitOfWork.ElogbookRepository<SigningMethod>().GetQuery(x => !x.IsDeleted && x.Active == (int)AccessStatus.Active)
                .Select(
                    e => new ValueLabelModel()
                    {
                        Value = e.ID,
                        Label = e.SigningMethodName
                    }).OrderBy(t => t.Label).ToList();
        }

        public List<ValueLabelModel> GetEmailTemplates()
        {
            return _unitOfWork.ElogbookRepository<EmailTemplate>().GetQuery(x => !x.IsDeleted && x.Active == (int)AccessStatus.Active)
                .Select(
                    e => new ValueLabelModel()
                    {
                        Value = e.ID,
                        Label = e.EmailSubject,
                    }).OrderBy(t => t.Label).ToList();
        }

        /// <summary>
        /// Validate username and password of user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool IsValidUserAndPassword(string userName, string password)
        {
            var userInfo = _unitOfWork.ElogbookRepository<User>().GetQuery(x => x.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)
                                                                   && !x.IsDeleted
                                                                   && x.Status == (int)AccessStatus.Active).FirstOrDefault();
            if (userInfo == null)
            {
                return false;
            }
            if (RuntimeManager.ValidatePassword(password, userInfo.Password))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Chức năng hoạt động trên mobile
        /// </summary>
        /// <returns></returns>
        public List<ValueLabelModel> GetMobileActivities()
        {
            return GetSystemData((int)SystemDataType.MobileActivity);
        }

        /// <summary>
        /// Lấy các event theo loại phiếu
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public List<ValueLabelModel> GetTicketEventTypes(int dataType)
        {
            var ticketEventTypes = new List<ValueLabelModel>
                            {
                                new ValueLabelModel { Value = CommonConstant.ALL_KEY, Label = CommonConstant.ALL_VALUE },
                            };
            ticketEventTypes.AddRange(GetSystemData(dataType));

            return ticketEventTypes;
        }

        /// <summary>
        /// Đồng bộ sử lý phiếu công tác sang theo dõi tồn tại ca
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="currentUserID"></param>
        /// <param name="workTicketID"></param>
        /// <param name="checkCompletedTime"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        public void AsyncStatusAndContent(int eventType, int currentUserID, int workTicketID, DateTime? checkCompletedTime, DateTime? startTime, DateTime? endTime)
        {
            try
            {
                var appSetting = _unitOfWork.ElogbookRepository<AppVersion>().GetQuery(e => !e.IsDeleted).FirstOrDefault();
                if (appSetting != null && appSetting.IsSyncData_PCT_TDTTC)
                {
                    var workTicket = _unitOfWork.ElogbookRepository<WorkTicket>().GetQuery(x => x.ID == workTicketID && !x.IsDeleted).FirstOrDefault();
                    if (workTicket != null)
                    {
                        var eventTypeDescription = ((EventType)eventType).AsString(EnumFormat.Description);
                        int screenType = workTicket.ScreenType.GetValueOrDefault() == 2 ? 6 : 5;
                        var ticketEvent = _unitOfWork.ElogbookRepository<TicketEvent>()
                            .GetQuery(x => x.FactoryID == workTicket.FactoryID
                                        //5: PCTĐ, 6: PCT-TCNH
                                        && x.DataType == screenType
                                        && x.EventType.Description.Equals(eventTypeDescription, StringComparison.OrdinalIgnoreCase)
                                        && !x.IsDeleted).FirstOrDefault();
                        var messageContent = string.Empty;
                        var statusID = 0;

                        if (ticketEvent != null)
                        {
                            switch (eventType)
                            {
                                case (int)EventType.PCTD_APPROVED:
                                case (int)EventType.PCT_TCNH_APPROVED:
                                    //Cấp PCT {NUMBER_TICKET} lúc {DATE}.
                                    //Cấp PCT {NUMBER_TICKET} lúc {DATE} (lưu sổ NKVH của trưởng ca).
                                    messageContent = ticketEvent.EventContent.Replace("{NUMBER_TICKET}", workTicket.WorkTicketNumber);
                                    messageContent = messageContent.Replace("{DATE}", checkCompletedTime.HasValue ? checkCompletedTime.Value.ToString("dd/MM/yyyy HH:mm") : "");
                                    statusID = (int)IssueOfShiftStatus.NOT_PROCESS;
                                    break;

                                case (int)EventType.PCTD_ALLOWED:
                                case (int)EventType.PCT_TCNH_ALLOWED:
                                    //Cho phép PCT {NUMBER_TICKET} bắt đầu làm việc lúc {DATE}.
                                    //Cho phép PCT {NUMBER_TICKET} bắt đầu làm việc lúc {DATE} (Lưu sổ VHV).
                                    messageContent = ticketEvent.EventContent.Replace("{NUMBER_TICKET}", workTicket.WorkTicketNumber);
                                    messageContent = messageContent.Replace("{DATE}", checkCompletedTime.HasValue ? checkCompletedTime.Value.ToString("dd/MM/yyyy HH:mm") : "");
                                    statusID = (int)IssueOfShiftStatus.IN_PROGRESS;
                                    break;

                                case (int)EventType.PCTD_START:
                                    var countWorkerStart = _unitOfWork.ElogbookRepository<WorkerInformation>().GetQuery(x => x.WorkTicketID == workTicketID
                                        && DbFunctions.TruncateTime(x.CheckInTime) >= DbFunctions.TruncateTime(startTime.Value) && !x.IsDeleted).Count();
                                    //PCT {NUMBER_TICKET} bắt đầu công tác hàng ngày lúc {DATE}. Số nhân viên đơn vị công tác: {NUMBER}.
                                    messageContent = ticketEvent.EventContent.Replace("{NUMBER_TICKET}", workTicket.WorkTicketNumber);
                                    messageContent = messageContent.Replace("{DATE}", startTime.HasValue ? startTime.Value.ToString("dd/MM/yyyy HH:mm") : "");
                                    messageContent = messageContent.Replace("{NUMBER}", countWorkerStart.ToString());
                                    statusID = (int)IssueOfShiftStatus.IN_PROGRESS;
                                    break;

                                case (int)EventType.PCTD_END:
                                    var countWorkerEnd = _unitOfWork.ElogbookRepository<WorkerInformation>().GetQuery(x => x.WorkTicketID == workTicketID
                                    && DbFunctions.TruncateTime(x.CheckOutTime) >= DbFunctions.TruncateTime(endTime.Value) && !x.IsDeleted).Count();

                                    //PCT {NUMBER_TICKET} kết thúc công tác hàng ngày lúc {DATE}. Số nhân viên đơn vị công tác: {NUMBER}.
                                    messageContent = ticketEvent.EventContent.Replace("{NUMBER_TICKET}", workTicket.WorkTicketNumber);
                                    messageContent = messageContent.Replace("{DATE}", endTime.HasValue ? endTime.Value.ToString("dd/MM/yyyy HH:mm") : "");
                                    messageContent = messageContent.Replace("{NUMBER}", countWorkerEnd.ToString());
                                    statusID = (int)IssueOfShiftStatus.IN_PROGRESS;
                                    break;

                                case (int)EventType.PCTD_LOCKED:
                                case (int)EventType.PCTD_COMPLETED:
                                case (int)EventType.PCT_TCNH_COMPLETED:
                                    //Khóa kết thúc công tác PCT {NUMBER_TICKET} lúc {DATE} (lưu sổ VHV).
                                    //Khóa kết thúc công tác PCT {NUMBER_TICKET} lúc {DATE} (sổ Trưởng ca).
                                    //Khóa kết thúc công tác PCT {NUMBER_TICKET} lúc {DATE} (lưu sổ VHV).
                                    messageContent = ticketEvent.EventContent.Replace("{NUMBER_TICKET}", workTicket.WorkTicketNumber);
                                    messageContent = messageContent.Replace("{DATE}", checkCompletedTime.HasValue ? checkCompletedTime.Value.ToString("dd/MM/yyyy HH:mm") : "");
                                    statusID = (int)IssueOfShiftStatus.DONE;
                                    break;
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(messageContent))
                        {
                            var entityExistInShift = _unitOfWork.ElogbookRepository<ExistInShift>().GetQuery(x => x.RegistrationTicketID == workTicketID && !x.IsDeleted).FirstOrDefault();
                            if (entityExistInShift != null)
                            {
                                // thêm nội dung vào table IssueOfShiftDetail
                                var entity = new IssueOfShiftDetail()
                                {
                                    ExistInShiftID = entityExistInShift.ID,
                                    Content = messageContent,
                                    CreatedUserID = currentUserID,
                                    LastUpdatedUserID = currentUserID,
                                };

                                _unitOfWork.ElogbookRepository<IssueOfShiftDetail>().Add(entity);
                                if (_unitOfWork.Complete() > 0)
                                {
                                    UpdateStatusIssueOfShift(entityExistInShift.ID, currentUserID, statusID);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "Common", "Lỗi đồng bộ dữ liệu phiếu thao tác:" + Environment.NewLine + ex.ToString());
            }
        }

        public AmSystemDataModel GetAmCommonData(string categoryNames, int factoryID, int companyID)
        {
            var result = new AmSystemDataModel();
            if (!string.IsNullOrWhiteSpace(categoryNames))
            {
                var categoryNameArray = categoryNames.Split(',');
                if (categoryNameArray.Length > 0)
                {
                    for (int i = 0; i < categoryNameArray.Length; i++)
                    {
                        var categoryName = categoryNameArray[i];
                        if (!string.IsNullOrWhiteSpace(categoryName))
                        {
                            categoryName = categoryName.Trim().ToLower();

                            switch (categoryName)
                            {
                                case AmCategories.AM_USAGE_STATUS:
                                    result.AmUsageStatus = new List<ValueLabelCheckedModel>
                                        {
                                            new ValueLabelCheckedModel { Value = CommonConstant.ALL_KEY, Label = CommonConstant.ALL_VALUE },
                                            new ValueLabelCheckedModel { Value = (int)AmEnumUsageStatus.Apply, Label = EnumExtensions.GetEnumDescription(AmEnumUsageStatus.Apply)},
                                            new ValueLabelCheckedModel { Value = (int)AmEnumUsageStatus.UnApply, Label = EnumExtensions.GetEnumDescription(AmEnumUsageStatus.UnApply) }
                                        };
                                    break;
                                case AmCategories.AM_STATUS:
                                    result.AmStatus = _unitOfWork.ElogbookRepository<AmStatus>()
                                        .GetQuery(x => !x.IsDeleted)
                                        .Select(e => new ValueLabelCheckedModel() { Value = e.ID, Label = e.StatusName }).OrderBy(t => t.Value).ToList();
                                    break;
                                case AmCategories.AM_DEVICE:
                                    result.AmDevices = _unitOfWork.ElogbookRepository<AmDevice>()
                                        .GetQuery(x => !x.IsDeleted && x.Active == (int)AmEnumUsageStatus.Apply)
                                        .Select(e => new ComboboxDataModel()
                                        {
                                            Value = e.ID,
                                            Label = e.AssetDesc,
                                            Code = e.AssetCode
                                        }).OrderBy(t => t.Value).ToList();
                                    break;
                                case AmCategories.AM_DEVICETYPE:
                                    result.AmDeviceTypes = _unitOfWork.ElogbookRepository<AmDeviceType>()
                                        .GetQuery(x => !x.IsDeleted && x.Active == (int)AmEnumUsageStatus.Apply)
                                        .Select(e => new ComboboxDataModel()
                                        {
                                            Value = e.ID,
                                            Label = e.TypeDesc,
                                            Code = e.TypeCode
                                        }).OrderBy(t => t.Value).ToList();
                                    break;
                                case AmCategories.AM_LOCATION:
                                    result.AmLocations = _unitOfWork.ElogbookRepository<AmLocation>()
                                        .GetQuery(x => !x.IsDeleted && x.Active == (int)AmEnumUsageStatus.Apply && x.OrganizationID == companyID)
                                        .Select(e => new ComboboxDataModel()
                                        {
                                            Value = e.ID,
                                            Label = e.SiteDesc,
                                            Code = e.SiteCode
                                        }).OrderBy(t => t.Value).ToList();
                                    break;
                                case AmCategories.AM_REASON_FOR_PENDING:
                                    result.AmReasonForPendings = _unitOfWork.ElogbookRepository<AmReasonForPending>()
                                        .GetQuery(x => !x.IsDeleted && x.Active == (int)AmEnumUsageStatus.Apply && (x.FactoryID == factoryID || factoryID == CommonConstant.ALL_KEY))
                                        .Select(e => new ValueLabelCheckedModel()
                                        {
                                            Value = e.ID,
                                            Label = e.ReasonDesc,
                                        }).OrderBy(t => t.Value).ToList();
                                    break;
                                case AmCategories.AM_REASON_FOR_STOP:
                                    result.AmReasonForStops = _unitOfWork.ElogbookRepository<AmReasonForStop>()
                                        .GetQuery(x => !x.IsDeleted && x.Active == (int)AmEnumUsageStatus.Apply && x.FactoryID == factoryID)
                                        .Select(e => new ComboboxDataModel()
                                        {
                                            Value = e.ID,
                                            Label = e.ReasonDesc,
                                            Code = e.ReasonType == 1 ? "Nguyên nhân không thực hiện bảo trì" : "Nguyên nhân dừng thực hiện công việc"
                                        }).OrderBy(t => t.Value).ToList();
                                    break;
                                case AmCategories.AM_WORK:
                                    result.AmWorks = _unitOfWork.ElogbookRepository<AmWork>()
                                        .GetQuery(x => !x.IsDeleted && x.Active == (int)AmEnumUsageStatus.Apply && (x.FactoryID == factoryID || factoryID == CommonConstant.ALL_KEY))
                                        .Select(e => new ValueLabelCheckedModel()
                                        {
                                            Value = e.ID,
                                            Label = e.WorkName
                                        }).OrderBy(t => t.Value).ToList();
                                    break;
                                case AmCategories.AM_WORKTYPE:
                                    result.AmWorkTypes = _unitOfWork.ElogbookRepository<AmWorkType>()
                                        .GetQuery(x => !x.IsDeleted && x.Active == (int)AmEnumUsageStatus.Apply && (x.FactoryID == factoryID || factoryID == CommonConstant.ALL_KEY))
                                        .Select(e => new ValueLabelCheckedModel()
                                        {
                                            Value = e.ID,
                                            Label = e.TypeDesc
                                        }).OrderBy(t => t.Value).ToList();
                                    break;
                                case AmCategories.AM_DEVICE_ABNORMAL_STATUS:
                                    result.AmDeviceAbnormalStatus = _unitOfWork.ElogbookRepository<AmDeviceAbnormalStatus>()
                                        .GetQuery(x => !x.IsDeleted)
                                        .Select(e => new ValueLabelCheckedModel()
                                        {
                                            Value = e.ID,
                                            Label = e.Name
                                        }).OrderBy(t => t.Value).ToList();
                                    break;
                                case AmCategories.AM_REASON_TYPE:
                                    result.AmReasonType = new List<ValueLabelCheckedModel>
                                        {
                                            new ValueLabelCheckedModel { Value = CommonConstant.ALL_KEY, Label = CommonConstant.ALL_VALUE },
                                            new ValueLabelCheckedModel { Value = (int)AmEnumReasonType.StopAM, Label = EnumExtensions.GetEnumDescription(AmEnumReasonType.StopAM)},
                                            new ValueLabelCheckedModel { Value = (int)AmEnumReasonType.StopWork, Label = EnumExtensions.GetEnumDescription(AmEnumReasonType.StopWork) }
                                        };
                                    break;
                                case AmCategories.AM_DEVICE_CATEGORY:
                                    result.AmDeviceCategories = _unitOfWork.ElogbookRepository<AmDeviceCategory>()
                                        .GetQuery(x => !x.IsDeleted && x.Active == (int)AmEnumUsageStatus.Apply && (x.OrgID == companyID || companyID == CommonConstant.ALL_KEY))
                                        .Select(e => new ComboboxDataModel()
                                        {
                                            Value = e.ID,
                                            Label = e.CategoryName,
                                            Code = e.CategoryCode
                                        }).OrderBy(t => t.Value).ToList();
                                    break;
                                case AmCategories.AM_COUNTRY_MANUFACTURE:
                                    result.AmCountryManufactures = _unitOfWork.ElogbookRepository<AmCountryManufacture>()
                                        .GetQuery(x => !x.IsDeleted && x.Active == (int)AmEnumUsageStatus.Apply)
                                        .Select(e => new ComboboxDataModel()
                                        {
                                            Value = e.ID,
                                            Label = e.CountryName,
                                            Code = e.CountryCode
                                        }).OrderBy(t => t.Value).ToList();
                                    break;
                                case AmCategories.AM_SUPPLIER:
                                    result.AmSuppliers = _unitOfWork.ElogbookRepository<AmSupplier>()
                                        .GetQuery(x => !x.IsDeleted && x.Active == (int)AmEnumUsageStatus.Apply)
                                        .Select(e => new ComboboxDataModel()
                                        {
                                            Value = e.ID,
                                            Label = e.SupplierName,
                                            Code = e.SupplierCode
                                        }).OrderBy(t => t.Value).ToList();
                                    break;
                                case AmCategories.AM_MANUFACTURER:
                                    result.AmManufacturers = _unitOfWork.ElogbookRepository<AmManufacturer>()
                                        .GetQuery(x => !x.IsDeleted && x.Active == (int)AmEnumUsageStatus.Apply)
                                        .Select(e => new ComboboxDataModel()
                                        {
                                            Value = e.ID,
                                            Label = e.ManufacturerName,
                                            Code = e.ManufacturerCode
                                        }).OrderBy(t => t.Value).ToList();
                                    break;
                                case AmCategories.AM_DEVICE_STATUS:
                                    result.AmDeviceStatus = _unitOfWork.ElogbookRepository<AmDeviceStatus>()
                                        .GetQuery(x => !x.IsDeleted)
                                        .Select(e => new ValueLabelCheckedModel()
                                        {
                                            Value = e.ID,
                                            Label = e.StatusDesc
                                        }).OrderBy(t => t.Value).ToList();
                                    break;
                                case AmCategories.AM_EMERGENCY:
                                    result.AmEmergencies = _unitOfWork.ElogbookRepository<AmEmergency>()
                                        .GetQuery(x => !x.IsDeleted && x.Active == (int)AmEnumUsageStatus.Apply && (x.FactoryID == factoryID || factoryID == CommonConstant.ALL_KEY))
                                        .Select(e => new ValueLabelCheckedModel()
                                        {
                                            Value = e.ID,
                                            Label = e.Name
                                        }).OrderBy(t => t.Value).ToList();
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// tìm mã số phiếu yêu cầu
        /// </summary>
        /// <param name="iD"></param>
        /// <returns></returns>
        public string GetRequestTicketNumber(int iD)
        {
            lock (_lockRequestTicket)
            {
                var ticketType = "PYC";
                var user = _unitOfWork.ElogbookRepository<User>().GetById(iD);
                var currentDate = DateTime.Now;
                var lstNumber = _unitOfWork.ElogbookRepository<ExclusiveData>()
                    .GetQuery(x => x.InsertedAt.Year == currentDate.Year && x.TicketType.Equals(ticketType)).Select(x => x.TicketNumber).ToList();

                var availableMin = 0;
                var isMin = false;
                var ticketName = string.Empty;
                if (lstNumber.Count > 0)
                {
                    lstNumber.Sort();
                    foreach (var x in lstNumber)
                    {
                        availableMin += 1;
                        if (availableMin < x)
                        {
                            isMin = true;
                            break;
                        }
                    }

                    if (!isMin)
                    {
                        availableMin += 1;
                    }
                }
                else
                {
                    availableMin += 1;
                }
                ticketName = $"{user.ProductionPlant?.ShortName}/PYC/{currentDate.Year}-{availableMin}";

                var pyc = _unitOfWork.ElogbookRepository<AmRequestTicket>().GetQuery(x => x.RequestTicketNumber.Equals(ticketName)).FirstOrDefault();
                if (pyc == null)
                {
                    var data = new ExclusiveData()
                    {
                        TicketNumber = availableMin,
                        TicketType = ticketType,
                        InsertedAt = currentDate,
                        UpdatedAt = currentDate,
                    };
                    _unitOfWork.ElogbookRepository<ExclusiveData>().Add(data);
                    _unitOfWork.Complete();
                }

                return ticketName;
            }
        }

        /// <summary>
        /// Tìm mã số phiếu yêu cầu theo công ty
        /// </summary>
        /// <param name="factoryID"></param>
        /// <returns></returns>
        public string GetRequestTicketNumberByFactory(int factoryID)
        {
            lock (_lockRequestTicketFactory)
            {
                var currentDate = DateTime.Now;
                var ticketType = "PYC";
                var fac = _unitOfWork.ElogbookRepository<Factory>().GetById(factoryID);
                var lstNumber = _unitOfWork.ElogbookRepository<ExclusiveData>()
                    .GetQuery(x => x.InsertedAt.Year == currentDate.Year && x.TicketType.Equals(ticketType)).Select(x => x.TicketNumber).ToList();

                var availableMin = 0;
                var isMin = false;
                var ticketName = string.Empty;
                if (lstNumber.Count > 0)
                {
                    lstNumber.Sort();
                    foreach (var x in lstNumber)
                    {
                        availableMin += 1;
                        if (availableMin < x)
                        {
                            isMin = true;
                            break;
                        }
                    }

                    if (!isMin)
                    {
                        availableMin += 1;
                    }
                }
                else
                {
                    availableMin += 1;
                }
                ticketName = $"{fac.ShortName}/PYC/{currentDate.Year}-{availableMin}";

                var data = new ExclusiveData()
                {
                    TicketNumber = availableMin,
                    TicketType = ticketType,
                    InsertedAt = currentDate,
                    UpdatedAt = currentDate,
                };
                _unitOfWork.ElogbookRepository<ExclusiveData>().Add(data);
                _unitOfWork.Complete();

                return ticketName;
            }
        }

        private void UpdateStatusIssueOfShift(int existInShiftID, int updatedUserID, int statusID)
        {
            // Cập nhật trạng thái của tồn tại
            var entityExistInShift = _unitOfWork.ElogbookRepository<ExistInShift>().GetById(existInShiftID);
            if (entityExistInShift != null)
            {
                entityExistInShift.StatusID = statusID;
                entityExistInShift.LastUpdatedUserID = updatedUserID;
                if (statusID == (int)IssueOfShiftStatus.DONE)
                    entityExistInShift.CompletedDateTime = DateTime.Now;

                entityExistInShift.SyncedDate = DateTime.Now;
                _unitOfWork.ElogbookRepository<ExistInShift>().Update(entityExistInShift);

                var operationDiary = _unitOfWork.ElogbookRepository<OperationDiary>().GetById(entityExistInShift.OperationDiaryID);
                if (operationDiary != null)
                {
                    operationDiary.LastUpdatedUserID = updatedUserID;
                    operationDiary.SyncedDate = DateTime.Now;
                    _unitOfWork.ElogbookRepository<OperationDiary>().Update(operationDiary);
                }

                _unitOfWork.Complete();
            }
        }

        /// <summary>
        /// lấy những user cùng nhà máy & và quyền được input
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public List<UserModel> GetUserAcceptTicketByFactory(int factory, string permissions)
        {
            var getUserRoles = _unitOfWork.ElogbookRepository<UserRole>().GetQuery(x => x.Permissions.Contains(permissions)).Select(x => x.ID).ToList();
            var userID = _unitOfWork.ElogbookRepository<RoleInUser>().GetQuery(x => getUserRoles.Contains(x.UserRoleID)).Select(x => x.UserID).ToList();
            List<UserModel> userModels = new List<UserModel>();
            foreach (var item in userID)
            {
                var user = _unitOfWork.ElogbookRepository<User>().GetById(item);
                if (user.FactoryID.HasValue && user.FactoryID.Value == factory)
                {
                    userModels.Add(new UserModel
                    {
                        Email = user.Email,
                        FullName = user.FullName
                    });
                }
            }

            return userModels;
        }

        public ResponseModel<bool> SyncData(DataSyncModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>
            {
                StatusCode = (int)HttpStatusCode.BadRequest
            };

            if (requestModel.DataSyncs.Count > 0)
            {
                // Remove duplicate
                List<TrackingLoginSyncModel> newList = new List<TrackingLoginSyncModel>();
                List<string> temps = new List<string>();
                foreach (var item in requestModel.DataSyncs)
                {
                    string text = item.OperationPositionID + "_" + item.CreatedUserID;

                    if (!temps.Contains(text))
                    {
                        newList.Add(item);
                        temps.Add(text);
                    }
                }

                // Insert DB
                foreach (var item in newList)
                {
                    var trackingLogin = _unitOfWork.ElogbookRepository<TrackingLoginOperation>()
                        .GetQuery(x => !x.IsDeleted && x.OperationPositionID == item.OperationPositionID && x.CreatedUserID == item.CreatedUserID)
                        .FirstOrDefault();

                    // Kiểm tra Người dùng và Cương vị có tồn tại trong hệ thống
                    bool isExistsUser = _unitOfWork.ElogbookRepository<User>().GetQuery(x => x.ID == item.CreatedUserID && !x.IsDeleted).Any();
                    bool isExistsOpePosition = _unitOfWork.ElogbookRepository<OperationPosition>().GetQuery(x => x.ID == item.OperationPositionID && !x.IsDeleted).Any();

                    if (!isExistsUser || !isExistsOpePosition)
                    {
                        continue;
                    }

                    if (trackingLogin != null)
                    {
                        trackingLogin.LastUpdatedUserID = item.LastUpdatedUserID;
                        trackingLogin.CreatedUserID = item.CreatedUserID;
                        trackingLogin.UpdatedAt = item.UpdatedAt;
                        trackingLogin.InsertedAt = item.InsertedAt;
                        trackingLogin.IsDeleted = item.IsDeleted;
                        _unitOfWork.ElogbookRepository<TrackingLoginOperation>().Update(trackingLogin);
                    }
                    else
                    {
                        var entity = new TrackingLoginOperation
                        {
                            OperationPositionID = item.OperationPositionID,
                            LastUpdatedUserID = item.LastUpdatedUserID > 0 ? item.LastUpdatedUserID : null,
                            CreatedUserID = item.CreatedUserID,
                            UpdatedAt = item.UpdatedAt,
                            InsertedAt = item.InsertedAt,
                            IsDeleted = item.IsDeleted,
                        };
                        _unitOfWork.ElogbookRepository<TrackingLoginOperation>().Add(entity);
                    }
                }
                if (_unitOfWork.Complete() > 0)
                {
                    responseModel.StatusCode = (int)HttpStatusCode.OK;
                    responseModel.Data = true;
                }
            }

            return responseModel;
        }

        public List<ValueLabelModel> GetLctTicketStatus(string ticketType)
        {
            return _unitOfWork.ElogbookRepository<Data.WorkPermit.LctTicketStatus>().GetQuery(x => !x.IsDeleted && x.Active == (int)AccessStatus.Active && (ticketType == null || ticketType == x.TicketType))
                            .Select(
                                e => new ValueLabelModel()
                                {
                                    Value = e.ID,
                                    Label = e.StatusName
                                }).OrderBy(t => t.Value).ToList();
        }

        #endregion

        #region Private methods

        private List<ValueLabelModel> GetProductionPlants(int companyID, string filterDescription = "")
        {
            return _unitOfWork.ElogbookRepository<Organization>().GetQuery(x => !x.IsDeleted
                                                                                    && x.Active == (int)AccessStatus.Active
                                                                                    && "Phân xưởng".Equals(x.Level, StringComparison.OrdinalIgnoreCase)
                                                                                    && (x.ParentID == companyID)
                                                                                    && x.Name.Contains(filterDescription))
                .Select(
                    e => new ValueLabelModel()
                    {
                        Value = e.ID,
                        Label = e.Name
                    }).OrderBy(t => t.Label).ToList();
        }

        private List<ValueLabelModel> GetUserRoles(int userID)
        {
            // Tài khoản người dùng có vai trò admin
            var isAdmin = _unitOfWork.ElogbookRepository<RoleInUser>().GetQuery(x => x.UserID == userID && x.UserRole.IsAdmin).Any();
            if (isAdmin)
            {
                return _unitOfWork.ElogbookRepository<UserRole>().GetQuery(x => !x.IsDeleted && x.Active == (int)AccessStatus.Active)
                .Select(
                    e => new ValueLabelModel()
                    {
                        Value = e.ID,
                        Label = e.RoleName
                    }).OrderBy(t => t.Label).ToList();
            }
            else
            {
                return _unitOfWork.ElogbookRepository<UserRole>().GetQuery(x => !x.IsDeleted && x.Active == (int)AccessStatus.Active && !x.IsAdmin)
                .Select(
                    e => new ValueLabelModel()
                    {
                        Value = e.ID,
                        Label = e.RoleName
                    }).OrderBy(t => t.Label).ToList();
            }
        }

        private List<ValueLabelModel> GetPowerUnits()
        {
            return _unitOfWork.ElogbookRepository<PowerUnit>().GetQuery(x => !x.IsDeleted && x.Active == (int)AccessStatus.Active)
                .Select(
                    e => new ValueLabelModel()
                    {
                        Value = e.ID,
                        Label = e.PowerUnitName
                    }).OrderBy(t => t.Label).ToList();
        }

        private List<ValueLabelModel> GetOperationShifts()
        {
            return _unitOfWork.ElogbookRepository<OperationShift>().GetQuery(x => !x.IsDeleted && x.Active == (int)AccessStatus.Active)
                            .Select(
                                e => new ValueLabelModel()
                                {
                                    Value = e.ID,
                                    Label = e.OperationShiftName
                                }).OrderBy(t => t.Label).ToList();
        }

        private List<ValueLabelModel> GetOperationCrews()
        {
            return _unitOfWork.ElogbookRepository<OperationCrew>().GetQuery(x => !x.IsDeleted && x.Active == (int)AccessStatus.Active)
                            .Select(
                                e => new ValueLabelModel()
                                {
                                    Value = e.ID,
                                    Label = e.OperationCrewName
                                }).OrderBy(t => t.Label).ToList();
        }

        private ValueLabelModel GetDeliveryShiftMethodTemplate()
        {
            return new ValueLabelModel();
        }

        private List<ValueLabelModel> GetTicketStatus()
        {
            return _unitOfWork.ElogbookRepository<Data.TicketStatus>().GetQuery(x => !x.IsDeleted && x.Active == (int)AccessStatus.Active)
                            .Select(
                                e => new ValueLabelModel()
                                {
                                    Value = e.ID,
                                    Label = e.StatusName
                                }).OrderBy(t => t.Value).ToList();
        }

        private List<ValueLabelModel> GetUsers(string filterDescription = "")
        {
            return _unitOfWork.ElogbookRepository<Data.User>().GetQuery(x => !x.IsDeleted && x.Status == (int)UserStatus.Active && x.FullName.Contains(filterDescription))
                            .Select(
                                e => new ValueLabelModel()
                                {
                                    Value = e.ID,
                                    Label = e.FullName
                                }).OrderBy(t => t.Value).ToList();
        }

        private List<ValueLabelModel> GetOperationPositions(string filterDescription = "")
        {
            return _unitOfWork.ElogbookRepository<OperationPosition>().GetQuery(x => !x.IsDeleted && x.Active == (int)AccessStatus.Active && x.OperationPositionName.Contains(filterDescription))
                            .Select(
                                e => new ValueLabelModel()
                                {
                                    Value = e.ID,
                                    Label = e.OperationPositionName
                                }).OrderBy(t => t.Value).ToList();
        }

        private List<ValueLabelModel> GetCycles(string filterDescription = "")
        {
            return _unitOfWork.ElogbookRepository<CycleManagement>().GetQuery(x => !x.IsDeleted && x.Active == (int)AccessStatus.Active && x.CycleName.Contains(filterDescription))
                            .Select(
                                e => new ValueLabelModel()
                                {
                                    Value = e.ID,
                                    Label = e.CycleName
                                }).OrderBy(t => t.Value).ToList();
        }

        private List<ValueLabelModel> GetParameterGroups(string filterDescription = "")
        {
            return _unitOfWork.ElogbookRepository<ParameterGroup>().GetQuery(x => !x.IsDeleted && x.Status == (int)AccessStatus.Active && x.ParameterGroupName.Contains(filterDescription))
                            .Select(
                                e => new ValueLabelModel()
                                {
                                    Value = e.ID,
                                    Label = e.ParameterGroupName
                                }).OrderBy(t => t.Value).ToList();
        }

        private List<ValueLabelModel> GetCompanies(string filterDescription = "")
        {
            return _unitOfWork.ElogbookRepository<Organization>().GetQuery(x => !x.IsDeleted
                                                                                && x.Active == (int)AccessStatus.Active
                                                                                && "Công ty".Equals(x.Level, StringComparison.OrdinalIgnoreCase)
                                                                                && x.Name.Contains(filterDescription))
                            .Select(
                                e => new ValueLabelModel()
                                {
                                    Value = e.ID,
                                    Label = e.Name
                                }).OrderBy(t => t.Value).ToList();
        }

        private List<ValueStringLabelModel> GetElectricRanks()
        {
            return _unitOfWork.ElogbookRepository<User>()
                .GetQuery(x => !x.IsDeleted && x.ElectricRank != null && x.ElectricRank != "")
                .Select(e => new ValueStringLabelModel()
                {
                    Value = e.ElectricRank,
                    Label = e.ElectricRank
                }).OrderBy(t => t.Value).Distinct().ToList();
        }

        private List<ValueStringLabelModel> GetLaborRanks()
        {
            return _unitOfWork.ElogbookRepository<User>()
                .GetQuery(x => !x.IsDeleted && x.LaborRank != null && x.LaborRank != "")
                .Select(e => new ValueStringLabelModel()
                {
                    Value = e.LaborRank,
                    Label = e.LaborRank
                }).OrderBy(t => t.Value).Distinct().ToList();
        }

        private List<ValueLabelModel> GetWorkTicketFeatures()
        {
            return _unitOfWork.ElogbookRepository<WorkTicketFeatures>()
                .GetQuery(x => !x.IsDeleted)
                .Select(e => new ValueLabelModel()
                {
                    Value = e.ID,
                    Label = e.FeatureName
                }).OrderBy(t => t.Value).ToList();
        }

        private List<ValueLabelModel> GetWorkLocation()
        {
            return _unitOfWork.ElogbookRepository<ContentPosition>()
                .GetQuery(x => !x.IsDeleted)
                .Select(e => new ValueLabelModel()
                {
                    Value = e.ID,
                    Label = e.Position
                }).OrderBy(t => t.Value).ToList();
        }

        private ValueLabelModel GetShiftByCurrentTime(int factoryID, double hour)
        {
            var query = _unitOfWork.ElogbookRepository<OperationShift>().GetQuery(x => x.FactoryID == factoryID
                                                                                  && x.Active == (int)AccessStatus.Active
                                                                                  && hour >= x.StartTime
                                                                                  && hour < x.EndTime
                                                                                  && !x.IsDeleted).FirstOrDefault();
            if (query != null)
            {
                return new ValueLabelModel { Value = query.ID, Label = query.OperationShiftName };
            }

            var listShifts = _unitOfWork.ElogbookRepository<OperationShift>().GetQuery(x => x.FactoryID == factoryID
                                                                                  && x.Active == (int)AccessStatus.Active
                                                                                  && x.StartTime > x.EndTime
                                                                                  && !x.IsDeleted).ToList();
            foreach (var itm in listShifts)
            {
                if (hour > itm.StartTime || hour < itm.EndTime)
                    return new ValueLabelModel { Value = itm.ID, Label = itm.OperationShiftName };
            }
            return null;
        }

        private List<ValueLabelModel> GetSystemData(int dataType)
        {
            return _unitOfWork.ElogbookRepository<SystemData>()
                .GetQuery(x => !x.IsDeleted && x.DataType == dataType)
                .Select(e => new ValueLabelModel()
                {
                    Value = e.ID,
                    Label = e.Description,
                    IsUseOperationParameterGroup = e.IsSyncDataMobile,
                }).OrderBy(t => t.Value).ToList();
        }

        public ResponseBaseModel QuickAutoComplete(QuickAutoCompleteModel requestModel)
        {
            if (requestModel.TableName.ToUpper() == "DEVICE")
            {
                var entities = _unitOfWork.ElogbookRepository<AmDevice>()
                .GetQuery(x => x.Active == (int)AmEnumUsageStatus.Apply && !x.IsDeleted);

                if (!string.IsNullOrWhiteSpace(requestModel.KeySearch))
                {
                    entities = entities.Where(x => x.AssetDesc.Contains(requestModel.KeySearch));
                }

                var pageData = entities.Select(
                e => new ComboboxDataModel()
                {
                    Value = e.ID,
                    Label = e.AssetDesc,
                    Code = e.AssetCode
                }).OrderByDescending(t => t.Label).ToPagedList(requestModel.PageNumber, requestModel.PageSize);

                return HttpResponse.OkList(pageData);
            }

            if (requestModel.TableName.ToUpper() == "LOCATION")
            {
                var entities = _unitOfWork.ElogbookRepository<AmLocation>()
                .GetQuery(x => x.Active == (int)AmEnumUsageStatus.Apply && !x.IsDeleted);

                if (!string.IsNullOrWhiteSpace(requestModel.KeySearch))
                {
                    entities = entities.Where(x => x.SiteDesc.Contains(requestModel.KeySearch));
                }

                var pageData = entities.Select(
                e => new ComboboxDataModel()
                {
                    Value = e.ID,
                    Label = e.SiteDesc,
                    Code = e.SiteCode
                }).OrderByDescending(t => t.Label).ToPagedList(requestModel.PageNumber, requestModel.PageSize);

                return HttpResponse.OkList(pageData);
            }

            if (requestModel.TableName.ToUpper() == "DEVICECATEGORY")
            {
                var entities = _unitOfWork.ElogbookRepository<AmDeviceCategory>()
                .GetQuery(x => x.Active == (int)AmEnumUsageStatus.Apply && !x.IsDeleted);

                if (!string.IsNullOrWhiteSpace(requestModel.KeySearch))
                {
                    entities = entities.Where(x => x.CategoryName.Contains(requestModel.KeySearch));
                }

                var pageData = entities.Select(
                e => new ComboboxDataModel()
                {
                    Value = e.ID,
                    Label = e.CategoryName,
                    Code = e.CategoryCode
                }).OrderByDescending(t => t.Label).ToPagedList(requestModel.PageNumber, requestModel.PageSize);

                return HttpResponse.OkList(pageData);
            }

            if (requestModel.TableName.ToUpper() == "DEVICETYPE")
            {
                var entities = _unitOfWork.ElogbookRepository<AmDeviceType>()
                .GetQuery(x => x.Active == (int)AmEnumUsageStatus.Apply && !x.IsDeleted);

                if (!string.IsNullOrWhiteSpace(requestModel.KeySearch))
                {
                    entities = entities.Where(x => x.TypeDesc.Contains(requestModel.KeySearch));
                }

                var pageData = entities.Select(
                e => new ComboboxDataModel()
                {
                    Value = e.ID,
                    Label = e.TypeDesc,
                    Code = e.TypeCode
                }).OrderByDescending(t => t.Label).ToPagedList(requestModel.PageNumber, requestModel.PageSize);

                return HttpResponse.OkList(pageData);
            }

            return HttpResponse.OkList(new List<ComboboxDataModel>());
        }

        public ResponseBaseModel GetAlertNotifications(AlertNotificationSearchModel requestModel)
        {
            var entities = _unitOfWork.ElogbookRepository<AlertNotification>().GetQuery(x => !x.IsDeleted && x.UserID == requestModel.CurrentUserID);

            // Tìm kiếm theo Title và Content
            if (!string.IsNullOrWhiteSpace(requestModel.SearchText))
            {
                entities = entities.Where(x => x.NoticeTitle.Contains(requestModel.SearchText) || x.NoticeContent.Contains(requestModel.SearchText));
            }

            // Tìm kiếm theo trạng thái
            if (requestModel.Status != CommonConstant.ALL_KEY)
            {
                entities = entities.Where(x => x.Status == requestModel.Status);
            }

            // tìm kiếm theo cờ đánh dấu
            if (requestModel.IsImportant)
            {
                entities = entities.Where(x => x.IsImportant == requestModel.IsImportant);
            }

            var dataResult = entities.Select(
                e => new AlertNotificationModel()
                {
                    ID = e.ID,
                    StatusValue = e.Status,
                    NoticeTitle = e.NoticeTitle,
                    NoticeContent = e.NoticeContent,
                    IsImportant = e.IsImportant,
                    UpdatedAt = e.UpdatedAt,
                    InsertedAt = e.InsertedAt
                })
                .OrderByDescending(t => t.StatusValue)
                .ThenByDescending(t => t.InsertedAt)
                .ToPagedList(requestModel.PageNumber, requestModel.PageSize);

            return HttpResponse.OkList(dataResult);
        }

        public int GetCountAlertNotice(int userID)
        {
            var entities = _unitOfWork.ElogbookRepository<AlertNotification>()
                .GetQuery(x => !x.IsDeleted && x.UserID == userID && x.Status == (int)EnumNotification.Unread).ToList();

            return entities.Count;
        }

        public AlertNotificationModel GetAlertNoticeById(int alertId)
        {
            var entity = _unitOfWork.ElogbookRepository<AlertNotification>().GetQuery(x => x.ID == alertId && !x.IsDeleted).Select(
              e => new AlertNotificationModel()
              {
                  ID = e.ID,
                  StatusValue = e.Status,
                  NoticeTitle = e.NoticeTitle,
                  NoticeContent = e.NoticeContent,
                  IsImportant = e.IsImportant,
                  UpdatedAt = e.UpdatedAt,
                  InsertedAt = e.InsertedAt
              }).FirstOrDefault();

            return entity;
        }

        public ResponseModel<bool> UpdateAlertNotice(UpdateAlertNoticeModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>
            {
                StatusCode = (int)HttpStatusCode.BadRequest
            };

            if (requestModel.ID <= 0)
            {
                responseModel.ErrorMessages.Add("ID của Thông báo không hợp lệ");

                return responseModel;
            }

            var entity = _unitOfWork.ElogbookRepository<AlertNotification>().GetQuery(x => x.ID == requestModel.ID && !x.IsDeleted).FirstOrDefault();

            if (entity != null)
            {
                entity.Status = requestModel.Status > 0 ? (int)requestModel.Status : entity.Status;
                entity.IsImportant = requestModel.IsImportant != null ? requestModel.IsImportant : entity.IsImportant;
                entity.LastUpdatedUserID = requestModel.CurrentUserID;
                entity.UpdatedAt = DateTime.Now;

                _unitOfWork.ElogbookRepository<AlertNotification>().Update(entity);

                responseModel.Data = _unitOfWork.Complete() > 0;
            }
            responseModel.StatusCode = (int)HttpStatusCode.OK;
            return responseModel;
        }

        public ResponseModel<int> GetCountAlertNoticeForMobile(int userID)
        {
            ResponseModel<int> responseModel = new ResponseModel<int>
            {
                StatusCode = (int)HttpStatusCode.NotFound
            };

            var entities = _unitOfWork.ElogbookRepository<AlertNotification>()
                .GetQuery(x => !x.IsDeleted && x.UserID == userID && x.Status == (int)EnumNotification.Unread).ToList();

            responseModel.Data = entities.Count;
            responseModel.StatusCode = (int)HttpStatusCode.OK;

            return responseModel;
        }

        #endregion
    }
}
