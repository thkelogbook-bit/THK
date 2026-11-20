namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Table lưu dữ liệu Người giám sát, Người thao tác
    /// </summary>
    [Table("Participant")]
    public partial class Participant : EntityBase
    {
        /// <summary>
        /// Mã phiếu thao tác được cấp
        /// </summary>
        public int? TicketID { get; set; }

        /// <summary>
        /// Ngày đồng bộ cuối
        /// </summary>
        public DateTime SyncedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Mã phiếu đăng ký thao tác
        /// </summary>
        public int? RegistrationManipulationID { get; set; }

        /// <summary>
        /// Nếu phiếu tạo thì = true, nếu phiếu cấp thì = false
        /// </summary>
        public bool IsCreated { get; set; }

        /// <summary>
        /// UserName
        /// </summary>
        [StringLength(100)]
        public string UserName { get; set; }

        /// <summary>
        /// Họ và tên
        /// </summary>
        [StringLength(100)]
        public string FullName { get; set; }

        /// <summary>
        /// Chức vụ: trưởng ca, trưởng kíp, ...
        /// </summary>
        [StringLength(255)]
        public string PositionName { get; set; }

        /// <summary>
        /// Vai trò trong phiếu thao tác
        /// Người giám sát = 6, Người thao tác = 8, Người tạo phiếu = 1, Người duyệt phiếu = 2
        /// </summary>
        public int Roles { get; set; }

        /// <summary>
        /// cấp bật ATĐ
        /// </summary>
        [StringLength(10)]
        public string ElectricRank { get; set; }

        /// <summary>
        /// Thẻ an toàn điện
        /// </summary>
        [StringLength(255)]
        public string ElectricRankCard { get; set; }

        /// <summary>
        /// Chữ ký
        /// </summary>
        public int? SignatureID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(CreatedUserID))]
        public virtual User CreatedUser { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(LastUpdatedUserID))]
        public virtual User LastUpdatedUser { get; set; }

        /// <summary>
        /// Loại tài khoản
        /// </summary>
        public bool? IsExternal { get; set; }

        /// <summary>
        /// vị trí
        /// </summary>
        public int? Order { get; set; }
    }
}
