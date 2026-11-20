using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các thuộc tính lỗi của sửa chữa lớn
    /// </summary>
    public class MajorRepairErrorModel
    {
        /// <summary>
        /// Row index
        /// </summary>
        public string ExcelRow { get; set; }

        /// <summary>
        /// Loại phiếu sửa chữa lớn
        /// </summary>
        public int TicketType { get; set; }

        /// <summary>
        /// ID Phiếu cha
        /// </summary>
        public bool IsParentTicket { get; set; } = false;

        /// <summary>
        /// Các loại lỗi
        /// </summary>
        public List<string> ErrorList { get; set; }
    }
}
