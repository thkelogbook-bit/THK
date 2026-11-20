using System;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin tiến trình thực hiện bảo trì tự quản
    /// </summary>
    public class AmWorkingProgressModel : EntityBaseModel
    {
        /// <summary>
        /// Mã bảo trì tự quản
        /// </summary>
        public ValueLabelCheckedModel AutonomousMaintenance { get; set; }

        /// <summary>
        /// Mã chi tiết thực hiện công việc AM
        /// </summary>
        public ValueLabelCheckedModel AmWorkDetail { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public ValueLabelCheckedModel Factory { get; set; }

        /// <summary>
        /// Loại công việc
        /// </summary>
        public ValueLabelCheckedModel WorkType { get; set; }

        /// <summary>
        /// Công việc
        /// </summary>
        public ValueLabelCheckedModel Work { get; set; }

        /// <summary>
        /// Người thực hiện
        /// </summary>
        public ValueLabelCheckedModel ImplementerUser { get; set; }

        /// <summary>
        /// ID Lý do dừng/ Tạm dừng công việc
        /// </summary>
        public int? ReasonID { get; set; }

        /// <summary>
        /// ReasonType : 1:Pending, 2:Stop
        /// </summary>
        public int? ReasonType { get; set; }

        /// <summary>
        /// Lý do dừng/ Tạm dừng công việc
        /// </summary>
        public ValueLabelCheckedModel Reason { get; set; }

        /// <summary>
        /// Ngày thực hiện
        /// </summary>
        public DateTime ActualDate { get; set; }

        /// <summary>
        /// Nội dung
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Remark { get; set; }

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
    }
}
