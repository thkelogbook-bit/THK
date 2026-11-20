namespace Elogbook.Core.Models.Response.WorkPermit
{
    /// <summary>
    /// Tham số tạo Header File PDF WorkTicket
    /// </summary>
    public class LctPdf881HeaderModel
    {
        /// <summary>
        /// Tên công ty
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Đơn vị cấp lệnh
        /// </summary>
        public string ApprovedUnit { get; set; }

        /// <summary>
        /// Tiêu đề
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Số lệnh công tác
        /// </summary>
        public string WorkTicketNumber { get; set; }

        /// <summary>
        /// Thủy
        /// </summary>
        public string Thuy { get; set; }

        /// <summary>
        /// Cơ
        /// </summary>
        public string Co { get; set; }

        /// <summary>
        /// Nhiệt
        /// </summary>
        public string Nhiet { get; set; }

        /// <summary>
        /// Hóa
        /// </summary>
        public string Hoa { get; set; }

        /// <summary>
        /// Xay dung
        /// </summary>
        public string XayDung { get; set; }
    }
}
