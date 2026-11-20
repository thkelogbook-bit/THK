namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin Chu kỳ
    /// </summary>
    public class MobileCycleManagementInfo : MobileEntityBaseModel
    {
        /// <summary>
        /// Tên Chu kỳ
        /// </summary>
        public string CycleName { get; set; }

        /// <summary>
        /// Thứ tự sắp xếp
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Các mốc thời gian
        /// </summary>
        public string TimeLine { get; set; }

        /// <summary>
        /// Mở nhập liệu trước (phút)
        /// </summary>
        public int OpenInputBefore { get; set; }

        /// <summary>
        /// Kháo nhập liệu sau (phút)
        /// </summary>
        public int BlockInputAfter { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Active { get; set; }
    }
}
