using Newtonsoft.Json;

namespace Elogbook.Core.Models
{
    /// <summary>
    /// Các thuộc tính để thêm mới/cập nhật sự kiện
    /// </summary>
    public class AddEditTicketEventModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Loại phiếu CTĐ, TCNH
        /// </summary>
        public int DataType { get; set; }

        /// <summary>
        /// Loại sự kiện
        /// </summary>
        public int EventTypeID { get; set; }

        /// <summary>
        /// Nội dung sự kiện
        /// </summary>
        public string EventContent { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
