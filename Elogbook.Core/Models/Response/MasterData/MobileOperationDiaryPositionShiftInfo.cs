namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin Cương vị trong ca của Nhật ký vận hành
    /// </summary>
    public class MobileOperationDiaryPositionShiftInfo : MobileEntityBaseModel
    {
        /// <summary>
        /// SyncDataKey
        /// </summary>
        public string SyncDataKey { get; set; }

        /// <summary>
        /// App ID
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// Mã nhật ký vận hành
        /// </summary>
        public int OperationDiaryID { get; set; }

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
    }
}
