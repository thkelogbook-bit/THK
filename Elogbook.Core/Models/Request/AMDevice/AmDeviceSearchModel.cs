using Elogbook.Core.Constants;
using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Tham số tìm kiếm 
    /// </summary>
    public class AmDeviceSearchModel : BaseListFilter
    {
        /// <summary>
        /// Thiết bị cha
        /// </summary>
        public int AssetParent { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Loại thiết bị
        /// </summary>
        public int TypeDescID { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Khu vực
        /// </summary>
        public int SiteID { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Tên thiết bị / Mã thiết bị
        /// </summary>
        public string SearchText { get; set; }

        /// <summary>
        /// Tình trạng thiết bị
        /// </summary>
        public int DeviceStatusID { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int Active { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Công ty
        /// </summary>
        public int CompanyID { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Bao gồm thiết bị con (True: trả về cả cha và con, False: chỉ trả về cha)
        /// </summary>
        public bool IncludeChildren { get; set; } = true;
    }

    /// <summary>
    /// Tham số tìm kiếm nhanh thiết bị
    /// </summary>
    public class AmDeviceQuickSearchModel : BaseListFilter
    {
        /// <summary>
        /// Tên thiết bị
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// Mã thiết bị
        /// </summary>
        public string DeviceCode { get; set; }

        /// <summary>
        /// Thiết bị cha (True: tìm kiếm theo thiết bị cha, False: Tìm kiếm tất cả)
        /// </summary>
        public bool IsParent { get; set; } = false;

        /// <summary>
        /// Loại thiết bị
        /// </summary>
        public int TypeDescID { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Khu vực
        /// </summary>
        public int SiteID { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Công ty
        /// </summary>
        public int CompanyID { get; set; } = CommonConstant.ALL_KEY;

    }
}
