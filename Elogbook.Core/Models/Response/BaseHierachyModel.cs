using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Model cơ bản cho cấu trúc phân cấp
    /// </summary>
    public class BaseHierachyModel
    {
        /// <summary>
        /// Thông tin đối tượng
        /// </summary>
        public BasicInfoModel Data { get; set; }

        /// <summary>
        /// Danh sách đối tượng con
        /// </summary>
        public List<BaseHierachyModel> Children { get; set; }
    }
}
