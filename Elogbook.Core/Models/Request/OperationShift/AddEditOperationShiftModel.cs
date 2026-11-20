using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request.OperationShift
{
    /// <summary>
    /// Ca vận hành
    /// </summary>
    public class AddEditOperationShiftModel
    {
        /// <summary>
        /// Mã ca vận hành
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Tên ca vận hành
        /// </summary>
        public string OperationShiftName { get; set; }

        /// <summary>
        /// Thứ tự sắp xếp
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Mã nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Thời gian bắt đầu ca
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// Thời gian kết thúc ca
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Active { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
