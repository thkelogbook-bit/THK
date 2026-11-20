using Elogbook.Core.Mapper;
using Elogbook.Data.DataModels.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    /// <summary>
    /// Lịch sử nhân viên được phân công
    /// </summary>
    [Table("AssignmentEmployeeHistory")]
    public partial class AssignmentEmployeeHistory : EntityBase
    {
        /// <summary>
        /// Slot phân công
        /// </summary>
        [Required]
        public int AssignmentSlotID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(AssignmentSlotID))]
        public virtual AssignmentSlot AssignmentSlot { get; set; }

        /// <summary>
        /// Người dùng cũ
        /// </summary>
        public int? OldUserID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(OldUserID))]
        public virtual User OldUser { get; set; }

        /// <summary>
        /// Người dùng mới
        /// </summary>
        public int? NewUserID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(NewUserID))]
        public virtual User NewUser { get; set; }

        /// <summary>
        /// Loại hành động
        /// </summary>
        [Required]
        public int ActionType { get; set; }

        /// <summary>
        /// Lý do
        /// </summary>
        [StringLength(1000)]
        public string Reason { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int? Active { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(CreatedUserID))]
        public virtual User CreatedUser { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(LastUpdatedUserID))]
        public virtual User LastUpdatedUser { get; set; }
    }
}
