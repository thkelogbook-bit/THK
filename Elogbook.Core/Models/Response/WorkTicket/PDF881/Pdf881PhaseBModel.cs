using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin phần B phiếu TCNH
    /// </summary>
    public class Pdf881PhaseBModel
    {
        /// <summary>
        /// Tiêu đề
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Thông tin cho phép
        /// </summary>
        public List<AllowedInfoDetailModel> AllowedInfos { get; set; }

        /// <summary>
        /// Người cho phép
        /// </summary>
        public string AllowerName { get; set; }

        /// <summary>
        /// Chữ ký người cho phép
        /// </summary>
        public string AllowerSignature { get; set; }

        /// <summary>
        /// Thời gian cho phép
        /// </summary>
        public string SignedTime1 { get; set; }

        /// <summary>
        /// Người chỉ huy
        /// </summary>
        public string CommanderName { get; set; }

        /// <summary>
        /// Chữ ký người chỉ huy
        /// </summary>
        public string CommanderSignature { get; set; }

        /// <summary>
        /// Thới gian khóa phiếu
        /// </summary>
        public string SignedTime2 { get; set; }

    }
}
