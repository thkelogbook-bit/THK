using System;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin xử lý bất thường sự cố
    /// </summary>
    public class AbnormalIncidentDetailSyncModel
    {
        /// <summary>
        /// AppID
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// SyncDataKey
        /// </summary>
        public string SyncDataKey { get; set; }

        /// <summary>
        /// Nội dung xử lý
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// File đính kèm
        /// </summary>
        public string Attachments { get; set; }

        /// <summary>
        /// Trạng thái của bất thường sự cố
        /// </summary>
        public int IsComplete { get; set; }

        /// <summary>
        /// Mã bất thường sự cố
        /// </summary>
        public int AbnormalIncidentID { get; set; }

        /// <summary>
        /// Tạo mới
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// Xóa
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        public int? CreatedUserID { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime InsertedAt { get; set; }

        /// <summary>
        /// Người cập nhật
        /// </summary>
        public int? LastUpdatedUserID { get; set; }

        /// <summary>
        /// Ngày cập nhật
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
