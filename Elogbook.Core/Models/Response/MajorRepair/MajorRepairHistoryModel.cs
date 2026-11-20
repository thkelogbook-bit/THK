using Newtonsoft.Json;
using System;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin tracking đợt công tác Sửa chữa lớn
    /// </summary>
    public class MajorRepairHistoryModel
    {
        /// <summary>
        /// ID tracking
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Sửa chữa lớn
        /// </summary>
        public ValueLabelCheckedModel MajorRepair { get; set; }

        /// <summary>
        /// Phiếu công tác
        /// </summary>
        public ValueLabelCheckedModel WorkTicket { get; set; }

        /// <summary>
        /// Loại màn hình
        /// </summary>
        public int? ScreenType { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public ValueLabelCheckedModel MajorRepairStatus { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        public ValueLabelCheckedModel CreatedUser { get; set; }

        /// <summary>
        /// Nội dung thực hiện
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Nội dung thực hiện
        /// </summary>
        public DateTime InsertedAt { get; set; }

        /// <summary>
        /// File đính kèm
        /// </summary>
        public ValueLabelCheckedModel Attachment { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        [JsonIgnore]
        public int? MajorRepairStatusID { get; set; }

        /// <summary>
        /// Chú thích
        /// </summary>
        public string Comment { get; set; }
    }
}
