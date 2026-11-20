namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("RelatedUser")]
    public partial class RelatedUser : EntityBase
    {
        /// <summary>
        /// Mã người dùng
        /// </summary>
        public int AssignToUserID { get; set; }

        /// <summary>
        /// Mã InspectionReport
        /// </summary>
        public int InspectionReportID { get; set; }

        /// <summary>
        /// Tên người dùng
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Xác định người trong hệ thống/ngoài hệ thống
        /// </summary>
        public bool IsExternal { get; set; }

        /// <summary>
        /// Tên tài khoản
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Bậc ATĐ
        /// </summary>
        public string ElectricRank { get; set; }

        /// <summary>
        /// Thẻ an toàn điện
        /// </summary>
        public string ElectricRankCard { get; set; }

        /// <summary>
        /// Thẻ an toàn lao động
        /// </summary>
        public string LaborCard { get; set; }

        /// <summary>
        /// Bậc thợ
        /// </summary>
        public string LaborRank { get; set; }

        /// <summary>
        /// Chức vụ trong ticket
        /// 1/Đại diện Đơn vị công tác 2/Đại diện (các) Đơn vị QLVH
        /// </summary>
        public int Roles { get; set; }

        /// <summary>
        /// Chức vụ của user
        /// </summary>
        public string PositionName { get; set; }

        /// <summary>
        /// Xác định người ký xác nhận hoàn thành, true nếu ký xác nhận, mặc định false
        /// </summary>
        public bool IsSigning { get; set; }

        /// <summary>
        /// Chữ ký
        /// </summary>
        public string Signature { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(CreatedUserID))]
        public virtual User CreatedUser { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(LastUpdatedUserID))]
        public virtual User LastUpdatedUser { get; set; }
    }
}