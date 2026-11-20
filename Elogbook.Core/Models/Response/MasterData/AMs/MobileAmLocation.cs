using Elogbook.Core.Enums;

namespace Elogbook.Core.Models.Response.MasterData.AMs
{
    /// <summary>
    /// Danh mục - Địa điểm/Khu vực
    /// </summary>
    public class MobileAmLocation : MobileEntityBaseModel
    {

        /// <summary>
        /// Công ty
        /// </summary>
        public int? OrganizationID { get; set; }

        /// <summary>
        /// Trạng thái  1: Áp dụng, 2: Không áp dụng
        /// </summary>
        public int? Active { get; set; }

        /// <summary>
        /// Parent
        /// </summary>
        public string SiteParent { get; set; }

        /// <summary>
        /// Tên địa điểm
        /// </summary>
        public string SiteDesc { get; set; }

        /// <summary>
        /// Code
        /// </summary>
        public string SiteCode { get; set; }

        /// <summary>
        /// Mã PMIS 
        /// </summary>
        public string PMISId { get; set; }

    }
}
