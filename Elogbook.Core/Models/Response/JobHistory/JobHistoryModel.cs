using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Lịch sử đồng bộ
    /// </summary>
    public class JobHistoryModel
    {
        /// <summary>
        /// Mã tồn tại trong ca
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// sync type
        /// </summary>
        public ValueLabelModel SyncType { get; set; }

        /// <summary>
        /// sync type id
        /// </summary>
        public int? SyncTypeID { get; set; }

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
        /// Trạng thái
        /// </summary>
        public ValueLabelModel Status { get; set; }

        /// <summary>
        /// ID Trạng thái
        /// </summary>
        public int? StatusID { get; set; }

        /// <summary>
        /// logs
        /// </summary>
        public string Logs { get; set; }
    }
}
