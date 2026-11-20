using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request.WorkTicket
{
    /// <summary>
    /// Công tác hằng ngày. Phiếu thủy cơ nhiệt
    /// </summary>
    public class AddWorkDailyMobileModel
    {
        /// <summary>
        /// Mã định danh nếu cập nhật
        /// </summary>
        public int? ID { get; set; }

        /// <summary>
        /// Id phiếu công tác
        /// </summary>
        public int WorkTicketID { get; set; }

        /// <summary>
        /// Vị trí
        /// </summary>
        public int WorkLocationID { get; set; }

        /// <summary>
        /// Vị trí dạng text
        /// </summary>
        public string WorkLocationText { get; set; }

        /// <summary>
        /// Ngày
        /// </summary>
        public DateTime WorkDay { get; set; }

        /// <summary>
        /// danh sách nhân viên công tác
        /// </summary>
        public List<AddStaffInformationModel> StaffInformations { get; set; }

        /// <summary>
        /// Người đang thực hiện yêu cầu
        /// </summary>
        /// 
        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
