using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Elogbook.Core.Models.Response.EmailNotification;
using Elogbook.Core.Models.Response.EmailService;
using Elogbook.Core.Models.Response.User;
using Elogbook.Data;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using static iTextSharp.text.pdf.AcroFields;

namespace Elogbook.Domain.Services
{
    /// <summary>
    /// Xử lý email
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;
        private readonly ISystemConfigurationService _systemConfigurationService;
        private readonly IAutonomousMaintenanceService _autonomousMaintenanceService;
        private readonly IEmailConfigurationService _emailConfigurationService;
        private readonly ICommonService _commonService;
        private readonly IOrganizationService _organizationService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="systemConfigurationService"></param>
        /// <param name="autonomousMaintenanceService"></param>
        /// <param name="emailConfigurationService"></param>
        /// <param name="commonService"></param>
        /// <param name="organizationService"></param>
        public EmailService(ElogbookUnitOfWork unitOfWork,
            ISystemConfigurationService systemConfigurationService,
            IAutonomousMaintenanceService autonomousMaintenanceService,
            IEmailConfigurationService emailConfigurationService,
            ICommonService commonService,
            IOrganizationService organizationService)
        {
            _unitOfWork = unitOfWork;
            _systemConfigurationService = systemConfigurationService;
            _autonomousMaintenanceService = autonomousMaintenanceService;
            _emailConfigurationService = emailConfigurationService;
            _commonService = commonService;
            _organizationService = organizationService;
        }

        /// <summary>
        /// Lấy thông tin Email Transmissions
        /// </summary>
        /// <returns></returns>
        public List<EmailTransmissionModel> GetEmailTransmissions()
        {
            return _unitOfWork.ElogbookRepository<EmailTransmissions>().GetQuery().Where(x => !x.IsSent && !x.TransmissionCompleteFlag && !x.IsDeleted).Select(
                            e => new EmailTransmissionModel()
                            {
                                ID = e.ID,
                                EmailReplyToAddr = e.EmailReplyToAddr,
                                Subject = e.Subject,
                                MessageBody = e.MessageBody,
                                IsBodyHtml = e.IsBodyHtml,
                                Retry = e.Retry,
                                DetailFailure = e.DetailFailure,
                                IsSent = e.IsSent,
                                CreatedUser = new ValueLabelModel { Value = e.CreatedUser.ID, Label = e.CreatedUser.FullName },
                                LastUpdatedUser = new ValueLabelModel { Value = e.LastUpdatedUser.ID, Label = e.LastUpdatedUser.FullName },
                                TransmissionCompleteFlag = e.TransmissionCompleteFlag,
                                RegistrationTicketId = e.RegistrationTicketId,
                                WorkTicketId = e.WorkTicketId,
                                AutonomousMaintenanceId = e.AutonomousMaintenanceId,
                                AmRequestTicketId = e.AmRequestTicketId
                            }).ToList();
        }

        /// <summary>
        /// Gửi mail
        /// </summary>
        /// <param name="id"></param>
        void IEmailService.SendEmailConfiguration(int id)
        {
            EmailTransmissionModel item = _unitOfWork.ElogbookRepository<EmailTransmissions>()
                .GetQuery().Where(x => !x.IsSent && !x.TransmissionCompleteFlag && !x.IsDeleted && x.ID == id).Select(
                            e => new EmailTransmissionModel()
                            {
                                ID = e.ID,
                                EmailReplyToAddr = e.EmailReplyToAddr,
                                Subject = e.Subject,
                                MessageBody = e.MessageBody,
                                IsBodyHtml = e.IsBodyHtml,
                                Retry = e.Retry,
                                DetailFailure = e.DetailFailure,
                                IsSent = e.IsSent,
                                CreatedUser = new ValueLabelModel { Value = e.CreatedUser.ID, Label = e.CreatedUser.FullName },
                                LastUpdatedUser = new ValueLabelModel { Value = e.LastUpdatedUser.ID, Label = e.LastUpdatedUser.FullName },
                                TransmissionCompleteFlag = e.TransmissionCompleteFlag,
                                RegistrationTicketId = e.RegistrationTicketId,
                                WorkTicketId = e.WorkTicketId,
                                AutonomousMaintenanceId = e.AutonomousMaintenanceId,
                                AmRequestTicketId = e.AmRequestTicketId
                            }).FirstOrDefault();

            var dataEmailSetting = _emailConfigurationService.GetEmailConfiguration();
            if (item != null)
            {
                var recipientList = GetEmailTransmissionRecipients(item.ID);

                foreach (var recipient in recipientList)
                {
                    SendEmailConfiguration(item, dataEmailSetting, recipient);
                }
            }
        }

        public List<EmailTransmissionRecipientsModel> GetEmailTransmissionRecipients(int EmailTransmissionID)
        {
            return _unitOfWork.ElogbookRepository<EmailTransmissionRecipients>().GetQuery().Where(x => x.EmailTransmissionID == EmailTransmissionID && !x.IsDeleted).Select(
                            e => new EmailTransmissionRecipientsModel()
                            {
                                EmailTransmissionID = e.EmailTransmissionID,
                                RecipientEmailAddr = e.RecipientEmailAddr,
                                RecipientEmailName = e.RecipientEmailName,
                                RecipientType = e.RecipientType,
                                ID = e.ID
                            }).ToList();
        }

