using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Tham số thêm mới/ cập nhật
    /// </summary>
    public class AmReasonForPendingAddEditModel
    {
        /// <summary>
        /// ID Địa điểm
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Nguyên nhân
        /// </summary>
        public string ReasonDesc { get; set; }

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
