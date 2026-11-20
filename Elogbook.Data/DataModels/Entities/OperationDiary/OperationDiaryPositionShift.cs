namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("OperationDiaryPositionShift")]
    public partial class OperationDiaryPositionShift : EntityBase
    {
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
        /// App ID
        /// </summary>
        [StringLength(100)]
        public string AppID { get; set; }

        /// <summary>
        /// Mã cương vị trong ca
        /// </summary>
        public int? PositionInShiftID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(PositionInShiftID))]
        public virtual PositionInShift PositionInShift { get; set; }

        /// <summary>
        /// Giá trị mặc định hiển thị Trưởng ca/Trưởng kíp/Vận hành viên
        /// </summary>
        [StringLength(255)]
        public string Label { get; set; }

        /// <summary>
        /// Người được phân công
        /// </summary>
        public int? AssignedUserID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(AssignedUserID))]
        public virtual User AssignedUser { get; set; }

        /// <summary>
        /// Giá trị mặc định để phân biệt danh sách từ cương vị trong ca hoặc mặc định Trưởng ca/Trưởng kíp/Vận hành viên
        /// </summary>
        public bool IsDefault { get; set; }
        /// <summary>
        /// Ngày đồng bộ cuối
        /// </summary>
        public DateTime SyncedDate { get; set; } = DateTime.Now;
        /// <summary>
        /// Vị trí hiển thị
        /// </summary>
        public int Position { get; set; }

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
