namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// khai báo model để group dữ liệu tổng quan
    /// </summary>
    public class GroupDataSearchModel
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public ComboboxDataModel Factory { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// tháng
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// tổng hoạt động
        /// </summary>
        public int TotalStatus { get; set; }

        /// <summary>
        /// đã thực hiện
        /// </summary>
        public int DoneMadeStatus { get; set; }

        /// <summary>
        /// không thực hiện
        /// </summary>
        public int DoNotMadeStatus { get; set; }

        /// <summary>
        /// chưa thực hiện
        /// </summary>
        public int UnfulfilledStatus { get; set; }

        /// <summary>
        /// đang thực hiện
        /// </summary>
        public int InprogressStatus { get; set; }
    }
}
