namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin kíp vận hành
    /// </summary>
    public class MobileOperationCrewInfo : MobileEntityBaseModel
    {
        /// <summary>
        /// Tên kíp vận hành
        /// </summary>
        public string OperationCrewName { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int? FactoryID { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Active { get; set; }
    }
}
