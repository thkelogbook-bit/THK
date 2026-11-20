namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin Cương vị trong ca
    /// </summary>
    public class MobilePositionInShiftInfo : MobileEntityBaseModel
    {
        /// <summary>
        /// Xác định cương vị trong ca thuộc cương vị nào
        /// </summary>
        public int ParentID { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public int OperationPositionID { get; set; }

        /// <summary>
        /// System data không cho phép xóa, sửa
        /// </summary>
        public bool IsSystem { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Active { get; set; }

        /// <summary>
        /// Số lượng
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Thứ tự sắp xếp
        /// </summary>
        public int Position { get; set; }
    }
}
