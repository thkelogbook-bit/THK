using Elogbook.Core.Models.BaseFilter;
using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số tìm kiếm Nhật ký vận hành (Web)
    /// </summary>
    public class SearchOperationDiaryWebModel : BaseListFilter
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public int OperationPositionID { get; set; }

        /// <summary>
        /// Ngày vận hành(Từ ngày)
        /// </summary>
        public string FromDate { get; set; }

        /// <summary>
        /// Ngày vận hành(Đến ngày)
        /// </summary>
        public string ToDate { get; set; }

        /// <summary>
        /// Ca vận hành
        /// </summary>
        public int OperationShiftID { get; set; }

        /// <summary>
        /// Kíp vận hành
        /// </summary>
        public int OperationCrewID { get; set; }

        /// <summary>
        /// Họ và tên người tạo
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Từ ngày
        /// </summary>
        public string DateGroup { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
