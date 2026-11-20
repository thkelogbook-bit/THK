using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Phương thức giao ca
    /// </summary>
    public class DeliveryShiftMethodModel
    {
        /// <summary>
        /// Mã Phương thức giao ca
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Mã nhật ký vận hành
        /// </summary>
        public int OperationDiaryID { get; set; }

        /// <summary>
        /// Nội dung Phương thức giao ca
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Danh sách file đính kèm
        /// </summary>
        public virtual List<AttachmentModel> FileAttachments { get; set; }

        /// <summary>
        /// Danh sách id file đính kèm
        /// </summary>
        public string Attackments { get; set; }
    }
}
