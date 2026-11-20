namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Organization")]
    public partial class Organization : EntityBase
    {
        /// <summary>
        /// ID của đối tượng cha, ví dụ: Phân xưởng thuộc công ty nào thì Phân xưởng sẽ chức ID của công ty đó
        /// </summary>
        public int? ParentID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(ParentID))]
        public virtual Organization Parent { get; set; }

        /// <summary>
        /// Tên Công ty, Phân xưởng, Tổ, Đội/Nhóm
        /// </summary>
        [StringLength(255)]
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Tên viết tắt Công ty, Phân xưởng, Tổ, Đội/Nhóm
        /// </summary>
        [StringLength(100)]
        [Required]
        public string ShortName { get; set; }

        /// <summary>
        /// Loại tổ chức
        /// </summary>
        [StringLength(100)]
        [Required(AllowEmptyStrings = true)]
        public string Level { get; set; }

        /// <summary>
        /// Phân loại công ty
        /// </summary>
        [StringLength(255)]
        public string CompanyType { get; set; }

        /// <summary>
        /// Phân loại phân xưởng(Vận hành/Sửa chữa)
        /// </summary>
        [StringLength(100)]
        public string ProductionPlantType { get; set; }

        /// <summary>
        /// Trạng thái hoạt động
        /// </summary>
        public int Active { get; set; }

        /// <summary>
        /// File đính kèm
        /// </summary>
        public int? AttachmentID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(AttachmentID))]
        public virtual Attachment AttachmentFile { get; set; }

        /// <summary>
        /// Tên phân xưởng hiển trị trên PCT/PTT
        /// </summary>
        [StringLength(255)]
        public string TicketDescription { get; set; }

        /// <summary>
        /// PMIS_ORGID
        /// </summary>
        [StringLength(25)]
        public string PMIS_ORGID { get; set; }

        /// <summary>
        /// PMIS_ORGCODE
        /// </summary>
        [StringLength(255)]
        public string PMIS_ORGCODE { get; set; }

        /// <summary>
        /// PMIS_ORGID_PARENT
        /// </summary>
        [StringLength(255)]
        public string PMIS_ORGID_PARENT { get; set; }

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
