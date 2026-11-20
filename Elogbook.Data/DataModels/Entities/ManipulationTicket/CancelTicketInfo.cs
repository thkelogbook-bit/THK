namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Table lưu dữ liệu thông tin hủy phiếu thao tác
    /// </summary>
    [Table("CancelTicketInfo")]
    public partial class CancelTicketInfo : EntityBase
    {
        /// <summary>
        /// App ID
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// Mã phiếu thao tác được cấp
        /// </summary>
        public int TicketID { get; set; }

        /// <summary>
        /// Ngày đồng bộ cuối
        /// </summary>
        public DateTime SyncedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Lý do hủy phiếu
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Danh sách hình ảnh/videos hủy phiếu
        /// </summary>
        [StringLength(500)]
        public string ReferAttachmentIDs { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        [MapperIgnore]
        [ForeignKey(nameof(CreatedUserID))]
        public virtual User CreatedUser { get; set; }

        /// <summary>
        /// Người cập nhật
        /// </summary>
        [MapperIgnore]
        [ForeignKey(nameof(LastUpdatedUserID))]
        public virtual User LastUpdatedUser { get; set; }
    }
}
