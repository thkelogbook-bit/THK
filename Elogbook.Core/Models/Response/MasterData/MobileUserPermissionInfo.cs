namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Định nghĩa các quyền trong hệ thống
    /// </summary>
    public class MobileUserPermissionInfo : MobileEntityBaseModel
    {
        /// <summary>
        /// Định danh quyền chức năng
        /// </summary>
        public string PermissionKey { get; set; }

        /// <summary>
        /// Tên quyền chức năng
        /// </summary>
        public string PermissionName { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Active { get; set; }

        /// <summary>
        /// Mã module
        /// </summary>
        public string ModuleValue { get; set; }

        /// <summary>
        /// Module thuộc Admin Portal hay Mobile App
        /// </summary>
        public string SystemType { get; set; }
    }
}
