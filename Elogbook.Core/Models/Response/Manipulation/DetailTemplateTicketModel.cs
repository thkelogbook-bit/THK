using System.Collections.Generic;

namespace Elogbook.Core.Models.Response.Manipulation
{
    /// <summary>
    /// DetailTemplateTicketModel
    /// </summary>
    public class DetailTemplateTicketModel
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
        /// Danh sách nhà máy
        /// </summary>
        public List<FactoryModel> Factorys { get; set; }

        /// <summary>
        /// Trình tự hạng mục thao tác
        /// </summary>
        public virtual List<SequenceOperationModel> SequenceOperations { get; set; }

        /// <summary>
        /// Trạng thái truy cập
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        public ValueLabelModel CreatedUser { get; set; }

        /// <summary>
        /// Người cập nhật
        /// </summary>
        public ValueLabelModel LastUpdatedUser { get; set; }
    }
}
