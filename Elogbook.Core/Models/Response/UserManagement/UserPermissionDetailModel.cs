namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Quyền Nhân viên
    /// </summary>
    public class UserPermissionDetailModel
    {
        /// <summary>
        /// Mã định danh Quyền
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Tên quyền
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// True: tương ứng node Admin Portal/Mobile App và node cha cho từng màn hình. False: thể hiện quyền chi tiết
        /// </summary>
        public bool IsParrent { get; set; }

        /// <summary>
        /// Quyền được chọn cho vai trò
        /// </summary>
        public bool IsSelected { get; set; }
    }
}
