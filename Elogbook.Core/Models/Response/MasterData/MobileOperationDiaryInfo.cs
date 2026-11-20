using System;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin Nhật ký vận hành
    /// </summary>
    public class MobileOperationDiaryInfo : MobileEntityBaseModel
    {
        /// <summary>
        /// App ID
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int? FactoryID { get; set; }

        /// <summary>
        /// Ngày ghi tạo nhật ký vận hành
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public int? OperationPositionID { get; set; }

        /// <summary>
        /// Ca vận hành
        /// </summary>
        public int? OperationShiftID { get; set; }

        /// <summary>
        /// Kíp vận hành
        /// </summary>
        public int? OperationCrewID { get; set; }

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
        /// Thứ tự hiển thị
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// SyncDataKey
        /// </summary>
        public string SyncDataKey { get; set; }

        /// <summary>
        /// Tình hình lúc nhận ca
        /// </summary>
        public string ShiftReceiving { get; set; }
    }
}
