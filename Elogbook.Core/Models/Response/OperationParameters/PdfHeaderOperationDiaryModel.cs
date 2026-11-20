namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin tồn tại trong ca
    /// </summary>
    public class PdfHeaderOperationDiaryModel
    {
        /// <summary>
        /// Tên cương vị
        /// </summary>
        public string OperationPositionName { get; set; }

        /// <summary>
        /// Tên công ty
        /// </summary>
        public string FactoryName { get; set; }

        /// <summary>
        /// Mục ISO
        /// </summary>
        public string ItemISO { get; set; }

        /// <summary>
        /// Ban hành lần thứ
        /// </summary>
        public string VersionRelease { get; set; }

        /// <summary>
        /// Ngày có hiệu lực
        /// </summary>
        public string EffectiveDate { get; set; }

        /// <summary>
        /// Mã tài liệu
        /// </summary>
        public string DocumentCode { get; set; }

        /// <summary>
        /// Trang số
        /// </summary>
        public string PageNumber { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public string CreateDate { get; set; }

        /// <summary>
        /// Tên ca vận hành đang ghi nhật ký
        /// </summary>
        public string OperationShiftName { get; set; }

        /// <summary>
        /// Path file logo công ty
        /// </summary>
        public string PathLogo { get; set; }
    }
}
