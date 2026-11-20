namespace Elogbook.Core.Models.Response.MasterData.AMs
{
    /// <summary>
    /// Danh mục - Công việc
    /// </summary>
    public class MobileAmWork : MobileEntityBaseModel
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int? FactoryID { get; set; }

        /// <summary>
        /// Loại công việc
        /// </summary>
        public int? AmWorkTypeID { get; set; }

        /// <summary>
        /// Trạng thái  1: Có thể sử dụng, 2: Không thể sử dụng
        /// </summary>
        public int? Active { get; set; }

        /// <summary>
        /// Tên công việc
        /// </summary>
        public string WorkName { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string WorkDesc { get; set; }

    }
}
