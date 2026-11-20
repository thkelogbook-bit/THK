using Newtonsoft.Json;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số báo bận hoặc báo dừng máy
    /// </summary>
    public class EditOperationTimeLineModel
    {
        /// <summary>
        /// Danh sách ID khung giờ
        /// </summary>
        public List<int> ListID { get; set; }

        /// <summary>
        /// Giá trị thông số
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public int OperationPositionID { get; set; }

        /// <summary>
        /// Chu kỳ
        /// </summary>
        public int CycleID { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
