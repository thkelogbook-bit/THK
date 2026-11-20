using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin ký hoàn thành bảo trì tự quản
    /// </summary>
    public class AmSignatureCompleteModel
    {
        /// <summary>
        /// Mã bảo trì tự quản
        /// </summary>
        public int AutonomousMaintenanceID { get; set; }

        /// <summary>
        /// File hình ảnh trước khi thực hiện AM
        /// </summary>
        public string BeforeImages { get; set; }

        /// <summary>
        /// File hình ảnh sau khi thực hiện AM
        /// </summary>
        public string AfterImages { get; set; }

        /// <summary>
        /// UserID
        /// </summary>
        public int? CompleterUserID { get; set; }

        /// <summary>
        /// Chữ ký
        /// </summary>
        public SignatureModel CompleterSignature { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
