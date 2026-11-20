namespace Elogbook.Core.Constants
{
    /// <summary>
    /// CaseNotificationByMailConstant
    /// </summary>
    public struct EmailCodeConstant
    {
        #region MODULE
        /// <summary>
        /// Bảo trì tự quản
        /// </summary>
        public const string MODULE_AM = "Bảo trì tự quản";

        /// <summary>
        /// Phiếu yêu cầu
        /// </summary>
        public const string MODULE_PYC = "Phiếu yêu cầu";

        #endregion

        #region CASE
        /// <summary>
        /// Nhắc lịch thực hiện hoạt động AM
        /// </summary>
        public const string CASE_AM_WORK_NOTIFY = "AmNotification001";

        /// <summary>
        /// Thông báo bất thường, thiết bị
        /// </summary>
        public const string CASE_AM_DEVICE_ABNORMAL_NOTIFY = "Thông báo bất thường, thiết bị";

        /// <summary>
        /// Có phiếu yêu cầu cần phê duyệt
        /// </summary>
        public const string CASE_PYC_NEED_APPROVE = "Có phiếu yêu cầu cần phê duyệt";

        #endregion

        #region AGRUMENT
        /// <summary>
        /// ElogbookSystem
        /// </summary>
        public const string ARGV_ELOGBOOK_SYSTEM = "[ElogbookSystem]";

        /// <summary>
        /// AccountUser
        /// </summary>
        public const string ARGV_ACCOUNT_USER = "[AccountUser]";

        /// <summary>
        /// Tên lịch
        /// </summary>
        public const string ARGV_MAINTENANCE_SCHEDULE_NAME = "[MaintenanceScheduleName]";

        /// <summary>
        /// Ngày cần thực hiện
        /// </summary>
        public const string ARGV_ACTUAL_DATE = "[ActualDate]";

        /// <summary>
        /// Khu vực
        /// </summary>
        public const string ARGV_AM_LOCATION = "[AmLocation]";

        /// <summary>
        /// Thiết bị
        /// </summary>
        public const string ARGV_AM_DEVICE = "[AmDevice]";

        /// <summary>
        /// Cương vị
        /// </summary>
        public const string ARGV_OPERATION_POSITION = "[OperationPosition]";

        /// <summary>
        /// Ca thực hiện
        /// </summary>
        public const string ARGV_OPERATION_SHIFT = "[OperationShift]";

        /// <summary>
        /// Loại công việc
        /// </summary>
        public const string ARGV_WORK_TYPE = "[AmWorkType]";

        /// <summary>
        /// Công việc
        /// </summary>
        public const string ARGV_WORK = "[AmWork]";

        /// <summary>
        /// Link hệ thống
        /// </summary>
        public const string ARGV_LINK_SYSTEM = "[LinkSystem]";
        #endregion
    }
}
