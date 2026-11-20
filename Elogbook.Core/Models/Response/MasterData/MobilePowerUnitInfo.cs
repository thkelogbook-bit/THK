namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// MobilePowerUnitInfo
    /// </summary>
    public class MobilePowerUnitInfo : MobileEntityBaseModel
    {
        /// <summary>
        /// Tên tổ máy
        /// </summary>
        public string PowerUnitName { get; set; }

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
