namespace Elogbook.Core.Identity
{
    /// <summary>
    /// Thông tin người dùng
    /// </summary>
    public class CustomIdentity
    {
        /// <summary>
        /// Mã người dùng
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Tài khoản người dùng
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Họ và tên
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Công ty
        /// </summary>
        public int CompanyID { get; set; }

        /// <summary>
        /// Phân xưởng
        /// </summary>
        public int ProductionPlantID { get; set; }

        /// <summary>
        /// api cho mobile
        /// </summary>
        public bool IsMobileSystem { get; set; }
    }
}
