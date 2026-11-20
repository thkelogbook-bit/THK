using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// các tham số Nhóm thông số vận hành
    /// </summary>
    public class ParameterGroupDetailModel
    {
        /// <summary>
        /// ID Nhóm thông số
        /// </summary>
        public int? ParameterGroupID { get; set; }

        /// <summary>
        /// Tên Nhóm thông số
        /// </summary>
        public string ParameterGroupName { get; set; }

        /// <summary>
        /// Tổng số thông số
        /// </summary>
        public int TotalParameters { get; set; }

        /// <summary>
        /// Danh sách thông số
        /// </summary>
        public List<ValueLabelCheckedModel> Parameters { get; set; }
    }
}
