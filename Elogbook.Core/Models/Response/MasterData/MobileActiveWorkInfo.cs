namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Công tác đang thực hiện
    /// </summary>
    public class MobileActiveWorkInfo : MobileEntityBaseModel
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
        /// Giờ, phút bắt đầu
        /// </summary>
        public double StartHour { get; set; }

        /// <summary>
        /// Giờ, phút kết thúc
        /// </summary>
        public double EndHour { get; set; }

        /// <summary>
        /// Công tác đang thực hiện
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Danh sách mã số file đính kèm
        /// </summary>
        public string Attachments { get; set; }
    }
}
