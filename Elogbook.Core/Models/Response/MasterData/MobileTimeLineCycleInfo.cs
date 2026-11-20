namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Các mốc thời gian của Chu kỳ
    /// </summary>
    public class MobileTimeLineCycleInfo : MobileEntityBaseModel
    {
        /// <summary>
        /// Mốc thời gian
        /// </summary>
        public int Hour { get; set; }

        /// <summary>
        /// Chu kỳ
        /// </summary>
        public int? CycleID { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Active { get; set; }
    }
}
