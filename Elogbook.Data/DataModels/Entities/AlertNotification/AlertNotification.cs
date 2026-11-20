using Elogbook.Core.Mapper;
using Elogbook.Data.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elogbook.Data
{
    /// <summary>
    /// Danh mục - Công việc
    /// </summary>
    [Table("AlertNotification")]
    public partial class AlertNotification : EntityBase
    {
        /// <summary>
        /// User nhận thông báo
        /// </summary>
        [Required]
        public int UserID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(UserID))]
        public virtual User User { get; set; }

        /// <summary>
        /// Trạng thái thông báo
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Tiêu đề thông báo
        /// </summary>
        public string NoticeTitle { get; set; }

        /// <summary>
        /// Đánh dấu
        /// </summary>
        public bool? IsImportant { get; set; }

        /// <summary>
        /// Nội dung thông báo
        /// </summary>
        public string NoticeContent { get; set; }
    }
}
