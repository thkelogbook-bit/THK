using System;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin tìm kiếm nhánh NKVH
    /// </summary>
    public class QuickSearchOperationDiaryResponseModel
    {
        /// <summary>
        /// Tên ca - Ngày vận hành
        /// </summary>
        public string ShiftDescription { get; set; }

        /// <summary>
        /// Tên nhà máy
        /// </summary>
        public string FactoryName { get; set; }

        /// <summary>
        /// Tiêu đề nội dung nhật ký vận hành
        /// </summary>
        public string HeaderContent { get; set; }

        /// <summary>
        /// Nội dung dữ liệu vận hành
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Số dòng dữ liệu map với điều kiện tìm kiếm
        /// </summary>
        public int TotalRecords { get; set; }

        /// <summary>
        /// Ngày ghi NKVH
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Loại dữ liệu NKVH
        /// </summary>
        public int DiaryType { get; set; }
    }
}
