using System;

namespace Elogbook.Core.Models.Request
{
    /// <summary>Danh sách dữ liệu  Tồn tại trong ca  </summary>
    public class ExistInShiftSyncModel
    {
        /// <summary>
        /// App ID
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// Mã tồn tại trong ca
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// SyncDataKey - {Ngày}_{Nhà máy}_{Cương vị}_{Ca} (07042022_1_1_1)
        /// </summary>
        public string SyncDataKey { get; set; } 

        /// <summary>
        /// Mã nhật ký vận hành
        /// </summary>
        public int OperationDiaryID { get; set; }

        /// <summary>
        /// Phiếu công tác
        /// </summary>
        public int? RegistrationTicketID { get; set; }

        /// <summary>
        /// Tên phiếu công tác
        /// </summary>
        public string RegistrationTicketName { get; set; }

        /// <summary>
        /// Nhóm trưởng
        /// </summary>
        public string TeamLeader { get; set; }

        /// <summary>
        /// Khu vực
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// Nội dung tồn tại
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Xoá tồn tại trong ca
        /// </summary>
        public bool IsDeleteExistInShift { get; set; }

        /// <summary>
        /// File đính kèm
        /// </summary>
        public string Attachments { get; set; }

        /// <summary>
        /// Trạng thái của tồn tại trong ca
        /// </summary>
        public int StatusID { get; set; }

        /// <summary>
        /// Xóa dữ liệu khi theo dõi tồn tại trong ca
        /// </summary>
        public bool IsDeleteIssueOfShift { get; set; }

        /// <summary>
        /// Ngày hoàn thành
        /// </summary>
        public DateTime CompletedDateTime { get; set; }

        /// <summary>
        /// Xoá
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
