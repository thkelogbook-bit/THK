namespace Elogbook.Core.Models.Response.MasterData.AMs
{
    /// <summary>
    /// Danh mục - Loại thiết bị
    /// </summary>
    public partial class MobileAmDeviceType : MobileEntityBaseModel
    {
        /// <summary>
        /// Trạng thái  1: Áp dụng, 2: Không áp dụng
        /// </summary>
        public int? Active { get; set; }

        /// <summary>
        /// Loại thiết bị
        /// </summary>
        public string TypeDesc { get; set; }

        /// <summary>
        /// Code
        /// </summary>
        public string TypeCode { get; set; }

        /// <summary>
        /// Mã PMIS 
        /// </summary>
        public string PMISId { get; set; }

        /// <summary>
        /// PMISType
        /// </summary>
        public string PMISType { get; set; }
    }
}
