namespace Elogbook.Core.Models.Request.RegistrationTicket
{
    /// <summary>
    /// Thông tin đơn vị liên quan đến đăng ký phiếu công tác
    /// </summary>
    public class ManagementUnitModel
    {
        /// <summary>
        /// Khóa chính của bảng dữ liệu
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Mã đơn vị - mối quan hệ liên kết với Đơn vị
        /// </summary>
        public int? UnitID { get; set; }

        /// <summary>
        /// Tên đơn vị/hoặc tên đơn vị ngoài hệ thống
        /// </summary>
        public string OtherUnitName { get; set; }
    }
}
