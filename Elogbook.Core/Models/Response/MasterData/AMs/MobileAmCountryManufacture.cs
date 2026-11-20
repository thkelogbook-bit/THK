namespace Elogbook.Core.Models.Response.MasterData.AMs
{
    /// <summary>
    /// Nước sản xuất
    /// </summary>
    public partial class MobileAmCountryManufacture : MobileEntityBaseModel
    {
        /// <summary>
        /// Nước sản xuất
        /// </summary>
        public string CountryName { get; set; }

        /// <summary>
        /// Nước sản xuất
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// Mã PMIS 
        /// </summary>
        public string PMISId { get; set; }
    }
}
