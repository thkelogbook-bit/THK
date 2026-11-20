namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Table lưu dữ liệu Người giám sát, Người thao tác
    /// </summary>
    public class MobileParticipantInfo : MobileEntityBaseModel
    {
        /// <summary>
        /// App ID
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// mã phiếu thao tác được cấp
        /// </summary>
        public int? TicketID { get; set; }

        /// <summary>
        /// Mã phiếu đăng ký thao tác
        /// </summary>
        public int? RegistrationManipulationID { get; set; }

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
        /// Người giám sát = 6, Người thao tác = 8, Người tạo phiếu = 1, Người duyệt phiếu = 2
        /// </summary>
        public int Roles { get; set; }

        /// <summary>
        /// cấp bật ATĐ
        /// </summary>
        public int? ElectricRank { get; set; }

        private string _electricRankStr;
        /// <summary>
        /// Bậc an toàn điện
        /// </summary>
        public string ElectricRankStr
        {
            get
            {
                return _electricRankStr;
            }
            set
            {
                _electricRankStr = value;
                if (!string.IsNullOrWhiteSpace(_electricRankStr))
                {
                    var rank = _electricRankStr.Split('/');
                    if (rank != null && rank.Length > 0)
                    {
                        int.TryParse(rank[0], out int rankData);
                        ElectricRank = rankData > 5 ? 5 : rankData;
                    }
                }
            }
        }

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
