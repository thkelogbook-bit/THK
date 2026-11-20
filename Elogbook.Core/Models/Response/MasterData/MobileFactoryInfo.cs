namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin Nhà máy
    /// </summary>
    public class MobileFactoryInfo : MobileEntityBaseModel
    {
        /// <summary>
        /// Tên nhà máy
        /// </summary>
        public string FactoryName { get; set; }

        /// <summary>
        /// Công ty
        /// </summary>
        public int CompanyID { get; set; }

        /// <summary>
        /// Phân xưởng
        /// </summary>
        public int ProductionPlantID { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Cấu hình sử dụng nhóm thông số trong thông số vận hành
        /// </summary>
        public bool IsUseOperationParameterGroup { get; set; }

        /// <summary>
        /// Cấu hình filter dữ liệu theo nhà máy
        /// </summary>
        public bool IsFilterFactory { get; set; }
    }
}
