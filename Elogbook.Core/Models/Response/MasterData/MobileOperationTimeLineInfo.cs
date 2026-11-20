namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// MobileOperationTimeLineInfo
    /// </summary>
    public class MobileOperationTimeLineInfo : MobileEntityBaseModel
    {
        /// <summary>
        /// App ID
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// Tên khung giờ
        /// </summary>
        public int TimeLineName { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int? FactoryID { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public int? OperationPositionID { get; set; }

        /// <summary>
        /// Chu kỳ
        /// </summary>
        public int? CycleID { get; set; }

        /// <summary>
        /// Tổng thông số mặc định
        /// </summary>
        public int TotalParametersDefault { get; set; }

        /// <summary>
        /// Tổng thông số đã ghi
        /// </summary>
        public int TotalParametersChecked { get; set; }

        /// <summary>
        /// Tổng thông số đã được đồng bộ
        /// </summary>
        public int TotalParametersSynchronized { get; set; }

        /// <summary>
        /// Key mapping
        /// </summary>
        public string TimeLineSyncDataKey { get; set; }
    }
}
