namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TicketEvent")]
    public partial class TicketEvent : EntityBase
    {
        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }
        [ForeignKey(nameof(FactoryID))]
        [MapperIgnore]
        public virtual Factory Factory { get; set; }

        /// <summary>
        /// Loại PCT-Đ, PCT-TCNH
        /// </summary>
        public int DataType { get; set; }

        /// <summary>
        /// Loại sự kiện
        /// </summary>
        public int EventTypeID { get; set; }
        [ForeignKey(nameof(EventTypeID))]
        [MapperIgnore]
        public virtual SystemData EventType { get; set; }

        /// <summary>
        /// Nội dung sự kiện
        /// </summary>
        public string EventContent { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public int Active { get; set; }

        /// <summary>
        /// Dữ liệu hệ thống
        /// </summary>
        public bool IsSystem { get; set; }
    }
}
