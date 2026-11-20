using Elogbook.Core.Models.BaseFilter;
using System;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số tìm kiếm phiếu yêu cầu (mobile)
    /// </summary>
    public class AmSearchRequestTicketMobileModel : BaseListFilter
    {
        /// <summary>
        /// Từ khoá
        /// </summary>
        public string SearchKey { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// ngày tạo (từ)
        /// </summary>
        public DateTime? DateFrom { get; set; }

        /// <summary>
        /// ngày tạo(đến)
        /// </summary>
        public DateTime? DateTo { get; set; }

        /// <summary>
        /// mức độ khẩn cấp
        /// </summary>
        public int? AmEmergencyId { get; set; }

        /// <summary>
        /// khu vực
        /// </summary>
        public int? LocationId { get; set; }

        /// <summary>
        /// thiết bị
        /// </summary>
        public int? DeviceId { get; set; }

        /// <summary>
        /// Tab ở mobile
        /// </summary>
        public int Tab { get; set; }
    }
}
