using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Danh sách công việc
    /// </summary>
    public class AmMaintenanceWorkAddEditModel
    {
        /// <summary>
        /// Mã danh sách công việc
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Mã lịch bảo trì
        /// </summary>
        public int AmMaintenanceScheduleID { get; set; }

        /// <summary>
        /// Loại công việc
        /// </summary>
        public int WorkTypeID { get; set; }

        /// <summary>
        /// Công việc
        /// </summary>
        public int WorkID { get; set; }

        /// <summary>
        /// Thứ tự
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Số phút
        /// </summary>
        public int? SLA { get; set; }

        /// <summary>
        /// Action (1: Thêm mới, 2: Cập nhật, 3: Xóa)
        /// </summary>
        public int Action { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
