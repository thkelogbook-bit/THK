using Elogbook.Core.Models.BaseFilter;
using System;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số tìm kiếm Khung giờ
    /// </summary>
    public class ExportExcelRequestModel : BaseListFilter
    {
        /// <summary>
        /// Tên file
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public int OperationPositionID { get; set; }

        /// <summary>
        /// Chu kỳ
        /// </summary>
        public int CycleID { get; set; }

        /// <summary>
        /// Từ ngày
        /// </summary>
        public string FromDate { get; set; }

        /// <summary>
        /// Đến ngày
        /// </summary>
        public string ToDate { get; set; }

        /// <summary>
        /// Loại template xuất dữ liệu
        /// </summary>
        public int TemplateType { get; set; }

        /// <summary>
        /// Loại thời gian
        /// </summary>
        public int DateType { get; set; }

        /// <summary>
        /// Year
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// Quý trong năm
        /// </summary>
        public int? Quarter { get; set; }

        /// <summary>
        /// Tháng/năm ghi thông số
        /// </summary>
        public DateTime? Month { get; set; }
    }
}
