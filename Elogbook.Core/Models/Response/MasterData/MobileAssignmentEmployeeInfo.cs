namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin nhân sự được phân công
    /// </summary>
    public class MobileAssignmentEmployeeInfo : MobileEntityBaseModel
    {
        /// <summary>
        /// Mã vị trí phân công nhân sự
        /// </summary>
        public int AssignmentSlotID { get; set; }

        /// <summary>
        /// Mã nhân sự
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int? Active { get; set; }
    }
}
