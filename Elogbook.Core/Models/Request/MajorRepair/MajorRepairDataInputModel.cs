using Newtonsoft.Json;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông chi tiết phiếu công tác con
    /// </summary>
    public class MajorRepairExcelDataModel
    {
        /// <summary>
        /// Row index
        /// </summary>
        public int RowIndex { get; set; }

        /// <summary>
        /// Nội dung công tác
        /// </summary>
        public string WorkContent { get; set; }

        /// <summary>
        /// Vị trí / KKS
        /// </summary>
        public string WorkTicketLocation { get; set; }

        /// <summary>
        /// Loại phiếu
        /// </summary>
        public int ScreenType { get; set; }

        /// <summary>
        /// Tổ máy / Khu vực
        /// </summary>
        public string WorkTicketUnitArea { get; set; }

        /// <summary>
        /// Excel error
        /// </summary>
        public List<string> Errors { get; set; }
    }

    /// <summary>
    /// Thông chi tiết phiếu công tác con
    /// </summary>
    public class MajorRepairAddTicketModel
    {
        /// <summary>
        /// Id sửa chữa lớn
        /// </summary>
        public int MajorRepairID { get; set; }

        /// <summary>
        /// Nội dung công tác
        /// </summary>
        public string WorkContent { get; set; }

        /// <summary>
        /// Vị trí / KKS
        /// </summary>
        public string WorkTicketLocation { get; set; }

        /// <summary>
        /// Loại phiếu
        /// </summary>
        public int ScreenType { get; set; }

        /// <summary>
        /// Tổ máy / Khu vực
        /// </summary>
        public string WorkTicketUnitArea { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
