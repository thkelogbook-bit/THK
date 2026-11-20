namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// khai báo model export excel detail báo cáo tuân thủ hoạt động
    /// </summary>
    public class ExportExcelDetailModel
    {
        /// <summary>
        /// stt
        /// </summary>
        public int STT { get; set; }

        /// <summary>
        /// Ngày thực hiện
        /// </summary>
        public string ActualDate { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public string Factory { get; set; }

        /// <summary>
        /// Cương vị thực hiện
        /// </summary>
        public string OperationPosition { get; set; }

        /// <summary>
        /// Thiết bị
        /// </summary>
        public string Device { get; set; }

        /// <summary>
        /// Lịch bảo trì
        /// </summary>
        public string MaintenanceSchedule { get; set; }

        /// <summary>
        /// Ca thực hiện
        /// </summary>
        public string OperationShift { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Trạng thái bất thường thiết bị
        /// </summary>
        public string DeviceAbnormalStatus { get; set; }

        /// <summary>
        /// Người hoàn thành
        /// </summary>
        public string CompleterUser { get; set; }

        /// <summary>
        /// Thời gian thực hiện
        /// </summary>
        public string CompleteDate { get; set; }

        /// <summary>
        /// tình trạng công việc
        /// </summary>
        public string WorkStatus { get; set; }
    }
}
