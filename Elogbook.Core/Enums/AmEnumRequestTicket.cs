using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Trạng thái phiếu yêu cầu
    /// </summary>
    public enum AmEnumRequestTicket
    {
        /// <summary>
        /// Đang xử lý - PYC đã được tạo và đang chờ trưởng ca duyệt
        /// </summary>
        [Description("Đang xử lý")]
        InProgress = 1,
        /// <summary>
        /// Chờ đăng ký - PYC đã được duyệt, phiếu đăng ký công tác chưa được tạo
        /// </summary>
        [Description("Chờ đăng ký")]
        Approved,
        /// <summary>
        /// Từ chối - PYC bị từ chối không được duyệt, có thể chỉnh sửa và gửi lại
        /// </summary>
        [Description("Từ chối")]
        Reject,
        /// <summary>
        /// Đã đăng ký - PYC đã được duyệt, Phiếu đăng ký công tác đã được tạo , Phiếu công tác đang tiến hành
        /// </summary>
        [Description("Đã đăng ký")]
        Registered,
        /// <summary>
        /// Chưa gửi phiếu - PYC vừa được tạo nhưng chưa gửi cho bộ phận duyệt
        /// </summary>
        [Description("Chưa gửi phiếu")]
        UnSend,
        /// <summary>
        /// Dừng xử lý - PYC bị từ chối không được duyệt, và không được thực hiện gửi lại.
        /// </summary>
        [Description("Dừng xử lý")]
        Stop,
    }
}
