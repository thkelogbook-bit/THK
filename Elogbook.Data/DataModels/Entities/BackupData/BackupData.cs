using Elogbook.Data.DataModels.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data.DataModels.Entities.BackupData
{
    [Table("BackupData")]
    public partial class BackupData : EntityBase
    {
        [StringLength(250)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        public int AttachmentId { get; set; }

    }
}
