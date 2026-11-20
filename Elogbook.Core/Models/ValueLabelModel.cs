using Newtonsoft.Json;
using System;

namespace Elogbook.Core.Models
{
    /// <summary>
    /// Danh sách các thuộc tính dùng combobox
    /// </summary>
    public class ValueLabelModel
    {
        /// <summary>
        /// Key của thuộc tính
        /// </summary>
        public int? Value { get; set; }

        /// <summary>
        /// Mô tả thuộc tính
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Thứ tư sắp xếp ca vận hành trong nhà máy
        /// </summary>
        [JsonIgnore]
        public int Position { get; set; }

        /// <summary>
        /// Thời gian tạo dữ liệu
        /// </summary>
        public DateTime InsertedAt { get; set; }

        /// <summary>
        /// Cho phép cấu ghi TSVH theo nhóm
        /// </summary>
        public bool IsUseOperationParameterGroup { get; set; } = false;

        /// <summary>
        /// Cho phép kiêm nhiệm nhiều vị trí trong ca vận hành
        /// </summary>
        public bool IsConcurrentlyInShift { get; set; }

        /// <summary>
        /// Thời gian bắt đầu ca
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// Thời gian kết thúc ca
        /// </summary>
        public string EndTime { get; set; }
    }
}
