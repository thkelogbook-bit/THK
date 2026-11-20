using System;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Tồn tại trong ca
    /// </summary>
    public class MobileExistInShiftInfo : MobileEntityBaseModel
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int? FactoryID { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public int? OperationPositionID { get; set; }

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
        /// Phiếu công tác
        /// </summary>
        public int? RegistrationTicketID { get; set; }

        /// <summary>
        /// Tên phiếu công tác
        /// </summary>
        public string RegistrationTicketName { get; set; }

        /// <summary>
        /// Nhóm trưởng
        /// </summary>
        public string TeamLeader { get; set; }

        /// <summary>
        /// Khu vực
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// Nội dung tồn tại
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// File đính kèm
        /// </summary>
        public string Attachments { get; set; }

        /// <summary>
        /// Trạng thái của tồn tại trong ca
        /// </summary>
        public int StatusID { get; set; }

        /// <summary>
        /// Xóa dữ liệu khi theo dõi tồn tại trong ca
        /// </summary>
        public bool IsDeleteIssueOfShift { get; set; }

        /// <summary>
        /// Ngày hoàn thành
        /// </summary>
        public DateTime? CompletedDateTime { get; set; }
    }
}
