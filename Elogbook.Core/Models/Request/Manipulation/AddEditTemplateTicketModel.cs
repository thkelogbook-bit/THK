using Newtonsoft.Json;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request.Manipulation
{
    /// <summary>
    /// AddEditTemplateTicketModel
    /// </summary>
    public class AddEditTemplateTicketModel
    {
        /// <summary>
        /// Mã số
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Tên phiếu mẫu
        /// </summary>
        public string TemplateTicketName { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public string IdFactorysTemplateTicket { get; set; }

        /// <summary>
        /// Trình tự thao tác
        /// </summary>
        public List<AddEditSequenceOperationModel> SequenceOperations { get; set; }

        /// <summary>
        /// Trạng thái truy cập
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
