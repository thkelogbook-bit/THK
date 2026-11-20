using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// khai báo response tổng quản báo cáo tuân thủ hoạt động am
    /// </summary>
    public class GeneralOperationalComplianceModel
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public ComboboxDataModel Factory { get; set; }

        /// <summary>
        /// tháng
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// trạng thái
        /// </summary>
        public ValueLabelModel Status { get; set; }
    }
}
