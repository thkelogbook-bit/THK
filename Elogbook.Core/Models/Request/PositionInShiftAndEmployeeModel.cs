using System;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thông tin tìm kiếm danh sách cương vị trong ca và nhân sự phân bổ
    /// </summary>
    public class PositionInShiftAndEmployeeModel
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public int OperationPositionID { get; set; }

        /// <summary>
        /// Ca vận hành
        /// </summary>
        public int OperationShiftID { get; set; }

        /// <summary>
        /// Ngày vận hành
        /// </summary>
        public DateTime WorkDate { get; set; }
    }
}
