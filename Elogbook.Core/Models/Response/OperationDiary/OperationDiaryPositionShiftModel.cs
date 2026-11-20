using Newtonsoft.Json;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin khai báo ghi nhật ký vận hành
    /// </summary>
    public class OperationDiaryPositionShiftModel
    {
        /// <summary>
        /// Mã cương vị trong ca
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Giá trị mặc định hiển thị Trưởng ca/Trưởng kíp/Vận hành viên
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Người được phân công
        /// </summary>
        public virtual ValueLabelModel AssignedUser { get; set; }

        /// <summary>
        /// Người dùng được chọn lúc tạo nhật ký vận hành
        /// </summary>
        public InternalExternalUserModel AvailableUser { get; set; }

        /// <summary>
        /// Vị trí hiển thị
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Mã cương vị
        /// </summary>
        [JsonIgnore]
        public int OperationPositionID { get; set; }

        /// <summary>
        /// Mã cương vị được cấu hình ở cương vị trong ca
        /// </summary>
        [JsonIgnore]
        public int PositionShiftID { get; set; }

        /// <summary>
        /// Mã cương vị trong ca
        /// </summary>
        public int PositionInShiftID { get; set; }
    }
}
