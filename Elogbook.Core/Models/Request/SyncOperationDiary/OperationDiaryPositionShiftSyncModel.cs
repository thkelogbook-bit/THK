using System;

namespace Elogbook.Core.Models.Request
{
    /// <summary> Cương vị trong ca </summary>
    public class OperationDiaryPositionShiftSyncModel
    {
        /// <summary>
        /// App ID
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// Mã Cương vị trong ca
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
        /// Mã cương vị trong ca
        /// </summary>
        public int? PositionInShiftID { get; set; }

        /// <summary>
        /// Giá trị mặc định hiển thị Trưởng ca/Trưởng kíp/Vận hành viên
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Người được phân công
        /// </summary>
        public int AssignedUserID { get; set; }

        /// <summary>
        /// Giá trị mặc định để phân biệt danh sách từ cương vị trong ca hoặc mặc định Trưởng ca/Trưởng kíp/Vận hành viên
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Vị trí hiển thị
        /// </summary>
        public int Position { get; set; }

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
