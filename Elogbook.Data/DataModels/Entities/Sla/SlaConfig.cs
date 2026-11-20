using Elogbook.Core.Mapper;
using Elogbook.Data.DataModels.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    /// <summary>
    /// Bảng cấu hình SLA
    /// </summary>
    [Table("SlaConfigs")]
    public partial class SlaConfig : EntityBase
    {
        public SlaConfig() {
            SlaLevels = new HashSet<SlaLevel>();
        }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }
        [ForeignKey(nameof(FactoryID))]
        [MapperIgnore]
        public virtual Factory Factory { get; set; }

        /// <summary>
        /// ID phân loại SLA liên quan
        /// </summary>
        public int SlaCategoryID { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(SlaCategoryID))]
        public virtual SlaCategory Category { get; set; }

        /// <summary>
        /// Tên cấu hình SLA
        /// </summary>
        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        /// <summary>
        /// Mô tả cấu hình SLA
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Trạng thái bật/tắt cấu hình SLA
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Danh sách cấp độ SLA liên quan đến cấu hình SLA này
        /// </summary>
        public virtual ICollection<SlaLevel> SlaLevels { get; set; }

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
