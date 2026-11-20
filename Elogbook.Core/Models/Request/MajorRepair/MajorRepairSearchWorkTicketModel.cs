using Elogbook.Core.Constants;
using Elogbook.Core.Models.BaseFilter;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Điều kiện tìm kiếm phiếu công tác thông tin sửa chữa lớn
    /// </summary>
    public class MajorRepairSearchWorkTicketModel : BaseListFilter
    {
        /// <summary>
        /// Loại màn hình
        /// </summary>
        public int ScreenType { get; set; }

        /// <summary>
        /// Mã số sửa chữa lớn
        /// </summary>
        public int MajorRepairID { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Đơn vị cấp phiếu 
        /// </summary>
        public int ApprovedUnitID { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Ngày tạo phiếu
        /// </summary>
        public string CreateDate { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int WorkTicketStatus { get; set; }

        /// <summary>
        /// Đánh dấu những PCT cần chú ý (Có xảy ra tai nạn, sự cố trong quá trình công tác)
        /// </summary>
        public bool IsFlagNote { get; set; } = false;

        /// <summary>
        /// Thông tin phiếu công tác (Số phiếu/Địa điểm/Nội dung/Phạm vi công tác)
        /// </summary>
        public string SearchText { get; set; }

        /// <summary>
        /// nhà máy
        /// </summary>
        public int FactoryID { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Danh sách trạng thái
        /// </summary>
        public List<int> WorkTicketStatusList { get; set; }
    }
}
