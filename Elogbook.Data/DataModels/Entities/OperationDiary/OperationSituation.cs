namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("OperationSituation")]
    public partial class OperationSituation : EntityBase
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
        /// Giờ ghi dữ liệu
        /// </summary>
        [Required]
        public double Hour { get; set; }

        /// <summary>
        /// Người ra lệnh
        /// </summary>
        public int? AssignedFromID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(AssignedFromID))]
        public virtual User AssignedFrom { get; set; }

        /// <summary>
        /// Người ra lệnh ngoài hệ thống
        /// </summary>
        [StringLength(255)]
        public string AssignedFromName { get; set; }

        /// <summary>
        /// Tên Người ra lệnh ngoài hệ thống
        /// </summary>
        [StringLength(255)]
        public string AssignedToName { get; set; }

        /// <summary>
        /// Người nhận lệnh
        /// </summary>
        public int? AssignedToID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(AssignedToID))]
        public virtual User AssignedTo { get; set; }

        /// <summary>
        /// Tình hình vận hành và các thao tác trong ca
        /// </summary>
        [Required]
        public string Description { get; set; }

        /// <summary>
        /// Danh sách mã số file đính kèm
        /// </summary>
        [StringLength(500)]
        public string Attachments { get; set; }

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
