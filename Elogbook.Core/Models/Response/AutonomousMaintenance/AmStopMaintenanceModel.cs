using Newtonsoft.Json;
using System;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin chi tiết lý do không thực hiện bảo trì tự quản
    /// </summary>
    public class AmStopMaintenanceModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Bảo trì tự quản
        /// </summary>
        public ValueLabelCheckedModel AutonomousMaintenance { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public ValueLabelCheckedModel Factory { get; set; }

        /// <summary>
        /// Địa điểm/ Khu vực
        /// </summary>
        public ValueLabelCheckedModel AmLocation { get; set; }

        /// <summary>
        /// Lý do dừng bảo trì
        /// </summary>
        public ValueLabelCheckedModel AmReasonForStop { get; set; }

        /// <summary>
        /// Mô tả thêm
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Thời gian dừng
        /// </summary>
        public DateTime StopTime { get; set; } = DateTime.Now;

        /// <summary>
        /// Người dừng thực hiện
        /// </summary>
        public ValueLabelCheckedModel Implementer { get; set; }

        /// <summary>
        /// Người dừng thực hiện
        /// </summary>
        [JsonIgnore]
        public int? ImplementerID { get; set; }

        /// <summary>
        /// Chữ ký Người dừng thực hiện
        /// </summary>
        public string ImplementerSignatureContent { get; set; }

        /// <summary>
        /// SyncDataKey
        /// </summary>
        public string SyncDataKey { get; set; } //{Ngày}_{Nhà máy}_{Cương vị}_{Ca} (07042022_1_1_1)

        /// <summary>
        /// Ngày đồng bộ cuối
        /// </summary>
        public DateTime SyncedDate { get; set; }

        /// <summary>
        /// App ID
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime InsertedAt { get; set; } = DateTime.Now;
    }
}
