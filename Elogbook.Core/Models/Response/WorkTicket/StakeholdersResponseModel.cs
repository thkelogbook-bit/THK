namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Các bên liên quan xử lý phiếu công tác
    /// </summary>
    public class StakeholdersResponseModel
    {
        /// <summary>
        /// Thẻ an toàn lao động
        /// </summary>
        public string LaborCard { get; set; }

        /// <summary>
        /// Khóa chính của bảng dữ liệu
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Người đảm nhiệm vai trò
        /// </summary>
        public int? AssignToUserID { get; set; }

        /// <summary>
        /// Họ và Tên
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// User Name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Loại user
        /// </summary>
        public bool IsExternal { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

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

        /// <summary>
        /// Thông tin bậc an toàn điện
        /// </summary>
        public ValueLabelModel ElectricRankInfo { get; set; }

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

        /// <summary>
        /// Thông tin bậc an toàn điện
        /// </summary>
        public ValueStringLabelModel ElectricRankInfoStr { get; set; }

        /// <summary>
        /// Thẻ an toàn điện
        /// </summary>
        public string ElectricRankCard { get; set; }

        /// <summary>
        /// Chức vụ trong ticket
        /// 1/Người giám sát an toàn điện 2/Người chỉ huy trực tiếp 3/Người được đề cử để thực hiện công việc
        /// </summary>
        public int Roles { get; set; }

        /// <summary>
        /// Chức vụ của user
        /// </summary>
        public string PositionName { get; set; }

        /// <summary>
        /// Nhiệm vụ. Chỉ dành cho mục 11 trong phiếu đăng ký.
        /// </summary>
        public string Mission { get; set; }

        private ValueLabelModel GetLevelDescription(int? value, bool isLabor = true)
        {
            var suffix = 5;
            if (isLabor) suffix = 7;

            if (value.HasValue && value.Value > 0)
                return new ValueLabelModel { Value = value, Label = $"{value}/{suffix}" };
            else
                return new ValueLabelModel();
        }

        private int? _laborRankValue;
        /// <summary>
        /// Bậc thợ
        /// </summary>
        public int? LaborRank
        {
            get { return _laborRankValue; }
            set
            {
                _laborRankValue = value;
                LaborRankInfo = GetLevelDescription(value);
            }
        }

        /// <summary>
        /// Thông tin bậc thợ
        /// </summary>
        public ValueLabelModel LaborRankInfo { get; set; }

        private string _laborRankStr;
        /// <summary>
        /// Bậc thợ
        /// </summary>
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
    }
}
