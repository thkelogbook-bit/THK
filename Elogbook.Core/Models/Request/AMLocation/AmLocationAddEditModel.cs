using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Tham số thêm mới/ cập nhật khu vực
    /// </summary>
    public class AmLocationAddEditModel
    {
        /// <summary>
        /// ID Khu vực
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Khu vực cha
        /// </summary>
        public int? SiteParent { get; set; }

        /// <summary>
        /// Công ty
        /// </summary>
        public int OrganizationID { get; set; }

        /// <summary>
        /// Tên Khu vực
        /// </summary>
        public string SiteDesc { get; set; }

        /// <summary>
        /// Mã
        /// </summary>
        public string SiteCode { get; set; }

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
