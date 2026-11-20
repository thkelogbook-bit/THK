using Elogbook.Core.Models;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// IEmailServiceBusiness
    /// </summary>
    public interface IEmailServiceBusiness
    {
        /// <summary>
        /// xử lý gửi và cập nhật trạng thái của mail
        /// </summary>
        void HandleEmailTransmission();
    }
}
