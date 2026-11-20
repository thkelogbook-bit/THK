using System;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin nhân sự cho mỗi vị trí theo kế hoạch
    /// </summary>
    public class AssignmentSlotResponseModel
    {
        /// <summary>
        /// Mã vị trí phân công nhân sự
        /// </summary>
        public int AssignmentSlotID { get; set; }

        /// <summary>
        /// Ngày kế hoạch
        /// </summary>
        public DateTime WorkDate { get; set; }

        /// <summary>
        /// Thông tin nhà máy
        /// </summary>
        public ValueLabelModel Factory { get; set; }

        /// <summary>
        /// Thông tin cương vị
        /// </summary>
        public ValueLabelModel OperationPosition { get; set; }

        /// <summary>
        /// Thông tin ca vận hành
        /// </summary>
        public ValueLabelModel OperationShift { get; set; }

        /// <summary>
        /// Lịch sử thay đổi nhân sự
        /// </summary>
        public List<AssignmentEmployeeHistoryInfoModel> ReasonHistories { get; set; }

        /// <summary>
        /// Danh sách nhân sự bố trí
        /// </summary>
        public List<AssignmentEmployeeInfoModel> AssignmentEmployees { get; set; }
    }
}
