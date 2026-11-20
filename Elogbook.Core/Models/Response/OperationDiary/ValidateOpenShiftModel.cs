namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tiên hiện thị message thông báo tồn tại ca đang mở lúc tạo ca mới
    /// </summary>
    public class ValidateOpenShiftModel
    {
        /// <summary>
        /// Tên ca
        /// </summary>
        public string OperationShiftName { get; set; }

        /// <summary>
        /// Ngày tạo nhật ký vận hành
        /// </summary>
        public string CreatedDate { get; set; }
    }
}
