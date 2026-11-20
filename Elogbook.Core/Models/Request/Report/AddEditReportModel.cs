
using Newtonsoft.Json;

namespace Elogbook.Core.Models.Response.Report
{
    /// <summary>
    /// 
    /// </summary>
    public class AddEditReportModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Module type
        /// </summary>
        public string ModuleType { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
