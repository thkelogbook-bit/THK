using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Đối tượng phân nhóm dữ liệu
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseGroupDataModel<T>
    {
        /// <summary>
        /// Danh sách dữ liệu của nhóm
        /// </summary>
        public List<T> MasterDetail { get; set; }

        /// <summary>
        /// Ngày phân nhóm dữ liệu
        /// </summary>
        public string GroupDate { get; set; }

        /// <summary>
        /// Mã tài liệu
        /// </summary>
        public string ProcessDocumentCode { get; set; }
    }
}
