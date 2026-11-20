namespace Elogbook.Data
{
    using Elogbook.Core.Models.Response;

    /// <summary>
    /// Danh mục - Trạng thái dùng chung
    /// </summary>
    public class MobileAmStatus : MobileEntityBaseModel
    {
        /// <summary>
        /// Loại thiết bị
        /// </summary>
        public string StatusName { get; set; }
    }
}
