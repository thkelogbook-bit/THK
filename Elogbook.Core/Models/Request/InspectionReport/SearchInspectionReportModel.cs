using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin tìm danh sách BBKSHT
    /// </summary>
    public class SearchInspectionReportModel : BaseListFilter
    {

        /// <summary>
        /// Đơn vị đăng ký BBKSHT
        /// </summary>
        public int ProductionPlantID { get; set; }

        /// <summary>
        /// Ngày đăng ký (Từ ngày)
        /// </summary>
        public string FromDate { get; set; }

        /// <summary>
        /// Ngày đăng ký (Đến ngày)
        /// </summary>
        public string ToDate { get; set; }

        /// <summary>
        /// Số phiếu BBKSHT
        /// </summary>
        public string TicketNumber { get; set; }

        /// <summary>
        /// Trạng thái BBKSHT
        /// </summary>
        public int StatusValue { get; set; }

        /// <summary>
        /// Lấy những BBKSHT đã đc add vòa phiếu công tác hay không
        /// </summary>
        public bool? IsAssigned { get; set; }

    }
}
