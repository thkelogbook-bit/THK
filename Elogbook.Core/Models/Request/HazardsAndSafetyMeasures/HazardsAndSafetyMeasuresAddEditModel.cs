using Elogbook.Core.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin chi tiết
    /// </summary>
    public class HazardsAndSafetyMeasuresAddEditModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; } = 0;

        /// <summary>
        /// Trạng thái  1: Áp dụng, 2: Không áp dụng
        /// </summary>
        public int Active { get; set; } = (int)AmEnumUsageStatus.Apply;

        /// <summary>
        /// Nội dung
        /// </summary>
        public string ContentText { get; set; }

        /// <summary>
        /// Loại 1: Mối nguy hiểm, 2: Biện pháp an toàn
        /// </summary>
        public int ContentType { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
