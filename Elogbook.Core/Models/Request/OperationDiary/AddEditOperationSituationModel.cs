using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin tình hình vận hành
    /// </summary>
    public class AddEditOperationSituationModel
    {
        /// <summary>
        /// Mã tình hình vận hành
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Giờ ghi dữ liệu
        /// </summary>
        public string Hour { get; set; }

        /// <summary>
        /// Người ra lệnh
        /// </summary>
        public int? AssignedFromID { get; set; }

        /// <summary>
        /// Người ra lệnh ngoài hệ thống
        /// </summary>
        public string AssignedFromName { get; set; }

        /// <summary>
        /// Người nhận lệnh
        /// </summary>
        public int? AssignedToID { get; set; }

        /// <summary>
        /// Người nhận lệnh
        /// </summary>
        public string AssignedToName { get; set; }

        /// <summary>
        /// Tình hình vận hành và các thao tác trong ca
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Xoá tình hình vận hành
        /// </summary>
        public bool IsDeleteOperationSituation { get; set; }

        /// <summary>
        /// File đính kèm
        /// </summary>
        public List<int> Attachments { get; set; }
    }
}
