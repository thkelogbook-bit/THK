namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Model thay đổi parent của Device hoặc Location
    /// </summary>
    public class ChangeParentLocationModel
    {
        /// <summary>
        /// Mã đối tượng
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Mã parent mới
        /// </summary>
        public string ParentCode { get; set; }

        /// <summary>
        /// Loại: Location hoặc Device
        /// </summary>
        public string Type { get; set; }
    }
}
