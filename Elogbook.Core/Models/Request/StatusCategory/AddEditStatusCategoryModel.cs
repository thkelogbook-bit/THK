using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số thêm mới/cập nhật trạng thái
    /// </summary>
    public class AddEditStatusCategoryModel
    {
        /// <summary>
        /// Mã trạng thái
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Tên trạng thái
        /// </summary>
        public string StatusName { get; set; }

        /// <summary>
        /// Loại dữ liệu trạng thái
        /// </summary>
        public int DataTypeID { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Active { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
