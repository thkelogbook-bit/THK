namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin các cương vị trong ca
    /// </summary>
    public class AddOperationDiaryPositionShiftModel
    {
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
        public int? AssignedUserID { get; set; }

        /// <summary>
        /// Nếu không tồn tại cương vị trong ca thì mặc định true/ngược lại thì false
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Thứ tự sắp xếp, lấy từ cương vị trong ca nếu tồn tại, ngược lại lấy mặc định(1: Trưởng ca, 2: Trưởng kíp, 3: Vận hành viên)
        /// </summary>
        public int Position { get; set; }
    }
}
