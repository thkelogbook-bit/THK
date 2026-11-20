using Newtonsoft.Json;
using System;

namespace Elogbook.Core.Models.Request.WorkPermit
{
    /// <summary>
    /// 
    /// </summary>
    public class AddEditWorkProcessSafetyMobileModel
    {
        /// <summary>
        /// Lệnh công tác
        /// </summary>
        public int? WorkTicketID { get; set; }

        /// <summary>
        /// Id
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Trình tự công việc
        /// </summary>
        public string ProcedureStep { get; set; }

        /// <summary>
        /// Điều kiện an toàn
        /// </summary>
        public string SafeCondition { get; set; }

        /// <summary>
        /// TThời gian bắt đầu xác nhận làm việc của người chỉ huy trực tiếp
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// Thời gian kết thúc xác nhận làm việc của người chỉ huy trực tiếp
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }

    }
}
