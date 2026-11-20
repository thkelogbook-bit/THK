namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// các tham số Nhóm thông số vận hành
    /// </summary>
    public class OperationParameterGroupModel
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
        /// Tổng số thông số đã ghi dữ liệu
        /// </summary>
        public int TotalParametersChecked { get; set; }
    }
}
