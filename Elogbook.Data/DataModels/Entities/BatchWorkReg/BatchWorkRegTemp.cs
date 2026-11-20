using Elogbook.Core.Mapper;
using Elogbook.Data.DataModels.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    /// <summary>
    /// Quản lý danh sách Phiếu đăng ký chờ tạo hàng loạt
    /// </summary>
    [Table("BatchWorkRegTemps")]
    public partial class BatchWorkRegTemp : EntityBase
    {
        /// <summary>
        /// Phiếu đăng ký hàng loạt
        /// </summary>
        public int BatchWorkRegID { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(BatchWorkRegID))]
        public virtual BatchWorkReg BatchWorkReg { get; set; }

        /// <summary>
        /// Phân loại màn hình (1:Điện, 2:Thủy cơ nhiệt hóa)
        /// </summary>
        public int ScreenType { get; set; }

        /// <summary>
        /// Row index
        /// </summary>
        public int RowIndex { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public int FactoryID { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(FactoryID))]
        public virtual Factory Factory { get; set; }

        /// <summary>
        /// Loại phiếu (1/kế hoạch 2/Ngoài kế hoạch 3/Xử lý sự cố)
        /// </summary>
        public int TicketType { get; set; }

        /// <summary>
        /// Lý do (1/Sửa chữa 2/Thí nghiệm 3/Xây dựng)
        /// </summary>
        public int ReasonType { get; set; }

        /// <summary>
        /// Nội dung công tác
        /// </summary>
        [StringLength(4000)]
        public string WorkContent { get; set; }

        /// <summary>
        /// Địa điểm (hoặc thiết bị) tiến hành công tác / Địa điểm/Thiết bị
        /// </summary>
        [StringLength(500)]
        public string WorkPlacement { get; set; }

        /// <summary>
        /// Điều kiện cần có để thực hiện công việc
        /// </summary>
        [StringLength(500)]
        public string WorkCondition { get; set; }

        /// <summary>
        /// Thời gian tiến hành công việc (bắt đầu)
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Thời gian tiến hành công việc (kết thúc)
        /// </summary>
        public DateTime EndDate { get; set; }
    }
}
