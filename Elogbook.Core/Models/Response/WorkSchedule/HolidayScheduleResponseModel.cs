using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Mô hình dữ liệu phản hồi cho cấu hình ngày nghỉ lễ
    /// </summary>
    public class HolidayScheduleResponseModel
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
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Ngày kết thúc nghỉ lễ
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}
