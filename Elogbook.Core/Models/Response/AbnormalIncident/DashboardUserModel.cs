namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin người dùng
    /// </summary>
    public class DashboardUserModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// FullName
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// UserName
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public ValueLabelCheckedModel Factory { get; set; }
    }
}
