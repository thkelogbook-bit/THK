namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ProbationaryStaff")]
    public partial class ProbationaryStaff : EntityBase
    {
        /// <summary>
        /// Mã nhật ký vận hành
        /// </summary>
        public int OperationDiaryID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(OperationDiaryID))]
        public virtual OperationDiary OperationDiary { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public int OperationPositionID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(OperationPositionID))]
        public virtual PositionInShift PositionInShift { get; set; }

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
        /// Mã nhân viên tập sự
        /// </summary>
        public int? AssignedUserID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(AssignedUserID))]
        public virtual User AssignedUser { get; set; }

        /// <summary>
        /// Họ và Tên nhân viên tập sự
        /// </summary>
        [StringLength(255)]
        public string AssignedUserName { get; set; }

        /// <summary>
        /// Chức vụ
        /// </summary>
        [StringLength(255)]
        public string PositionName { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Description { get; set; }

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
