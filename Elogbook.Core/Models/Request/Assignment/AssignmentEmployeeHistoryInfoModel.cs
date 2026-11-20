namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Lịch sử thay đổi nhân sự
    /// </summary>
    public class AssignmentEmployeeHistoryInfoModel : EntityBaseModel
    {
        /// <summary>
        /// Vị trí phân công nhân sự
        /// </summary>
        public int AssignmentSlotID { get; set; }

        /// <summary>
        /// Nhân sự cũ
        /// </summary>
        public int? OldUserID { get; set; }

        /// <summary>
        /// Nhân sự mới
        /// </summary>
        public int? NewUserID { get; set; }

        /// <summary>
        /// Loại hành động
        /// </summary>
        public int ActionType { get; set; }

        /// <summary>
        /// Lý do
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int? Active { get; set; }

        /// <summary>
        /// Người thực hiện
        /// </summary>
        public int? CreatedUserID { get; set; }
    }
}
