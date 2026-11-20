using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin chi tiết cảnh báo AI của 1 thông số vận hành.
    /// Nguồn dữ liệu: Parameters + OperationParameterHistory + GetOperationParameterWarning SP.
    /// </summary>
    public class OperationParameterWarningDetailModel
    {
        /// <summary>ID thông số</summary>
        public int ParameterID { get; set; }

        /// <summary>Tên thông số</summary>
        public string ParameterName { get; set; }

        /// <summary>Mã KKS thiết bị</summary>
        public string KKSCode { get; set; }

        /// <summary>Đơn vị đo</summary>
        public string UnitMeasure { get; set; }

        /// <summary>Giới hạn dưới</summary>
        public float? FromRange { get; set; }

        /// <summary>Giới hạn trên</summary>
        public float? ToRange { get; set; }

        /// <summary>Tên nhóm thông số</summary>
        public string ParameterGroupName { get; set; }

        /// <summary>Tên nhà máy</summary>
        public string FactoryName { get; set; }

        /// <summary>Cương vị vận hành</summary>
        public string CuongVi { get; set; }

        /// <summary>
        /// Mức độ cảnh báo: 1 = Thấp, 2 = Trung bình, 3 = Cao
        /// </summary>
        public int WarningLevel { get; set; }

        /// <summary>
        /// Nội dung cảnh báo
        /// </summary>
        public string WarningMessage { get; set; }

        /// <summary>
        /// Lịch sử giá trị ghi nhận (tối đa 50 bản ghi gần nhất, từ OperationParameterHistory)
        /// </summary>
        public List<OperationParameterHistoryModel> HistoryValues { get; set; }
            = new List<OperationParameterHistoryModel>();
    }

    /// <summary>
    /// Một bản ghi lịch sử giá trị — từ bảng OperationParameterHistory
    /// </summary>
    public class OperationParameterHistoryModel
    {
        /// <summary>ID bản ghi (bigint)</summary>
        public long ID { get; set; }

        /// <summary>Ngày ghi nhận</summary>
        public DateTime RecordDate { get; set; }

        /// <summary>Khung giờ (số nguyên 0–23)</summary>
        public int TimeSlot { get; set; }

        /// <summary>Ngưỡng dưới</summary>
        public float? RangeFrom { get; set; }

        /// <summary>Ngưỡng trên</summary>
        public float? RangeTo { get; set; }

        /// <summary>Giá trị thực tế được ghi</summary>
        public string CurrentValue { get; set; }

        /// <summary>Giá trị nằm ngoài ngưỡng</summary>
        public bool IsOutOfRange { get; set; }
    }
}
