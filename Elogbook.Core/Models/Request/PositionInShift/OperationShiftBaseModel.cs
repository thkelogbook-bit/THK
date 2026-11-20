namespace Elogbook.Core.Models.Response.OperationShift
{
    /// <summary>
    /// Lớp cơ sở cho các model liên quan đến ca vận hành
    /// </summary>
    public class OperationShiftBaseModel : ValueLabelModel
    {
        /// <summary>
        /// Thời gian bắt đầu ca
        /// </summary>
        public double StartTime { get; set; }

        /// <summary>
        /// Thời gian kết thúc ca
        /// </summary>
        public double EndTime { get; set; }
    }
}
