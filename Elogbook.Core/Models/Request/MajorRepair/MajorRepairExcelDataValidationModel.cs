using System.Collections.Generic;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông chi tiết phiếu công
    /// </summary>
    public class MajorRepairExcelDataValidationModel
    {
        /// <summary>
        /// Danh sách dữ liệu excel
        /// </summary>
        public List<MajorRepairExcelDataModel> MajorRepairExcelDatas { get; set; }

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
