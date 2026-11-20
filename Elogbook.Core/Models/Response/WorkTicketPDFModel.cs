using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Tham số tạo file template WorkTicket
    /// </summary>
    public class WorkTicketPdfModel
    {
        /// <summary>
        /// Số phiếu công tác
        /// </summary>
        public string WorkTicketNumber { get; set; }

        /// <summary>
        /// Tên đơn vị cấp phiếu
        /// </summary>
        public string ApproveUnitName { get; set; }

        /// <summary>
        /// Người lãnh đạo
        /// </summary>
        public string LearderName { get; set; }

        /// <summary>
        /// Người chỉ huy
        /// </summary>
        public string CommanderName { get; set; }

        /// <summary>
        /// Số người công tác
        /// </summary>
        public string StaffNumber { get; set; }

        /// <summary>
        /// Địa điểm tiến hành công tác
        /// </summary>
        public string WorkPlacement { get; set; }

        /// <summary>
        /// Nội dung công tác
        /// </summary>
        public string WorkContent { get; set; }

        /// <summary>
        /// Thời gian bắt đầu theo kế hoạch
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// Thời gian kết thúc theo kế hoạch
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// Điều kiện cần có để thực hiện công việc
        /// </summary>
        public string WorkCondition { get; set; }

        /// <summary>
        /// Ngày cấp phiếu
        /// </summary>
        public string ApproveDate { get; set; }

        /// <summary>
        /// Tên Người cấp phiếu
        /// </summary>
        public string ApproverName { get; set; }

        /// <summary>
        /// Chức vụ Người cấp phiếu
        /// </summary>
        public string ApproverPosition { get; set; }

        /// <summary>
        /// Chữ ký Người cấp phiếu
        /// </summary>
        public string ApproverSignature { get; set; }


        /// <summary>
        /// Những thiết bị, đường dây, đoạn đường dây đã cắt điện
        /// </summary>
        public string PowerOffDevices { get; set; }

        /// <summary>
        /// Đã tiếp đất tại các vị trí
        /// </summary>
        public string GroundingAtLocations { get; set; }

        /// <summary>
        /// Đã làm rào chắn và treo biển báo
        /// </summary>
        public string FenceAndHangingSign { get; set; }

        /// <summary>
        /// Phạm vi được phép làm việc
        /// </summary>
        public string ScopeOfWorkAllowed { get; set; }

        /// <summary>
        /// Cảnh báo, chỉ dẫn cần thiết
        /// </summary>
        public string WarningsAndInstructions { get; set; }

        /// <summary>
        /// Thời gian cho phép làm việc
        /// </summary>
        public string AllowedTime { get; set; }

        /// <summary>
        /// Tên người cho phép
        /// </summary>
        public string AcceptorName { get; set; }

        /// <summary>
        /// Chức vụ người cho phép
        /// </summary>
        public string AcceptorPosition { get; set; }

        /// <summary>
        /// Chữ ký người cho phép
        /// </summary>
        public string AcceptorSignature { get; set; }

        /// <summary>
        /// Đã kiểm tra những biện pháp an toàn tại hiện trường
        /// </summary>
        public string CheckedSafetyMeasuresAtScene { get; set; }

        /// <summary>
        /// Đã làm thêm các biện pháp an toàn và tiếp đất tại
        /// </summary>
        public string AddedSafetyMeasuresAndGrounding { get; set; }

        /// <summary>
        /// Thời gian bắt đầu tiến hành công việc
        /// </summary>
        public string StartWorkingDate { get; set; }

        /// <summary>
        /// Tên người giám sát
        /// </summary>
        public string SupervisorName { get; set; }

        /// <summary>
        /// Chức vụ người giám sát
        /// </summary>
        public string SupervisorPosition { get; set; }

        /// <summary>
        /// Chữ ký người giám sát
        /// </summary>
        public string SupervisorSignature { get; set; }

        /// <summary>
        /// Thời gian kết thúc công tác - trao trả nơi làm việc
        /// </summary>
        public string FinishTime { get; set; }

        /// <summary>
        /// Thời gian tiếp nhận nơi làm việc, kiểm tra khóa phiếu
        /// </summary>
        public string CheckTime { get; set; }

        /// <summary>
        /// Thời gian hoàn thành phiếu công tác
        /// </summary>
        public string TicketLockTime { get; set; }

        /// <summary>
        /// Danh sách Nhân viên đơn vị công tác
        /// </summary>
        public List<PdfStaffInfoModel> StaffInfo { get; set; }

        /// <summary>
        ///  Cho phép làm việc và kết thúc công tác hàng ngày, di chuyển nơi làm việc
        /// </summary>
        public List<PdfWorkPlacementModel> WorkPlacementInfo { get; set; }
    }
}
