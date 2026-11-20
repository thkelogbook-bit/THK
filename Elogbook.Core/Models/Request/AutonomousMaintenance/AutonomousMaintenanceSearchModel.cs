using Elogbook.Core.Constants;
using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các điều kiện tìm kiếm
    /// </summary>
    public class AutonomousMaintenanceSearchModel : BaseListFilter
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Cương vị thực hiện
        /// </summary>
        public int OperationPositionID { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Địa điểm/ Khu vực
        /// </summary>
        public int LocationID { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Thiết bị
        /// </summary>
        public int DeviceID { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Tên lịch công tác
        /// </summary>
        public string MaintenanceScheduleName { get; set; }

        /// <summary>
        /// Ngày thực hiện (From)
        /// </summary>
        public string StartDate { get; set; }

        /// <summary>
        /// Ngày thực hiện (To)
        /// </summary>
        public string EndDate { get; set; }

        /// <summary>
        /// Trạng thái bất thường thiết bị
        /// </summary>
        public int AmDeviceAbnormalStatusID { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int StatusID { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Tên thiết bị
        /// </summary>
        public string DeviceName { get; set; }
    }
}
