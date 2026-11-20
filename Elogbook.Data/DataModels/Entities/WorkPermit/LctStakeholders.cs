namespace Elogbook.Data.WorkPermit
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("LctStakeholders")]
    public class LctStakeholders : EntityBase
    {
        /// <summary>
        /// Người đảm nhiệm vai trò
        /// </summary>
        public int? AssignToUserID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(AssignToUserID))]
        public virtual User AssignToUser { get; set; }

        /// <summary>
        /// Họ và Tên
        /// </summary>
        [StringLength(100)]
        public string FullName { get; set; }

        /// <summary>
        /// Loại user
        /// </summary>
        public bool IsExternal { get; set; }

        /// <summary>
        /// User Name
        /// </summary>
        [StringLength(100)]
        public string UserName { get; set; }

        /// <summary>
        /// Bậc ATĐ
        /// </summary>
        [StringLength(10)]
        public string ElectricRank { get; set; }

        /// <summary>
        /// Thẻ an toàn điện
        /// </summary>
        [StringLength(255)]
        public string ElectricRankCard { get; set; }

        /// <summary>
        /// Thẻ an toàn lao động
        /// </summary>
        public string LaborCard { get; set; }

        /// <summary>
        /// Bậc thợ
        /// </summary>
        [StringLength(10)]
        public string LaborRank { get; set; }

        /// <summary>
        /// Chức vụ trong ticket
        /// 1/Người giám sát an toàn điện 2/Người chỉ huy trực tiếp 3/Người được đề cử để thực hiện công việc
        /// </summary>
        public int Roles { get; set; }

        /// <summary>
        /// Chức vụ của user
        /// </summary>
        [StringLength(500)]
        public string PositionName { get; set; }

        /// <summary>
        /// Nhiệm vụ
        /// </summary>
        public string Mission { get; set; }

        /// <summary>
        /// Mã lệnh đăng ký công tác
        /// </summary>
        public int? RegistrationTicketID { get; set; }

        /// <summary>
        /// Mã vị trí người tham gia công việc
        /// </summary>
        public int? ContentPositionID { get; set; }

        /// <summary>
        /// Bậc thợ
        /// </summary>
        [StringLength(500)]
        public string JobRank { get; set; }

        /// <summary>
        /// vị trí
        /// </summary>
        public string Location { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(RegistrationTicketID))]
        public virtual LctRegistrationTicket RegistrationTicket { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(ContentPositionID))]
        public virtual LctContentPosition ContentPosition { get; set; }

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
