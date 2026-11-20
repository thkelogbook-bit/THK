using System;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Kiểm tra hoàn thành khóa phiếu
    /// </summary>
    public class CheckCompletedModel : EntityBaseModel
    {
        /// <summary>
        /// Thời gian kiểm tra hoàn thành phiếu công tác
        /// </summary>
        public DateTime CheckCompletedTime { get; set; }

        /// <summary>
        /// Người cấp phiếu
        /// </summary>
        public string ApprovedUser { get; set; }

        /// <summary>
        /// Chức vụ người cấp phiếu
        /// </summary>
        public string PositionApprovedUser { get; set; }

        /// <summary>
        /// Chữ ký người cấp phiếu
        /// </summary>
        public string SignatureApprovedUser { get; set; }
    }
}
