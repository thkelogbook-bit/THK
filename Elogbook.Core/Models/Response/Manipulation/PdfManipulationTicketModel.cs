namespace Elogbook.Core.Models.Response.Manipulation
{
    /// <summary>
    /// Thông tin cần thiết để tạo file PDF
    /// </summary>
    public class PdfManipulationTicketModel
    {
        /// <summary>
        /// Phiếu công tác
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Công ty
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// Phân xưởng
        /// </summary>
        public string RegistrationUnit { get; set; }

        /// <summary>
        /// Số phiếu
        /// </summary>
        public string TicketNumber { get; set; }

        /// <summary>
        /// Số trang
        /// </summary>
        public string PageNo { get; set; }

        /// <summary>
        /// Tên phiếu
        /// </summary>
        public string TicketName { get; set; }

        /// <summary>
        /// tên người tạo phiếu
        /// </summary>
        public string CreatedUserName { get; set; }

        /// <summary>
        /// chức vụ người tạo phiếu
        /// </summary>
        public string CreatedUserPosition { get; set; }

        /// <summary>
        /// tên người duyệt phiếu
        /// </summary>
        public string ApprovedUserName { get; set; }

        /// <summary>
        /// chức vụ người duyệt phiếu
        /// </summary>
        public string ApprovedUserPosition { get; set; }

        /// <summary>
        /// tên người giám sát
        /// </summary>
        public string SupervisorUserName { get; set; }

        /// <summary>
        /// chức vụ người giám sát
        /// </summary>
        public string SupervisorUserPosition { get; set; }

        /// <summary>
        /// tên người thao tác
        /// </summary>
        public string WorkerUserName { get; set; }

        /// <summary>
        /// chức vụ người thao tác
        /// </summary>
        public string WorkerUserPosition { get; set; }

        /// <summary>
        /// Mục đích
        /// </summary>
        public string Purpose { get; set; }

        /// <summary>
        /// 3. Thời gian dự kiến (bắt đầu)
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// 3. Thời gian dự kiến (kết thúc)
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// 4. Đơn vị đề nghị thao tác
        /// </summary>
        public string RecommendedUnit { get; set; }

        /// <summary>
        /// Điều kiện cần có để thực hiện
        /// </summary>
        public string ConditionRequest { get; set; }

        /// <summary>
        /// Lưu ý
        /// </summary>
        public string NoteRequest { get; set; }

        /// <summary>
        /// Giao nhận, nghiệm thu đường dây, thiết bị điện trước khi thao tác: 
        /// </summary>
        public string DeliveryAccepDevicesBefore { get; set; }

        /// <summary>
        /// Trình tự hạng mục thao tác
        /// </summary>
        public string SequenceOperations { get; set; }

        /// <summary>
        /// Giao nhận, nghiệm thu đường dây, thiết bị điện sau thao tác: 
        /// </summary>
        public string DeliveryAccepDeviceseAfter { get; set; }

        /// <summary>
        /// thời gian người viết phiếu ký
        /// </summary>
        public string SignTime1 { get; set; }

        /// <summary>
        /// thời gian người duyệt phiếu ký
        /// </summary>
        public string SignTime2 { get; set; }

        /// <summary>
        /// Các sự kiện bất thường trong thao tác:
        /// </summary>
        public string CaseAbnormal { get; set; }

        /// <summary>
        /// Ký ghi rõ họ tên
        /// </summary>
        public string TitleSign { get; set; }

        /// <summary>
        /// người viết phiếu ký
        /// </summary>
        public string SignTitle1 { get; set; }

        /// <summary>
        /// người duyệt phiếu ký
        /// </summary>
        public string SignTitle2 { get; set; }
    }
}
