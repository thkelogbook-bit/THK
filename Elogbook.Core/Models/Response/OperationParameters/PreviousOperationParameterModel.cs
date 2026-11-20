namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Các giá trị thông số vận hành yêu thích
    /// </summary>
    public class PreviousOperationParameterModel
    {
        /// <summary>
        /// Giá trị được ghi
        /// </summary>
        public string CurrentValue { get; set; }

        /// <summary>
        /// ID thông số
        /// </summary>
        public int ParameterID { get; set; }

        /// <summary>
        /// Mã lỗi
        /// </summary>
        public int? ErrorCode { get; set; }

        /// <summary>
        /// Khung giờ
        /// </summary>
        public int? OperationTimeLineID { get; set; }
    }
}
