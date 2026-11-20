using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request.WorkPermit
{
    /// <summary>
    /// Các bên liên quan xử lý lệnh công tác
    /// </summary>
    public class LctStakeholdersModel
    {
        /// <summary>
        /// Khóa chính của bảng dữ liệu. Chỉ cần dùng khi update.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Người đảm nhiệm vai trò
        /// </summary>
        public int? AssignToUserID { get; set; }

        /// <summary>
        /// Họ tên
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// User Name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Chức vụ của user
        /// </summary>
        public string PositionName { get; set; }

        /// <summary>
        /// Thẻ an toàn điện
        /// </summary>
        public string ElectricRankCard { get; set; }

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

        private int? _laborRankValue;
        /// <summary>
        /// Bậc thợ
        /// </summary>
        [JsonIgnore]
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

                if (!string.IsNullOrWhiteSpace(value))
                {
                    ElectricRankInfoStr = new ValueStringLabelModel { Value = _electricRankStr, Label = _electricRankStr };
                }
                else
                {
                    ElectricRankInfoStr = new ValueStringLabelModel();
                }
            }
        }

        private string _laborRankValueStr;
        /// <summary>
        /// Bậc thợ
        /// </summary>
        [JsonIgnore]
        public string LaborRankStr
        {
            get { return _laborRankValueStr; }
            set
            {
                _laborRankValueStr = value;

                if (!string.IsNullOrWhiteSpace(_laborRankValueStr))
                {
                    var rank = _laborRankValueStr.Split('/');
                    if (rank != null && rank.Length > 0)
                    {
                        int.TryParse(rank[0], out int rankData);
                        LaborRank = rankData > 7 ? 7 : rankData;
                    }
                }

                if (!string.IsNullOrWhiteSpace(value))
                {
                    LaborRankInfoStr = new ValueStringLabelModel { Value = _laborRankValueStr, Label = _laborRankValueStr };
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
        /// Loại user
        /// </summary>
        public bool IsExternal { get; set; }

        /// <summary>
        /// Chức vụ trong ticket
        /// 1/Người giám sát an toàn điện 2/Người chỉ huy trực tiếp 3/Người được đề cử để thực hiện công việc 4/ Lãnh đạo 5/ Cảnh giới
        /// </summary>
        public int Roles { get; set; }

        /// <summary>
        /// Nhiệm vụ. Chỉ dành cho mục 11 trong lệnh đăng ký.
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
    }
}
