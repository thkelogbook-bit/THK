using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin giao/nhận ca
    /// </summary>
    public class AddEditDeliveryReceiveShiftModel
    {
        /// <summary>
        /// Mã nhật ký vận hành
        /// </summary>
        public int OperationDiaryID { get; set; }

        /// <summary>
        /// Xác định cương vị trong ca được khai báo khi ghi nhật ký vận hành
        /// </summary>
        public int PositionShiftID { get; set; }

        /// <summary>
        /// Tài khoản nhân viên
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Mật khẩu nhân viên
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Xác định người giao ca/nhận ca, true thì người đó giao ca và ngược lại
        /// </summary>
        public bool IsDeliveryShift { get; set; }

        /// <summary>
        /// Thời gian ký (hh:mm)
        /// </summary>
        public string SignTime { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }

        /// <summary>
        /// Bỏ qua bước xác thực mật khẩu (dùng khi sync từ mobile, password đã được xác thực ở phía client)
        /// </summary>
        [JsonIgnore]
        public bool SkipPasswordValidation { get; set; }
    }
}
