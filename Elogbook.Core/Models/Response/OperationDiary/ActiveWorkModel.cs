using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Công tác đang thực hiện
    /// </summary>
    public class ActiveWorkModel
    {
        /// <summary>
        /// Mã công tác đang thực hiện
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Mà nhật ký vận hành
        /// </summary>
        public int OperationDiaryID { get; set; }

        /// <summary>
        /// Giờ, phút bắt đầu
        /// </summary>
        public string StartHour { get; set; }

        /// <summary>
        /// Giờ, phút kết thúc
        /// </summary>
        public string EndHour { get; set; }

        /// <summary>
        /// Giờ để sắp xếp
        /// </summary>
        [JsonIgnore]
        public double HourFloat { get; set; }

        /// <summary>
        /// Tình hình vận hành và các thao tác trong ca
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

        /// <summary>
        /// Ngày tạo dữ liệu
        /// </summary>
        [JsonIgnore]
        public DateTime InsertedAt { get; set; }

        /// <summary>
        /// Sắp xếp
        /// </summary>
        [JsonIgnore]
        public int SortOrder { get; set; }
    }
}
