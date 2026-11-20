namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Table lưu dữ liệu giao nhận, nghiệm thu đường dây, thiết bị điện trước và sau khi thao tác
    /// </summary>
    [Table("DeliveryAccepDevice")]
    public partial class DeliveryAccepDevice : EntityBase
    {
        /// <summary>
        /// Mã phiếu thao tác được cấp
        /// </summary>
        public int TicketID { get; set; }

        /// <summary>
        /// Ngày đồng bộ cuối
        /// </summary>
        public DateTime SyncedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// True nếu giao nhận, nghiệm thu trước. False nếu giao nhận, nghiệm thu sau
        /// </summary>
        public bool IsBeforeDelivery { get; set; }

        /// <summary>
        /// Thời gian
        /// </summary>
        public DateTime DeliveryTime { get; set; } = System.Data.SqlTypes.SqlDateTime.MinValue.Value;

        /// <summary>
        /// Đơn vị
        /// </summary>
        [StringLength(500)]
        public string Unit { get; set; }

        /// <summary>
        /// Họ tên
        /// </summary>
        [StringLength(255)]
        public string FullName { get; set; }

        /// <summary>
        /// Nội dung
        /// </summary>
        public string Content { get; set; }

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
