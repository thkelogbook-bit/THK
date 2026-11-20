using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Danh sách dữ liệu yêu cầu thêm mới/cập nhật người dùng
    /// </summary>
    public class AddEditUserModel
    {
        /// <summary>
        /// Mã người dùng
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Nguồn tài khoản(AD hoặc thêm từ portal)
        /// </summary>
        public int SourceType { get; set; }

        /// <summary>
        /// Loại tài khoản
        /// </summary>
        public int AccountType { get; set; }

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
        /// Danh sách vai trò của người dùng
        /// </summary>
        public string UserRoleIDs { get; set; }

        /// <summary>
        /// Tên đăng nhập
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Họ và tên
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Mật khẩu người dùng
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Chức vụ
        /// </summary>
        public string PositionName { get; set; }

        /// <summary>
        /// Ngày sinh
        /// </summary>
        public string DateOfBirth { get; set; }

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
        public int NationalAttachmentID { get; set; }

        /// <summary>
        /// Bậc an toàn điện
        /// </summary>
        public string ElectricRank { get; set; }

        /// <summary>
        /// Thẻ an toàn điện
        /// </summary>
        public string ElectricRankCard { get; set; }

        /// <summary>
        /// Hình ảnh thẻ an toàn điện
        /// </summary>
        public int ElectricRankAttachmentID { get; set; }

        /// <summary>
        /// Thẻ an toàn lao động
        /// </summary>
        public string LaborCard { get; set; }

        /// <summary>
        /// Bậc thợ
        /// </summary>
        public string LaborRank { get; set; }

        /// <summary>
        /// Hình ảnh thẻ an toàn lao động
        /// </summary>
        public int LaborCardAttachmentID { get; set; }

        /// <summary>
        /// Chữ ký người dùng
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// Trạng thái truy cập
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Khi login với người dùng AD
        /// </summary>
        [JsonIgnore]
        public bool IsLoginAD { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }

        /// <summary>
        /// Xác định người dùng login từ Mobile/Web
        /// </summary>
        public bool IsMobileSystem { get; set; }

        /// <summary>
        /// Ảnh đại diện
        /// </summary>
        public int? AvatarAttachmentID { get; set; }

        /// <summary>
        /// Phương thức ký cấp/khóa phiếu công tác
        /// </summary>
        public int SigningMethodID { get; set; }

        /// <summary>
        /// Bỏ qua việc kiểm tra quyền
        /// </summary>
        public bool IsBypassRight { get; set; } = false;
    }
}
