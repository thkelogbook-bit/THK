using System;

namespace Elogbook.Core.Models.Response.WorkPermit
{
    /// <summary>
    /// Tiếp nhận nơi làm việc, kiểm tra khóa lệnh
    /// </summary>
    public class LctCheckAndLockTicketModel : EntityBaseModel
    {
        /// <summary>
        /// Thời gian khóa lệnh
        /// </summary>
        public DateTime WorkTicketLockTime { get; set; }

        /// <summary>
        /// Người cho phép
        /// </summary>
        public string Allower { get; set; }

        /// <summary>
        /// Chức vụ người cho phép
        /// </summary>
        public string PositionAllower { get; set; }

        /// <summary>
        /// Chữ ký người cho phép
        /// </summary>SignatureAllower
        public string SignatureAllower { get; set; }
    }
}
