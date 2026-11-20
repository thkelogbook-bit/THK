using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request.WorkPermit
{
    /// <summary>
    /// Các tham số để thêm mới tiếp nhân nơi làm việc
    /// </summary>
    public class LctAddReceiveWorkplaceMobileModel
    {
        /// <summary>
        /// Danh sách các biện pháp
        /// </summary>
        public List<LctAddSafetyMeasureModel> SafetyMeasures { get; set; }

        /// <summary>
        /// Thời gian bắt đầu
        /// </summary>
        public DateTime StartWorkingDate { get; set; }

        /// <summary>
        /// Người giám sát
        /// </summary>
        public LctLeadershipUserSigningModel Supervisor { get; set; }

        /// <summary>
        /// Người chỉ huy
        /// </summary>
        public LctLeadershipUserSigningModel Commander { get; set; }

        /// <summary>
        /// Người lảnh đạo
        /// </summary>
        public LctLeadershipUserSigningModel Leader { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }

        /// <summary>
        /// Id ticket hiện tại
        /// </summary>
        public int WorkTicketID { get; set; }

        /// <summary>
        /// Số người tham gia
        /// </summary>
        public string AmountPersons { get; set; }
    }
}
