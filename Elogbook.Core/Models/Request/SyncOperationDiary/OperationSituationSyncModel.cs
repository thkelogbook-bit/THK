using System;

namespace Elogbook.Core.Models.Request
{
    /// <summary> Tình hình vận hành </summary>
    public class OperationSituationSyncModel
    {
        /// <summary>
        /// App ID
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// Mã Tình hình vận hành
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
        /// Giờ ghi dữ liệu
        /// </summary>
        public string Hour { get; set; }

        /// <summary>
        /// Người ra lệnh
        /// </summary>
        public int? AssignedFromID { get; set; }

        /// <summary>
        /// Người ra lệnh ngoài hệ thống
        /// </summary>
        public string AssignedFromName { get; set; }

        /// <summary>
        /// Người nhận lệnh
        /// </summary>
        public int? AssignedToID { get; set; }

        /// <summary>
        /// Người nhận lệnh
        /// </summary>
        public string AssignedToName { get; set; }

        /// <summary>
        /// Tình hình vận hành và các thao tác trong ca
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Danh sách mã số file đính kèm
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
