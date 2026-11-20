using Elogbook.Data.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elogbook.Data
{
    /// <summary>
    /// Cấu hình ngày nghỉ lễ
    /// </summary>
    [Table("HolidaySchedule")]
    public partial class HolidaySchedule : EntityBase
    {
        /// <summary>
        /// Tên ngày nghỉ lễ
        /// </summary>
        public string HolidayName { get; set; }

        /// <summary>
        /// Ngày bắt đầu nghỉ lễ
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Ngày kết thúc nghỉ lễ
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Description { get; set; }
    }
}
