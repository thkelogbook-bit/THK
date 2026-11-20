using Elogbook.Core.Mapper;
using Elogbook.Data.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    public class ManipulationTemplateTicket : EntityBase
    {
        /// <summary>
        /// Tên phiếu mẫu
        /// </summary>
        public string TemplateTicketName { get; set; }

        /// <summary>
        /// Danh sách mã nhà máy
        /// </summary>
        public string IdFactorysTemplateTicket { get; set; }

        /// <summary>
        /// Danh sách tên nhà máy
        /// </summary>
        public string FactorysTemplateTicket { get; set; }

        /// <summary>
        /// Trạng thái truy cập
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        [MapperIgnore]
        [ForeignKey(nameof(CreatedUserID))]
        public virtual User CreatedUser { get; set; }

        /// <summary>
        /// Người update
        /// </summary>
        [MapperIgnore]
        [ForeignKey(nameof(LastUpdatedUserID))]
        public virtual User LastUpdatedUser { get; set; }
    }
}
