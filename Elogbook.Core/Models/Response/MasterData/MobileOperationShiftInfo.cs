namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin ca vận hành
    /// </summary>
    public class MobileOperationShiftInfo : MobileEntityBaseModel
    {
        /// <summary>
        /// Tên ca
        /// </summary>
        public string OperationShiftName { get; set; }

        /// <summary>
        /// Thứ tự sắp xếp
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Thời gian bắt đầu ca
        /// </summary>
        public double StartTime { get; set; }

        /// <summary>
        /// Thời gian kết thúc ca
        /// </summary>
        public double EndTime { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Active { get; set; }
    }
}
