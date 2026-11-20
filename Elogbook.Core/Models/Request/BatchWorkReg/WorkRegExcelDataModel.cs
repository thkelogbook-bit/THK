using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông chi tiết phiếu đăng ký công tác Điện
    /// </summary>
    public class WorkRegDienExcelDataModel
    {
        /// <summary>
        /// Row index
        /// </summary>
        public int RowIndex { get; set; }

        /// <summary>
        /// Loại phiếu
        /// </summary>
        public int TicketType { get; set; }

        /// <summary>
        /// 1/ Nội dung công việc
        /// </summary>
        public string WorkContent { get; set; }

        /// <summary>
        /// 2/ Địa điểm (hoặc thiết bị) tiến hành công tác
        /// </summary>
        public string WorkPlacement { get; set; }

        /// <summary>
        /// 3/ Điều kiện cần có để thực hiện công việc
        /// </summary>
        public string WorkCondition { get; set; }

        /// <summary>
        /// 4/ Thời gian tiến hành công việc (bắt đầu)
        /// </summary>
        public DateTime StartDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 4/ Thời gian tiến hành công việc (kết thúc)
        /// </summary>
        public DateTime EndDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Excel error
        /// </summary>
        public List<string> Errors { get; set; }
    }

    /// <summary>
    /// Thông chi tiết phiếu đăng ký công tác Thủy cơ nhiệt hóa
    /// </summary>
    public class WorkRegTcnhExcelDataModel
    {
        /// <summary>
        /// Row index
        /// </summary>
        public int RowIndex { get; set; }

        /// <summary>
        /// Loại phiếu
        /// </summary>
        public int TicketType { get; set; }

        /// <summary>
        /// Nội dung công việc
        /// </summary>
        public string WorkContent { get; set; }

        /// <summary>
        /// Địa điểm/Thiết bị
        /// </summary>
        public string WorkPlacement { get; set; }

        /// <summary>
        /// Lý do
        /// </summary>
        public int ReasonType { get; set; }

        /// <summary>
        /// Thời gian (bắt đầu)
        /// </summary>
        public DateTime StartDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Thời gian (kết thúc)
        /// </summary>
        public DateTime EndDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Excel error
        /// </summary>
        public List<string> Errors { get; set; }
    }
}
