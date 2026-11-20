namespace Elogbook.Core.Models.Response.MasterData.AMs
{
    /// <summary>
    /// Nhà cung cấp
    /// </summary>
    public partial class MobileAmSupplier : MobileEntityBaseModel
    {
        /// <summary>
        /// Tên nhà cung cấp
        /// </summary>
        public string SupplierName { get; set; }

        /// <summary>
        /// Code
        /// </summary>
        public string SupplierCode { get; set; }

        /// <summary>
        /// Mã PMIS 
        /// </summary>
        public string PMISId { get; set; }
    }
}
