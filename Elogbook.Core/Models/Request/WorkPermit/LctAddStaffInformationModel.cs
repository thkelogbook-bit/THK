namespace Elogbook.Core.Models.Request.WorkPermit
{
    /// <summary>
    /// Thông tin nhân viên Thủy cơ nhiệt hóa
    /// </summary>
    public class LctAddStaffInformationModel
    {
        /// <summary>
        /// Mã định danh nếu cập nhật
        /// </summary>
        public int? ID { get; set; }
        /// <summary>
        /// Tên nhân viên
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Tên đăng nhập
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Là nhân viên bên ngoài
        /// </summary>
        public bool? IsExternal { get; set; }

        /// <summary>
        /// Chức vụ
        /// </summary>
        public string PositionName { get; set; }

        /// <summary>
        /// Nhiệm vụ. Chỉ dành cho mục 11 trong lệnh đăng ký.
        /// </summary>
        public string Mission { get; set; }

        /// <summary>
        /// Thẻ ATĐ
        /// </summary>
        public string ElectricRankCard { get; set; }

        /// <summary>
        /// Thời gian CheckIn
        /// </summary>
        public string CheckInTime { get; set; }

        /// <summary>
        /// Thời gian CheckOut
        /// </summary>
        public string CheckOutTime { get; set; }

        /// <summary>
        /// Ký CheckIn
        /// </summary>
        public SignatureModel CheckInSignature { get; set; }

        /// <summary>
        /// Ký CheckOut
        /// </summary>
        public SignatureModel CheckOutSignature { get; set; }
    }
}