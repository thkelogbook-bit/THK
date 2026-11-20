using Newtonsoft.Json;
using System;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin chi tiết trả về của công việc thực hiện công việc
    /// </summary>
    public class AmWorkDetailModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public ValueLabelCheckedModel Factory { get; set; }

        /// <summary>
        /// Bảo trì tự quản
        /// </summary>
        public ValueLabelCheckedModel AutonomousMaintenance { get; set; }

        /// <summary>
        /// Loại công việc
        /// </summary>
        public ValueLabelCheckedModel WorkType { get; set; }

        /// <summary>
        /// Công việc
        /// </summary>
        public ValueLabelCheckedModel Work { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public AmStatusInfoModel Status { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        [JsonIgnore]
        public int StatusID { get; set; }

        /// <summary>
        /// Thứ tự
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Số phút
        /// </summary>
        public int? SLA { get; set; }

        /// <summary>
        /// Mô tả hướng dẫn
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Thời gian lưu tạm (hh:mm:ss)
        /// </summary>
        public string StickTime { get; set; }

        /// <summary>
        /// Thời gian lưu tạm (hh:mm:ss)
        /// </summary>
        public string StickTimeInProgress { get; set; }

        /// <summary>
        /// Người thực hiện
        /// </summary>
        public ValueLabelCheckedModel Implementer { get; set; }

        /// <summary>
        /// SyncDataKey
        /// </summary>
        public string SyncDataKey { get; set; } //{Ngày}_{Nhà máy}_{Cương vị}_{Ca} (07042022_1_1_1)

        /// <summary>
        /// App ID
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// Ngày đồng bộ cuối
        /// </summary>
        public DateTime SyncedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Thời gian cập nhật
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Thời gian tạo
        /// </summary>
        public DateTime InsertedAt { get; set; }
    }
}
