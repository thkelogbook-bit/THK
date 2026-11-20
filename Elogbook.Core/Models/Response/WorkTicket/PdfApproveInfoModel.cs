namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin cấp cho
    /// </summary>
    public class PdfApproveInfoModel
    {
        /// <summary>
        /// Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Người lãnh đạo công việc
        /// </summary>
        public string WorkLeader { get; set; }

        /// <summary>
        /// Người chỉ huy trực tiếp
        /// </summary>
        public string WorkCommander { get; set; }

        /// <summary>
        /// Nhân viên đơn vị công tác
        /// </summary>
        public string UnitNumber { get; set; }

        /// <summary>
        /// Địa điểm tiến hành công tác
        /// </summary>
        public string WorkPlacement { get; set; }

        /// <summary>
        /// Nội dung công tác
        /// </summary>
        public string WorkContent { get; set; }

        /// <summary>
        /// tiêu đề thời gian
        /// </summary>
        public string TitleTime { get; set; }

        /// <summary>
        /// Thời gian bắt đầu theo kết hoạch
        /// </summary>
        public string EstimatedStartTime { get; set; }

        /// <summary>
        /// Thời gian kết thúc theo kết hoạch
        /// </summary>
        public string EstimatedEndTime { get; set; }

        /// <summary>
        /// Điều kiện tiến hành công tác
        /// </summary>
        public string WorkCondition { get; set; }

        /// <summary>
        /// Ngày cấp phiếu
        /// </summary>
        public string ApprovedDate { get; set; }

        /// <summary>
        /// Người cấp phiếu
        /// </summary>
        public string ApproverUser { get; set; }

        /// <summary>
        /// Chức vụ người cấp phiếu
        /// </summary>
        public string ApproverPosition { get; set; }

        /// <summary>
        /// Chữ ký người cấp phiếu
        /// </summary>
        public string ApproverSignature { get; set; }
    }
}
