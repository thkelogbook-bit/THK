using Elogbook.Core.Mapper;
using Elogbook.Data.DataModels.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    /// <summary>
    /// Config nhận mail
    /// </summary>
    public class EmailTransmissionRecipients : EntityBase
    {
        /// <summary>
        /// mã số của Email Transmission
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public int EmailTransmissionID { get; set; }

        /// <summary>
        /// địa chỉ mail nhận
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        public string RecipientEmailAddr { get; set; }

        /// <summary>
        /// tên mail
        /// </summary>
        public string RecipientEmailName { get; set; }

        /// <summary>
        /// loại mail: 1:CC,2:BCC,3:To
        /// </summary>
        public int RecipientType { get; set; }

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
