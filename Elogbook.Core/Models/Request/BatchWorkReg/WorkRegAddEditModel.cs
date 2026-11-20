using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số để thêm mới phiếu đăng ký
    /// </summary>
    public class WorkRegAddEditModel
    {
        /// <summary>
        /// Mã số Phiếu đăng ký công tác
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Phân loại màn hình (1:Điện, 2:Thủy cơ nhiệt hóa)
        /// </summary>
        public int ScreenType { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Thời gian tạo phiếu đăng ký công tác
        /// </summary>
        public DateTime PlanCreateTicketTime { get; set; } = DateTime.MinValue;

        /// <summary>
        /// ID file excel đã được import
        /// </summary>
        public int AttachmentID { get; set; }

        /// <summary>
        /// Danh sách dữ liệu excel của Phiếu đăng ký điện đã validate
        /// </summary>
        public List<WorkRegDienExcelDataModel> WorkRegDienExcelDatas { get; set; }

        /// <summary>
        /// Danh sách dữ liệu excel Phiếu đăng ký điện đã validate
        /// </summary>
        public List<WorkRegTcnhExcelDataModel> WorkRegTcnhExcelDatas { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
