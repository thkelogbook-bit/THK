using Newtonsoft.Json;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin cơ bản của đối tượng
    /// </summary>
    public class BasicInfoModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Tên
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Mã
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Mã parent
        /// </summary>
        [JsonIgnore]
        public string ParentCode { get; set; }

        /// <summary>
        /// Location ID
        /// </summary>
        [JsonIgnore]
        public int LocationId { get; set; }

        /// <summary>
        /// Loại: Location hoặc Device
        /// </summary>
        public string Type { get; set; }
    }
}
