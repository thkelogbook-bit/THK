namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("CycleManagement")]
    public class CycleManagement : EntityBase
    {
        /// <summary>
        /// Tên Chu kỳ
        /// </summary>
        [Required]
        [StringLength(255)]
        public string CycleName { get; set; }

        /// <summary>
        /// Thứ tự sắp xếp
        /// </summary>
        [Required]
        public int Position { get; set; }

        /// <summary>
        /// Danh sách nhà máy sử dụng Chu kỳ
        /// </summary>
        [MapperIgnore]
        public virtual ICollection<CycleFactory> CycleFactories { get; set; }

        /// <summary>
        /// Các mốc thời gian
        /// </summary>
        [StringLength(1000)]
        public string TimeLine { get; set; }

        /// <summary>
        /// Mở nhập liệu trước (phút)
        /// </summary>
        public int OpenInputBefore { get; set; }

        /// <summary>
        /// Kháo nhập liệu sau (phút)
        /// </summary>
        public int BlockInputAfter { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Active { get; set; }

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
