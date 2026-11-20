using Newtonsoft.Json;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response.Manipulation
{
    /// <summary>
    /// thông tin hủy phiếu
    /// </summary>
    public class CancelTicketInfoModel : EntityBaseModel
    {
        /// <summary>
        /// App ID
        /// </summary>
        public string AppID { get; set; }
        /// <summary>
        /// Lý do hủy phiếu
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Danh sách hình ảnh/videos hủy phiếu
        /// </summary>
        public List<AttachmentModel> ReferAttachments { get; set; }

        /// <summary>
        /// Danh sách hình ảnh/videos hủy phiếu
        /// </summary>
        [JsonIgnore]
        public string ReferAttachmentIDs { get; set; }

        /// <summary>
        /// Thời gian hủy phiếu
        /// </summary>
        public string CancelTime { get; set; }
    }
}
