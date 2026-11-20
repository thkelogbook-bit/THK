using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// AmSupplierAddEditModel
    /// </summary>
    public class AmSupplierAddEditModel
    {
        /// <summary>
        /// ID nhà cung cấp
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Tên nhà cung cấp
        /// </summary>
        public string SupplierName { get; set; }

        /// <summary>
        /// Code
        /// </summary>
        public string SupplierCode { get; set; }

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
