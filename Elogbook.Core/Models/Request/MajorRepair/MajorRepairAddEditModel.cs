using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số để thêm mới thông tin sửa chữa lớn
    /// </summary>
    public class MajorRepairAddEditModel
    {
        /// <summary>
        /// Mã số sửa chữa lớn
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Tên đợt
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Kế hoạch thực hiện (bắt đầu)
        /// </summary>
        public DateTime StartDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Kế hoạch thực hiện (kết thúc)
        /// </summary>
        public DateTime EndDate { get; set; } = DateTime.MaxValue;

        /// <summary>
        /// Thời gian tạo phiếu công tác
        /// </summary>
        public DateTime PlanCreateTicketTime { get; set; } = DateTime.MinValue;

        /// <summary>
        /// ID file excel đã được import
        /// </summary>
        public int AttachmentID { get; set; }

        /// <summary>
        /// Dữ liệu file excel đã validate
        /// </summary>
        public List<MajorRepairExcelDataModel> MajorRepairExcelDatas {get;set;}

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }

    /// <summary>
    /// Các tham số để thêm mới phiếu vào đợt sửa chữa lớn
    /// </summary>
    public class InsertNewTicketModel
    {
        /// <summary>
        /// Mã số sửa chữa lớn
        /// </summary>
        public int MajorRepairID { get; set; }

        /// <summary>
        /// Id file data mới
        /// </summary>
        public int AttachmentID { get; set; }

        /// <summary>
        /// Dữ liệu file excel đã validate
        /// </summary>
        public List<MajorRepairExcelDataModel> MajorRepairExcelDatas { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
