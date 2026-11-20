namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("StaffInformation")]
    public partial class StaffInformation : EntityBase
    {
        /// <summary>
        /// Mã phiếu công tác
        /// </summary>
        public int WorkDailyID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(WorkDailyID))]
        public virtual WorkDaily WorkDaily { get; set; }

        /// <summary>
        /// Tên công nhân
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        [StringLength(100)]
        public string FullName { get; set; }

        /// <summary>
        /// Tên công nhân
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        [StringLength(100)]
        public string UserName { get; set; }

        /// <summary>
        /// Là nhân viên bên ngoài
        /// </summary>
        public bool? IsExternal { get; set; }

        /// <summary>
        /// Là Người chỉ huy trực tiếp
        /// </summary>
        public bool IsDirectCommander { get; set; }

        /// <summary>
        /// Mã số người dùng của nhân viên
        /// </summary>
        public int CommanderUserID { get; set; }

        /// <summary>
        /// Nhiệm vụ. Chỉ dành cho mục 11 trong phiếu đăng ký.
        /// </summary>
        public string Mission { get; set; }

        /// <summary>
        /// Chức vụ
        /// </summary>
        [StringLength(255)]
        public string PositionName { get; set; }

        /// <summary>
        /// Thẻ ATĐ
        /// </summary>
        public string ElectricRankCard { get; set; }

        /// <summary>
        /// Thời gian CheckIn
        /// </summary>
        [StringLength(100)]
        public string CheckInTime { get; set; }

        /// <summary>
        /// Ký CheckIn
        /// </summary>
        public int? CheckInSignature { get; set; }

        /// <summary>
        /// Thời gian CheckOut
        /// </summary>
        [StringLength(100)]
        public string CheckOutTime { get; set; }

        /// <summary>
        /// Ký CheckOut
        /// </summary>
        public int? CheckOutSignature { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(CreatedUserID))]
        public virtual User CreatedUser { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(LastUpdatedUserID))]
        public virtual User LastUpdatedUser { get; set; }
    }
}
