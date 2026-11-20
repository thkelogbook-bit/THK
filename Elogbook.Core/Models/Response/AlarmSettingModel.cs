using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Dữ liệu setting cảnh báo
    /// </summary>
    public class AlarmSettingModel
    {
        /// <summary>
        /// Danh sách loại cảnh báo
        /// </summary>
        public List<string> Alarms { get; set; }

        /// <summary>
        /// Danh sách toán tử
        /// </summary>
        public List<string> Operators { get; set; }

        /// <summary>
        /// Danh sách màu
        /// </summary>
        public List<string> Colors { get; set; }
    }
}
