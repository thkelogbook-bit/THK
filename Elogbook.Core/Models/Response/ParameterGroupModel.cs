namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Nhóm thông số
    /// </summary>
    public class ParameterGroupModel : EntityBaseModel
    {
        /// <summary>
        /// Tên Nhóm thông số
        /// </summary>
        public string ParameterGroupName { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public ValueLabelModel Factory { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public ValueLabelModel OperationPosition { get; set; }

        /// <summary>
        /// Chu kỳ cần ghi thông số
        /// </summary>
        public ValueLabelModel Cycle { get; set; }

        /// <summary>
        /// Số thứ tự sắp xếp
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Sử dụng
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Tổng số thông số
        /// </summary>
        public int TotalParameter { get; set; }
    }
}
