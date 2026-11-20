namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Chi tiết công việc
    /// </summary>
    public class AmMaintenanceWorkModel 
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Lịch bảo trì
        /// </summary>
        public ValueLabelCheckedModel MaintenanceSchedule { get; set; }

        /// <summary>
        /// Loại công việc
        /// </summary>
        public ValueLabelCheckedModel WorkType { get; set; }

        /// <summary>
        /// Công việc
        /// </summary>
        public ValueLabelCheckedModel Work { get; set; }

        /// <summary>
        /// Thứ tự
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Số phút
        /// </summary>
        public int? SLA { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        public ValueLabelCheckedModel CreateUser { get; set; }
    }
}
