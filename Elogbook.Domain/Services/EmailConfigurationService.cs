using Elogbook.Core;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request.EmailNotification;
using Elogbook.Core.Models.Response.EmailNotification;
using Elogbook.Data;
using System.Linq;

namespace Elogbook.Domain.Services
{
    /// <summary>
    /// Triển khai các phương thức cấu hình email hệ thống
    /// </summary>
    public class EmailConfigurationService : IEmailConfigurationService
    {
        #region Properties field

        private readonly ElogbookUnitOfWork _unitOfWork;

        #endregion

        #region Contructor

        public EmailConfigurationService(ElogbookUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        public EmailConfigurationResponseModel GetEmailConfiguration()
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

        public EmailConfigurationResponseModel Save(EmailConfigurationRequestModel requestModel)
        {
            var entity = _unitOfWork.ElogbookRepository<EmailConfiguration>().GetQuery(e => !e.IsDeleted).FirstOrDefault();
            if (entity != null)
            {
                entity.EmailAddress = requestModel.EmailAddress;
                entity.EmailServer = requestModel.EmailServer;
                entity.Port = requestModel.Port;
                entity.EmailType = requestModel.EmailType;
                entity.UserName = requestModel.UserName;
                if (!string.IsNullOrWhiteSpace(requestModel.PassWord))
                {
                    entity.PassWord = EncryptDecrypt.Encrypt(requestModel.PassWord);
                }
                entity.LastUpdatedUserID = requestModel.CurrentUserID;

                _unitOfWork.ElogbookRepository<EmailConfiguration>().Update(entity);
            }
            else
            {
                entity = new EmailConfiguration
                {
                    EmailAddress = requestModel.EmailAddress,
                    EmailServer = requestModel.EmailServer,
                    Port = requestModel.Port,
                    EmailType = requestModel.EmailType,
                    UserName = requestModel.UserName,
                    PassWord = EncryptDecrypt.Encrypt(requestModel.PassWord),
                    CreatedUserID = requestModel.CurrentUserID
                };

                _unitOfWork.ElogbookRepository<EmailConfiguration>().Add(entity);
            }
            _unitOfWork.Complete();

            return GetEmailConfiguration();
        }
    }
}
