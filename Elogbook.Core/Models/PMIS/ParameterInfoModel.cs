namespace Elogbook.Core.Models.PMIS
{
    /// <summary>
    /// Thông tin thông số PMIS
    /// </summary>
    public class ParameterInfoModel
    {
        /// <summary>
        /// Mã QLKT
        /// </summary>
        public string ADMETERID { get; set; }

        /// <summary>
        /// Thông số
        /// </summary>
        public string METERDESC { get; set; }

        /// <summary>
        /// Đơn vị
        /// </summary>
        public string UOMID { get; set; }

        /// <summary>
        /// Order
        /// </summary>
        public int? ROWORD { get; set; }
    }
}
