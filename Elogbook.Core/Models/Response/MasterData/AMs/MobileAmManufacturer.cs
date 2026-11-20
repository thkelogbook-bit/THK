namespace Elogbook.Core.Models.Response.MasterData.AMs
{
    /// <summary>
    /// Nhà chế tạo
    /// </summary>  
    public class MobileAmManufacturer : MobileEntityBaseModel
    {
        /// <summary>
        /// Tên nhà chế tạo
        /// </summary>
        public string ManufacturerName { get; set; }

        /// <summary>
        /// Code
        /// </summary>
        public string ManufacturerCode { get; set; }

        /// <summary>
        /// Mã PMIS 
        /// </summary>
        public string PMISId { get; set; }
    }
}
