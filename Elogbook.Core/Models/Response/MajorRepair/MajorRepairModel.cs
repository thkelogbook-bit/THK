using Elogbook.Core.Constants;
using Elogbook.Core.Models.Request;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Chi tiết thông tin sửa chữa lớn
    /// </summary>
    public class MajorRepairModel
    {
        /// <summary>
        /// Mã số thông tin sửa chữa lớn
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public ValueLabelCheckedModel Factory { get; set; }

        /// <summary>
        /// Tên đợt sửa chữa lớn
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public ValueLabelCheckedModel MajorRepairStatus { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Kế hoạch thực hiên (bắt đầu)
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Kế hoạch thực hiên (kết thúc)
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Thời gian tạo phiếu công tác
        /// </summary>
        public DateTime PlanCreateTicketTime { get; set; }

        /// <summary>
        /// File đính kèm
        /// </summary>
        public List<ExcelDataFileInfo> AttachmentExcelFile { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        public ValueLabelCheckedModel CreaterUser { get; set; }

        /// <summary>
        /// Thông tin phiếu công tác điện
        /// </summary>
        public MajorRepairTicketInforModel PCTDienInfo { get; set; }

        /// <summary>
        /// Thông tin phiếu công tác Thủy Cơ Nhiệt Hóa
        /// </summary>
        public MajorRepairTicketInforModel PCTTCNHInfo { get; set; }

        /// <summary>
        /// Danh sách dữ liệu excel
        /// </summary>
        public List<MajorRepairExcelDataModel> MajorRepairExcelDatas { get; set; }

        /// <summary>
        /// Id file đính kèm cuối cùng
        /// </summary>
        public int LastAttachmentId { get; set; }

        /// <summary>
        /// Tổng số phiếu
        /// </summary>
        public int? TotalTicket { get; set; }

        /// <summary>
        /// Tổng số phiếu thành công
        /// </summary>
        public int? TotalSuccess { get; set; }

        /// <summary>
        /// Danh sách file excel id
        /// </summary>
        [JsonIgnore]
        public string ExcelAttIds { get; set; }

        /// <summary>
        /// Path file đính kèm
        /// </summary>
        [JsonIgnore]
        public string ExcelPath { get; set; }

        /// <summary>
        /// Lịch sử thao tác
        /// </summary>
        public List<MajorRepairHistoryModel> MajorRepairHistories { get; set; }
    }

    /// <summary>
    /// Thông tin file dữ liệu đã import
    /// </summary>
    public class ExcelDataFileInfo
    {
        /// <summary>
        /// Id file
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// File Path
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// File Name
        /// </summary>
        public string FileName { get; set; }
    }
}
