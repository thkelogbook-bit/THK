using Newtonsoft.Json;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request.WorkPermit
{
    /// <summary>
    /// Add thông tin tổ chức thực hiện mobile
    /// </summary>
    public class LctAddExecutingOrgMobileModel
    {
        /// <summary>
        /// Danh sách hình ảnh công tác
        /// </summary>
        public List<string> WorkPictures { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }

        /// <summary>
        /// Id ticket hiện tại
        /// </summary>
        public int WorkTicketID { get; set; }

        /// <summary>
        /// Chữ ký người ra lệnh tổ chức thực hiện
        /// </summary>
        public LctLeadershipUserSigningModel ExecutingOrgApprover { get; set; }
    }
}
