using System.Collections.Generic;

namespace Elogbook.Core.Models.Request.WorkPermit
{
    /// <summary>
    /// 
    /// </summary>
    public class LctContentPositionModel
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
        public List<LctStakeholdersModel> Stakeholders { get; set; }
    }
}
