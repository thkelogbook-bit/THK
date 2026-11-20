using System.Collections.Generic;

namespace Elogbook.Core.Models.Request.WorkPermit
{
    /// <summary>
    /// Công tác hằng ngày
    /// </summary>
    public class LctAddWorkDailyModel
    {
        /// <summary>
        /// Vị trí
        /// </summary>
        public int? WorkLocationID { get; set; }

        /// <summary>
        /// Ngày
        /// </summary>
        public string WorkDay { get; set; }

        /// <summary>
        /// Ngày
        /// </summary>
        public string WorkLocationText { get; set; }

        /// <summary>
        /// danh sách nhân viên công tác
        /// </summary>
        public List<LctAddStaffInformationModel> StaffInformations { get; set; }
    }
}
