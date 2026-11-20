using Elogbook.Core.Mapper;
using Elogbook.Data.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    /// <summary>
    /// Phân loại thiết bị
    /// </summary>
    [Table("AmDeviceCategory")]
    public partial class AmDeviceCategory : EntityBase
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int? OrgID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(OrgID))]
        public virtual Organization Organization { get; set; }

        /// <summary>
        /// Tên
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// Phân loại cha
        /// </summary>
        public int? CategoryParent { get; set; }

        /// <summary>
        /// mã
        /// </summary>
        public string CategoryCode { get; set; }

        /// <summary>
        /// Trạng thái 1: Áp dụng, 2 Không áp dụng
        /// </summary>
        public int Active { get; set; }

        /// <summary>
        /// Mã PMIS 
        /// </summary>
        public string PMISId { get; set; }

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
