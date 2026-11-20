using System;

namespace Elogbook.Core.Models.Response.SystemConfiguration
{
    /// <summary>
    /// Cấu hình hệ thống
    /// </summary>
    public class SystemConfigurationModel
    {

        /// <summary>
        /// Format date
        /// </summary>
        public string FormatDate { get; set; }

        /// <summary>
        /// Format number
        /// </summary>
        public string FormatNumber { get; set; }

        /// <summary>
        /// Delete log cron
        /// </summary>
        public string DeleteLogCron { get; set; }

        /// <summary>
        /// Time refresh interval dashboard (Seconds)
        /// </summary>
        public int RefreshIntervalDashboard { get; set; }

        /// <summary>
        /// Maximum file size for upload (Bytes)
        /// </summary>
        public int MaxFileSizeUpload { get; set; }

    }
}
