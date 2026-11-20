using System;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin liên bảo trì tự quản
    /// </summary>
    public class RelateInfoModel
    {
        /// <summary>
        /// Loại
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Mã phiếu bảo trì tự quản
        /// </summary>
        public int? MaintenanceId { get; set; }

        /// <summary>
        /// Mã phiếu đăng ký
        /// </summary>
        public int? RegistrationId { get; set; }

        /// <summary>
        /// Mã phiếu công tác
        /// </summary>
        public int? WorkticketId { get; set; }

        /// <summary>
        /// Thông tin
        /// </summary>
        public ValueLabelModel Info { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime InsertedAt { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Thông tin thiết bị
        /// </summary>
        public ValueLabelModel DeviceInfo { get; set; }

        /// <summary>
        /// Link chuyển đến chức năng liên quan
        /// </summary>
        public string RouterLink { get; set; }
    }
}
