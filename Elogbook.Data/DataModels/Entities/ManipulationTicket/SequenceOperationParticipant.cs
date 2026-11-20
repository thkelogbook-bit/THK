namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Table lưu dữ liệu ra lệnh và nhận lệnh trên phiếu thao tác
    /// </summary>
    [Table("SequenceOperationParticipant")]
    public partial class SequenceOperationParticipant : EntityBase
    {
        /// <summary>
        /// Mã phiếu thao tác được tạo hoặc mã phiếu thao tác được cấp
        /// </summary>
        public int TicketID { get; set; }

        /// <summary>
        /// Mã công việc thực hiện
        /// </summary>
        public int SequenceOperationID { get; set; }

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
        /// 1/Người giám sát, 3/Người thao tác
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
        public string Signature { get; set; }

        /// <summary>
        /// Loại tài khoản
        /// </summary>
        public bool? IsExternal { get; set; }

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
