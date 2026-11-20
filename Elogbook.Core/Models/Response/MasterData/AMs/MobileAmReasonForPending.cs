namespace Elogbook.Core.Models.Response.MasterData.AMs
{
    /// <summary>
    /// Danh mục - Nguyên nhân tạm dừng thực hiện
    /// </summary>
    public class MobileAmReasonForPending : MobileEntityBaseModel
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
        /// Nguyên nhân
        /// </summary>
        public string ReasonDesc { get; set; }
    }
}
