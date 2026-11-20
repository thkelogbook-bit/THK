namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations.Schema;
		using System.Text;

		[Table("Signature")]
    public partial class Signature : EntityBase
    {
        /// <summary>
        /// Nội dung chữ ký, string base64
        /// </summary>
        public string SignatureContent { get; set; }

        /// <summary>
        /// Nội dung chữ ký, base64
        /// </summary>
        [Column(TypeName = "image")]
        public byte[] SignatureContentByte { get; set; }

        /// <summary>
        /// 1: cả hai hệ thống, 2:AdminPortal, 3: MobileApp
        /// </summary>
        public int Type { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(CreatedUserID))]
        public virtual User CreatedUser { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(LastUpdatedUserID))]
        public virtual User LastUpdatedUser { get; set; }
    }
}
