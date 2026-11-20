using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số để cập nhật 1 thông số vận hành
    /// </summary>
    public class EditOperationParameterModel
    {
        /// <summary>
        /// Id thông số vận hành
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Khung giờ
        /// </summary>
        public int OperationTimeLineID { get; set; }

        /// <summary>
        /// Giá trị được ghi
        /// </summary>
        public string CurrentValue { get; set; }

        /// <summary>
        /// Mã lỗi
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
