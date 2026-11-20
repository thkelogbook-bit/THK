using Elogbook.Core.Constants;
using Elogbook.Core.Models.BaseFilter;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin tìm kiếm người dùng
    /// </summary>
    public class GetInternalExternalUserModel : BaseListFilter
    {
        /// <summary>
        /// Mã số công ty
        /// </summary>
        public int FactoryID { get; set; } = CommonConstant.ALL_KEY;

        /// <summary>
        /// Mã số công ty
        /// </summary>
        public int CompanyID { get; set; }

        /// <summary>
        /// Nội dung tìm kiếm
        /// </summary>
        public string SearchText { get; set; } = string.Empty;

        /// <summary>
        /// Thẻ an toàn điện
        /// </summary>
        public string ElectricRank { get; set; } = string.Empty;

        /// <summary>
        /// Thẻ an toàn lao động
        /// </summary>
        public string LaborRank { get; set; } = string.Empty;

        /// <summary>
        /// Loại người dùng
        /// </summary>
        public int UserType { get; set; }

        /// <summary>
        /// Loại phiếu (PCTĐ-TCNH-NKVH-PTT)
        /// </summary>
        public string TicketType { get; set; }

        /// <summary>
        /// lấy data cho form đăng ký (true: get all user rồi tìm kiếm điều kiện search / false: chỉ lấy những user đáp ứng với nhu cầu nhất định
        /// </summary>
        public bool IsFormRegistration { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> PermissionKey { get; set; }

        /// <summary>
        /// Phân xưởng
        /// </summary>
        [JsonIgnore]
        public int ProductionPlantID { get; set; }
    }
}
