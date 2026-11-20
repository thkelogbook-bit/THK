namespace Elogbook.Data
{
    using Elogbook.Data.DataModels.Base;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("PMIS_JOB_HISTORY")]
    public partial class PmisJobHistory : PmisSyncEntityBase
    {
        /// <summary>
        /// LAST_RUN_DATE
        /// </summary>
        public DateTime? LAST_RUN_DATE { get; set; }

        /// <summary>
        /// SyncType
        /// </summary>
        public int? SyncType { get; set; }

        /// <summary>
        /// SyncDataKey
        /// </summary>
        public int? UserID { get; set; }

        /// <summary>
        /// Object
        /// </summary>
        public string Object { get; set; }

        /// <summary>
        /// Start time
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// End time
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// Duration
        /// </summary>
        public int? Duration { get; set; }

        /// <summary>
        /// Number record
        /// </summary>
        public int? NumRecord { get; set; }

        /// <summary>
        /// Khu vực
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// Logs
        /// </summary>
        public string Logs { get; set; }
    }
}
