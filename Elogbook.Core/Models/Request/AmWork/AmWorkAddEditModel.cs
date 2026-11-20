using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin công việc
    /// </summary>
    public class AmWorkAddEditModel
    {
        /// <summary>
        /// Mã công việc
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Loại công việc
        /// </summary>
        public int WorkTypeID { get; set; }

        /// <summary>
        /// Tên công việc
        /// </summary>
        public string WorkName { get; set; }

        /// <summary>
        /// Áp dụng
        /// </summary>
        public int Active { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string WorkDesc { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
