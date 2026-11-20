using Newtonsoft.Json;
using System;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin nhân viên tập sự
    /// </summary>
    public class ProbationaryStaffModel
    {
        /// <summary>
        /// Mã thông tin nhân viên tập sự
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Mã nhật ký vận hành
        /// </summary>
        public int OperationDiaryID { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public ValueLabelModel OperationPosition { get; set; }

        /// <summary>
        /// Họ và Tên nhân viên tập sự
        /// </summary>
        public ValueLabelModel AssignedUser { get; set; }

        /// <summary>
        /// Chức vụ
        /// </summary>
        public string PositionName { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Ngày tạo dữ liệu
        /// </summary>
        [JsonIgnore]
        public DateTime InsertedAt { get; set; }
    }
}
