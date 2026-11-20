namespace Elogbook.Core.Models.Response.WorkPermit
{
    /// <summary>
    /// Thông tin người tham gia thao tác
    /// </summary>
    public class LctUserInTicketModel : EntityBaseModel
    {
        /// <summary>
        /// Lệnh thao tác
        /// </summary>
        public int TicketID { get; set; }

        /// <summary>
        /// Họ và tên
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// UserName
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Vai trò của người tham gia trong ticket
        /// </summary>
        public int? Roles { get; set; }

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
        /// Chữ ký
        /// </summary>
        public string SignatureContent { get; set; }

        /// <summary>
        /// Loại tài khoản
        /// </summary>
        public bool IsExternal { get; set; }

        /// <summary>
        /// Chức vụ: trưởng ca, trưởng kíp, ...
        /// </summary>
        public string PositionName { get; set; }
    }
}
