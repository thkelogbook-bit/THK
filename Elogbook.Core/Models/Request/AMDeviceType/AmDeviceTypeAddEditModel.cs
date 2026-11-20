using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Tham số thêm mới/cập nhật thiết bị
    /// </summary>
    public class AmDeviceTypeAddEditModel
    {
        /// <summary>
        /// ID loại thiết bị
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Loại thiết bị
        /// </summary>
        public string TypeDesc { get; set; }

        /// <summary>
        /// Mã Loại thiết bị
        /// </summary>
        public string TypeCode { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int Active { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
