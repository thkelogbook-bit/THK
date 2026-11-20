using System;

namespace Elogbook.Core.Models.Request
{
    /// <summary>  Nhật ký vận hành </summary>
    public class OperationDiarySyncModel
    {
        /// <summary>
        /// App ID
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// Mã Nhật ký vận hành
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// SyncDataKey - {Ngày}_{Nhà máy}_{Cương vị}_{Ca} (Ex: 07042022_1_1_1)
        /// </summary>
        public string SyncDataKey { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public int OperationPositionID { get; set; }

        /// <summary>
        /// Ngày vận hành
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Ca vận hành
        /// </summary>
        public int OperationShiftID { get; set; }

        /// <summary>
        /// Kíp vận hành
        /// </summary>
        public int OperationCrewID { get; set; }

        /// <summary>
        /// 5.Nhận xét thông số
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// Trạng thái nhận ca
        /// </summary>
        public bool IsReceived { get; set; }

        /// <summary>
        /// Trạng thái giao ca
        /// </summary>
        public bool IsDelivered { get; set; }

        /// <summary>
        /// Có thể chỉnh sửa dữ liệu nhật ký vận hành
        /// </summary>
        public bool IsCanEdit { get; set; }

        /// <summary>
        /// Ngày ghi tạo nhật ký vận hành
        /// </summary>
        public DateTime? TimeDelivery { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Tạo mới
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
        /// Tình hình lúc nhận ca
        /// </summary>
        public string ShiftReceiving { get; set; }
    }
}
