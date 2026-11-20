namespace Elogbook.Core.Models.Response.WorkPermit
{
    /// <summary>
    /// thông tin nhân viên
    /// </summary>
    public class LctStaffInfoModel
    {
        /// <summary>
        /// UserName
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Họ tên
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Ngày sinh
        /// </summary>
        public string DateOfBirth { get; set; }

        /// <summary>
        /// Bậc thợ
        /// </summary>
        public int LaborRank { get; set; }

        private string _laborRankStr;
        /// <summary>
        /// Bậc an toàn điện
        /// </summary>
        public string LaborRankStr
        {
            get
            {
                return _laborRankStr;
            }
            set
            {
                _laborRankStr = value;
                if (!string.IsNullOrWhiteSpace(_laborRankStr))
                {
                    var rank = _laborRankStr.Split('/');
                    if (rank != null && rank.Length > 0)
                    {
                        int.TryParse(rank[0], out int rankData);
                        LaborRank = rankData > 7 ? 7 : rankData;
                    }
                }
            }
        }

        /// <summary>
        /// Thẻ an toàn điện
        /// </summary>
        public string ElectricRankCard { get; set; }

        /// <summary>
        /// Đang rãnh
        /// </summary>
        public bool IsAvailable { get; set; }
        /// <summary>
        /// Là người ngoài hệ thống
        /// </summary>
        public bool IsExternal { get; set; }
    }
}
