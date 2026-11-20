using Elogbook.Core.Constants;
using Elogbook.Core.Models.BaseFilter;
using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Tiêu chí get Thông báo
    /// </summary>
    public class AlertNotificationSearchModel : BaseListFilter
    {
        /// <summary>
        /// Title or Content
        /// </summary>
        public string SearchText { get; set; }

        /// <summary>
        /// Đánh dấu
        /// </summary>
        public bool IsImportant { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public int Status { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
