namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số xuất dữ liệu nhân sự ca vận hành 
    /// </summary>
    public class ExportOperationDiaryModel
    {
        /// <summary>
        /// Nhà máy (0 hoặc không truyền = tất cả nhà máy)
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Ngày vận hành(Từ ngày)
        /// </summary>
        public string FromDate { get; set; }

        /// <summary>
        /// Ngày vận hành(Đến ngày)
        /// </summary>
        public string ToDate { get; set; }
    }
}
