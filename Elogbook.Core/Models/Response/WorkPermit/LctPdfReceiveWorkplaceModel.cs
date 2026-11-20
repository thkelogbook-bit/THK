namespace Elogbook.Core.Models.Response.WorkPermit
{
    /// <summary>
    /// Thông tin tiếp nhận nơi làm việc
    /// </summary>
    public class LctPdfReceiveWorkplaceModel
    {
        /// <summary>
        /// 3. Tiếp nhận nơi làm việc
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Đã kiểm tra những biện pháp an toàn
        /// </summary>
        public string CheckedSafetyMeasures { get; set; }

        /// <summary>
        /// Đã làm thêm các biện pháp an toàn
        /// </summary>
        public string AddedSafetyMeasures { get; set; }

        /// <summary>
        /// Bắt đầu tiến hành công tác
        /// </summary>
        public string StartWorkingTime { get; set; }

        /// <summary>
        /// Người lãnh đạo
        /// </summary>
        public string LeaderName { get; set; }

        /// <summary>
        /// Chức vụ Người lãnh đạo
        /// </summary>
        public string LeaderPosition { get; set; }

        /// <summary>
        /// Chữ ký Người lãnh đạo
        /// </summary>
        public string LeaderSignature { get; set; }

        /// <summary>
        /// Người chỉ huy
        /// </summary>
        public string CommanderName { get; set; }

        /// <summary>
        /// Chức vụ Người chỉ huy
        /// </summary>
        public string CommanderPosition { get; set; }


        /// <summary>
        /// Chữ ký Người chỉ huy
        /// </summary>
        public string CommanderSignature { get; set; }

        /// <summary>
        /// Người giám sát
        /// </summary>
        public string SupervisorName { get; set; }

        /// <summary>
        /// Chức vụ Người giám sát
        /// </summary>
        public string SupervisorPosition { get; set; }

        /// <summary>
        /// Chữ ký Người giám sát
        /// </summary>
        public string SupervisorSignature { get; set; }
    }
}
