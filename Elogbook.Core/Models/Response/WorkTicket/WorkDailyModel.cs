using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Công tác hàng ngày. Phiếu thủy cơ nhiệt
    /// </summary>
    public class WorkDailyModel
    {
        /// <summary>  ID </summary>
        public int ID { get; set; }

        /// <summary> Vị trí </summary>
        public int? WorkLocationID { get; set; }

        /// <summary>Vị trí dạng text</summary>
        public string WorkLocationText { get; set; }

        /// <summary>Ngày làm việc</summary>
        public DateTime WorkDay { get; set; }

        /// <summary>Danh sách người tham gia</summary>
        public List<StaffInformationsModel> StaffInformations { get; set; }

    }
}
