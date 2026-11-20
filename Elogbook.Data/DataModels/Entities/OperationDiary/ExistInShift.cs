namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ExistInShift")]
    public partial class ExistInShift : EntityBase
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int? FactoryID { get; set; }

        /// <summary>
        /// Mã nhật ký vận hành
        /// </summary>
        public int OperationDiaryID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(OperationDiaryID))]
        public virtual OperationDiary OperationDiary { get; set; }

        /// <summary>
        /// SyncDataKey
        /// </summary>
        [StringLength(100)]
        public string SyncDataKey { get; set; } //{Ngày}_{Nhà máy}_{Cương vị}_{Ca} (07042022_1_1_1)
        /// <summary>
        /// Ngày đồng bộ cuối
        /// </summary>
        public DateTime SyncedDate { get; set; } = DateTime.Now;
        /// <summary>
        /// App ID
        /// </summary>
        [StringLength(100)]
        public string AppID { get; set; }

        /// <summary>
        /// Phiếu công tác
        /// </summary>
        public int? RegistrationTicketID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(RegistrationTicketID))]
        public virtual WorkTicket RegistrationTicket { get; set; }

        /// <summary>
        /// Tên phiếu công tác
        /// </summary>
        [StringLength(255)]
        public string RegistrationTicketName { get; set; }

        /// <summary>
        /// Nhóm trưởng
        /// </summary>
        [StringLength(255)]
        public string TeamLeader { get; set; }

        /// <summary>
        /// Khu vực
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// Nội dung tồn tại
        /// </summary>
        [Required]
        public string Content { get; set; }

        /// <summary>
        /// File đính kèm
        /// </summary>
        [StringLength(500)]
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

        /// <summary>
        /// 4.Tồn tại trong ca
        /// </summary>
        [MapperIgnore]
        public virtual ICollection<IssueOfShiftDetail> IssueOfShiftDetails { get; set; }

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
