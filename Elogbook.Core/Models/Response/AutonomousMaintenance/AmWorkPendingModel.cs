using System;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin dừng hoặc tạm dừng công việc AM
    /// </summary>
    public class AmWorkPendingModel
    {
        /// <summary>
        /// Khóa của đối tượng được quản lý
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Bảo trì
        /// </summary>
        public ValueLabelCheckedModel AutonomousMaintenance { get; set; }

        /// <summary>
        /// Thực hiện công viêc
        /// </summary>
        public ValueLabelCheckedModel AmWorkDetail { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public ValueLabelCheckedModel Factory { get; set; }

        /// <summary>
        /// Địa điểm/ Khu vực
        /// </summary>
        public ValueLabelCheckedModel Location { get; set; }

        /// <summary>
        /// Lý do dừng công việc
        /// </summary>
        public ValueLabelCheckedModel ReasonForStop { get; set; }

        /// <summary>
        /// Lý do tạm dừng công việc
        /// </summary>
        public ValueLabelCheckedModel ReasonForPending { get; set; }

        /// <summary>
        /// Mô tả thêm
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Thời gian tạm dừng
        /// </summary>
        public DateTime PendingTime { get; set; } = DateTime.Now;

        /// <summary>
        /// Người tạm dừng thực hiện
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
        public DateTime SyncedDate { get; set; }

        /// <summary>
        /// Thời gian tạo
        /// </summary>
        public DateTime InsertedAt { get; set; }
    }
}
