using Elogbook.Core.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data.DataModels.Base
{
    public class PmisSyncEntityBase : IEntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public bool IsDeleted { get; set; }

        public int? CreatedUserID { get; set; }

        public DateTime InsertedAt { get; set; } = DateTime.Now;

        public int? LastUpdatedUserID { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
