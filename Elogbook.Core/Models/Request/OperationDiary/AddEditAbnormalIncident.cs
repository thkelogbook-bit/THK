using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin bất thường sự cố
    /// </summary>
    public class AddEditAbnormalIncident
    {
        /// <summary>
        /// Mã bất thường sự cố
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 2.Bất thường sự cố
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// File đính kèm
        /// </summary>
        public List<int> Attachments { get; set; }
    }
}
