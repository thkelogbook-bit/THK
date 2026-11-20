namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Users")]
    public partial class User : EntityBase
    {
        /// <summary>
        /// Họ và tên
        /// </summary>
        [Required]
        [StringLength(255)]
        public string FullName { get; set; }

        /// <summary>
        /// Tên đăng nhập
        /// </summary>
        [Required]
        [StringLength(255)]
        public string UserName { get; set; }

        /// <summary>
        /// Mật khẩu
        /// </summary>
        [StringLength(100)]
        public string Password { get; set; }

        /// <summary>
        /// Chức vụ
        /// </summary>
        [StringLength(255)]
        public string PositionName { get; set; }

        /// <summary>
        /// Ngày sinh
        /// </summary>
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Số điện thoại
        /// </summary>
        [StringLength(30)]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [StringLength(255)]
        public string Email { get; set; }

        /// <summary>
        /// Số CMND/CCCD
        /// </summary>
        [StringLength(50)]
        public string NationalID { get; set; }

        /// <summary>
        /// Hình ảnh số CMND/CCCD
        /// </summary>
        public int NationalAttachmentID { get; set; }

        /// <summary>
        /// Bậc an toàn điện
        /// </summary>
        [StringLength(10)]
        public string ElectricRank { get; set; }

        /// <summary>
        /// Thẻ an toàn điện
        /// </summary>
        [StringLength(255)]
        public string ElectricRankCard { get; set; }

        /// <summary>
        /// Hình ảnh thẻ an toàn điện
        /// </summary>
        public int ElectricRankAttachmentID { get; set; }

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
        public int LaborCardAttachmentID { get; set; }

        /// <summary>
        /// Chữ ký nhân viên
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// Công ty
        /// </summary>
        [Required]
        public int CompanyID { get; set; }

        [ForeignKey(nameof(CompanyID))]
        [MapperIgnore]
        public virtual Organization Company { get; set; }

        /// <summary>
        /// Phân xưởng
        /// </summary>
        public int? ProductionPlantID { get; set; }

        [ForeignKey(nameof(ProductionPlantID))]
        [MapperIgnore]
        public virtual Organization ProductionPlant { get; set; }

        /// <summary>
        /// Tổ
        /// </summary>
        public int? TeamID { get; set; }

        [ForeignKey(nameof(TeamID))]
        [MapperIgnore]
        public virtual Organization Team { get; set; }

        /// <summary>
        /// Đội/nhóm
        /// </summary>
        public int? GroupID { get; set; }

        [ForeignKey(nameof(GroupID))]
        [MapperIgnore]
        public virtual Organization Group { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int? FactoryID { get; set; }

        [ForeignKey(nameof(FactoryID))]
        [MapperIgnore]
        public virtual Factory Factory { get; set; }

        /// <summary>
        /// Loại tài khoản
        /// </summary>
        [Required]
        public int AccountType { get; set; }

        /// <summary>
        /// Nguồn tài khoản
        /// </summary>
        [Required]
        public int SourceType { get; set; }

        /// <summary>
        /// Vai trò
        /// </summary>
        [MapperIgnore]
        public virtual ICollection<RoleInUser> RoleInUsers { get; set; }

        /// <summary>
        /// Trạng thái truy cập
        /// </summary>
        [Required]
        public int Status { get; set; }

        /// <summary>
        /// Ảnh đại diện
        /// </summary>
        public int? AvatarAttachmentID { get; set; }

        /// <summary>
        /// Phương thức ký cấp/khóa phiếu công tác
        /// </summary>
        public int SigningMethodID { get; set; }
        [ForeignKey(nameof(SigningMethodID))]
        [MapperIgnore]
        public virtual SigningMethod SigningMethod { get; set; }

        /// <summary>
        /// Số lần đăng nhập sai mật khẩu
        /// </summary>
        public int NumberLoginFailed { get; set; }
    }
}
