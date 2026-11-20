namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("GuestUsers")]
    public partial class GuestUser : EntityBase
    {
        /// <summary>
        /// Tên đại diện cho người dùng ngoài hệ thống
        /// </summary>
        [StringLength(255)]
        public string UserName { get; set; }

        /// <summary>
        /// Họ và tên
        /// </summary>
        [Required]
        [StringLength(255)]
        public string FullName { get; set; }

        /// <summary>
        /// Ngày sinh
        /// </summary>
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Số CMND/CCCD
        /// </summary>
        [StringLength(50)]
        public string NationalID { get; set; }

        /// <summary>
        /// Hình ảnh số CMND/CCCD
        /// </summary>
        public int? NationalAttachmentID { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Thẻ an toàn điện
        /// </summary>
        [StringLength(255)]
        public string ElectricRankCard { get; set; }

        /// <summary>
        /// Bậc an toàn điện
        /// </summary>
        [StringLength(10)]
        public string ElectricRank { get; set; }

        /// <summary>
        /// Hình ảnh thẻ an toàn điện
        /// </summary>
        public int? ElectricRankAttachmentID { get; set; }

        /// <summary>
        /// Thẻ an toàn lao động
        /// </summary>
        [StringLength(255)]
        public string LaborCard { get; set; }

        /// <summary>
        /// Bậc thợ
        /// </summary>
        [StringLength(10)]
        public string LaborRank { get; set; }

        /// <summary>
        /// Hình ảnh thẻ an toàn lao động
        /// </summary>
        public int? LaborCardAttachmentID { get; set; }

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
