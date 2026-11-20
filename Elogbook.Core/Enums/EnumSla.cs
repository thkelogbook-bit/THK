using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Phân loại SLA
    /// </summary>
    public enum EnumSlaCategory
    {
        /// <summary>
        /// Phiếu công tác
        /// </summary>
        [Description("Phiếu công tác")]
        PCT = 1,

        /// <summary>
        /// Phiếu thao tác
        /// </summary>
        [Description("Phiếu thao tác")]
        PTT,

        /// <summary>
        /// Phiếu yêu cầu
        /// </summary>
        [Description("Phiếu yêu cầu")]
        PYC,
    }

    /// <summary>
    /// Điều kiện SLA
    /// </summary>
    public enum EnumSlaCondition
    {
        /// <summary>
        /// Đến hạn
        /// </summary>
        [Description("Đến hạn")]
        BeforeDue = 1,

        /// <summary>
        /// Đúng hạn
        /// </summary>
        [Description("Đúng hạn")]
        DueToday,

        /// <summary>
        /// Quá hạn
        /// </summary>
        [Description("Quá hạn")]
        Overdue,
    }
}
