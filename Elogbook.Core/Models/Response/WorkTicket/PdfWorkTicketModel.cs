using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Tham số tạo file template WorkTicket
    /// </summary>
    public class PdfWorkTicketModel
    {
        /// <summary>
        /// Thông tin tiêu đề của file PDF
        /// </summary>
        public PdfHeaderWorkTicketModel PdfHeaderWorkTicket { get; set; }

        /// <summary>
        /// Cấp cho
        /// </summary>
        public PdfApproveInfoModel PdfApproveInfo { get; set; }

        /// <summary>
        /// Thủ tục cho phép công tác
        /// </summary>
        public PdfAllowedInfoModel PdfAllowedInfo { get; set; }

        /// <summary>
        /// Tiếp nhận nơi làm việc
        /// </summary>
        public PdfReceiveWorkplaceModel PdfReceiveWorkplace { get; set; }

        /// <summary>
        /// Danh sách nhân viên đơn vị công tác
        /// </summary>
        public List<PdfStaffInfoModel> PdfStaffInfos { get; set; }

        /// <summary>
        /// Cho phép làm việc và kết thúc công tác hằng ngày, di chuyên nơi làm việc
        /// </summary>
        public List<PdfWorkPlacementModel> PdfWorkPlacement { get; set; }

        /// <summary>
        /// Kết thúc công tác
        /// </summary>
        public PdfWorkFinishModel PdfWorkFinish { get; set; }

        /// <summary>
        /// Kết quả công tác
        /// </summary>
        public string WorkResult { get; set; }
    }
}
