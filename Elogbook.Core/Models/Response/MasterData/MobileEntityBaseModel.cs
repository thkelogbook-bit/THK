using System;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin dữ liệu chung sử dụng trên Mobile
    /// </summary>
    public class MobileEntityBaseModel
    {
        /// <summary>
        /// Mã
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Xác định dữ liệu đã xóa
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        public int? CreatedUserID { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime InsertedAt { get; set; }

        /// <summary>
        /// Người cập nhật
        /// </summary>
        public int? LastUpdatedUserID { get; set; }

        /// <summary>
        /// Ngày cập nhật
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
