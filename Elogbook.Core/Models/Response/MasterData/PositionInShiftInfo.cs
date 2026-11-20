using System;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Cương vị trong ca
    /// </summary>
    public class PositionInShiftInfo
    {
        /// <summary>
        /// Mã chu kỳ
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Khóa để phân biệt khi 1 cương vị có số lượng người thực hiện lớn hơn 1
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        /// Tên cương vị trong ca
        /// </summary>
        public string PositionInShiftName { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public ValueLabelModel OperationPosition { get; set; }

        /// <summary>
        /// Vị trí hiển thị
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Active { get; set; }

        /// <summary>
        /// Flag đánh dấu dữ liệu đã xóa hay chưa, true đã xóa
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Đánh dấu dữ liệu custom là bắt buộc
        /// </summary>
        public bool IsCustomRequired { get; set; }

        /// <summary>
        /// Mã nhân sự
        /// </summary>
        public int? UserID { get; set; }

        /// <summary>
        /// Tài khoản nhân sự
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Họ và tên nhân sự
        /// </summary>
        public string FullName { get; set; }
    }
}
