namespace Elogbook.Core.Models.Response.MasterData.AMs
{
    /// <summary>
    /// Danh muc - Loại công việc
    /// </summary>
    public class MobileAmWorkType : MobileEntityBaseModel
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int? FactoryID { get; set; }

        /// <summary>
        /// Trạng thái  1: Có thể sử dụng, 2: Không thể sử dụng
        /// </summary>
        public int? Active { get; set; }

        /// <summary>
        /// Loại công việc
        /// </summary>
        public string TypeDesc { get; set; }
    }
}
