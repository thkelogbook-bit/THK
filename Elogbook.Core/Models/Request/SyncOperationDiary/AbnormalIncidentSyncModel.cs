using System;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Tham số của bất thường sự cố
    /// </summary>
    public class AbnormalIncidentSyncModel
    {
        /// <summary>
        /// App ID
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// ID Bất thường sự cố
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Mã nhật ký vận hành
        /// </summary>
        public int OperationDiaryID { get; set; }

        /// <summary>
        /// SyncDataKey - {Ngày}_{Nhà máy}_{Cương vị}_{Ca} (Ex: 07042022_1_1_1)
        /// </summary>
        public string SyncDataKey { get; set; } 

        /// <summary>
        /// 3.Phương thức giao ca
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// File đính kèm
        /// </summary>
        public string Attachments { get; set; }

        /// <summary>
        /// Xóa
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Tạo mới
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        public int? CreatedUserID { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime InsertedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Người cập nhật
        /// </summary>
        public int? LastUpdatedUserID { get; set; }

        /// <summary>
        /// Ngày cập nhật
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
