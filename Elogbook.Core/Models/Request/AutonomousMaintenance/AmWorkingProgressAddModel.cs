using Newtonsoft.Json;
using System;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin tiến hành công việc
    /// </summary>
    public class AmWorkingProgressAddModel
    {
        /// <summary>
        /// Loại hành động (3: Bắt đầu , 4: Tạm dừng, 5: Dừng thực hiện, 6: Hoàn thành, 7 Tiếp tục)
        /// </summary>
        public int ActionType { get; set; }

        /// <summary>
        /// Mã bảo trì tự quản
        /// </summary>
        public int AutonomousMaintenanceID { get; set; }

        /// <summary>
        /// Mã Thực hiện AM
        /// </summary>
        public int WorkDetailID { get; set; }

        /// <summary>
        /// Người thực hiện
        /// </summary>
        public int? ImplementerID { get; set; }

        /// <summary>
        /// Lý do tạm dừng/ Dừng công việc
        /// </summary>
        public int? ReasonID { get; set; }

        /// <summary>
        /// ReasonType : 1:Pending, 2:Stop
        /// </summary>
        public int? ReasonType { get; set; }

        /// <summary>
        /// Nội dung
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Thời gian tạm dừng or dừng
        /// </summary>
        public DateTime? PendingDate { get; set; }

        /// <summary>
        /// Chữ ký người dừng thực hiện
        /// </summary>
        [JsonIgnore]
        public SignatureModel ImplementerSignature { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
