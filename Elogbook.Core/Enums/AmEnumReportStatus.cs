using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Các trạng thái của report
    /// </summary>
    public enum AmEnumReportStatus
    {
        /// <summary>
        /// Chờ đăng ký công tác
        /// </summary>
        [Description("Chờ đăng ký công tác")]
        WaitForRegistPCT = 1,
        /// <summary>
        /// Đã đăng ký công tác
        /// </summary>
        [Description("Đã đăng ký công tác")]
        RegisteredPCT,
        /// <summary>
        /// Đã cấp phiếu
        /// </summary>
        [Description("Đã cấp phiếu")]
        ApprovedPCT,
        /// <summary>
        /// Đang thực hiện sửa chữa
        /// </summary>
        [Description("Đang thực hiện sửa chữa")]
        OnGoingPCT,
        /// <summary>
        /// Đã hoàn thành sửa chữa
        /// </summary>
        [Description("Đã hoàn thành sửa chữa")]
        CompletedPCT,

        /// <summary>
        /// Chưa gửi phiếu yêu cầu
        /// </summary>
        [Description("Chưa gửi phiếu")]
        UnSentPYC,
        /// <summary>
        /// Đang xử lý phiếu yêu cầu (Đang xử lý + Từ chối)
        /// </summary>
        [Description("Đang xử lý")]
        InProgessPYC,
        /// <summary>
        /// Đã được duyệt phiếu yêu cầu (Chờ đăng ký + Đã đăng ký)
        /// </summary>
        [Description("Đã được duyệt")]
        ApprovePYC,
        /// <summary>
        /// Dừng xử lý phiếu yêu cầu
        /// </summary>
        [Description("Dừng xử lý")]
        StopPYC,
    }
}
