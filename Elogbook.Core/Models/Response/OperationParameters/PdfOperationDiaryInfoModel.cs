using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin trực ca
    /// </summary>
    public class PdfOperationDiaryInfoModel
    {
        /// <summary>
        /// Ca trực
        /// </summary>
        public string ShiftName { get; set; }

        /// <summary>
        /// Nhận xét
        /// </summary>
        public string Comments { get; set; }


        /// <summary>
        /// Danh sách nhân viên trực ca
        /// </summary>
        public List<PdfDeliveryReceiveShiftUserInfo> UserInfo { get; set; }
    }
}
