using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elogbook.Core.Models.Request.SystemConfiguration
{
    /// <summary>
    /// Update system configuration model
    /// </summary>
    public class UpdateSystemConfigurationModel
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
