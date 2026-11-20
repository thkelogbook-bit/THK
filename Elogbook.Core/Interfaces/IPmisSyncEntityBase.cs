using System;

namespace Elogbook.Core.Interfaces
{
    /// <summary>
    /// IPmisSyncEntityBase
    /// </summary>
    public interface IPmisSyncEntityBase
    {
        /// <summary>
        /// Khóa bảng dữ liệu
        /// </summary>
        long ID { get; set; }

        /// <summary>
        /// Flag đánh dấu dòng dữ liệu xóa hay chưa
        /// </summary>
        bool IsDeleted { get; set; }

        /// <summary>
        /// Khóa người dùng thêm mới dữ liệu
        /// </summary>
        int? CreatedUserID { get; set; }

        /// <summary>
        /// Thời gian thêm mới dữ liệu
        /// </summary>
        DateTime? InsertedAt { get; set; }

        /// <summary>
        /// Khóa người dùng cập nhật dữ liệu
        /// </summary>
        int? LastUpdatedUserID { get; set; }

        /// <summary>
        /// Thời gian cập nhật dữ liệu
        /// </summary>
        DateTime UpdatedAt { get; set; }
    }
}
