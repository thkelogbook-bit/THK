using System;

namespace Elogbook.Core.Models.Response.WorkPermit
{
    /// <summary>
    /// Kiểm tra hoàn thành khóa lệnh
    /// </summary>
    public class LctCheckCompletedModel : EntityBaseModel
    {
        /// <summary>
        /// Thời gian kiểm tra hoàn thành lệnh công tác
        /// </summary>
        public DateTime CheckCompletedTime { get; set; }

        /// <summary>
        /// Người cấp lệnh
        /// </summary>
        public string ApprovedUser { get; set; }

        /// <summary>
        /// Chức vụ người cấp lệnh
        /// </summary>
        public string PositionApprovedUser { get; set; }

        /// <summary>
        /// Chữ ký người cấp lệnh
        /// </summary>
        public string SignatureApprovedUser { get; set; }
    }
}
