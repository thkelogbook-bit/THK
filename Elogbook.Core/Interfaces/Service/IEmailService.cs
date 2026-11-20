using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response.EmailNotification;
using Elogbook.Core.Models.Response.EmailService;
using System.Collections.Generic;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// IEmailService
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Lấy thông tin email
        /// </summary>
        /// <returns></returns>
        List<EmailTransmissionModel> GetEmailTransmissions();

        /// <summary>
        /// Gửi mail
        /// </summary>
        /// <param name="item"></param>
        /// <param name="dataEmailSetting"></param>
        /// <param name="transmissionRecipient"></param>
        void SendEmailConfiguration(EmailTransmissionModel item, EmailConfigurationResponseModel dataEmailSetting, EmailTransmissionRecipientsModel transmissionRecipient);

        /// <summary>
        /// Gửi mail
        /// </summary>
        /// <param name="id"></param>
        void SendEmailConfiguration(int id);

        /// <summary>
        /// Xử lý email
        /// </summary>
        /// <param name="requestModel"></param>
        void ProcessEmail(ProcessEmailRequestModel requestModel);

        /// <summary>
        /// get danh sách mail gửi từ id
        /// </summary>
        /// <param name="EmailTransmissionID"></param>
        /// <returns></returns>
        List<EmailTransmissionRecipientsModel> GetEmailTransmissionRecipients(int EmailTransmissionID);

        /// <summary>
        /// Xử lý email thông báo hoàn thành phiếu công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        bool ProcessEmailWorkTicketCompletion(ProcessEmailRequestModel requestModel);
    }
}
