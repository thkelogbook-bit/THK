namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin chi tiết của người nhận lệnh và ra lệnh
    /// </summary>
    public class SequenceOperationUserModel : EntityBaseModel
    {
        /// <summary>
        /// Mã phiếu thao tác được tạo hoặc mã phiếu thao tác được cấp
        /// </summary>
        public int TicketID { get; set; }

        /// <summary>
        /// Mã công việc thực hiện
        /// </summary>
        public int SequenceOperationID { get; set; }

        /// <summary>
        /// UserName
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Họ và tên
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Chức vụ: trưởng ca, trưởng kíp, ...
        /// </summary>
        public string PositionName { get; set; }

        /// <summary>
        /// Vai trò trong phiếu thao tác
        /// 1/Người giám sát, 3/Người thao tác
        /// </summary>
        public int Roles { get; set; }

        /// <summary>
        /// cấp bật ATĐ
        /// </summary>
        public int? ElectricRank { get; set; }

        /// <summary>
        /// Thẻ an toàn điện
        /// </summary>
        public string ElectricRankCard { get; set; }

        /// <summary>
        /// Chữ ký
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// Loại tài khoản
        /// </summary>
        public bool? IsExternal { get; set; }
    }
}
