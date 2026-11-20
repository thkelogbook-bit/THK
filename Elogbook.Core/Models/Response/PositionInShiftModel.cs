namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Cương vị trong ca
    /// </summary>
    public class PositionInShiftModel : EntityBaseModel
    {
        /// <summary>
        /// Tên Cương vị trong ca
        /// </summary>
        public string PositionInShiftName { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public ValueLabelModel OperationPosition { get; set; }

        /// <summary>
        /// Số thứ tự sắp xếp
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Sử dụng
        /// </summary>
        public int Active { get; set; }

        /// <summary>
        /// Số lượng
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Dữ liệu hệ thống không được xóa
        /// </summary>
        public bool IsSystem { get; set; }
    }
}
