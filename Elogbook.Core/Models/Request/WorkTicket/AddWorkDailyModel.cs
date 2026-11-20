using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Công tác hằng ngày
    /// </summary>
    public class AddWorkDailyModel
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
        public List<AddStaffInformationModel> StaffInformations { get; set; }
    }
}
