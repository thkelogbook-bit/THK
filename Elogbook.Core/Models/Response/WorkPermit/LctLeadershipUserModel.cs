using Newtonsoft.Json;

namespace Elogbook.Core.Models.Response.WorkPermit
{
    /// <summary>
    /// Thông tin người tham gia công tác
    /// </summary>
    public class LctLeadershipUserModel : EntityBaseModel
    {
        #region Old version

        /// <summary>
        /// Thông tin bậc an toàn điện
        /// </summary>
        public ValueLabelModel ElectricRankInfo { get; set; }

        private int? _electricRank;
        /// <summary>
        /// Bậc an toàn điện
        /// </summary>
        public int? ElectricRank
        {
            get
            {
                return _electricRank;
            }
            set
            {
                _electricRank = value;
                ElectricRankInfo = GetLevelDescription(value, false);
            }
        }
        private ValueLabelModel GetLevelDescription(int? value, bool isLabor = true)
        {
            var suffix = 5;
            if (isLabor) suffix = 7;

            if (value.HasValue && value.Value > 0)
                return new ValueLabelModel { Value = value, Label = $"{value}/{suffix}" };
            else
                return new ValueLabelModel();
        }
        private int? _laborRank;
        /// <summary>
        /// Bậc thợ
        /// </summary>
        [JsonIgnore]
        public int? LaborRank
        {
            get { return _laborRank; }
            set
            {
                _laborRank = value;
                LaborRankInfo = GetLevelDescription(value);
            }
        }

        /// <summary>
        /// Thông tin bậc thợ
        /// </summary>
        public ValueLabelModel LaborRankInfo { get; set; }

        #endregion

        /// <summary>
        /// Thông tin bậc an toàn điện
        /// </summary>
        public ValueStringLabelModel ElectricRankInfoStr { get; set; }

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

                if (!string.IsNullOrWhiteSpace(_electricRankStr))
                {
                    ElectricRankInfoStr = new ValueStringLabelModel { Value = _electricRankStr, Label = _electricRankStr };
                }
                else
                {
                    ElectricRankInfoStr = new ValueStringLabelModel();
                }
            }
        }

        private string _laborRankStr;
        /// <summary>
        /// Bậc thợ
        /// </summary>
        [JsonIgnore]
        public string LaborRankStr
        {
            get { return _laborRankStr; }
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

                if (!string.IsNullOrWhiteSpace(_laborRankStr))
                {
                    LaborRankInfoStr = new ValueStringLabelModel { Value = _laborRankStr, Label = _laborRankStr };
                }
                else
                {
                    LaborRankInfoStr = new ValueStringLabelModel();
                }
            }
        }

        /// <summary>
        /// Thông tin bậc thợ
        /// </summary>
        public ValueStringLabelModel LaborRankInfoStr { get; set; }

        /// <summary>
        /// Thẻ an toàn lao động
        /// </summary>
        public string LaborCard { get; set; }

        /// <summary>
        /// lệnh công tác
        /// </summary>
        public int WorkTicketID { get; set; }

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
        public bool IsExternal { get; set; }

        /// <summary>
        /// Chức vụ: trưởng ca, trưởng kíp, ...
        /// </summary>
        public string PositionName { get; set; }

        /// <summary>
        /// Đơn vị
        /// </summary>
        public string Unit { get; set; }
    }
}
