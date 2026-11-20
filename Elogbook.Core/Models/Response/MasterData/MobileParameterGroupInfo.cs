namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin nhóm thông số
    /// </summary>
    public class MobileParameterGroupInfo : MobileEntityBaseModel
    {
        /// <summary>
        /// Tên nhóm thông số
        /// </summary>
        public string ParameterGroupName { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int? FactoryID { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public int? OperationPositionID { get; set; }

        /// <summary>
        /// Chu kỳ cần ghi thông số
        /// </summary>
        public int? CycleID { get; set; }

        /// <summary>
        /// Thứ tự sắp xếp
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Status { get; set; }
    }
}
