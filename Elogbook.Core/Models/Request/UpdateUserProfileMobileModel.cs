using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin hồ sơ người dùng cần cập nhật trên mobile
    /// </summary>
    public class UpdateUserProfileMobileModel
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
