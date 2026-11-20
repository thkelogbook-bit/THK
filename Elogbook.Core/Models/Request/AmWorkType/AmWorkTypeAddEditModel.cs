using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin loại công việc
    /// </summary>
    public class AmWorkTypeAddEditModel
    {
        /// <summary>
        /// ID Loại công việc
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// ID nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Tên loại công việc
        /// </summary>
        public string TypeDesc { get; set; }

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
