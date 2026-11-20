using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// mô tả model chi tiết báo cáo thời gian xử lý bất thường
    /// </summary>
    public class GroupDataDetailTimeHandleModel
    {
        /// <summary>
        /// mã số
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// ngày có hoạt động
        /// </summary>
        public DateTime DateActivity { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public string Factory { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public string OperationPosition { get; set; }

        /// <summary>
        /// Thiết bị
        /// </summary>
        public ValueLabelCheckedModel Device { get; set; }

        /// <summary>
        /// lịch công tác
        /// </summary>
        public ValueLabelCheckedModel AmMaintenanceSchedule { get; set; }

        /// <summary>
        /// Ca
        /// </summary>
        public string OperationShift { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public string Status { get; set; } = "Đã tạo PYC";

        /// <summary>
        /// Trạng thái xử lý
        /// </summary>
        public string HandleStatus { get; set; } = "Đã hoàn thành sửa chữa";

        /// <summary>
        /// người thực hiện
        /// </summary>
        public string ActualUser { get; set; }

        /// <summary>
        /// ngày thực hiện
        /// </summary>
        public DateTime? ActualDate { get; set; }
    }
}
