using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// model thêm/sửa
    /// </summary>
    public class AddEditAmDeviceCategoryModel
    {
        /// <summary>
        /// Mã mức độ khẩn cấp
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int? OrgID { get; set; }

        /// <summary>
        /// mã số phân loại thiết bị cha
        /// </summary>
        public int? CategoryParent { get; set; }

        /// <summary>
        /// tên
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// mã
        /// </summary>
        public string CategoryCode { get; set; }

        /// <summary>
        /// Trạng thái 1: Áp dụng, 2 Không áp dụng
        /// </summary>
        public int Active { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
