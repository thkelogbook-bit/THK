using Elogbook.Core.Models.Request;
using Newtonsoft.Json;
using System;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin phiếu thao tác
    /// </summary>
    public class UpdateSequenceOperationModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Ngày bắt đầu
        /// </summary>
        public DateTime BeginDate { get; set; }

        /// <summary>
        /// Ngày kết thúc
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 1.1 Người ra lệnh
        /// </summary>
        public UserInfoInTicketModel LeaderUser { get; set; }

        /// <summary>
        /// 1.2 Người nhận lệnh
        /// </summary>
        public UserInfoInTicketModel WorkerUser { get; set; }

        /// <summary>
        /// IsDone
        /// </summary>
        public bool IsDone { get; set; }

        /// <summary>
        /// Mã số step cha
        /// </summary>
        public int ParentStep { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }

    }
}
