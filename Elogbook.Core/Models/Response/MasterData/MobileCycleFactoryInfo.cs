namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Danh sách Nhà máy sử dụng Cương vị
    /// </summary>
    public class MobileCycleFactoryInfo : MobileEntityBaseModel
    {
        /// <summary>
        /// Chu kỳ
        /// </summary>
        public int CycleID { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }
    }
}
