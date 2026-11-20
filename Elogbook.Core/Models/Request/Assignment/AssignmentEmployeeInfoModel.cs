namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin nhân sự
    /// </summary>
    public class AssignmentEmployeeInfoModel
    {
        /// <summary>
        /// Mã vị trí nhân sự
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Mã nhân sự
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// Tên tài khoản
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Họ và tên nhân sự
        /// </summary>
        public string FullName { get; set; }
    }
}
