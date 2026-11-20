namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin khung giờ
    /// </summary>
    public class TimeLineInfoModel
    {
        /// <summary>
        /// Tên Khung giờ
        /// </summary>
        public int TimeLineName { get; set; }

        /// <summary>
        /// Tổng số thông số cho mỗi khung giờ
        /// </summary>
        public int TotalParameters { get; set; }
    }
}
