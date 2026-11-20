using Elogbook.Core.Enums;

namespace Elogbook.Core.Models.Response.MasterData.AMs
{
    /// <summary>
    /// MobileAmEmergency model
    /// </summary>
    public class MobileAmEmergency : MobileEntityBaseModel
    {
        /// <summary>
        /// Tên mức độ khẩn cấp
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int? FactoryID { get; set; }

        /// <summary>
        /// Trạng thái  1: Có thể sử dụng, 2: Không thể sử dụng
        /// </summary>
        public int? Active { get; set; }

    }
}
