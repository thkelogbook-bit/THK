using System;

namespace Elogbook.Core.Models.Request
{
    /// <summary>Giao/Nhận ca  </summary>
    public class DeliveryReceiveShiftSyncModel
    {
        /// <summary>
        /// App ID
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// Mã Giao/Nhận ca
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Mã nhật ký vận hành
        /// </summary>
        public int OperationDiaryID { get; set; }

        /// <summary>
        /// SyncDataKey - {Ngày}_{Nhà máy}_{Cương vị}_{Ca} (Ex: 07042022_1_1_1)
        /// </summary>
        public string SyncDataKey { get; set; }

        /// <summary>
        /// Xác định cương vị trong ca được khai báo khi ghi nhật ký vận hành
        /// </summary>
        public int OperationDiaryPositionShiftID { get; set; }

        /// <summary>
        /// Xác định mối liên hệ dánh sách cương vị khai báo để ký giao nhận ca
        /// </summary>
        public string ReferAppID { get; set; }

        /// <summary>
        /// Trưởng ca/Trưởng kíp/Vận hành viên
        /// </summary>
        public int SupervisorID { get; set; }

        /// <summary>
        /// Xác định người giao ca/nhận ca, true thì người đó giao ca và ngược lại
        /// </summary>
        public bool IsDeliveryShift { get; set; }

        /// <summary>
        /// Xóa
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// True Tạo mới dữ liệu, False Cập nhật dữ liệu
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        public int? CreatedUserID { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime InsertedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Người cập nhật
        /// </summary>
        public int? LastUpdatedUserID { get; set; }

        /// <summary>
        /// Ngày cập nhật
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Thời gian ký (hh:mm)
        /// </summary>
        public string SignTime { get; set; }

        /// <summary>
        /// Trưởng ca/Trưởng kíp/Vận hành viên
        /// </summary>
        public int positionShiftID { get; set; }
    }
}
