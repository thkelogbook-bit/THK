using System;

namespace Elogbook.Core.Models.Response.Manipulation
{
    /// <summary>
    /// SearchTemplateTicketModel
    /// </summary>
    public class SearchTemplateTicketModel
    {
        /// <summary>
        /// tên phiếu thao tác mẫu
        /// </summary>
        public string TemplateTicketName { get; set; }

        /// <summary>
        /// danh sách mã số công ty trong ticket mẫu
        /// </summary>
        public string FactorysTemplateTicket { get; set; }


        /// <summary>
        /// trạng thái sử dụng phiếu mẫu
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Thời gian tạo
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        public ValueLabelModel CreateUser { get; set; }

        /// <summary>
        /// mã số
        /// </summary>
        public int ID { get; set; }
    }
}
