
namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Danh sách cương vị dùng cho selecbox
    /// </summary>
    public class ComboboxOperationPositionModel
    {
        /// <summary>
        /// Key của thuộc tính
        /// </summary>
        public ValueModel Value { get; set; }

        /// <summary>
        /// Mô tả thuộc tính
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Thứ tự sắp xếp
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Cho phép kiêm nhiệm nhiều vị trí trong ca vận hành
        /// </summary>
        public bool IsConcurrentlyInShift { get; set; }

        /// <summary>
        /// số lượng
        /// </summary>
        public int Quantity { get; set; }
    }
}
