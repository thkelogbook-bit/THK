namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Phương thức giao ca
    /// </summary>
    public class MobileDeliveryShiftMethodInfo : MobileEntityBaseModel
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
        /// 3.Phương thức giao ca
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Danh sách mã số file đính kèm
        /// </summary>
        public string Attachments { get; set; }
    }
}
