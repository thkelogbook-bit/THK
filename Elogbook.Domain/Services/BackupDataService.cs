using Elogbook.Core;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Response;
using Elogbook.Core.Models.Response.EmailNotification;
using Elogbook.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using Elogbook.Core.Enums;
using Elogbook.Core.Log;
using System.Text;
using Elogbook.Core.Models.BaseFilter;
using Elogbook.Core.Extensions;
using Elogbook.Core.Models.Response.BackupData;
using Elogbook.Data.DataModels.Entities.BackupData;
using Elogbook.Data.DataModels.Entities.Report;

namespace Elogbook.Domain.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class BackupDataService : IBackupDataService
    {
        private readonly ElogbookUnitOfWork _unitOfWork;
        private readonly IFileManagementService _fileManagementService;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="backupDataService"></param>
        public BackupDataService(ElogbookUnitOfWork unitOfWork, IFileManagementService fileManagementService)
        {
            _unitOfWork = unitOfWork;
            _fileManagementService = fileManagementService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public UploadFileModel BackupData()
        {
            var zipResult = _fileManagementService.ZipFiles("C:\\wwwRoot\\Elogbook\\Attachments\\zipTest", new List<string>(), "BackupData_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".zip", null);
            var dataEmailSetting = GetEmailConfiguration();
            DateTime currentDate = DateTime.Now;
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
            var schedule = _unitOfWork.ElogbookRepository<Schedule>().GetQuery().Where(s => s.ScheduleName.ToLower() == "backupdata").FirstOrDefault(); 
            if (schedule != null && schedule.IsSendEmail)
            {
                // Send mail
                using (var mailMessage = new MailMessage())
                {
                    try
                    {
                        mailMessage.Sender = new MailAddress(dataEmailSetting.EmailAddress);
                        mailMessage.From = new MailAddress(dataEmailSetting.EmailAddress);

                        //Subject & Body
                        mailMessage.Subject = schedule.EmailSubject;
                        mailMessage.Body = schedule.EmailContent;
                        mailMessage.BodyEncoding = Encoding.UTF8;
                        mailMessage.IsBodyHtml = true;
                        mailMessage.To.Add(schedule.EmailTo);

                        smtpSend.Send(mailMessage);
                    }
                    catch (Exception ex)
                    {
                        LogCustom.LogMessage(TrackingLogLevel.INFO, "Backup data: Send mail report", "Eror send mail: " + schedule.EmailTo
                            + Environment.NewLine + ex.Message);
                    }
                }
            }
           
            return zipResult;
        }

        public ResponseBaseModel Find(BaseListFilter filter)
        {
            // Filter data by condition
            var reports = _unitOfWork.ElogbookRepository<BackupData>().GetQuery(x => !x.IsDeleted);
            // Return and paging data
            var page = reports.Select(
                e => new BackupDataModel()
                {
                    ID = e.ID,
                    Name = e.Name,
                    Status = e.Status,
                    AttachmentId = e.AttachmentId,
                    InsertedAt = e.InsertedAt,
                }).OrderByDescending(x => x.InsertedAt).ToPagedList(filter.PageNumber, filter.PageSize);

            return HttpResponse.OkList(page);
        }

        public bool Delete(int id, int userID)
        {
            var entity = _unitOfWork.ElogbookRepository<BackupData>().GetQuery(x => !x.IsDeleted && x.ID == id).FirstOrDefault();
            if (entity != null)
            {
                _unitOfWork.ElogbookRepository<BackupData>().Delete(entity, userID);
                return _unitOfWork.Complete() > 0;
            }
            return false;
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
    }
}
