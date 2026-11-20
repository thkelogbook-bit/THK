using Elogbook.Core.Constants;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response.User
{
    /// <summary>
    /// Thông tin người dùng
    /// </summary>
    public class UserModel : EntityBaseModel
    {
        /// <summary>
        /// Họ và tên
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Tên đăng nhập
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        [JsonIgnore]
        public string Password { get; set; }

        /// <summary>
        /// Chức vụ
        /// </summary>
        public string PositionName { get; set; }

        /// <summary>
        /// Ngày sinh
        /// </summary>
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Số điện thoại
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Số CMND/CCCD
        /// </summary>
        public string NationalID { get; set; }

        /// <summary>
        /// Hình ảnh số CMND/CCCD
        /// </summary>
        [JsonIgnore]
        public int NationalAttachmentID { get; set; }

        /// <summary>
        /// Hình ảnh số CMND/CCCD
        /// </summary>
        public AttachmentModel NationalAttachment { get; set; }

        /// <summary>
        /// Bậc an toàn điện
        /// </summary>
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

        /// <summary>
        /// Thẻ an toàn điện
        /// </summary>
        public string ElectricRankCard { get; set; }

        /// <summary>
        /// Hình ảnh thẻ an toàn điện
        /// </summary>
        [JsonIgnore]
        public int ElectricRankAttachmentID { get; set; }

        /// <summary>
        /// Hình ảnh thẻ an toàn điện
        /// </summary>
        public AttachmentModel ElectricRankAttachment { get; set; }

        /// <summary>
        /// Thẻ an toàn lao động
        /// </summary>
        public string LaborCard { get; set; }

        /// <summary>
        /// Bậc thợ
        /// </summary>
        private int? _laborRank;
        /// <summary>
        /// Bậc thợ
        /// </summary>
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

        /// <summary>
        /// Hình ảnh thẻ an toàn lao động
        /// </summary>
        [JsonIgnore]
        public int LaborCardAttachmentID { get; set; }

        /// <summary>
        /// Hình ảnh thẻ an toàn lao động
        /// </summary>
        public AttachmentModel LaborCardAttachment { get; set; }

        /// <summary>
        /// Chữ ký
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// Công ty
        /// </summary>
        public ValueLabelModel Company { get; set; }

        /// <summary>
        /// Phân xưởng
        /// </summary>
        public ValueLabelModel ProductionPlant { get; set; }

        /// <summary>
        /// Tổ
        /// </summary>
        public ValueLabelModel Team { get; set; }

        /// <summary>
        /// Đội/nhóm
        /// </summary>
        public ValueLabelModel Group { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public ValueLabelModel Factory { get; set; }

        private int _accountTypeValue;
        /// <summary>
        /// Loại tài khoản
        /// </summary>
        [JsonIgnore]
        public int AccountTypeValue
        {
            get
            {
                return _accountTypeValue;
            }
            set
            {
                _accountTypeValue = value;
                switch (value)
                {
                    case 1:
                        AccountType = new ValueLabelModel { Value = value, Label = CommonConstant.TWO_SYSTEM };
                        break;
                    case 2:
                        AccountType = new ValueLabelModel { Value = value, Label = CommonConstant.ADMIN_PORTAL };
                        break;
                    case 3:
                        AccountType = new ValueLabelModel { Value = value, Label = CommonConstant.MOBILE_APP };
                        break;
                    default:
                        AccountType = null;
                        break;
                }
            }
        }

        /// <summary>
        /// Thông tin loại tài khoản
        /// </summary>
        public ValueLabelModel AccountType { get; set; }

        /// <summary>
        /// Nguồn (AD hoặc thêm mới từ hệ thống)
        /// </summary>
        public ValueLabelModel SourceType { get; set; }

        /// <summary>
        /// Danh sách vai trò người dùng
        /// </summary>
        public List<ValueLabelModel> RoleInUsers { get; set; }

        /// <summary>
        /// Truy cập
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Nếu tồn tại vai trò đang hoạt động thì trả về true/ngược lại thì false
        /// </summary>
        [JsonIgnore]
        public bool IsExistActiveRole { get; set; }

        private ValueLabelModel GetLevelDescription(int? value, bool isLabor = true)
        {
            var suffix = 5;
            if (isLabor) suffix = 7;

            if (value.HasValue && value.Value > 0)
                return new ValueLabelModel { Value = value, Label = $"{value}/{suffix}" };
            else
                return new ValueLabelModel();
        }

        /// <summary>
        /// Ảnh đại diện
        /// </summary>
        public int? AvatarAttachmentID { get; set; }

        /// <summary>
        /// Đường dẫn ảnh đại diện
        /// </summary>
        public string AvatarImage { get; set; }

        /// <summary>
        /// Thông tin chi tiết ảnh đại diện
        /// </summary>
        public AttachmentModel AvatarAttachment { get; set; }

        /// <summary>
        /// Phương thức ký cấp/khóa phiếu công tác
        /// </summary>
        public int SigningMethodID { get; set; }

        /// <summary>
        /// Phương thức ký cấp/khóa phiếu công tác
        /// </summary>
        public ValueLabelModel SigningMethod { get; set; }

        /// <summary>
        /// Yêu cầu ký cấp/khóa phiểu công tác bằng mobile
        /// </summary>
        public bool IsRequiredSigningMobile { get; set; }

        /// <summary>
        /// Số lần đăng nhập sai mật khẩu
        /// </summary>
        public int NumberLoginFailed { get; set; }

        /// <summary>
        /// Trạng thái tài khoản (True: hết hạn, False: còn hạn)
        /// </summary>
        public bool? IsExpired { get; set; }
    }
}
