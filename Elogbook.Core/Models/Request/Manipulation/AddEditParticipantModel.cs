namespace Elogbook.Core.Models.Request.Manipulation
{
    /// <summary>
    /// Người giám sát, Người thao tác...
    /// </summary>
    public class AddEditParticipantModel
    {
        /// <summary>
        /// Mã phiếu thao tác được tạo hoặc mã phiếu thao tác được cấp
        /// </summary>
        public int TicketID { get; set; }

        /// <summary>
        /// mã số
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Nếu phiếu tạo thì = true, nếu phiếu cấp thì = false
        /// </summary>
        public bool IsCreated { get; set; }

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
        /// Người giám sát = 6, Người thao tác = 8
        /// </summary>
        public int Roles { get; set; }

        /// <summary>
        /// Cấp bật ATĐ
        /// </summary>
        public string ElectricRank { get; set; }

        /// <summary>
        /// Thẻ an toàn điện
        /// </summary>
        public string ElectricRankCard { get; set; }

        /// <summary>
        /// Chữ ký
        /// </summary>
        public int? SignatureID { get; set; }

        /// <summary>
        /// Loại tài khoản
        /// </summary>
        public bool? IsExternal { get; set; }

        /// <summary>
        /// vị trí
        /// </summary>
        public int? Order { get; set; }
    }
}
