using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số để thêm mới một Tổ máy
    /// </summary>
    public class AddEditPowerUnitModel
    {
        /// <summary>
        /// Mã Tổ máy
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Tên Tổ máy
        /// </summary>
        public string PowerUnitName { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

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
