using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin nhân viên tập sự
    /// </summary>
    public class AddEditProbationaryStaffModel
    {
        /// <summary>
        /// Mã thông tin nhân viên tập sự
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public int OperationPositionID { get; set; }

        /// <summary>
        /// Mã nhân viên tập sự
        /// </summary>
        public int? AssignedUserID { get; set; }

        /// <summary>
        /// Họ và Tên nhân viên tập sự
        /// </summary>
        public string AssignedUserName { get; set; }

        /// <summary>
        /// Chức vụ
        /// </summary>
        public string PositionName { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
