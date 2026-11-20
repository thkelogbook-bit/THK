namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin kết thúc làm việc
    /// </summary>
    public class PdfWorkFinishModel
    {
        /// <summary>
        /// Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Nội dung tiêu đề
        /// </summary>
        public string TitleContent { get; set; }

        /// <summary>
        /// Tên Người chỉ huy
        /// </summary>
        public string CommanderName { get; set; }

        /// <summary>
        /// Chữ ký Người chỉ huy
        /// </summary>
        public string CommanderSignature { get; set; }

        /// <summary>
        /// Tên Người lãnh đạo công việc
        /// </summary>
        public string LeaderName { get; set; }

        /// <summary>
        /// Chữ ký Người lãnh đạo công việc
        /// </summary>
        public string LeaderSignature { get; set; }

        /// <summary>
        /// Thời gian tiếp nhận nơi làm việc, kiểm tra khóa phiếu
        /// </summary>
        public string CheckReceivedContent { get; set; }

        /// <summary>
        /// Tên người cho phép
        /// </summary>
        public string AllowerName { get; set; }

        /// <summary>
        /// Chữ ký người cho phép
        /// </summary>
        public string AllowerSignature { get; set; }

        /// <summary>
        /// Thời gian hoàn thành phiếu công tác
        /// </summary>
        public string CheckCompletedContent { get; set; }

        /// <summary>
        /// Tên người cấp phiếu
        /// </summary>
        public string ApproverName { get; set; }

        /// <summary>
        /// Chữ ký người cấp phiếu
        /// </summary>
        public string ApproverSignature { get; set; }

        /// <summary>
        /// Tên người xác nhận hoàn thành
        /// </summary>
        public string CompleteConfirmName { get; set; }

        /// <summary>
        /// Chữ ký người xác nhận hoàn thành
        /// </summary>
        public string CompleteConfirmSignature { get; set; }
    }
}
