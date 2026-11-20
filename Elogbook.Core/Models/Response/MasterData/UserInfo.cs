using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin Nhân viên
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// Mã Nhân viên
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Họ và tên
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Tên đăng nhập
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Số điện thoại
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Ngày sinh
        /// </summary>
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Số CMND/CCCD
        /// </summary>
        public string NationalID { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Công ty
        /// </summary>
        public ValueLabelModel Company { get; set; }

        /// <summary>
        /// Phân xưởng
        /// </summary>
        public ValueLabelModel ProductionPlant { get; set; }

        /// <summary>
        /// Vai trò người dùng
        /// </summary>
        public List<ValueLabelModel> RoleInUsers { get; set; }

        /// <summary>
        /// Trạng thái truy cập
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Loại tài khoản
        /// </summary>
        public int AccountType { get; set; }

        /// <summary>
        /// Flag đánh dấu dữ liệu đã xóa hay chưa, true đã xóa
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Bậc an toàn điện
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
        /// Thẻ an toàn lao động
        /// </summary>
        public string LaborCard { get; set; }

        /// <summary>
        /// Bậc thợ
        /// </summary>
        public int? LaborRank { get; set; }

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
        /// Chữ ký
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// Tên chức vụ
        /// </summary>
        public string PositionName { get; set; }

        /// <summary>
        /// Công ty
        /// </summary>
        public int CompanyID { get; set; }

        /// <summary>
        /// Phân xưởng
        /// </summary>
        public int? ProductionPlantID { get; set; }

        /// <summary>
        /// Tổ
        /// </summary>
        public int? TeamID { get; set; }

        /// <summary>
        /// Đội/nhóm
        /// </summary>
        public int? GroupID { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int? FactoryID { get; set; }

        /// <summary>
        /// Nguồn tài khoản
        /// </summary>
        public int SourceType { get; set; }

        /// <summary>
        /// Loại tài khoản
        /// </summary>
        public bool IsExternal { get; set; }

        /// <summary>
        /// Nguyên nhân lỗi
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Phương thức ký cấp/khóa phiếu công tác
        /// </summary>
        public int SigningMethodID { get; set; }

        /// <summary>
        /// Phương thức ký cấp/khóa phiếu công tác
        /// </summary>
        public ValueLabelModel SigningMethod { get; set; }
    }
}
