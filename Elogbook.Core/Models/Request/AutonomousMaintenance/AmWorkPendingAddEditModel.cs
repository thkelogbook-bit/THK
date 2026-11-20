using Newtonsoft.Json;
using System;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin dừng/tạm dừng thực hiện AM
    /// </summary>
    public class AmWorkPendingAddEditModel
    {
        /// <summary>
        /// ID Loại công việc
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Thực hiện AM
        /// </summary>
        public int AutonomousMaintenanceID { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Địa điểm/ Khu vực
        /// </summary>
        public int LocationID { get; set; }

        /// <summary>
        /// Lý do dừng công việc
        /// </summary>
        public int? ReasonForStopID { get; set; }

        /// <summary>
        /// Lý do tạm dừng công việc
        /// </summary>
        public int? ReasonForPendingID { get; set; }

        /// <summary>
        /// Mô tả thêm
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Thời gian tạm dừng
        /// </summary>
        public DateTime PendingTime { get; set; }

        /// <summary>
        /// Chữ ký người dừng thực hiện
        /// </summary>
        public SignatureModel ImplementerSignature { get; set; }

        /// <summary>
        /// ID Người dừng thực hiện
        /// </summary>
        public int? ImplementerUserID { get; set; }

        /// <summary>
        /// UserName Người dừng thực hiện
        /// </summary>
        public string ImplementerUserName { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
