using Elogbook.Core.Constants;
using Newtonsoft.Json;
using System;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin người dùng nội bộ/ngoài hệ thống
    /// </summary>
    public class InternalExternalUserModel
    {
        /// <summary>
        /// Mã người dùng
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Họ và tên
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Ngày sinh
        /// </summary>
        public string DateOfBirth { get; set; }

        private DateTime? _tempDateOfBirth;
        /// <summary>
        /// Ngày sinh
        /// </summary>
        [JsonIgnore]
        public DateTime? TempDateOfBirth
        {
            get { return _tempDateOfBirth; }
            set
            {
                _tempDateOfBirth = value;
                if (_tempDateOfBirth.HasValue)
                    DateOfBirth = _tempDateOfBirth.Value.ToString(CommonConstant.FORMAT_DATE_DDMMYYYY);
            }
        }

        [JsonIgnore]
        private int? _electricRankValue;
        /// <summary>
        /// Bậc an toàn điện
        /// </summary>
        [JsonIgnore]
        public int? ElectricRankValue
        {
            get
            {
                return _electricRankValue;
            }
            set
            {
                ElectricRank = GetLevelDescription(value, false);
                _electricRankValue = value;
            }
        }

        /// <summary>
        /// Bậc an toàn điện
        /// </summary>
        public ValueLabelModel ElectricRank { get; set; }

        [JsonIgnore]
        private string _electricRankValueStr;
        /// <summary>
        /// Bậc an toàn điện
        /// </summary>
        [JsonIgnore]
        public string ElectricRankValueStr
        {
            get
            {
                return _electricRankValueStr;
            }
            set
            {
                _electricRankValueStr = value;

                if (!string.IsNullOrWhiteSpace(_electricRankValueStr))
                {
                    var rank = _electricRankValueStr.Split('/');
                    if (rank != null && rank.Length > 0)
                    {
                        int.TryParse(rank[0], out int rankData);
                        ElectricRankValue = rankData > 5 ? 5 : rankData;
                    }
                }

                if (!string.IsNullOrWhiteSpace(value))
                {
                    ElectricRankStr = new ValueStringLabelModel { Value = _electricRankValueStr, Label = _electricRankValueStr };
                }
                else
                {
                    ElectricRankStr = new ValueStringLabelModel();
                }
            }
        }

        /// <summary>
        /// Bậc an toàn điện
        /// </summary>
        public ValueStringLabelModel ElectricRankStr { get; set; }

        /// <summary>
        /// Thẻ an toàn điện
        /// </summary>
        public string ElectricRankCard { get; set; }

        private int? _laborRankValue;
        /// <summary>
        /// Bậc thợ
        /// </summary>
        public int? LaborRankValue
        {
            get { return _laborRankValue; }
            set
            {
                _laborRankValue = value;
                LaborRank = GetLevelDescription(value);
            }
        }

        /// <summary>
        /// Thông tin bậc thợ
        /// </summary>
        public ValueLabelModel LaborRank { get; set; }

        private string _laborRankValueStr;
        /// <summary>
        /// Bậc thợ
        /// </summary>
        public string LaborRankValueStr
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
                        LaborRankValue = rankData > 7 ? 7 : rankData;
                    }
                }

                if (!string.IsNullOrWhiteSpace(value))
                {
                    LaborRankStr = new ValueStringLabelModel { Value = _laborRankValueStr, Label = _laborRankValueStr };
                }
                else
                {
                    LaborRankStr = new ValueStringLabelModel();
                }
            }
        }

        /// <summary>
        /// Thông tin bậc thợ
        /// </summary>
        public ValueStringLabelModel LaborRankStr { get; set; }

        /// <summary>
        /// Thẻ an toàn lao động
        /// </summary>
        public string LaborCard { get; set; }

        /// <summary>
        /// Thuộc quyền quản lý
        /// </summary>
        public string UserType { get; set; }

        /// <summary>
        /// Thuộc đơn vị công tác
        /// </summary>
        public ValueLabelModel Unit { get; set; }

        /// <summary>
        /// Thuộc phân xưởng
        /// </summary>
        public ValueLabelModel ProductionPlant { get; set; }

        /// <summary>
        /// Phân loại phân xưởng(Vận hành/Sửa chữa)
        /// </summary>
        public ValueLabelModel ProductionPlantType { get; set; }

        /// <summary>
        /// Tên đăng nhập
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Loại người dùng trong hay ngoài hệ thống
        /// </summary>
        public bool IsExternal { get; set; }

        /// <summary>
        /// Chức vụ
        /// </summary>
        public string PositionName { get; set; }

        /// <summary>
        /// Xác định người dùng đang làm việc trong PCT
        /// </summary>
        public bool IsInUse { get; set; }

        /// <summary>
        /// Xác định người dùng được Quyền cho phép công tác điện
        /// </summary>
        public bool IsAllowCTD { get; set; }

        /// <summary>
        /// Xác định người dùng được Quyền cho phép công tác Thuỷ-cơ-nhiệt-hoá
        /// </summary>
        public bool IsAllowCTTCNH { get; set; }

        /// <summary>
        /// Số CMND/CCCD
        /// </summary>
        public string NationalID { get; set; }

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