        /// <summary>
        /// Gửi mail
        /// </summary>
        /// <param name="item"></param>
        /// <param name="dataEmailSetting"></param>
        /// <param name="transmissionRecipient"></param>
        public void SendEmailConfiguration(EmailTransmissionModel item, EmailConfigurationResponseModel dataEmailSetting, EmailTransmissionRecipientsModel transmissionRecipient)
        {
            var smtpSend = new SmtpClient
            {
                Host = dataEmailSetting.EmailServer,
                Port = dataEmailSetting.Port,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(dataEmailSetting.UserName, dataEmailSetting.PassWord),
                EnableSsl = true,
                Timeout = CommonConstant.TIMEOUT_SENDMAIL,
            };

            using (var mailMessage = new MailMessage())
            {
                try
                {
                    mailMessage.Sender = new MailAddress(dataEmailSetting.EmailAddress);
                    mailMessage.From = new MailAddress(dataEmailSetting.EmailAddress);

                    var mail = ReplaceContentSubjectBodyMail(item, transmissionRecipient);

                    //Subject & Body
                    mailMessage.Subject = mail.Subject;
                    mailMessage.SubjectEncoding = Encoding.UTF8;
                    mailMessage.Body = mail.MessageBody;
                    mailMessage.BodyEncoding = Encoding.UTF8;
                    mailMessage.To.Add(dataEmailSetting.EmailAddress);
                    mailMessage.IsBodyHtml = true;

                    #region ADD RECIPIENTS
                    var splits = new char[] { ',', ';' };
                    string[] listEmail = transmissionRecipient.RecipientEmailAddr.Split(splits);

                    for (int i = CommonConstant.ZERO; i < listEmail.Length; i++)
                    {
                        string tempEmail = listEmail[i].Trim();
                        if (!string.IsNullOrWhiteSpace(tempEmail))
                        {
                            var mailTo = new MailAddress(tempEmail, i == CommonConstant.ZERO ? transmissionRecipient.RecipientEmailName : string.Empty);
                            switch (transmissionRecipient.RecipientType)
                            {
                                case (int)RecipientType.Cc:
                                    mailMessage.CC.Add(mailTo);
                                    break;

                                case (int)RecipientType.Bcc:
                                    mailMessage.Bcc.Add(mailTo);
                                    break;

                                case (int)RecipientType.To:
                                    mailMessage.To.Add(mailTo);
                                    break;
                            }
                        }
                    }
                    #endregion

                    #region ADD ATTACHMENTS

                    if (item.AmRequestTicketId.GetValueOrDefault() > CommonConstant.ZERO)
                    {
                        var requestTicket = _unitOfWork.ElogbookRepository<AmRequestTicket>().GetById(item.AmRequestTicketId.Value);
                        if (requestTicket != null
                            && !string.IsNullOrWhiteSpace(requestTicket.Attachments)
                            && requestTicket.AmStatusID == (int)EnumNotificationType.Register)
                        {
                            List<string> attachments = requestTicket.Attachments.Split(',').ToList();
                            if (attachments.Any())
                            {
                                foreach (var attachment in attachments)
                                {
                                    if (int.TryParse(attachment, out int idItem))
                                    {
                                        var att = _unitOfWork.ElogbookRepository<Data.Attachment>().GetQuery(x => !x.IsDeleted && x.ID == idItem).FirstOrDefault();
                                        if (att != null)
                                        {
                                            string filePath = AppSettings.FILE_STORAGE_SERVER_DIR + att.FilePath;
                                            System.Net.Mail.Attachment attach = new System.Net.Mail.Attachment(filePath, MediaTypeNames.Application.Octet);
                                            ContentDisposition disposition = attach.ContentDisposition;
                                            disposition.CreationDate = File.GetCreationTime(filePath);
                                            disposition.ModificationDate = File.GetLastWriteTime(filePath);
                                            disposition.ReadDate = File.GetLastAccessTime(filePath);
                                            disposition.FileName = Path.GetFileName(filePath);
                                            disposition.Size = new FileInfo(filePath).Length;
                                            disposition.DispositionType = DispositionTypeNames.Attachment;
                                            mailMessage.Attachments.Add(attach);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    #endregion

                    smtpSend.Send(mailMessage);

                    item.IsSent = true;
                    item.DetailFailure = string.Empty;
                    item.TransmissionCompleteFlag = true;
                    item.Retry = CommonConstant.ZERO;
                }
                catch (Exception ex)
                {
                    item.IsSent = false;
                    item.DetailFailure = ex.ToString();
                    item.Retry = item.Retry > CommonConstant.ZERO ? item.Retry + CommonConstant.START_NUMBER : CommonConstant.START_NUMBER;
                    item.TransmissionCompleteFlag = item.Retry > CommonConstant.RETRY_NUMBER;
                    if (!item.TransmissionCompleteFlag)
                    {
                        BackgroundJob.Enqueue<IEmailService>(x => x.SendEmailConfiguration(item.ID));
                    }
                }

                // update data to database
                var data = _unitOfWork.ElogbookRepository<EmailTransmissions>().GetById(item.ID);
                if (data != null)
                {
                    data.IsSent = item.IsSent;
                    data.DetailFailure = item.DetailFailure;
                    data.Retry = item.Retry;
                    data.TransmissionCompleteFlag = item.TransmissionCompleteFlag;
                    data.UpdatedAt = DateTime.Now;
                    _unitOfWork.ElogbookRepository<EmailTransmissions>().Update(data);
                    _unitOfWork.Complete();
                }
            }
        }

        private EmailTransmissionModel ReplaceContentSubjectBodyMail(EmailTransmissionModel item, EmailTransmissionRecipientsModel recipient)
        {
            var system = _systemConfigurationService.GetAppVersion();
            EmailTransmissionModel email = new EmailTransmissionModel
            {
                ID = item.ID,
                EmailReplyToAddr = item.EmailReplyToAddr,
                Subject = item.Subject,
                MessageBody = item.MessageBody,
                IsBodyHtml = item.IsBodyHtml,
                Retry = item.Retry,
                DetailFailure = item.DetailFailure,
                IsSent = item.IsSent,
                CreatedUser = item.CreatedUser,
                LastUpdatedUser = item.LastUpdatedUser,
                TransmissionCompleteFlag = item.TransmissionCompleteFlag,
                RegistrationTicketId = item.RegistrationTicketId,
                WorkTicketId = item.WorkTicketId,
                AutonomousMaintenanceId = item.AutonomousMaintenanceId,
                AmRequestTicketId = item.AmRequestTicketId
            };

            //replace subject
            email.Subject = email.Subject.Replace(CommonConstant.MAIL_MERGE_TITLE, system?.AppTitle);

            //replace body
            if (email.RegistrationTicketId.GetValueOrDefault() > CommonConstant.ZERO)
            {
                var registration = _unitOfWork.ElogbookRepository<RegistrationTicket>().GetById(email.RegistrationTicketId.Value);
                if (registration != null)
                {
                    var tempSubject = string.Empty;

                    var body = email.MessageBody;

                    if (system != null)
                    {
                        body = body.Replace(CommonConstant.MAIL_MERGE_TITLE, system.AppTitle);
                    }

                    body = body.Replace(CommonConstant.MAIL_MERGE_ACCOUNT_USER, recipient.RecipientEmailName);
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
                            tempSubject = $" - Số phiếu: {workTicket.WorkTicketNumber}";
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
                            tempSubject = $" - Số phiếu: {registration.TicketNumber}";
                            body = body.Replace(CommonConstant.MAIL_MERGE_THOI_GIAN_BAT_DAU, registration.StartDate.ToString(CommonConstant.FORMAT_DATE_DDMMYYYYHHMMSS));
                            body = body.Replace(CommonConstant.MAIL_MERGE_THOI_GIAN_KET_THUC, registration.EndDate.ToString(CommonConstant.FORMAT_DATE_DDMMYYYYHHMMSS));
                            body = body.Replace(CommonConstant.MAIL_MERGE_SO_PHIEU_CONG_TAC, registration.TicketNumber);
                        }
                    }
                    else
                    {
                        tempSubject = $" - Số phiếu: {registration.TicketNumber}";
                        body = body.Replace(CommonConstant.MAIL_MERGE_THOI_GIAN_BAT_DAU, registration.StartDate.ToString(CommonConstant.FORMAT_DATE_DDMMYYYYHHMMSS));
                        body = body.Replace(CommonConstant.MAIL_MERGE_THOI_GIAN_KET_THUC, registration.EndDate.ToString(CommonConstant.FORMAT_DATE_DDMMYYYYHHMMSS));
                        body = body.Replace(CommonConstant.MAIL_MERGE_SO_PHIEU_CONG_TAC, registration.TicketNumber);
                    }

                    email.Subject += tempSubject;

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

                    if (registration.RegistrationTicketType == CommonConstant.TYPE_PCTD)
                    {
                        //body = body.Replace(CommonConstant.MAIL_MERGE_LINK_HE_THONG, AppSettings.DOMAIN + CommonConstant.MAIL_MERGE_PDKD + email.RegistrationTicketId.Value);
                        body = body.Replace(CommonConstant.MAIL_MERGE_LINK_HE_THONG, AppSettings.DOMAIN);
                    }
                    else
                    {
                        //body = body.Replace(CommonConstant.MAIL_MERGE_LINK_HE_THONG, AppSettings.DOMAIN + CommonConstant.MAIL_MERGE_PDKD_TCNH + email.RegistrationTicketId.Value);
                        body = body.Replace(CommonConstant.MAIL_MERGE_LINK_HE_THONG, AppSettings.DOMAIN);
                    }

                    email.MessageBody = body;
                    return email;
                }
            }

            var workTicketId = email.WorkTicketId.GetValueOrDefault();
            if (workTicketId > CommonConstant.ZERO)
            {
                var workTicket = _unitOfWork.ElogbookRepository<WorkTicket>().GetById(workTicketId);
                if (workTicket != null)
                {
                    email.Subject = email.Subject + " - Số phiếu: " + workTicket.WorkTicketNumber;
                    var body = email.MessageBody;
                    if (system != null)
                    {
                        body = body.Replace(CommonConstant.MAIL_MERGE_TITLE, system.AppTitle);
                    }
                    body = body.Replace(CommonConstant.MAIL_MERGE_ACCOUNT_USER, recipient.RecipientEmailName);
                    body = body.Replace(CommonConstant.MAIL_MERGE_DON_VI_CONG_TAC_DANG_KY, workTicket.Factory?.FactoryName);
                    body = body.Replace(CommonConstant.MAIL_MERGE_NOI_DUNG_CONG_VIEC, workTicket.WorkContent);
                    body = body.Replace(CommonConstant.MAIL_MERGE_DIA_DIEM, workTicket.WorkPlacement);
                    body = body.Replace(CommonConstant.MAIL_MERGE_THOI_GIAN_BAT_DAU, workTicket.StartDate.ToString(CommonConstant.FORMAT_DATE_DDMMYYYYHHMMSS));
                    body = body.Replace(CommonConstant.MAIL_MERGE_THOI_GIAN_KET_THUC, workTicket.EndDate.ToString(CommonConstant.FORMAT_DATE_DDMMYYYYHHMMSS));
                    body = body.Replace(CommonConstant.MAIL_MERGE_SO_PHIEU_CONG_TAC, workTicket.WorkTicketNumber);
                    body = body.Replace(CommonConstant.MAIL_MERGE_ACCOUNT_CAP, workTicket.CreatedUser?.FullName);
                    // Thông tin cho phép
                    var allowedInfo = _unitOfWork.ElogbookRepository<AllowedInfo>()
                        .GetQuery(x => x.WorkTicketID == workTicketId && !x.IsDeleted).FirstOrDefault();
                    if (allowedInfo != null)
                    {
                        var acceptorUserEntity = GetLeadershipById(workTicketId, (int)LeadershipUserRoles.Acceptor);
                        body = body.Replace(CommonConstant.MAIL_MERGE_ACCOUNT_CHO_PHEP, acceptorUserEntity?.FullName);
                        body = body.Replace(CommonConstant.MAIL_MERGE_THOI_GIAN_CHO_PHEP, allowedInfo.AllowedTime.ToString(CommonConstant.FORMAT_DATE_DDMMYYYYHHMM));
                    }
                    var workFinish = _unitOfWork.ElogbookRepository<WorkFinish>().GetQuery(x => x.WorkTicketID == workTicketId && !x.IsDeleted).FirstOrDefault();

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

                    email.MessageBody = body;

                    return email;
                }
            }

            if (email.AutonomousMaintenanceId.GetValueOrDefault() > CommonConstant.ZERO)
            {
                var am = _autonomousMaintenanceService.GetAutonomousMaintenanceById(email.AutonomousMaintenanceId.Value);
                if (am != null)
                {
                    email.Subject = email.Subject + " - Lịch bảo trì: " + am.MaintenanceSchedule?.Label;
                    var body = email.MessageBody;
                    if (system != null)
                    {
                        body = body.Replace(CommonConstant.MAIL_MERGE_TITLE, system.AppTitle);
                    }

                    body = body.Replace(CommonConstant.MAIL_MERGE_ACCOUNT_USER, recipient.RecipientEmailName);
                    body = body.Replace(CommonConstant.MAIL_MERGE_KHU_VUC, am.Location?.Label);
                    body = body.Replace(CommonConstant.MAIL_MERGE_THIET_BI, am.Device?.Label);
                    body = body.Replace(CommonConstant.MAIL_MERGE_ACCOUNT_THUC_HIEN_AM, am.LastUpdatedUser?.Label);
                    body = body.Replace(CommonConstant.MAIL_MERGE_NGAY_CUA_HOAT_DONG_AM, am.UpdatedAt.ToString(CommonConstant.FORMAT_DATE_DDMMYYYYHHMM));
                    body = body.Replace(CommonConstant.MAIL_MERGE_TEN_LICH_BAO_TRI, am.MaintenanceSchedule?.Label);
                    body = body.Replace(CommonConstant.MAIL_MERGE_CUONG_VI, am.OperationPosition?.Label);
                    body = body.Replace(CommonConstant.MAIL_MERGE_CA, am.OperationShift?.Label);
                    body = body.Replace(CommonConstant.MAIL_MERGE_NOI_DUNG_BAT_THUONG_THIET_BI, am.DeviceAbnormalDescription);
                    body = body.Replace(CommonConstant.MAIL_MERGE_LINK_HE_THONG, AppSettings.DOMAIN);

                    email.MessageBody = body;
                    return email;
                }
            }

            if (email.AmRequestTicketId.GetValueOrDefault() > CommonConstant.ZERO)
            {
                var requestTicket = _unitOfWork.ElogbookRepository<AmRequestTicket>().GetById(email.AmRequestTicketId.Value);
                if (requestTicket != null)
                {
                    email.Subject = email.Subject + " - Số phiếu: " + requestTicket.RequestTicketNumber;
                    var body = email.MessageBody;
                    if (system != null)
                    {
                        body = body.Replace(CommonConstant.MAIL_MERGE_TITLE, system.AppTitle);
                    }
                    body = body.Replace(CommonConstant.MAIL_MERGE_ACCOUNT_USER, recipient.RecipientEmailName);
                    body = body.Replace(CommonConstant.MAIL_MERGE_SO_PHIEU_YEU_CAU, requestTicket.RequestTicketNumber);
                    body = body.Replace(CommonConstant.MAIL_MERGE_MUC_DO_KHAN_CAP, requestTicket.AmEmergency?.Name);
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

                    //body = body.Replace(CommonConstant.MAIL_MERGE_LINK_HE_THONG, AppSettings.DOMAIN + CommonConstant.MAIL_MERGE_TICKET_LINK + email.AmRequestTicketId.Value + "/detail");
                    body = body.Replace(CommonConstant.MAIL_MERGE_LINK_HE_THONG, AppSettings.DOMAIN);

                    email.MessageBody = body;
                    return email;
                }
            }

            return email;
        }

        public void ProcessEmail(ProcessEmailRequestModel requestModel)
        {
            try
            {
                //Thông tin current user
                var userInfo = _unitOfWork.ElogbookRepository<User>().GetQuery(x => x.ID == requestModel.CurrentUserID && !x.IsDeleted).Select(
                    e => new UserModel()
                    {
                        ID = e.ID,
                        FullName = e.FullName,
                        UserName = e.UserName,
                        Company = new ValueLabelModel { Value = e.Company.ID, Label = e.Company.Name },
                        Email = e.Email,
                        RoleInUsers = e.RoleInUsers.Where(x => !x.IsDeleted).Select(
                                                t => new ValueLabelModel()
                                                {
                                                    Value = t.UserRole.ID,
                                                    Label = t.UserRole.RoleName,
                                                }).ToList(),
                    }).FirstOrDefault();

                if (userInfo == null || string.IsNullOrWhiteSpace(userInfo.Email)) return;

                var notificationTypeValue = requestModel.NotificationTypeValue;
                var permissionKey = string.Empty;

                if (requestModel.NotificationTypeValue <= CommonConstant.ZERO)
                {
                    if (requestModel.RegistrationTicketType > CommonConstant.ZERO)
                    {
                        if (!requestModel.IsPCT)
                        {
                            if (requestModel.RegistrationTicketType == (int)EnumModuleNotification.PCTD)
                            {
                                permissionKey = CommonConstant.PERMISSION_ADD_CPCTD;
                                switch (requestModel.TicketStatus)
                                {
                                    case (int)EnumTicketStatus.Waiting:
                                        notificationTypeValue = 1;
                                        break;

                                    case (int)EnumTicketStatus.Cancel:
                                        notificationTypeValue = 2;
                                        break;

                                    case (int)EnumTicketStatus.Reject:
                                        notificationTypeValue = 3;
                                        break;

                                    case (int)EnumTicketStatus.Approved:
                                        notificationTypeValue = 4;
                                        break;
                                }
                            }
                            else if (requestModel.RegistrationTicketType == (int)EnumModuleNotification.TCNH)
                            {
                                permissionKey = CommonConstant.PERMISSION_ADD_CPCTTCNH;
                                switch (requestModel.TicketStatus)
                                {
                                    case (int)EnumTicketStatus.Waiting:
                                        notificationTypeValue = 8;
                                        break;

                                    case (int)EnumTicketStatus.Cancel:
                                        notificationTypeValue = 9;
                                        break;

                                    case (int)EnumTicketStatus.Reject:
                                        notificationTypeValue = 10;
                                        break;

                                    case (int)EnumTicketStatus.Approved:
                                        notificationTypeValue = 11;
                                        break;
                                }
                            }
                        }
                        else
                        {
                            if (requestModel.RegistrationTicketType == (int)EnumModuleNotification.PCTD)
                            {
                                permissionKey = CommonConstant.PERMISSION_ADD_PCTD;
                                switch (requestModel.TicketStatus)
                                {
                                    case (int)EnumTicketStatus.Approved:
                                        notificationTypeValue = 5;
                                        break;

                                    case (int)EnumTicketStatus.Allowed:
                                        notificationTypeValue = 6;
                                        break;

                                    case (int)EnumTicketStatus.Completed:
                                        notificationTypeValue = 7;
                                        break;
                                }
                            }
                            else if (requestModel.RegistrationTicketType == (int)EnumModuleNotification.TCNH)
                            {
                                permissionKey = CommonConstant.PERMISSION_ADD_PCTTCNH;
                                switch (requestModel.TicketStatus)
                                {
                                    case (int)EnumTicketStatus.Approved:
                                        notificationTypeValue = 12;
                                        break;

                                    case (int)EnumTicketStatus.Allowed:
                                        notificationTypeValue = 13;
                                        break;

                                    case (int)EnumTicketStatus.Completed:
                                        notificationTypeValue = 14;
                                        break;
                                }
                            }
                        }
                    }
                    else if (requestModel.AmRequestTicketID > CommonConstant.ZERO)
                    {
                        permissionKey = CommonConstant.PERMISSION_EDIT_QLPYCTC;
                        switch (requestModel.TicketStatus)
                        {
                            case (int)AmEnumRequestTicket.InProgress:
                                notificationTypeValue = 18;
                                break;

                            case (int)AmEnumRequestTicket.Reject:
                                notificationTypeValue = 19;
                                break;
                            //case 20-21
                            case (int)AmEnumRequestTicket.Approved:
                                notificationTypeValue = 20;
                                break;
                            //case 22
                            case (int)AmEnumRequestTicket.Stop:
                                notificationTypeValue = 22;
                                break;
                        }
                    }
                }
                else
                {
                    notificationTypeValue = requestModel.NotificationTypeValue;
                }

                //Thông tin mail server
                //get system data
                var moduleName = EnumExtensions.GetEnumDescription(requestModel.ModuleNotification);
                var moduleValue = _unitOfWork.ElogbookRepository<SystemData>()
                    .GetQuery(x => x.Description.Equals(moduleName, StringComparison.OrdinalIgnoreCase) && x.DataType == (int)SystemDataType.ModuleName)
                    .Select(x => x.ID).FirstOrDefault();

                if (moduleValue <= CommonConstant.ZERO) return;

                var mailConfig = _emailConfigurationService.GetEmailConfiguration();
                AddEmailTransmissionsModel transmissionsModel = new AddEmailTransmissionsModel()
                {
                    EmailReplyToAddr = mailConfig.EmailAddress,
                    Subject = "",
                    MessageBody = "",
                    IsBodyHtml = true,
                    DetailFailure = "",
                };

                AddEmailTransmissionsModel transmissionsVHVModel = new AddEmailTransmissionsModel()
                {
                    EmailReplyToAddr = mailConfig.EmailAddress,
                    Subject = "",
                    MessageBody = "",
                    IsBodyHtml = true,
                    DetailFailure = "",
                };

                List<AddEmailTransmissionRecipientsModel> recipientsModels = new List<AddEmailTransmissionRecipientsModel>();
                List<AddEmailTransmissionRecipientsModel> recipientsVHVModels = new List<AddEmailTransmissionRecipientsModel>();
                var eventName = string.Empty;
                var eventValue = CommonConstant.ZERO;
                EmailTemplate entity;

                switch (notificationTypeValue)
                {
                    //Phiếu công tác Điện
                    //Đăng ký phiếu công tác
                    case 1:
                    //Đăng ký phiếu công tác
                    case 8:
                        eventName = EnumExtensions.GetEnumDescription(EnumNotificationType.Register);
                        eventValue = _unitOfWork.ElogbookRepository<SystemData>()
                            .GetQuery(x => x.Description.Equals(eventName, StringComparison.OrdinalIgnoreCase)
                                && x.DataType == (int)SystemDataType.EmailNotificationEvent)
                            .Select(x => x.ID).FirstOrDefault();

                        entity = _unitOfWork.ElogbookRepository<EmailTemplate>()
                            .GetQuery(x => x.ModuleID == moduleValue && x.NotificationTypeID == eventValue).FirstOrDefault();

                        if (entity != null)
                        {
                            transmissionsModel.Subject = entity.EmailSubject;
                            transmissionsModel.MessageBody = entity.EmailContent;
                        }
                        break;

                    //Thu hồi phiếu đăng ký công tác
                    case 2:
                    //Thu hồi phiếu đăng ký công tác
                    case 9:
                        eventName = EnumExtensions.GetEnumDescription(EnumNotificationType.Cancel);
                        eventValue = _unitOfWork.ElogbookRepository<SystemData>()
                            .GetQuery(x => x.Description.Equals(eventName, StringComparison.OrdinalIgnoreCase)
                                 && x.DataType == (int)SystemDataType.EmailNotificationEvent)
                            .Select(x => x.ID).FirstOrDefault();

                        entity = _unitOfWork.ElogbookRepository<EmailTemplate>()
                            .GetQuery(x => x.ModuleID == moduleValue && x.NotificationTypeID == eventValue).FirstOrDefault();

                        if (entity != null)
                        {
                            transmissionsModel.Subject = entity.EmailSubject;
                            transmissionsModel.MessageBody = entity.EmailContent;
                        }

                        //Account thực hiện thu hồi
                        recipientsModels.Add(new AddEmailTransmissionRecipientsModel
                        {
                            RecipientEmailAddr = userInfo.Email,
                            RecipientEmailName = userInfo.FullName,
                            UserID = userInfo.ID,
                            RecipientType = 3,
                        });
                        break;

                    //Từ chối cấp phiếu cho phiếu đăng ký công tác
                    case 3:
                    //Từ chối cấp phiếu cho phiếu đăng ký công tác
                    case 10:
                        eventName = EnumExtensions.GetEnumDescription(EnumNotificationType.Reject);
                        eventValue = _unitOfWork.ElogbookRepository<SystemData>()
                            .GetQuery(x => x.Description.Equals(eventName, StringComparison.OrdinalIgnoreCase)
                                 && x.DataType == (int)SystemDataType.EmailNotificationEvent)
                            .Select(x => x.ID).FirstOrDefault();

                        entity = _unitOfWork.ElogbookRepository<EmailTemplate>()
                            .GetQuery(x => x.ModuleID == moduleValue && x.NotificationTypeID == eventValue).FirstOrDefault();

                        if (entity != null)
                        {
                            transmissionsModel.Subject = entity.EmailSubject;
                            transmissionsModel.MessageBody = entity.EmailContent;
                        }

                        //Account thực hiện Từ chối
                        recipientsModels.Add(new AddEmailTransmissionRecipientsModel
                        {
                            RecipientEmailAddr = userInfo.Email,
                            RecipientEmailName = userInfo.FullName,
                            UserID = userInfo.ID,
                            RecipientType = 3,
                        });
                        break;

                    //Cấp phiếu công tác từ Phiếu đăng ký công tác
                    case 4:
                    //Cấp phiếu công tác từ Phiếu đăng ký công tác
                    case 11:
                        eventName = EnumExtensions.GetEnumDescription(EnumNotificationType.Approver);
                        eventValue = _unitOfWork.ElogbookRepository<SystemData>()
                            .GetQuery(x => x.Description.Equals(eventName, StringComparison.OrdinalIgnoreCase)
                                 && x.DataType == (int)SystemDataType.EmailNotificationEvent)
                            .Select(x => x.ID).FirstOrDefault();

                        entity = _unitOfWork.ElogbookRepository<EmailTemplate>()
                            .GetQuery(x => x.ModuleID == moduleValue && x.NotificationTypeID == eventValue).FirstOrDefault();

                        if (entity != null)
                        {
                            transmissionsModel.Subject = entity.EmailSubject;
                            transmissionsModel.MessageBody = entity.EmailContent;
                        }

                        //Account thực hiện Cấp phiếu
                        recipientsModels.Add(new AddEmailTransmissionRecipientsModel
                        {
                            RecipientEmailAddr = userInfo.Email,
                            RecipientEmailName = userInfo.FullName,
                            UserID = userInfo.ID,
                            RecipientType = 3,
                        });

                        //Các account có tên trong phiếu đăng ký công tác tại các mục Người giám sát an toàn điện, Người chỉ huy trực tiếp, Người được cử thực hiện công tác
                        RegistrationTicketModel ticket = new RegistrationTicketModel
                        {
                            // Người lãnh đạo (nếu có)
                            LeaderUser = GetStakeholders(requestModel.RegistrationTicketID, RolesStakeholder.Leader).FirstOrDefault(),
                            // Người chỉ huy trực tiếp
                            Commanders = GetStakeholders(requestModel.RegistrationTicketID, RolesStakeholder.Commander),
                            // những người thực hiện công việc
                            Workers = GetStakeholders(requestModel.RegistrationTicketID, RolesStakeholder.Worker),
                        };

                        //chỉ huy trực tiếp
                        if (ticket.Commanders.Any())
                        {
                            foreach (var item in ticket.Commanders)
                            {
                                var isExistUser = recipientsModels.Find(x => x.UserID == item.ID);
                                if (!string.IsNullOrWhiteSpace(item.Email) && isExistUser == null)
                                {
                                    recipientsModels.Add(new AddEmailTransmissionRecipientsModel
                                    {
                                        RecipientEmailAddr = item.Email,
                                        RecipientEmailName = item.FullName,
                                        UserID = item.AssignToUserID > 0 ? item.AssignToUserID.Value : 0,
                                        RecipientType = 3,
                                    });
                                }
                            }
                        }

                        //lãnh đạo công việc
                        if (ticket.LeaderUser != null)
                        {
                            var user = _unitOfWork.ElogbookRepository<User>().GetById(ticket.LeaderUser.ID);
                            if (user != null && !string.IsNullOrWhiteSpace(user.Email))
                            {
                                recipientsModels.Add(new AddEmailTransmissionRecipientsModel
                                {
                                    RecipientEmailAddr = user.Email,
                                    RecipientEmailName = user.FullName,
                                    UserID = user.ID,
                                    RecipientType = 3,
                                });
                            }
                        }

                        // những người thực hiện công việc
                        if (ticket.Workers.Any())
                        {
                            foreach (var user in ticket.Workers)
                            {
                                var isExistUser = recipientsModels.Find(x => x.UserID == user.ID);
                                if (user != null && !string.IsNullOrWhiteSpace(user.Email) && isExistUser == null)
                                {
                                    recipientsModels.Add(new AddEmailTransmissionRecipientsModel
                                    {
                                        RecipientEmailAddr = user.Email,
                                        RecipientEmailName = user.FullName,
                                        UserID = user.AssignToUserID > 0 ? user.AssignToUserID.Value : 0,
                                        RecipientType = 3,
                                    });
                                }
                            }
                        }

                        //Các account có tên trong Phiếu công tác tại các mục Người lãnh đạo công việc, Người chỉ huy trực tiếp, Người thực hiện cho phép
                        if (requestModel.WorkTicketID > CommonConstant.ZERO)
                        {
                            // lãnh đạo (nếu có)
                            StakeholdersResponseModel LeaderUserWorkticket = GetLeadership(requestModel.WorkTicketID, LeadershipUserRoles.Leader);
                            if (LeaderUserWorkticket != null)
                            {
                                recipientsModels.Add(new AddEmailTransmissionRecipientsModel
                                {
                                    RecipientEmailAddr = LeaderUserWorkticket.Email,
                                    RecipientEmailName = LeaderUserWorkticket.FullName,
                                    UserID = LeaderUserWorkticket.ID,
                                    RecipientType = 3,
                                });
                            }

                            // Người chỉ huy trực tiếp
                            StakeholdersResponseModel CommandersWorkticket = GetLeadership(requestModel.WorkTicketID, LeadershipUserRoles.Commander);
                            if (CommandersWorkticket != null)
                            {
                                recipientsModels.Add(new AddEmailTransmissionRecipientsModel
                                {
                                    RecipientEmailAddr = CommandersWorkticket.Email,
                                    RecipientEmailName = CommandersWorkticket.FullName,
                                    UserID = CommandersWorkticket.ID,
                                    RecipientType = 3,
                                });
                            }

                            // người cho phép
                            StakeholdersResponseModel AcceptorWorkticket = GetLeadership(requestModel.WorkTicketID, LeadershipUserRoles.Acceptor);
                            if (AcceptorWorkticket != null)
                            {
                                recipientsModels.Add(new AddEmailTransmissionRecipientsModel
                                {
                                    RecipientEmailAddr = AcceptorWorkticket.Email,
                                    RecipientEmailName = AcceptorWorkticket.FullName,
                                    UserID = AcceptorWorkticket.ID,
                                    RecipientType = 3,
                                });
                            }
                        }

                        break;

                    //Cấp phiếu công tác (tự thêm mới)
                    case 5:
                    case 12:
                        eventName = EnumExtensions.GetEnumDescription(EnumNotificationType.ApproverByAdd);
                        eventValue = _unitOfWork.ElogbookRepository<SystemData>()
                            .GetQuery(x => x.Description.Equals(eventName, StringComparison.OrdinalIgnoreCase)
                                 && x.DataType == (int)SystemDataType.EmailNotificationEvent)
                            .Select(x => x.ID).FirstOrDefault();

                        entity = _unitOfWork.ElogbookRepository<EmailTemplate>()
                            .GetQuery(x => x.ModuleID == moduleValue && x.NotificationTypeID == eventValue).FirstOrDefault();

                        if (entity != null)
                        {
                            transmissionsModel.Subject = entity.EmailSubject;
                            transmissionsModel.MessageBody = entity.EmailContent;
                        }

                        if (requestModel.RegistrationTicketType == (int)EnumModuleNotification.PCTD)
                        {
                            // lãnh đạo (nếu có)
                            StakeholdersResponseModel LeaderUserWorkticketAddnew = GetLeadership(requestModel.WorkTicketID, LeadershipUserRoles.Leader);
                            if (LeaderUserWorkticketAddnew != null)
                            {
                                recipientsModels.Add(new AddEmailTransmissionRecipientsModel
                                {
                                    RecipientEmailAddr = LeaderUserWorkticketAddnew.Email,
                                    RecipientEmailName = LeaderUserWorkticketAddnew.FullName,
                                    UserID = LeaderUserWorkticketAddnew.ID,
                                    RecipientType = 3,
                                });
                            }

                            // Người chỉ huy trực tiếp
                            StakeholdersResponseModel CommandersWorkticketAddnew = GetLeadership(requestModel.WorkTicketID, LeadershipUserRoles.Commander);
                            if (CommandersWorkticketAddnew != null)
                            {
                                recipientsModels.Add(new AddEmailTransmissionRecipientsModel
                                {
                                    RecipientEmailAddr = CommandersWorkticketAddnew.Email,
                                    RecipientEmailName = CommandersWorkticketAddnew.FullName,
                                    UserID = CommandersWorkticketAddnew.ID,
                                    RecipientType = 3,
                                });
                            }
                        }

                        // người cho phép
                        StakeholdersResponseModel AcceptorWorkticketAddNew = GetLeadership(requestModel.WorkTicketID, LeadershipUserRoles.Acceptor);
                        if (AcceptorWorkticketAddNew != null)
                        {
                            recipientsModels.Add(new AddEmailTransmissionRecipientsModel
                            {
                                RecipientEmailAddr = AcceptorWorkticketAddNew.Email,
                                RecipientEmailName = AcceptorWorkticketAddNew.FullName,
                                UserID = AcceptorWorkticketAddNew.ID,
                                RecipientType = 3,
                            });
                        }
                        break;

                    //Cho phép công tác
                    case 6:
                    case 13:
                        eventName = EnumExtensions.GetEnumDescription(EnumNotificationType.Allow);
                        eventValue = _unitOfWork.ElogbookRepository<SystemData>()
                            .GetQuery(x => x.Description.Equals(eventName, StringComparison.OrdinalIgnoreCase)
                                 && x.DataType == (int)SystemDataType.EmailNotificationEvent)
                            .Select(x => x.ID).FirstOrDefault();

                        entity = _unitOfWork.ElogbookRepository<EmailTemplate>()
                            .GetQuery(x => x.ModuleID == moduleValue && x.NotificationTypeID == eventValue).FirstOrDefault();

                        if (entity != null)
                        {
                            transmissionsModel.Subject = entity.EmailSubject;
                            transmissionsModel.MessageBody = entity.EmailContent;
                        }

                        //Account ký cho phép
                        recipientsModels.Add(new AddEmailTransmissionRecipientsModel
                        {
                            RecipientEmailAddr = userInfo.Email,
                            RecipientEmailName = userInfo.FullName,
                            UserID = userInfo.ID,
                            RecipientType = 3,
                        });

                        if (requestModel.RegistrationTicketType == (int)EnumModuleNotification.PCTD)
                        {
                            // lãnh đạo (nếu có)
                            StakeholdersResponseModel LeaderUserWorkticket = GetLeadership(requestModel.WorkTicketID, LeadershipUserRoles.Leader);
                            if (LeaderUserWorkticket != null)
                            {
                                recipientsModels.Add(new AddEmailTransmissionRecipientsModel
                                {
                                    RecipientEmailAddr = LeaderUserWorkticket.Email,
                                    RecipientEmailName = LeaderUserWorkticket.FullName,
                                    UserID = LeaderUserWorkticket.ID,
                                    RecipientType = 3,
                                });
                            }

                            // những người thực hiện
                            var Workers = GetStakeholders(requestModel.RegistrationTicketID, RolesStakeholder.Worker);
                            if (Workers.Any())
                            {
                                foreach (var user in Workers)
                                {
                                    var isExistUser = recipientsModels.Find(x => x.UserID == user.ID);
                                    if (user != null && !string.IsNullOrWhiteSpace(user.Email) && isExistUser == null)
                                    {
                                        recipientsModels.Add(new AddEmailTransmissionRecipientsModel
                                        {
                                            RecipientEmailAddr = user.Email,
                                            RecipientEmailName = user.FullName,
                                            UserID = user.AssignToUserID > 0 ? user.AssignToUserID.Value : 0,
                                            RecipientType = 3,
                                        });
                                    }
                                }
                            }
                        }

                        // Người chỉ huy trực tiếp
                        StakeholdersResponseModel CommandersWorkticketAllow = GetLeadership(requestModel.WorkTicketID, LeadershipUserRoles.Commander);
                        if (CommandersWorkticketAllow != null)
                        {
                            recipientsModels.Add(new AddEmailTransmissionRecipientsModel
                            {
                                RecipientEmailAddr = CommandersWorkticketAllow.Email,
                                RecipientEmailName = CommandersWorkticketAllow.FullName,
                                UserID = CommandersWorkticketAllow.ID,
                                RecipientType = 3,
                            });
                        }

                        // người chp phép
                        StakeholdersResponseModel AcceptorWorkticketAllow = GetLeadership(requestModel.WorkTicketID, LeadershipUserRoles.Acceptor);
                        if (AcceptorWorkticketAllow != null)
                        {
                            recipientsModels.Add(new AddEmailTransmissionRecipientsModel
                            {
                                RecipientEmailAddr = AcceptorWorkticketAllow.Email,
                                RecipientEmailName = AcceptorWorkticketAllow.FullName,
                                UserID = AcceptorWorkticketAllow.ID,
                                RecipientType = 3,
                            });
                        }
                        break;

                    //Đã hoàn thành công tác
                    case 7:
                    case 14:
                        eventName = EnumExtensions.GetEnumDescription(EnumNotificationType.Complete);
                        eventValue = _unitOfWork.ElogbookRepository<SystemData>()
                            .GetQuery(x => x.Description.Equals(eventName, StringComparison.OrdinalIgnoreCase)
                                 && x.DataType == (int)SystemDataType.EmailNotificationEvent)
                            .Select(x => x.ID).FirstOrDefault();

                        entity = _unitOfWork.ElogbookRepository<EmailTemplate>()
                            .GetQuery(x => x.ModuleID == moduleValue && x.NotificationTypeID == eventValue).FirstOrDefault();

                        if (entity != null)
                        {
                            transmissionsModel.Subject = entity.EmailSubject;
                            transmissionsModel.MessageBody = entity.EmailContent;
                        }


                        //Account ký hòan thành
                        recipientsModels.Add(new AddEmailTransmissionRecipientsModel
                        {
                            RecipientEmailAddr = userInfo.Email,
                            RecipientEmailName = userInfo.FullName,
                            UserID = userInfo.ID,
                            RecipientType = 3,
                        });

                        if (requestModel.RegistrationTicketType == (int)EnumModuleNotification.PCTD)
                        {
                            // lãnh đạo (nếu có)
                            StakeholdersResponseModel LeaderUserWorkticket = GetLeadership(requestModel.WorkTicketID, LeadershipUserRoles.Leader);
                            if (LeaderUserWorkticket != null)
                            {
                                recipientsModels.Add(new AddEmailTransmissionRecipientsModel
                                {
                                    RecipientEmailAddr = LeaderUserWorkticket.Email,
                                    RecipientEmailName = LeaderUserWorkticket.FullName,
                                    UserID = LeaderUserWorkticket.ID,
                                    RecipientType = 3,
                                });
                            }

                            // những người thực hiện
                            var Workers = GetStakeholders(requestModel.RegistrationTicketID, RolesStakeholder.Worker);
                            if (Workers.Any())
                            {
                                foreach (var user in Workers)
                                {
                                    var isExistUser = recipientsModels.Find(x => x.UserID == user.ID);
                                    if (user != null && !string.IsNullOrWhiteSpace(user.Email) && isExistUser == null)
                                    {
                                        recipientsModels.Add(new AddEmailTransmissionRecipientsModel
                                        {
                                            RecipientEmailAddr = user.Email,
                                            RecipientEmailName = user.FullName,
                                            UserID = user.AssignToUserID > 0 ? user.AssignToUserID.Value : 0,
                                            RecipientType = 3,
                                        });
                                    }
                                }
                            }
                        }

                        // Người chỉ huy trực tiếp
                        StakeholdersResponseModel CommandersWorkticketComplete = GetLeadership(requestModel.WorkTicketID, LeadershipUserRoles.Commander);
                        if (CommandersWorkticketComplete != null)
                        {
                            recipientsModels.Add(new AddEmailTransmissionRecipientsModel
                            {
                                RecipientEmailAddr = CommandersWorkticketComplete.Email,
                                RecipientEmailName = CommandersWorkticketComplete.FullName,
                                UserID = CommandersWorkticketComplete.ID,
                                RecipientType = 3,
                            });
                        }

                        // người chp phép
                        StakeholdersResponseModel AcceptorWorkticketComplete = GetLeadership(requestModel.WorkTicketID, LeadershipUserRoles.Acceptor);
                        if (AcceptorWorkticketComplete != null)
                        {
                            recipientsModels.Add(new AddEmailTransmissionRecipientsModel
                            {
                                RecipientEmailAddr = AcceptorWorkticketComplete.Email,
                                RecipientEmailName = AcceptorWorkticketComplete.FullName,
                                UserID = AcceptorWorkticketComplete.ID,
                                RecipientType = 3,
                            });
                        }
                        break;

                    //Bảo trì tự quản
                    //Nhắc lịch thực hiện hoạt động AM
                    case 15:
                        break;

                    //Thông báo bất thường, thiết bị
                    case 16:
                        break;

                    //Báo cáo, thống kê hoạt động AM
                    case 17:
                        break;

                    //Phiếu yêu cầu
                    //Có phiếu yêu cầu cần phê duyệt
                    case 18:
                        permissionKey = CommonConstant.PERMISSION_EDIT_QLPYCTC;
                        moduleName = EnumExtensions.GetEnumDescription(EnumModuleNotification.PYC);
                        moduleValue = _unitOfWork.ElogbookRepository<SystemData>()
                            .GetQuery(x => x.Description.Equals(moduleName, StringComparison.OrdinalIgnoreCase) && x.DataType == (int)SystemDataType.ModuleName)
                            .Select(x => x.ID).FirstOrDefault();

                        eventName = EnumExtensions.GetEnumDescription(EnumNotificationType.Register);
                        eventValue = _unitOfWork.ElogbookRepository<SystemData>()
                            .GetQuery(x => x.Description.Equals(eventName, StringComparison.OrdinalIgnoreCase)
                                 && x.DataType == (int)SystemDataType.EmailNotificationEvent)
                            .Select(x => x.ID).FirstOrDefault();

                        entity = _unitOfWork.ElogbookRepository<EmailTemplate>()
                            .GetQuery(x => x.ModuleID == moduleValue && x.NotificationTypeID == eventValue).FirstOrDefault();

                        if (entity != null)
                        {
                            transmissionsModel.Subject = entity.EmailSubject;
                            transmissionsModel.MessageBody = entity.EmailContent;
                        }
                        break;
                    case 19:
                        moduleName = EnumExtensions.GetEnumDescription(EnumModuleNotification.PYC);
                        moduleValue = _unitOfWork.ElogbookRepository<SystemData>()
                            .GetQuery(x => x.Description.Equals(moduleName, StringComparison.OrdinalIgnoreCase) && x.DataType == (int)SystemDataType.ModuleName)
                            .Select(x => x.ID).FirstOrDefault();

                        eventName = EnumExtensions.GetEnumDescription(EnumNotificationType.Reject);
                        eventValue = _unitOfWork.ElogbookRepository<SystemData>()
                            .GetQuery(x => x.Description.Equals(eventName, StringComparison.OrdinalIgnoreCase)
                                 && x.DataType == (int)SystemDataType.EmailNotificationEvent)
                            .Select(x => x.ID).FirstOrDefault();

                        entity = _unitOfWork.ElogbookRepository<EmailTemplate>()
                            .GetQuery(x => x.ModuleID == moduleValue && x.NotificationTypeID == eventValue).FirstOrDefault();

                        if (entity != null)
                        {
                            transmissionsModel.Subject = entity.EmailSubject;
                            transmissionsModel.MessageBody = entity.EmailContent;
                        }
                        break;
                    // Dừng xử lý phiếu yêu cầu
                    case 22:
                        moduleName = EnumExtensions.GetEnumDescription(EnumModuleNotification.PYC);
                        moduleValue = _unitOfWork.ElogbookRepository<SystemData>()
                            .GetQuery(x => x.Description.Equals(moduleName, StringComparison.OrdinalIgnoreCase) && x.DataType == (int)SystemDataType.ModuleName)
                            .Select(x => x.ID).FirstOrDefault();

                        eventName = EnumExtensions.GetEnumDescription(EnumNotificationType.PycStopTC);
                        eventValue = _unitOfWork.ElogbookRepository<SystemData>()
                            .GetQuery(x => x.Description.Equals(eventName, StringComparison.OrdinalIgnoreCase)
                                 && x.DataType == (int)SystemDataType.EmailNotificationEvent)
                            .Select(x => x.ID).FirstOrDefault();

                        entity = _unitOfWork.ElogbookRepository<EmailTemplate>()
                            .GetQuery(x => x.ModuleID == moduleValue && x.NotificationTypeID == eventValue).FirstOrDefault();

                        if (entity != null)
                        {
                            transmissionsModel.Subject = entity.EmailSubject;
                            transmissionsModel.MessageBody = entity.EmailContent;
                        }
                        break;
                    //Phê duyệt phiếu yêu cầu - Gửi nội bộ
                    //Phê duyệt phiếu yêu cầu - Gửi đơn vị sửa chữa
                    case 20:
                    case 21:
                        moduleName = EnumExtensions.GetEnumDescription(EnumModuleNotification.PYC);
                        moduleValue = _unitOfWork.ElogbookRepository<SystemData>()
                            .GetQuery(x => x.Description.Equals(moduleName, StringComparison.OrdinalIgnoreCase) && x.DataType == (int)SystemDataType.ModuleName)
                            .Select(x => x.ID).FirstOrDefault();

                        if (userInfo.RoleInUsers.Any())
                        {
                            var idRoleInUser = userInfo.RoleInUsers.Select(x => x.Value.Value);
                            permissionKey = CommonConstant.PERMISSION_EDIT_QLPYCTC;
                            //Trưởng ca
                            var roleTC = _unitOfWork.ElogbookRepository<UserRole>().GetQuery(x => idRoleInUser.Contains(x.ID) && x.Permissions.Contains("EDIT_QLPYCTC"));
                            if (roleTC.Any())
                            {
                                //Phê duyệt phiếu yêu cầu - Gửi nội bộ
                                eventName = EnumExtensions.GetEnumDescription(EnumNotificationType.PycApprovedTC);
                                eventValue = _unitOfWork.ElogbookRepository<SystemData>()
                                    .GetQuery(x => x.Description.Equals(eventName, StringComparison.OrdinalIgnoreCase)
                                         && x.DataType == (int)SystemDataType.EmailNotificationEvent)
                                    .Select(x => x.ID).FirstOrDefault();

                                entity = _unitOfWork.ElogbookRepository<EmailTemplate>()
                                    .GetQuery(x => x.ModuleID == moduleValue && x.NotificationTypeID == eventValue).FirstOrDefault();

                                if (entity != null)
                                {
                                    transmissionsModel.Subject = entity.EmailSubject;
                                    transmissionsModel.MessageBody = entity.EmailContent;
                                }

                                //Phê duyệt phiếu yêu cầu - Gửi đơn vị sửa chữa
                                eventName = EnumExtensions.GetEnumDescription(EnumNotificationType.PycApprovedVHV);
                                eventValue = _unitOfWork.ElogbookRepository<SystemData>()
                                    .GetQuery(x => x.Description.Equals(eventName, StringComparison.OrdinalIgnoreCase)
                                         && x.DataType == (int)SystemDataType.EmailNotificationEvent)
                                    .Select(x => x.ID).FirstOrDefault();

                                entity = _unitOfWork.ElogbookRepository<EmailTemplate>()
                                    .GetQuery(x => x.ModuleID == moduleValue && x.NotificationTypeID == eventValue).FirstOrDefault();

                                if (entity != null)
                                {
                                    transmissionsVHVModel.Subject = entity.EmailSubject;
                                    transmissionsVHVModel.MessageBody = entity.EmailContent;
                                }
                            }
                        }
                        break;
                }

                if (!string.IsNullOrWhiteSpace(transmissionsModel.Subject))
                {
                    if (notificationTypeValue <= 14)
                    {
                        if (!requestModel.IsPCT)
                        {
                            //Account tạo Phiếu đăng ký công tác
                            var registrationTicket = _unitOfWork.ElogbookRepository<RegistrationTicket>().GetById(requestModel.RegistrationTicketID);

                            if (registrationTicket.CreatedUserID.HasValue)
                            {
                                var user = _unitOfWork.ElogbookRepository<User>().GetById(registrationTicket.CreatedUserID.Value);
                                if (!string.IsNullOrWhiteSpace(user.Email))
                                {
                                    recipientsModels.Add(new AddEmailTransmissionRecipientsModel
                                    {
                                        RecipientEmailAddr = user.Email,
                                        RecipientEmailName = user.FullName,
                                        UserID = user.ID,
                                        RecipientType = 3,
                                    });
                                }
                            }

                            //Account có quyền Cấp phiếu công tác Điện thuộc đúng Nhà máy nhận phiếu
                            //Account có quyền Cấp phiếu công tác Thủy-cơ-nhiệt-hóa thuộc đúng Nhà máy nhận phiếu
                            var users = GetRelatedUsers(registrationTicket.FactoryID, permissionKey);
                            if (users.Any())
                            {
                                foreach (var item in users)
                                {
                                    var isExistUser = recipientsModels.Find(x => x.UserID == item.ID);
                                    if (!string.IsNullOrWhiteSpace(item.Email) && isExistUser == null)
                                    {
                                        recipientsModels.Add(new AddEmailTransmissionRecipientsModel
                                        {
                                            RecipientEmailAddr = item.Email,
                                            RecipientEmailName = item.FullName,
                                            UserID = item.ID,
                                            RecipientType = 3
                                        });
                                    }
                                }
                            }
                            if (!string.IsNullOrWhiteSpace(transmissionsModel.Subject) && recipientsModels.Any())
                            {
                                if (registrationTicket.FactoryID > 0)
                                {
                                    var factory = _unitOfWork.ElogbookRepository<Factory>().GetById(registrationTicket.FactoryID);

                                    if (factory != null)
                                    {
                                        transmissionsModel.FatoryEmails = !string.IsNullOrWhiteSpace(factory.Email) ? factory.Email : string.Empty;
                                        transmissionsModel.FatoryName = factory.FactoryName;
                                    }
                                }
                                _commonService.AddEmailTransmissions(transmissionsModel, recipientsModels, requestModel.CurrentUserID, requestModel.RegistrationTicketID);
                            }
                        }
                        else
                        {
                            //Account tạo Phiếu đăng ký công tác
                            var workTicket = _unitOfWork.ElogbookRepository<WorkTicket>().GetById(requestModel.WorkTicketID);
                            if (workTicket.CreatedUserID.HasValue)
                            {
                                var user = _unitOfWork.ElogbookRepository<User>().GetById(workTicket.CreatedUserID.Value);
                                if (!string.IsNullOrWhiteSpace(user.Email))
                                {
                                    recipientsModels.Add(new AddEmailTransmissionRecipientsModel
                                    {
                                        RecipientEmailAddr = user.Email,
                                        RecipientEmailName = user.FullName,
                                        UserID = user.ID,
                                        RecipientType = 3,
                                    });
                                }
                                if (!string.IsNullOrWhiteSpace(transmissionsModel.Subject) && recipientsModels.Any())
                                {
                                    if (workTicket.FactoryID > 0)
                                    {
                                        var factory = _unitOfWork.ElogbookRepository<Factory>().GetById(workTicket.FactoryID.Value);

                                        if (factory != null)
                                        {
                                            transmissionsModel.FatoryEmails = !string.IsNullOrWhiteSpace(factory.Email) ? factory.Email : string.Empty;
                                            transmissionsModel.FatoryName = factory.FactoryName;
                                        }
                                    }
                                    _commonService.AddEmailTransmissions(transmissionsModel, recipientsModels, requestModel.CurrentUserID, CommonConstant.ZERO, requestModel.WorkTicketID);
                                }
                            }
                        }
                    }

                    if (requestModel.AmRequestTicketID > CommonConstant.ZERO)
                    {
                        //account tạo PYC
                        var requestTicket = _unitOfWork.ElogbookRepository<AmRequestTicket>().GetById(requestModel.AmRequestTicketID);
                        if (requestTicket != null)
                        {
                            //acc tạo
                            var userCreate = _unitOfWork.ElogbookRepository<User>().GetById(requestTicket.CreatedUserID ?? CommonConstant.ZERO);
                            if (userCreate != null)
                            {
                                recipientsModels.Add(new AddEmailTransmissionRecipientsModel
                                {
                                    RecipientEmailAddr = userCreate.Email,
                                    RecipientEmailName = userCreate.FullName,
                                    UserID = userCreate.ID,
                                    RecipientType = 3,
                                });
                            }

                            //TC
                            var users = GetRelatedUsers(requestTicket.FactoryID, permissionKey);
                            if (users.Any())
                            {
                                foreach (var item in users)
                                {
                                    var isExistUser = recipientsModels.Find(x => x.UserID == item.ID);
                                    if (!string.IsNullOrWhiteSpace(item.Email) && isExistUser == null)
                                    {
                                        recipientsModels.Add(new AddEmailTransmissionRecipientsModel
                                        {
                                            RecipientEmailAddr = item.Email,
                                            RecipientEmailName = item.FullName,
                                            UserID = item.ID,
                                            RecipientType = 3
                                        });
                                    }
                                }
                            }
                            if (!string.IsNullOrWhiteSpace(transmissionsModel.Subject) && recipientsModels.Any())
                            {
                                if (requestTicket.FactoryID > 0)
                                {
                                    var factory = _unitOfWork.ElogbookRepository<Factory>().GetById(requestTicket.FactoryID);

                                    if (factory != null && !string.IsNullOrWhiteSpace(factory.Email))
                                    {
                                        transmissionsModel.FatoryEmails = factory.Email;
                                        transmissionsModel.FatoryName = factory.FactoryName;
                                    }
                                }
                                _commonService.AddEmailTransmissions(transmissionsModel, recipientsModels, requestModel.CurrentUserID, CommonConstant.ZERO, CommonConstant.ZERO, CommonConstant.ZERO, requestModel.AmRequestTicketID);
                            }

                            if (notificationTypeValue == 20 || notificationTypeValue == 21)
                            {
                                //VHV
                                var usersPYC = GetUserDVSC(requestTicket.ID);
                                if (usersPYC.Any())
                                {
                                    foreach (var item in usersPYC)
                                    {
                                        if (!string.IsNullOrWhiteSpace(item.Email))
                                        {
                                            recipientsVHVModels.Add(new AddEmailTransmissionRecipientsModel
                                            {
                                                RecipientEmailAddr = item.Email,
                                                RecipientEmailName = item.FullName,
                                                UserID = item.ID,
                                                RecipientType = 3
                                            });
                                        }
                                    }
                                }
                                if (!string.IsNullOrWhiteSpace(transmissionsVHVModel.Subject) && recipientsVHVModels.Any())
                                {
                                    if (requestTicket.FactoryID > 0)
                                    {
                                        var factory = _unitOfWork.ElogbookRepository<Factory>().GetById(requestTicket.FactoryID);

                                        if (factory != null && !string.IsNullOrWhiteSpace(factory.Email))
                                        {
                                            transmissionsModel.FatoryEmails = factory.Email;
                                            transmissionsModel.FatoryName = factory.FactoryName;
                                        }
                                    }
                                    _commonService.AddEmailTransmissions(transmissionsVHVModel, recipientsVHVModels, requestModel.CurrentUserID, CommonConstant.ZERO, CommonConstant.ZERO, CommonConstant.ZERO, requestModel.AmRequestTicketID);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "ProcessEmail",
                        "Có lỗi xảy ra khi xử lý các sự kiện gửi email!" + Environment.NewLine + ex.ToString());
            }
        }

        private List<UserModel> GetRelatedUsers(int factoryID, string permissionKey)
        {
            var roleIDs = _unitOfWork.ElogbookRepository<UserRole>().GetQuery(x => x.Permissions.Contains(permissionKey)).Select(x => x.ID).ToList();
            var userIDs = _unitOfWork.ElogbookRepository<RoleInUser>().GetQuery(x => roleIDs.Contains(x.UserRoleID)).Select(x => x.UserID).ToList();

            List<UserModel> userModels = _unitOfWork.ElogbookRepository<User>()
                .GetQuery(x => userIDs.Contains(x.ID) && x.FactoryID == factoryID && !x.IsDeleted).Select(
                e => new UserModel()
                {
                    ID = e.ID,
                    FullName = e.FullName,
                    UserName = e.UserName,
                    Email = e.Email,
                }).ToList();

            return userModels;
        }

        private List<UserModel> GetUserDVSC(int requestTicketID)
        {
            List<UserModel> result = new List<UserModel>();
            var requestTicket = _unitOfWork.ElogbookRepository<AmRequestTicket>().GetById(requestTicketID);
            if (requestTicket != null && requestTicket.OrganizationId.HasValue)
            {
                SearchOrganizationModel search = new SearchOrganizationModel { Active = -1 };
                var orgs = _organizationService.GetOrganizations(search);
                //Thông tin Phân xưởng
                if (orgs != null && orgs.Count > 0 && requestTicket.OrganizationId.HasValue)
                {
                    var org = orgs.Find(x => x.Data.ID == requestTicket.OrganizationId.Value && x.Children.Any());
                    if (org != null)
                    {
                        var child = org.Children.Find(c => c.Data.ProductionPlantType == "Sửa chữa");
                        if (child != null)
                        {
                            result = _unitOfWork.ElogbookRepository<User>()
                            .GetQuery(x => x.ProductionPlantID == child.Data.ID && !x.IsDeleted).Select(
                            e => new UserModel()
                            {
                                ID = e.ID,
                                FullName = e.FullName,
                                UserName = e.UserName,
                                Email = e.Email,
                            }).ToList();
                        }
                    }
                }
            }

            return result;
        }

        private LeadershipUserModel GetLeadershipById(int workTicketID, int roles, bool isGetLastCommander = false, int screenType = 1)
        {
            if (roles == (int)LeadershipUserRoles.Commander && isGetLastCommander)
            {
                int lastCommanderID = CommonConstant.ZERO;
                bool isExternalUser = false;
                int signtureCheckoutID = CommonConstant.ZERO;

                // Trường hợp Phiếu Điện
                if (screenType == CommonConstant.TYPE_PCTD)
                {
                    var dataWorkPlacement = _unitOfWork.ElogbookRepository<WorkPlacement>().GetQuery(x => x.WorkTicketID == workTicketID && !x.IsDeleted)
                            .Select(e => new WorkPlacementModel()
                            {
                                ID = e.ID,
                                Place = e.Place,
                                //Người chỉ huy trực tiếp
                                CommanderName = e.CommanderName,
                                StartTimeTemp = e.StartTime,
                                EndTimeTemp = e.EndTime,
                                CommanderSignatureID = e.CommanderSignature,
                                //Người chi phép
                                AllowerName = e.AcceptorName,
                                AllowerSignatureID = e.AcceptorSignature,
                            })
                     .ToList();

                    if (dataWorkPlacement.Count > CommonConstant.ZERO)
                    {
                        var lastCommander = dataWorkPlacement.Where(a => a.CommanderSignatureID != null).OrderByDescending(x => x.EndTimeTemp).FirstOrDefault();
                        if (lastCommander != null)
                        {
                            lastCommanderID = lastCommander.CommanderUserID != null ? (int)lastCommander.CommanderUserID : CommonConstant.ZERO;
                        }
                    }
                }
                // Trường hợp Phiếu Thủy Cơ Nhiệt Hóa
                else
                {
                    // Lấy ngày làm việc cuối cùng
                    var lastWorkDaily = _unitOfWork.ElogbookRepository<WorkDaily>()
                        .GetQuery(x => x.WorkTicketID == workTicketID && !x.IsDeleted)
                        .OrderByDescending(x => x.WorkDay).ToList();

                    if (lastWorkDaily.Count > CommonConstant.ZERO)
                    {
                        foreach (var workDaily in lastWorkDaily)
                        {
                            // Lấy ID của người chỉ huy trực tiếp ký checkout cuối cùng
                            var lastStaffInformation = _unitOfWork.ElogbookRepository<StaffInformation>()
                                .GetQuery(x => x.WorkDailyID == workDaily.ID && !x.IsDeleted && x.CheckOutSignature != null)
                                .OrderByDescending(x => x.UpdatedAt).FirstOrDefault();

                            if (lastStaffInformation != null)
                            {
                                isExternalUser = (bool)lastStaffInformation.IsExternal;
                                signtureCheckoutID = lastStaffInformation.CheckOutSignature != null ? (int)lastStaffInformation.CheckOutSignature : CommonConstant.ZERO;
                                break;
                            }
                        }
                    }
                }

                if (lastCommanderID > CommonConstant.ZERO)
                {
                    // Người chỉ huy trực tiếp thuộc ngoài hệ thống
                    if (isExternalUser)
                    {
                        var guestUser = _unitOfWork.ElogbookRepository<GuestUser>().GetQuery(x => x.ID == lastCommanderID && !x.IsDeleted).Select(
                            e => new LeadershipUserModel()
                            {
                                WorkTicketID = workTicketID,
                                UserName = e.UserName,
                                FullName = e.FullName,
                                ElectricRankStr = e.ElectricRank,
                                ElectricRankCard = e.ElectricRankCard,
                                LaborRankStr = e.LaborRank,
                                LaborCard = e.LaborCard,
                                ID = e.ID
                            }).FirstOrDefault();

                        if (guestUser != null)
                        {
                            if (signtureCheckoutID > CommonConstant.ZERO)
                            {
                                var signtureInfo = _commonService.GetSignatureById(signtureCheckoutID);

                                if (signtureInfo != null)
                                {
                                    guestUser.Signature = BytesToString(signtureInfo.SignatureContentByte);
                                }
                            }

                            return guestUser;
                        }
                    }

                    // Người chỉ huy trực tiếp thuộc nội bộ
                    if (!isExternalUser)
                    {
                        var user = _unitOfWork.ElogbookRepository<User>().GetQuery(x => x.ID == lastCommanderID && !x.IsDeleted).Select(
                            e => new LeadershipUserModel()
                            {
                                FullName = e.FullName,
                                UserName = e.UserName,
                                ElectricRankStr = e.ElectricRank,
                                ElectricRankCard = e.ElectricRankCard,
                                LaborRankStr = e.LaborRank,
                                LaborCard = e.LaborCard,
                                Roles = roles,
                                Signature = e.Signature,
                                ID = e.ID,
                                PositionName = e.PositionName,
                            }).FirstOrDefault();

                        if (user != null)
                        {
                            return user;
                        }
                    }
                }
            }

            // trường hợp người chỉ huy chưa được thay đổi thì lấy mặc định ban đầu
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

        private string BytesToString(byte[] bytes)
        {
            return bytes != null ? Encoding.UTF8.GetString(bytes) : null;
        }

        public List<StakeholdersResponseModel> GetStakeholders(int registrationTicketID, RolesStakeholder rolesStakeholder)
        {
            return _unitOfWork.ElogbookRepository<Stakeholders>().GetQuery(x => !x.IsDeleted && x.Roles == (int)rolesStakeholder &&
                                                                            x.RegistrationTicketID == registrationTicketID)
                .Select(t => new StakeholdersResponseModel()
                {
                    AssignToUserID = t.AssignToUserID,
                    FullName = t.AssignToUserID > CommonConstant.ZERO ? t.AssignToUser.FullName : t.FullName,
                    UserName = t.UserName,
                    Email = t.AssignToUser.Email
                }).ToList();
        }

        public StakeholdersResponseModel GetLeadership(int workTicketID, LeadershipUserRoles role)
        {
            var leader = _unitOfWork.ElogbookRepository<LeadershipUser>()
                             .GetQuery(x => x.WorkTicketID == workTicketID && x.Roles == (int)role && !x.IsDeleted).FirstOrDefault();
            if (leader != null && !string.IsNullOrWhiteSpace(leader.UserName))
            {
                return _unitOfWork.ElogbookRepository<User>().GetQuery(x => x.UserName == leader.UserName)
                    .Select(t => new StakeholdersResponseModel()
                    {
                        ID = t.ID,
                        FullName = t.FullName,
                        UserName = t.UserName,
                        Email = t.Email
                    }).FirstOrDefault();
            }

            return null;
        }

        /// <summary>
        /// Xử lý gửi email thông báo trước và sau khi hoàn thành phiếu công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public bool ProcessEmailWorkTicketCompletion(ProcessEmailRequestModel requestModel)
        {
            var result = false;

            try
            {
                //get system data
                var moduleName = EnumExtensions.GetEnumDescription(requestModel.ModuleNotification);
                var moduleValue = _unitOfWork.ElogbookRepository<SystemData>()
                    .GetQuery(x => x.Description.Equals(moduleName, StringComparison.OrdinalIgnoreCase) && x.DataType == (int)SystemDataType.ModuleName)
                    .Select(x => x.ID).FirstOrDefault();

                if (moduleValue <= CommonConstant.ZERO)
                {
                    return false;
                }

                //Thông tin mail server
                var mailConfig = _emailConfigurationService.GetEmailConfiguration();
                AddEmailTransmissionsModel transmissionsModel = new AddEmailTransmissionsModel()
                {
                    EmailReplyToAddr = mailConfig.EmailAddress,
                    Subject = "",
                    MessageBody = "",
                    IsBodyHtml = true,
                    DetailFailure = "",
                };

                List<AddEmailTransmissionRecipientsModel> recipientsModels = new List<AddEmailTransmissionRecipientsModel>();
                var eventName = string.Empty;
                var eventValue = CommonConstant.ZERO;
                EmailTemplate entity;

                if (requestModel.NotificationTypeValue == 1)
                {
                    eventName = EnumExtensions.GetEnumDescription(EnumNotificationType.BeforeDueCompletion);
                } 
                else if(requestModel.NotificationTypeValue == 2)
                {
                    eventName = EnumExtensions.GetEnumDescription(EnumNotificationType.DueTodayCompletion);
                }
                else
                {
                    eventName = EnumExtensions.GetEnumDescription(EnumNotificationType.OverdueCompletion);
                }

                eventValue = _unitOfWork.ElogbookRepository<SystemData>()
                    .GetQuery(x => x.Description.Equals(eventName, StringComparison.OrdinalIgnoreCase)
                         && x.DataType == (int)SystemDataType.EmailNotificationEvent)
                    .Select(x => x.ID).FirstOrDefault();

                entity = _unitOfWork.ElogbookRepository<EmailTemplate>()
                    .GetQuery(x => x.ModuleID == moduleValue && x.NotificationTypeID == eventValue).FirstOrDefault();

                if (entity != null)
                {
                    entity.EmailSubject = entity.EmailSubject.Replace("[Gio]", requestModel.TimeDue);
                    entity.EmailContent = entity.EmailContent.Replace("[Gio]", requestModel.TimeDue);
                    transmissionsModel.Subject = entity.EmailSubject;
                    transmissionsModel.MessageBody = entity.EmailContent;
                }

                //var leadershipUsers = _unitOfWork.ElogbookRepository<LeadershipUser>()
                //        .GetQuery(x => x.WorkTicketID == requestModel.WorkTicketID && !x.IsDeleted)
                //        .Select(e => new LeadershipUserModel() { UserName = e.UserName, IsExternal = e.IsExternal ?? false }).ToList();

                //// Người cấp phiếu
                //// Người chỉ huy trực tiếp
                //// Người cho phép
                //if (leadershipUsers.Count > 0)
                //{
                //    foreach (var userName in leadershipUsers.Select(x => x.UserName))
                //    {
                //        // Người dùng trong hệ thống
                //        var userInfo = _unitOfWork.ElogbookRepository<User>()
                //            .GetQuery(x => x.UserName == userName && !x.IsDeleted).FirstOrDefault();

                //        if (userInfo != null && !string.IsNullOrWhiteSpace(userInfo.Email))
                //        {
                //            recipientsModels.Add(new AddEmailTransmissionRecipientsModel
                //            {
                //                RecipientEmailAddr = userInfo.Email,
                //                RecipientEmailName = userInfo.FullName,
                //                UserID = userInfo.ID,
                //                RecipientType = 3,
                //            });
                //        }
                //    }
                //}

                // Lấy danh sách thông tin mail người dùng thông qua nhóm người dùng.
                if (requestModel.UserGroupIds != null && requestModel.UserGroupIds.Any())
                {
                    var userIds = _unitOfWork.ElogbookRepository<UserInGroup>()
                        .GetQuery(x =>
                            requestModel.UserGroupIds.Contains(x.UserGroupsID) &&
                            !x.IsDeleted)
                        .Select(x => x.UserID)
                        .Distinct()
                        .ToList();

                    var users = _unitOfWork.ElogbookRepository<User>()
                        .GetQuery(x =>
                            userIds.Contains(x.ID) && !x.IsDeleted &&
                            x.Email != null && x.Email != "")
                        .ToList();

                    recipientsModels.AddRange(users.Select(user => new AddEmailTransmissionRecipientsModel
                    {
                        RecipientEmailAddr = user.Email,
                        RecipientEmailName = user.FullName,
                        UserID = user.ID,
                        RecipientType = 3
                    }));
                }

                if (!string.IsNullOrWhiteSpace(transmissionsModel.Subject)
                    && requestModel.CurrentUserID > 0)
                {
                    var user = _unitOfWork.ElogbookRepository<User>().GetById(requestModel.CurrentUserID);
                    if (!string.IsNullOrWhiteSpace(user.Email))
                    {
                        recipientsModels.Add(new AddEmailTransmissionRecipientsModel
                        {
                            RecipientEmailAddr = user.Email,
                            RecipientEmailName = user.FullName,
                            UserID = user.ID,
                            RecipientType = 3,
                        });
                    }

                    if (!string.IsNullOrWhiteSpace(transmissionsModel.Subject) && recipientsModels.Any())
                    {
                        if (requestModel.FactoryID > 0)
                        {
                            var factory = _unitOfWork.ElogbookRepository<Factory>().GetById(requestModel.FactoryID);
                            if (factory != null)
                            {
                                transmissionsModel.FatoryEmails = !string.IsNullOrWhiteSpace(factory.Email) ? factory.Email : string.Empty;
                                transmissionsModel.FatoryName = factory.FactoryName;
                            }
                        }
                        _commonService.AddEmailTransmissions(transmissionsModel, recipientsModels, requestModel.CurrentUserID, CommonConstant.ZERO, requestModel.WorkTicketID);
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                LogCustom.LogMessage(TrackingLogLevel.ERROR, "ProcessEmailWorkTicketCompletion",
                        "Có lỗi xảy ra khi xử lý các sự kiện gửi email!" + Environment.NewLine + ex.ToString());
            }

            return result;
        }
    }

}
