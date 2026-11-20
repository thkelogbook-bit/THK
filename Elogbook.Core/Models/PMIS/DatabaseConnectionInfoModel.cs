namespace Elogbook.Core.Models.PMIS
{
    /// <summary>
    /// Thông tin kết nối database PMIS QLKT_OP
    /// </summary>
    public class DatabaseConnectionInfoModel
    {
        /// <summary>
        /// Database server
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// Database name
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// User login database
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Password User login
        /// </summary>
        public string Password { get; set; }
    }
}
