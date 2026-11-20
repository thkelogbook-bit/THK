namespace Elogbook.Core.Models.Response.MasterData.AMs
{

    /// <summary>
    /// Danh mục - Nguyên nhân không thực hiện (Dùng cho Không thực hiện Bảo trì tự quản và Dừng 1 thực hiện AM cụ thể )
    /// </summary>
    public class MobileAmReasonForStop : MobileEntityBaseModel
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

        /// <summary>
        /// Loại 1: Không thực hiện bảo trì, 2: Dừng công việc
        /// </summary>
        public int? ReasonType { get; set; }

    }
}
