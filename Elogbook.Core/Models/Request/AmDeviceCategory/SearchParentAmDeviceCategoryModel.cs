namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// define model tìm kiếm parent
    /// </summary>
    public class SearchParentAmDeviceCategoryModel
    {
        /// <summary>
        /// mã phân loại thiết bị
        /// </summary>
        public int? AmDeviceCategoryID { get; set; }

        /// <summary>
        /// công ty
        /// </summary>
        public int? OrgID { get; set; }

        /// <summary>
        /// từ khóa
        /// </summary>
        public string SearchText { get; set; }
    }
}
