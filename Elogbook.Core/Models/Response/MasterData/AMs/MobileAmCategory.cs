namespace Elogbook.Core.Models.Response.MasterData.AMs
{
    /// <summary>
    /// Phân loại
    /// </summary>    
    public class MobileAmCategory : MobileEntityBaseModel
    {
        /// <summary>
        /// Công ty
        /// </summary>
        public int? OrgID { get; set; }

        /// <summary>
        /// CategoryParent
        /// </summary>
        public int? CategoryParent { get; set; }

        /// <summary>
        /// Phân loại
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// Phân loại cha
        /// </summary>
        public string CategoryCode { get; set; }

        /// <summary>
        /// Mã PMIS 
        /// </summary>
        public string PMISId { get; set; }
    }
}
