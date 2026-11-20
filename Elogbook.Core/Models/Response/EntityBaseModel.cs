using Elogbook.Core.Mapper;
using Newtonsoft.Json;
using System;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin của một đối tượng cần được quản lý
    /// </summary>
    public class EntityBaseModel
    {
        /// <summary>
        /// Khóa của đối tượng được quản lý
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Tracking dữ liệu đã bị xóa
        /// </summary>
        [MapperIgnore]
        [JsonIgnore]
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        public ValueLabelModel CreatedUser { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime InsertedAt { get; set; }

        /// <summary>
        /// Người cập nhật
        /// </summary>
        public ValueLabelModel LastUpdatedUser { get; set; }

        /// <summary>
        /// Ngày cập nhật
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
