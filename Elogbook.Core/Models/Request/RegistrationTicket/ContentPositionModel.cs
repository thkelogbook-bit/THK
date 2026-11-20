using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// ContentPositionModel
    /// </summary>
    public class ContentPositionModel
    {
        /// <summary>
        /// Khóa chính của bảng dữ liệu. Chỉ cần dùng khi update.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Vị trí
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// Danh sách người tham gia công việc
        /// </summary>
        public List<StakeholdersModel> Stakeholders { get; set; }
    }
}
