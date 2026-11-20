namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin module dashboard (dùng để chọn phân loại tài liệu)
    /// </summary>
    public class DashboardModuleModel
    {
        /// <summary>
        /// ID bản ghi
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Tên module
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Giá trị module (key phân loại)
        /// </summary>
        public string ModuleValue { get; set; }

        /// <summary>
        /// Thứ tự hiển thị
        /// </summary>
        public int? Order { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string Description { get; set; }
    }
}
