using Elogbook.Core.Models.Request;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông chi tiết phiếu đăng ký công tác
    /// </summary>
    public class WorkRegExcelDataValidationModel
    {
        /// <summary>
        /// Danh sách dữ liệu excel của Phiếu đăng ký điện
        /// </summary>
        public List<WorkRegDienExcelDataModel> WorkRegDienExcelDatas { get; set; }

        /// <summary>
        /// Danh sách dữ liệu excel Phiếu đăng ký điện
        /// </summary>
        public List<WorkRegTcnhExcelDataModel> WorkRegTcnhExcelDatas { get; set; }

        /// <summary>
        /// Kết quả validate
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Upload file excel id
        /// </summary>
        public int AttachmentId { get; set; }

        /// <summary>
        /// Nội dung lỗi
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
