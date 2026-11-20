namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin nhân viên tập sự
    /// </summary>
    public class MobileProbationaryStaffInfo : MobileEntityBaseModel
    {
        /// <summary>
        /// SyncDataKey
        /// </summary>
        public string SyncDataKey { get; set; }

        /// <summary>
        /// App ID
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// Mã nhật ký vận hành
        /// </summary>
        public int OperationDiaryID { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public int OperationPositionID { get; set; }

        /// <summary>
        /// Mã nhân viên tập sự
        /// </summary>
        public int? AssignedUserID { get; set; }

        /// <summary>
        /// Họ và Tên nhân viên tập sự
        /// </summary>
        public string AssignedUserName { get; set; }

        /// <summary>
        /// Chức vụ
        /// </summary>
        public string PositionName { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Description { get; set; }
    }
}
