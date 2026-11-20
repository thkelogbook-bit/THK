using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Quyền Nhân viên
    /// </summary>
    public class UserPermissionModel
    {
        /// <summary>
        /// Node Admin Portal/Mobile App và node cha cho từng màn hình
        /// </summary>
        public UserPermissionDetailModel Data { get; set; }

        /// <summary>
        /// Quyền của từng màn hình
        /// </summary>
        public List<UserPermissionModel> Children { get; set; }

        /// <summary>
        /// Lỗi trả về khi gọi chức năng lấy thông tin quyền mà người gửi request không phải làm người đang đăng nhập
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
