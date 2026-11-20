using Elogbook.Core.Constants;
using Elogbook.Core.Models.BaseFilter;
using Newtonsoft.Json;
using System;

namespace Elogbook.Core.Models
{
    /// <summary>
    /// Filter model for position shift status statistics
    /// </summary>
    public class FilterPositionShiftStatusModel
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; } = -1;

        /// <summary>
        /// Cương vị
        /// </summary>
        public int OperationPositionID { get; set; }

        /// <summary>
        /// Ngày vận hành(Từ ngày)
        /// </summary>
        public string FromDate { get; set; }

        /// <summary>
        /// Ngày vận hành(Đến ngày)
        /// </summary>
        public string ToDate { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }

        /// <summary>
        /// Ngày vận hành(Từ ngày)
        /// </summary>
        [JsonIgnore]
        public DateTime From { get; set; }

        /// <summary>
        /// Ngày vận hành(Đến ngày)
        /// </summary>
        [JsonIgnore]
        public DateTime To { get; set; }
    }

    /// <summary>
    /// Filter model for plant shift status statistics
    /// </summary>
    public class FilterPlantShiftStatusModel
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; } = -1;

        /// <summary>
        /// Ngày vận hành(Từ ngày)
        /// </summary>
        public string FromDate { get; set; }

        /// <summary>
        /// Ngày vận hành(Đến ngày)
        /// </summary>
        public string ToDate { get; set; }

        /// <summary>
        /// Ngày vận hành(Từ ngày)
        /// </summary>
        [JsonIgnore]
        public DateTime From { get; set; }

        /// <summary>
        /// Ngày vận hành(Đến ngày)
        /// </summary>
        [JsonIgnore]
        public DateTime To { get; set; }
    }

    /// <summary>
    /// Filter model for monthly shift count statistics
    /// </summary>
    public class FilterMonthlyShiftCountModel
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Năm
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Danh sách tháng
        /// </summary>
        public string Months { get; set; }
    }

    /// <summary>
    /// Thống kê các thông số có giá trị vượt ngưỡng
    /// </summary>
    public class FilterThresholdExceededParametersModel : BaseListFilter
    {
        /// <summary>
        /// Lọc theo nhà máy (-1 = tất cả)
        /// </summary>
        public int FactoryID { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Từ ngày (dd/MM/yyyy)
        /// </summary>
        public string FromDate { get; set; }

        /// <summary>
        /// Đến ngày (dd/MM/yyyy)
        /// </summary>
        public string ToDate { get; set; }

        /// <summary>
        /// Từ ngày (parsed, dùng nội bộ)
        /// </summary>
        [JsonIgnore]
        public DateTime? From { get; set; }

        /// <summary>
        /// Đến ngày (parsed, dùng nội bộ)
        /// </summary>
        [JsonIgnore]
        public DateTime? To { get; set; }
    }

    /// <summary>
    /// Báo cáo thống kê chi tiết lỗi thiết bị
    /// </summary>
    public class FilterGetDeviceErrorDetailReportModel : BaseListFilter
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public int OperationPositionID { get; set; }

        /// <summary>
        /// Chu kỳ
        /// </summary>
        public int CycleID { get; set; }

        /// <summary>
        /// Năm
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Từ ngày
        /// </summary>
        public string FromDate { get; set; }

        /// <summary>
        /// Đến ngày
        /// </summary>
        public string ToDate { get; set; }

        /// <summary>
        /// Ngày vận hành(Từ ngày)
        /// </summary>
        [JsonIgnore]
        public DateTime From { get; set; }

        /// <summary>
        /// Ngày vận hành(Đến ngày)
        /// </summary>
        [JsonIgnore]
        public DateTime To { get; set; }
    }

    /// <summary>
    /// Bảng thống kê số lượng báo bận công tác và dừng máy 
    /// </summary>
    public class FilterBusyAndMachineStopStatisticsModel : BaseListFilter
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public int OperationPositionID { get; set; }

        /// <summary>
        /// Chu kỳ
        /// </summary>
        public int CycleID { get; set; }

        /// <summary>
        /// Năm
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Từ ngày
        /// </summary>
        public string FromDate { get; set; }

        /// <summary>
        /// Đến ngày
        /// </summary>
        public string ToDate { get; set; }

        /// <summary>
        /// Ngày vận hành(Từ ngày)
        /// </summary>
        [JsonIgnore]
        public DateTime From { get; set; }

        /// <summary>
        /// Ngày vận hành(Đến ngày)
        /// </summary>
        [JsonIgnore]
        public DateTime To { get; set; }
    }

}
