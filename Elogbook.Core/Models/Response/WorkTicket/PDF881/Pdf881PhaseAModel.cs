using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin phần A phiếu TCNH
    /// </summary>
    public class Pdf881PhaseAModel
    {
        /// <summary>
        /// Tiêu đề
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Vị trí làm việc
        /// </summary>
        public string WorkLocaltion { get; set; }

        /// <summary>
        /// Nội dung công việc
        /// </summary>
        public string WorkContent { get; set; }

        /// <summary>
        /// Phạm vi công việc
        /// </summary>
        public string WorkScope { get; set; }

        /// <summary>
        /// Thời gian bắt đầu
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// Thời gian kết thúc
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// Tiêu chí an toàn
        /// </summary>
        public List<WarnAndSafeModel> WarnAndSafes { get; set; }

        /// <summary>
        /// Người phê duyệt
        /// </summary>
        public string ApproverName { get; set; }

        /// <summary>
        /// Chữ ký người phê duyệt
        /// </summary>
        public string ApproverSignature { get; set; }

        /// <summary>
        /// Thời gian phê duyệt
        /// </summary>
        public string ApproveTime { get; set; }
    }
}
