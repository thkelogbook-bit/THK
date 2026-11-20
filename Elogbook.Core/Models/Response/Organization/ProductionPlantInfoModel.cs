namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Danh sách phân xưởng
    /// </summary>
    public class ProductionPlantInfoModel
    {
        /// <summary>
        /// Key của thuộc tính
        /// </summary>
        public int? Value { get; set; }

        /// <summary>
        /// Mô tả thuộc tính
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Xác định loại phân xưởng Sửa chữa/Vận hành
        /// </summary>
        public bool Isoperate { get; set; }
    }
}
