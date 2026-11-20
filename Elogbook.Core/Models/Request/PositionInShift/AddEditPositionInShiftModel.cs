using Elogbook.Core.Models.Response;
using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số để thêm mới một Cương vị trong ca
    /// </summary>
    public class AddEditPositionInShiftModel : EntityBaseModel
    {
        /// <summary>
        /// Cương vị
        /// </summary>
        public int ParentID { get; set; }

        /// <summary>
        /// Cương vị trong ca
        /// </summary>
        public int OperatinPositionID { get; set; }

        /// <summary>
        /// Thứ tự sắp xếp
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Số lượng
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Active { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
