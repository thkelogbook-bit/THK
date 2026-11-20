using Newtonsoft.Json;
using System;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Các tham số để thêm thông số vận hành theo khung giờ
    /// </summary>
    public class AddOperationParameterByTimeModel
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }
        /// <summary>
        /// Chu kỳ
        /// </summary>
        public int CycleID { get; set; }
        /// <summary>
        /// Cương vị
        /// </summary>
        public int OperationPositionID { get; set; }

        /// <summary>
        /// Khung giờ
        /// </summary>
        public int OperationTimeLineID { get; set; }

        /// <summary>
        /// Ngày tạo thông số
        /// </summary>
        public DateTime DateCreate { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
