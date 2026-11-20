using Elogbook.Core.Constants;
using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// tham số tìm kiếm lịch bảo trì
    /// </summary>
    public class AmMaintenanceScheduleSearchModel : BaseListFilter
    {
        /// <summary>
        /// Tên lịch trình
        /// </summary>
        public string MaintenanceScheduleName { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Địa điểm/ Khu vực
        /// </summary>
        public int SiteDescID { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Thiết bị
        /// </summary>
        public int AssetDescID { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int Active { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Loại công việc
        /// </summary>
        public int WorkTypeID { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Ngày bắt đầu của 1 lịch trình
        /// </summary>
        public string StartDate { get; set; }

        /// <summary>
        /// Ngày kết thúc của 1 lịch trình
        /// </summary>
        public string EndDate { get; set; }

        /// <summary>
        /// Tên thiết bị
        /// </summary>
        public string DeviceName { get; set; }
    }
}
