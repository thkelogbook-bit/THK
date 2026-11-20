namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin cần để xem dữ liệu nhật ký vận hành
    /// </summary>
    public class ViewOperationDiaryRequestModel
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public int OperationPositionID { get; set; }

        /// <summary>
        /// Ngày tạo nhật ký
        /// </summary>
        public string CreatedDate { get; set; }

        /// <summary>
        /// Ca vận hành
        /// </summary>
        public int OperationShiftID { get; set; }
    }
}
