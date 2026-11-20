namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Tình hình vận hành
    /// </summary>
    public class MobileOperationSituationInfo : MobileEntityBaseModel
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
        /// Giờ ghi dữ liệu
        /// </summary>
        public double Hour { get; set; }

        /// <summary>
        /// Người ra lệnh
        /// </summary>
        public int? AssignedFromID { get; set; }

        /// <summary>
        /// Người ra lệnh ngoài hệ thống
        /// </summary>
        public string AssignedFromName { get; set; }

        /// <summary>
        /// Người nhận lệnh
        /// </summary>
        public int? AssignedToID { get; set; }
        /// <summary>
        /// Người nhận lệnh ngoài hệ thống
        /// </summary>
        public string AssignedToName { get; set; }

        /// <summary>
        /// Tình hình vận hành và các thao tác trong ca
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Danh sách mã số file đính kèm
        /// </summary>
        public string Attachments { get; set; }
    }
}
