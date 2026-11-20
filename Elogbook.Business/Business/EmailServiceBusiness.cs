using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Core.Log;
using System;

namespace Elogbook.Business
{
    public class EmailServiceBusiness : IEmailServiceBusiness
    {
        private readonly IEmailService _emailService;
        private readonly IEmailConfigurationService _emailConfigurationService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userService"></param>
        public EmailServiceBusiness(IEmailService emailService, IEmailConfigurationService emailConfigurationService)
        {
            _emailService = emailService;
            _emailConfigurationService = emailConfigurationService;
        }

        public void HandleEmailTransmission()
        {
            try
            {
                // lấy danh sách mail chưa được xử lý
                var dataEmails = this._emailService.GetEmailTransmissions();

                // lấy cấu hình email đã được setting
                var dataEmailSetting = _emailConfigurationService.GetEmailConfiguration();

                // đọc danh sách mail đã lấy rồi thực hiện gửi mail
                foreach (var item in dataEmails)
                {
                    var emailTransmissionRecipients = _emailService.GetEmailTransmissionRecipients(item.ID);

                    foreach(var transmissionRecipient in emailTransmissionRecipients)
                    {
                        // gửi và cập nhật kết quả vào database
                        _emailService.SendEmailConfiguration(item, dataEmailSetting, transmissionRecipient);
                    }
                }
            }
            catch (Exception ex)
            {
                LogCustom.LogMessage(TrackingLogLevel.ERROR, Category.WorkTicketBusiness.ToString(),
                    Environment.NewLine + "Gửi mail không thành công!" + Environment.NewLine + ex.ToString());
            }
        }
    }
}
