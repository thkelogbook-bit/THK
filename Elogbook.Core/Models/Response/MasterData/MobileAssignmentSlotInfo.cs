using System;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Vị trí phân công nhân sự
    /// </summary>
    public class MobileAssignmentSlotInfo : MobileEntityBaseModel
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// Cương vị (OperationPosition)
        /// </summary>
        public int OperationPositionID { get; set; }

        /// <summary>
        /// Ngày làm việc
        /// </summary>
        public DateTime WorkDate { get; set; }

        /// <summary>
        /// Ca vận hành
        /// </summary>
        public int OperationShiftID { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int? Active { get; set; }
    }
}
