using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// AmCountryManufactureAddEditModel
    /// </summary>
    public class AmCountryManufactureAddEditModel
    {
        /// <summary>
        /// ID nhà cung cấp
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Tên
        /// </summary>
        public string CountryName { get; set; }

        /// <summary>
        /// Code
        /// </summary>
        public string CountryCode { get; set; }

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
