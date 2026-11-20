using Newtonsoft.Json;
using System;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin không thực hiện bảo trì
    /// </summary>
    public class AmStopMaintenanceAddEditModel
    {
        /// <summary>
        /// ID Loại công việc
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Bảo trì tự quản
        /// </summary>
        public int AutonomousMaintenanceID { get; set; }

        /// <summary>
        /// Lý do không thực hiện
        /// </summary>
        public int? ReasonForStopID { get; set; }

        /// <summary>
        /// Mô tả thêm
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Thời gian tạm dừng
        /// </summary>
        public DateTime StopTime { get; set; }

        /// <summary>
        /// Chữ ký người dừng thực hiện
        /// </summary>
        public SignatureModel ImplementerSignature { get; set; }

        /// <summary>
        /// ID Người dừng thực hiện
        /// </summary>
        public int? ImplementerUserID { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
