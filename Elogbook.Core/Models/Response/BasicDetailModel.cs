namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin dữ liệu
    /// </summary>
    public class BasicDetailModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Tiêu đề
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public string Status { get; set; }
    }
}
