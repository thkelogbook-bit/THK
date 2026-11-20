
using System;

namespace Elogbook.Core.Models.Response.Report
{
    /// <summary>
    /// 
    /// </summary>
    public class ReportViewModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Là report mặc định
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Report is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Flag đã được add vào dashboard chưa
        /// </summary>
        public bool IsAddToDashboard { get; set; }

        /// <summary>
        /// Find by module type
        /// </summary>
        public string ModuleType { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime InsertedAt { get; set; }
    }
}
