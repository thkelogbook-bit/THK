using Elogbook.Core.Models.BaseFilter;
using System;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số tìm kiếm phiếu yêu cầu
    /// </summary>
    public class AmSearchRequestTicketModel : BaseListFilter
    {
        /// <summary>
        /// 1/VHV 2/trưởng ca 3/đơn vị sửa chữa
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int? FactoryID { get; set; }

        /// <summary>
        /// khu vực
        /// </summary>
        public int? LocationId { get; set; }

        /// <summary>
        /// thiết bị
        /// </summary>
        public int? DeviceId { get; set; }

        /// <summary>
        /// đơn vị công tác
        /// </summary>
        public int? OrganizationId { get; set; }

        /// <summary>
        /// mức độ khẩn cấp
        /// </summary>
        public int? AmEmergencyId { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int? StatusID { get; set; }

        /// <summary>
        /// nội dung/số phiếu
        /// </summary>
        public string ContentTicketNumber { get; set; }

        /// <summary>
        /// ngày tạo (từ)
        /// </summary>
        public string DateFrom { get; set; }

        /// <summary>
        /// ngày tạo(đến)
        /// </summary>
        public string DateTo { get; set; }
    }
}
