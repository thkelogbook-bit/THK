using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số báo bận hoặc báo dừng máy từng phần
    /// </summary>
    public class StopParameterGroupModel
    {
        /// <summary>
        /// Nhóm thông số id
        /// </summary>
        public int ParameterGroupID { get; set; }

        /// <summary>
        /// Dừng tất cả thông số
        /// </summary>
        public bool IsStopAllParameter { get; set; }

        /// <summary>
        /// Danh sách thông số không dừng
        /// </summary>
        public List<int> NoneStopParameterIds { get; set; }
    }
}
