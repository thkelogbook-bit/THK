using Newtonsoft.Json;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin tổ chức thực hiện
    /// </summary>
    public class AddWorkPlanInfoModel
    {
        /// <summary>
        /// ID phiếu công tác
        /// </summary>
        public int WorkTicketID { get; set; }

        /// <summary>
        /// đơn vị công tác (cấp tổ)
        /// </summary>
        public int? WorkUnitID { get; set; }

        /// <summary>
        /// tên đơn vị công tác (trường hợp nhập)
        /// </summary>
        public string WorkUnitName { get; set; }

        /// <summary>
        /// số lượng người
        /// </summary>
        public string AmountPersons { get; set; }

        /// <summary>
        /// Các biện pháp an toàn đơn vị công tác thực hiện
        /// </summary>
        public List<AddSafetyMeasureModel> SafetyMeasures { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
