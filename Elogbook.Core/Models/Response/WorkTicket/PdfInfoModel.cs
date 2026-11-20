namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// thông tin file pdf
    /// </summary>
    public class PdfInfoModel
    {
        /// <summary>
        /// Trạng thái
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// Vị trí chữ ký x
        /// </summary>
        public int? SignatureX { get; set; }

        /// <summary>
        /// Vị trí chữ ký y
        /// </summary>
        public int? SignatureY { get; set; }

        /// <summary>
        /// Thông tin chữ ký
        /// </summary>
        public string ApproverSignature { get; set; }
    }
}
