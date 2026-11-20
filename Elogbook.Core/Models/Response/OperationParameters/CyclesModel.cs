namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Chu kỳ thông số vận hành
    /// </summary>
    public class CyclesModel
    {
        /// <summary>
        /// Chu kỳ id
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Tên chu kỳ
        /// </summary>
        public string CycleName { get; set; }

        /// <summary>
        /// Mở nhập liệu trước khi khung giờ bắt đầu (phút)
        /// </summary>
        public int OpenInputBeforeStartTime { get; set; }

        /// <summary>
        /// Khóa nhập liệu trước khi khung giờ kết thúc (phút)
        /// </summary>
        public int LockInputBeforeEndTime { get; set; }

        /// <summary>
        /// Số lượng thông số
        /// </summary>
        public int TotalParameters { get; set; }

        /// <summary>
        /// Cho phép thêm mới dữ liệu: true thì hiển thị icon add, flase thì ẩn button add
        /// </summary>
        public bool IsAdd { get; set; }
    }
}
