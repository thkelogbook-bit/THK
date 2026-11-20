namespace Elogbook.Core.Constants
{
    /// <summary>
    /// Định nghĩa mà lỗi của các api trong hệ thống
    /// </summary>
    public static class StatusCodeCustom
    {
        /// <summary>
        /// Bất kì lỗi gì xảy ra khi thao tác với AD
        /// </summary>
        public const int ADCommonError = 1;
        /// <summary>
        /// Người dùng không tìm thấy trên AD
        /// </summary>
        public const int ADUserNotFound = 2;
        /// <summary>
        /// Người dùng đã tồn tại trong hệ thống
        /// </summary>
        public const int ADUserInuse = 3;
        /// <summary>
        /// Tài khoản hoặc mật khẩu chưa chính xác. Vui lòng thử lại.
        /// </summary>
        public const int InvalidAccount = 4;
        /// <summary>
        /// Bạn không có quyền truy cập hệ thống. Vui lòng liên hệ Quản trị viên để được hỗ trợ.
        /// </summary>
        public const int NoPermission = 5;
        /// <summary>
        /// Dữ liệu đã tồn tại trong hệ thống
        /// </summary>
        public const int DuplicateData = 6;
        /// <summary>
        /// Dữ liệu đang được sử dụng
        /// </summary>
        public const int ActiveData = 7;
        /// <summary>
        /// Dữ liệu không hợp lệ
        /// </summary>
        public const int InValid = 8;
        
    }
}
