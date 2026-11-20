using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// AmManufacturerAddEditModel
    /// </summary>
    public class AmManufacturerAddEditModel
    {
        /// <summary>
        /// ID nhà cung cấp
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Tên
        /// </summary>
        public string ManufacturerName { get; set; }

        /// <summary>
        /// Code
        /// </summary>
        public string ManufacturerCode { get; set; }

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
