using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số để thay đổi trạng thái tổ chức
    /// </summary>
    public class ChangeOrganizationStatusModel
    {
        /// <summary>
        /// Mã tổ chức
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Trạng thái hoạt động
        /// </summary>
        public int Active { get; set; }

        /// <summary>
        /// Cập nhật trạng thái cho level con
        /// </summary>
        public bool IsUpdateChildLevel { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
