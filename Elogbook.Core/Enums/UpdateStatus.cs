namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Trạng thái cập nhật dữ liệu
    /// </summary>
    public enum UpdateStatus
    {
        /// <summary>
        /// Thành công
        /// </summary>
        Success = 1,
        /// <summary>
        /// Đang sử dụng
        /// </summary>
        InUse,
        /// <summary>
        /// Không tìm thấy dữ liệu
        /// </summary>
        NotFound,
        /// <summary>
        /// Lỗi trong quá trình thực hiện chức năng
        /// </summary>
        Error,
    }
}
