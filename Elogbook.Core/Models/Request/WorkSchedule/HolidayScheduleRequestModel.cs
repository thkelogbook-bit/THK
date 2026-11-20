using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Mô hình dữ liệu yêu cầu cho cấu hình ngày nghỉ lễ
    /// </summary>
    public class HolidayScheduleRequestModel
    {
        /// <summary>
        /// ID của ngày nghỉ lễ
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Tên ngày nghỉ lễ
        /// </summary>
        public string HolidayName { get; set; }

        /// <summary>
        /// Ngày bắt đầu nghỉ lễ
        /// </summary>
        public string StartDate { get; set; }

        /// <summary>
        /// Ngày kết thúc nghỉ lễ
        /// </summary>
        public string EndDate { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}
