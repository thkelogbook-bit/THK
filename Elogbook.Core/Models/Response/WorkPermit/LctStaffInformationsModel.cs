namespace Elogbook.Core.Models.Response.WorkPermit
{
    /// <summary>
    ///  thông tin nhân viên làm việc hằng ngày
    /// </summary>
    public class LctStaffInformationsModel
    {
        /// <summary>
        /// STT
        /// </summary>
        public string No { get; set; }

        /// <summary>
        /// STT
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Tên công nhân
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Chức vụ
        /// </summary>
        public string PositionName { get; set; }

        /// <summary>
        /// Là nhân viên bên ngoài
        /// </summary>
        public bool? IsExternal { get; set; }

        /// <summary>
        /// Tên đăng nhập
        /// </summary>
        public string UserName { get; set; }

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
        /// Ký CheckIn
        /// </summary>
        public int? CheckInSignatureID { get; set; }

        /// <summary>
        /// Ký CheckIn
        /// </summary>
        public string CheckInSignature { get; set; }

        /// <summary>
        /// Thời gian CheckOut
        /// </summary>
        public string CheckOutTime { get; set; }

        /// <summary>
        /// Ký CheckOut
        /// </summary>
        public int? CheckOutSignatureID { get; set; }

        /// <summary>
        /// Ký CheckOut
        /// </summary>
        public string CheckOutSignature { get; set; }
    }
}
