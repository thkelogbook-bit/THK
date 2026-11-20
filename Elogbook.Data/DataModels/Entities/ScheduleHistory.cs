using Elogbook.Data.DataModels.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    /// <summary>
    /// Lịch sử chạy schedule
    /// </summary>
    [Table("ScheduleHistories")]
    public partial class ScheduleHistory : EntityBase
    {
        /// <summary>
        /// Tên schedule
        /// </summary>
        [StringLength(500)]
        public string ScheduleName { get; set; }

        /// <summary>
        /// Trạng thái run schedule, 1: Thất bại, 2 Thành công
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Thời gian chạy run job
        /// </summary>
        public DateTime? ScheduleTime { get; set; }

        /// <summary>
        /// Error
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
