using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin hồ sơ người dùng cần cập nhật web
    /// </summary>
    public class UpdateUserProfileWebModel
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

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Số điện thoại
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Số CMND/CCCD
        /// </summary>
        public string NationalID { get; set; }

        /// <summary>
        /// Thẻ an toàn điện
        /// </summary>
        public string ElectricRankCard { get; set; }

        /// <summary>
        /// Bậc an toàn điện
        /// </summary>
        public string ElectricRank { get; set; }

        /// <summary>
        /// Thẻ an toàn lao động
        /// </summary>
        public string LaborCard { get; set; }

        /// <summary>
        /// Bậc thợ
        /// </summary>
        public string LaborRank { get; set; }

        /// <summary>
        /// Phương thức ký cấp/khóa phiếu công tác
        /// </summary>
        public int SigningMethodID { get; set; }

        /// <summary>
        /// Hình ảnh số CMND/CCCD
        /// </summary>
        public int NationalAttachmentID { get; set; }

        /// <summary>
        /// Hình ảnh thẻ an toàn điện
        /// </summary>
        public int ElectricRankAttachmentID { get; set; }

        /// <summary>
        /// Hình ảnh thẻ an toàn lao động
        /// </summary>
        public int LaborCardAttachmentID { get; set; }

        /// <summary>
        /// Ảnh đại diện
        /// </summary>
        public int? AvatarAttachmentID { get; set; }

        /// <summary>
        /// Chữ ký người dùng
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
