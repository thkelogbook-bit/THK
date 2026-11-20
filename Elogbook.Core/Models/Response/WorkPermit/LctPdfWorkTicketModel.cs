using System.Collections.Generic;

namespace Elogbook.Core.Models.Response.WorkPermit
{
    /// <summary>
    /// Tham số tạo file template WorkTicket
    /// </summary>
    public class LctPdfWorkTicketModel
    {
        /// <summary>
        /// Thông tin tiêu đề của file PDF
        /// </summary>
        public LctPdfHeaderWorkTicketModel PdfHeaderWorkTicket { get; set; }

        /// <summary>
        /// Cấp cho
        /// </summary>
        public LctPdfApproveInfoModel PdfApproveInfo { get; set; }

        /// <summary>
        /// Thủ tục cho phép công tác
        /// </summary>
        //public LctPdfAllowedInfoModel PdfAllowedInfo { get; set; }

        /// <summary>
        /// Tiếp nhận nơi làm việc
        /// </summary>
        public LctPdfReceiveWorkplaceModel PdfReceiveWorkplace { get; set; }

        /// <summary>
        /// Danh sách nhân viên đơn vị công tác
        /// </summary>
        public List<LctPdfStaffInfoModel> PdfStaffInfos { get; set; }

        /// <summary>
        /// Cho phép làm việc và kết thúc công tác hằng ngày, di chuyên nơi làm việc
        /// </summary>
        public List<LctPdfWorkPlacementModel> PdfWorkPlacement { get; set; }

        /// <summary>
        /// Kết thúc công tác
        /// </summary>
        public LctPdfWorkFinishModel PdfWorkFinish { get; set; }

        /// <summary>
        /// Kết quả công tác
        /// </summary>
        public string WorkResult { get; set; }
    }
}
