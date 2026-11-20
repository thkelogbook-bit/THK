
namespace Elogbook.Core.Models.Request.InspectionReport
{
    /// <summary>
    /// Ký xác nhận
    /// </summary>
    public class SigngingModel
    {
        /// <summary>
        /// Mã BBKSHT
        /// </summary>
        public int InspectionReportId { get; set; }

        /// <summary>
        /// Tên đăng nhập
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Chức vụ trong ticket
        /// 1/Đại diện Đơn vị công tác 2/Đại diện (các) Đơn vị QLVH 3/Các bên liên quan
        /// </summary>
        public int Roles { get; set; }

        /// <summary>
        /// Chữ ký
        /// </summary>
        public string Signature { get; set; }

    }
}
