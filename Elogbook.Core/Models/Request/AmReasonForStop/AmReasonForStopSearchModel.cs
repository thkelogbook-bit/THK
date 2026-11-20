using Elogbook.Core.Constants;
using Elogbook.Core.Models.BaseFilter;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// tham số tìm kiếm
    /// </summary>
    public class AmReasonForStopSearchModel : BaseListFilter
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Nguyên nhân
        /// </summary>
        public string ReasonDesc { get; set; }

        /// <summary>
        /// Loại 1: Không thực hiện bảo trì, 2: Dừng công việc
        /// </summary>
        public int ReasonType { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int Active { get; set; } = CommonConstant.ALL_KEY;
    }
}
