using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models
{
    /// <summary>
    /// Filter model for position shift status statistics
    /// </summary>
    public class PositionShiftStatusModel
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public ValueLabelModel Factory { get; set; }

        /// <summary>
        /// Cương vị ghi NKVH
        /// </summary>
        public ValueLabelModel OperationPosition { get; set; }

        /// <summary>
        /// Tổng số chưa giao/ nhân ca
        /// </summary>
        public int TotalReceived { get; set; } = 0;

        /// <summary>
        /// Tổng số đã giao ca
        /// </summary>
        public int TotalDelivered { get; set; } = 0;

        /// <summary>
        /// tổng số ca
        /// </summary>
        public int Total { get; set; } = 0;
    }

    /// <summary>
    /// Filter model for plant shift status statistics
    /// </summary>
    public class PlantShiftStatusModel
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public ValueLabelModel Factory { get; set; }

        /// <summary>
        /// Tổng số chưa xử lý
        /// </summary>
        public int TotalPendingCount { get; set; } = 0;

        /// <summary>
        /// Tổng số đang xử lý
        /// </summary>
        public int TotalProcessingCount { get; set; } = 0;

        /// <summary>
        /// Tổng số đã xử lý
        /// </summary>
        public int TotalProcessedCount { get; set; } = 0;

        /// <summary>
        /// Tổng số ca
        /// </summary>
        public int Total { get; set; } = 0;
    }

    /// <summary>
    /// Filter model for monthly shift count statistics
    /// </summary>
    public class MonthlyShiftCountModel
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public ValueLabelModel Factory { get; set; }

        /// <summary>
        /// Chi tiết thống kê ca vận hành theo tháng
        /// </summary>
        public List<MonthlyShiftCountDetailsModel> MonthlyShiftCountDetails { get; set; }

        /// <summary>
        /// Tổng số tồn tại ca vận hành theo tháng
        /// </summary>
        public int Total { get; set; } = 0;
    }

    /// <summary>
    /// Thông tin một thông số vượt ngưỡng
    /// </summary>
    public class ThresholdExceededParametersModel
    {
        /// <summary>
        /// STT
        /// </summary>
        public int RowIndex { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public ValueLabelModel Factory { get; set; }

        /// <summary>
        /// Ngày ghi thông số (CheckedDate)
        /// </summary>
        public DateTime? CheckedDate { get; set; }

        /// <summary>
        /// Nhóm thông số
        /// </summary>
        public string ParameterGroupName { get; set; }

        /// <summary>
        /// Tên thông số
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// Mã KKS
        /// </summary>
        public string KKSCode { get; set; }

        /// <summary>
        /// Khoảng giá trị cho phép (Range)
        /// </summary>
        public string Range { get; set; }

        /// <summary>
        /// Giá trị vượt ngưỡng được ghi
        /// </summary>
        public string ExceededValue { get; set; }
    }

    /// <summary>
    /// Báo cáo thống kê chi tiết lỗi thiết bị
    /// </summary>
    public class GetDeviceErrorDetailReportModel
    {
        /// <summary>
        /// Số thứ tự
        /// </summary>
        public int No { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public ReportKeyValueModel Factory { get; set; }

        /// <summary>
        /// Ngày ghi thông số
        /// </summary>
        public DateTime InsertedAt { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public ReportKeyValueModel OperationPosition { get; set; }

        /// <summary>
        /// Chu kỳ ghi thông số
        /// </summary>
        public ReportKeyValueModel Cycle { get; set; }

        /// <summary>
        /// Khung giờ
        /// </summary>
        public ReportKeyValueModel OperationTimeLine { get; set; }

        /// <summary>
        /// Nhóm thông số
        /// </summary>
        public ReportKeyValueModel ParameterGroup { get; set; }

        /// <summary>
        /// Thông số
        /// </summary>
        public ReportKeyValueModel Parameter { get; set; }

        /// <summary>
        /// Mã KKS
        /// </summary>
        public string KKSCode { get; set; }

        /// <summary>
        /// Báo lỗi thiết bị
        /// </summary>
        public string DeviceError { get; set; }

        /// <summary>
        /// Giá trị lỗi
        /// </summary>
        public int _errorCode { get; set; }

        private int _workTicketTypeValue;
        /// <summary>
        /// Phân loại lỗi thiết bị
        /// </summary>
        [JsonIgnore]
        public int ErrorCode
        {
            get
            {
                return _errorCode;
            }
            set
            {
                _errorCode = value;
                switch (value)
                {
                    case (int)DeviceErrorCode.Error:
                        DeviceError = EnumExtensions.GetEnumDescription(DeviceErrorCode.Error);
                        break;
                    case (int)DeviceErrorCode.Stop:
                        DeviceError = EnumExtensions.GetEnumDescription(DeviceErrorCode.Stop);
                        break;
                    case (int)DeviceErrorCode.Sim:
                        DeviceError = EnumExtensions.GetEnumDescription(DeviceErrorCode.Sim);
                        break;
                    case (int)DeviceErrorCode.Jump:
                        DeviceError = EnumExtensions.GetEnumDescription(DeviceErrorCode.Jump);
                        break;
                    default:
                        DeviceError = "";
                        break;
                }
            }
        }

        /// <summary>
        /// Nhà máy
        /// </summary>
        [JsonIgnore]
        public int FactoryID { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        [JsonIgnore]
        public int? OperationPositionID { get; set; }

        /// <summary>
        /// Chu kỳ
        /// </summary>
        [JsonIgnore]
        public int? CycleID { get; set; }

        /// <summary>
        /// Khung giờ
        /// </summary>
        [JsonIgnore]
        public int? OperationTimeLineID { get; set; }

        /// <summary>
        /// Nhóm thông số
        /// </summary>
        [JsonIgnore]
        public int? ParameterGroupID { get; set; }

        /// <summary>
        /// Thông số ID
        /// </summary>
        [JsonIgnore]
        public int ParameterID { get; set; }
    }

    /// <summary>
    /// Thống kê báo bận công tác và dừng thiết bị
    /// </summary>
    public class BusyAndMachineStopStatisticsModel
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public ReportKeyValueModel Factory { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public ReportKeyValueModel OperationPosition { get; set; }

        /// <summary>
        /// Chu kỳ ghi thông số
        /// </summary>
        public ReportKeyValueModel Cycle { get; set; }

        /// <summary>
        /// Ngày ghi thông số
        /// </summary>
        public DateTime InsertedAt { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        [JsonIgnore]
        public int FactoryID { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        [JsonIgnore]
        public int? OperationPositionID { get; set; }

        /// <summary>
        /// Chu kỳ
        /// </summary>
        [JsonIgnore]
        public int? CycleID { get; set; }

        /// <summary>
        /// Danh sách OperationTimeLineID có báo bận công tác (danh sách id phân tách bởi dấu phẩy)
        /// </summary>
        public string BusyOperationTimeLines { get; set; }

        /// <summary>
        /// Danh sách OperationTimeLineID có báo dừng thiết bị (danh sách id phân tách bởi dấu phẩy)
        /// </summary>
        public string StopOperationTimeLines { get; set; }
    }

    /// <summary>
    /// Filter model for monthly shift count statistics
    /// </summary>
    public class MonthlyShiftCountDetailsModel
    {
        /// <summary>
        /// Tháng
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// Tổng số ca vận hành
        /// </summary>
        public int TotalReceived { get; set; } = 0;

        /// <summary>
        /// Trạng thái
        /// </summary>
        public bool IsActive { get; set; } = false;
    }

    /// <summary>
    /// Filter model for key-value pair in report
    /// </summary>
    public class ReportKeyValueModel
    {
        /// <summary>
        /// Giá trị
        /// </summary>
        public int? Value { get; set; }
        /// <summary>
        /// Nhãn
        /// </summary>
        public string Label { get; set; }
    }

    /// <summary>
    /// Filter model for key-value pair in report
    /// </summary>
    public class ReportKeyValueTimeLineModel
    {
        /// <summary>
        /// Giá trị
        /// </summary>
        public int? Value { get; set; }
        /// <summary>
        /// Nhãn
        /// </summary>
        public int Label { get; set; }
    }
}
