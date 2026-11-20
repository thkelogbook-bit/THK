using Newtonsoft.Json;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Loại phiếu
    /// </summary>
    public class WorkTicketFeaturesModel
    {
        /// <summary>
        /// STT
        /// </summary>
        [JsonIgnore]
        public int ID { get; set; }

        /// <summary>
        /// Tên loại
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// ID loại
        /// </summary>
        public int FeatureID { get; set; }

        /// <summary>
        /// Mã phiếu công tác
        /// </summary>
        [JsonIgnore]
        public int WorkTicketID { get; set; }
    }
}
