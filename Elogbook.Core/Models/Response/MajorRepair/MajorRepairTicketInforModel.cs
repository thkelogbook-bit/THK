using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin chi tiết phiếu (SCL)
    /// </summary>
    public class MajorRepairTicketInforModel
    {
        /// <summary>
        /// Tổng số phiếu
        /// </summary>
        public MRValueLabelModel TotalAll { get; set; }

        /// <summary>
        /// Tổng số phiếu chờ đăng ký( là trạng thái phiếu đã được Cấp hàng loạt lúc tạo mới đợt sửa chữa lớn)
        /// </summary>
        public MRValueLabelModel TotalWaitForRegist { get; set; }

        /// <summary>
        /// Tổng số phiếu chờ cấp phiếu công tác( là trạng thái phiếu đã được đăng ký và đang chờ cấp duyệt)
        /// </summary>
        public MRValueLabelModel TotalWaiting { get; set; }

        /// <summary>
        /// Tổng số phiếu đã cấp( là trạng thái phiếu đã được Cấp)
        /// </summary>
        public MRValueLabelModel TotalApproved { get; set; }

        /// <summary>
        /// Tổng số phiếu đã cho phép thực hiện công tác
        /// </summary>
        public MRValueLabelModel TotalAccepted { get; set; }

        /// <summary>
        /// Tổng số phiếu đang thực hiện công tác
        /// </summary>
        public MRValueLabelModel TotalInProgress { get; set; }

        /// <summary>
        /// Tổng số phiếu chờ xác nhận hoàn thành
        /// </summary>
        public MRValueLabelModel TotalWaitForConfirmation { get; set; }

        /// <summary>
        /// Tổng số phiếu đã hoàn thành
        /// </summary>
        public MRValueLabelModel TotalCompleted { get; set; }

        /// <summary>
        /// Tổng số phiếu đã hủy
        /// </summary>
        public MRValueLabelModel TotalAnnulled { get; set; }

        /// <summary>
        /// Danh sách Phiếu Công tác
        /// </summary>
        public List<WorkTicketModel> WorkTickets { get; set; }
    }
}
