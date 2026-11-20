namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Giao/nhận ca
    /// </summary>
    public class MobileDeliveryReceiveShiftInfo : MobileEntityBaseModel
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
        /// Xác định cương vị trong ca được khai báo khi ghi nhật ký vận hành
        /// </summary>
        public int OperationDiaryPositionShiftID { get; set; }

        /// <summary>
        /// Trưởng ca/Trưởng kíp/Vận hành viên
        /// </summary>
        public int SupervisorID { get; set; }

        /// <summary>
        /// Xác định người giao ca/nhận ca, true thì người đó giao ca và ngược lại
        /// </summary>
        public bool IsDeliveryShift { get; set; }
    }
}
