namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin cấu hình ứng dụng
    /// </summary>
    public class MobileConfigurationInfo
    {
        /// <summary>
        /// Đường dẫn chưa file
        /// </summary>
        public string ServerPath { get; set; }

        /// <summary>
        /// Thời gian chạy đồng bộ dữ liệu
        /// </summary>
        public int SyncOfflineTime { get; set; }

        /// <summary>
        /// Thời gian xóa dữ liệu offline
        /// </summary>
        public int DeleteOffline { get; set; }

        /// <summary>
        /// Cho phép tạo NKVH Ca 3 cho ngày kế tiếp của ngày hiện tại
        /// </summary>
        public bool IsExtendTime { get; set; }

        /// <summary>
        /// Cho phép hiển thị thông tin Tình hình lúc nhận ca cho nhật ký vận hành hay không
        /// </summary>
        public bool IsShowShiftReceiving { get; set; }

        /// <summary>
        /// Show thông báo hay không
        /// </summary>
        public bool IsShowNotification { get; set; } = false;
    }
}
