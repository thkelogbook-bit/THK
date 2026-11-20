namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Danh mục trạng thái
    /// </summary>
    public class StatusCategoryModel : EntityBaseModel
    {
        /// <summary>
        /// Mã trang thái
        /// </summary>
        public new int ID { get; set; }

        /// <summary>
        /// Tên trang thái
        /// </summary>
        public string StatusName { get; set; }

        /// <summary>
        /// Loại dữ liệu trạng thái
        /// </summary>
        public ValueLabelModel DataType { get; set; }

        /// <summary>
        /// Sử dụng
        /// </summary>
        public int Active { get; set; }
    }
}
