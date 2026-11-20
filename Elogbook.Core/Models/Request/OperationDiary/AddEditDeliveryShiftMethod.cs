using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// AddEditDeliveryShiftMethod
    /// </summary>
    public class AddEditDeliveryShiftMethod
    {
        /// <summary>
        /// Mã bất thường sự cố
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 3.Phương thức giao ca
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// File đính kèm
        /// </summary>
        public List<int> Attachments { get; set; }
    }
}
