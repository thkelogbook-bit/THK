using Elogbook.Core.Mapper;
using Elogbook.Data.DataModels.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    /// <summary>
    /// Thông tin nhân sự được phân công
    /// </summary>
    [Table("AssignmentEmployee")]
    public partial class AssignmentEmployee : EntityBase
    {
        /// <summary>
        /// Mã vị trí phân công nhân sự
        /// </summary>
        [Required]
        public int AssignmentSlotID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(AssignmentSlotID))]
        public virtual AssignmentSlot AssignmentSlot { get; set; }

        /// <summary>
        /// Mã nhân sự
        /// </summary>
        [Required]
        public int UserID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(UserID))]
        public virtual User User { get; set; }

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
