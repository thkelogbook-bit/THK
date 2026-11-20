using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số để thêm mới một Nhóm thông số
    /// </summary>
    public class AddEditParameterGroupModel
    {
        /// <summary>
        /// ID Nhóm thông số
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Tên Nhóm thông số
        /// </summary>
        public string ParameterGroupName { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public int OperationPositionID { get; set; }

        /// <summary>
        /// Chu kỳ cần ghi thông số
        /// </summary>
        public int CycleID { get; set; }

        /// <summary>
        /// Thứ tự sắp xếp
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
