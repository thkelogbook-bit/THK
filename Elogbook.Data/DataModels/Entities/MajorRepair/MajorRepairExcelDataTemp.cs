using Elogbook.Core.Mapper;
using Elogbook.Data.DataModels.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    /// <summary>
    /// Thông tin chi tiết dữ liệu excel
    /// </summary>
    public class MajorRepairExcelDataTemp : EntityBase
    {
        /// <summary>
        /// Marjor repair id
        /// </summary>
        public int MajorRepairId { get; set; }
        [MapperIgnore]
        [ForeignKey(nameof(MajorRepairId))]
        public virtual MajorRepair MajorRepair { get; set; }

        /// <summary>
        /// Row index
        /// </summary>
        public int RowIndex { get; set; }

        /// <summary>
        /// Nội dung công tác
        /// </summary>
        [StringLength(4000)]
        public string WorkContent { get; set; }

        /// <summary>
        /// Vị trí / KKS
        /// </summary>
        [StringLength(500)]
        public string WorkTicketLocation { get; set; }

        /// <summary>
        /// Loại phiếu
        /// </summary>
        public int ScreenType { get; set; }

        /// <summary>
        /// Tổ máy / Khu vực
        /// </summary>
        [StringLength(500)]
        public string WorkTicketUnitArea { get; set; }
    }
}
