using Elogbook.Data.DataModels.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    [Table("PMIS_S_SITE")]
#pragma warning disable S101 // Types should be named in PascalCase
    public partial class PMIS_S_SITE : PmisSyncEntityBase
#pragma warning restore S101 // Types should be named in PascalCase
    {
        [Required]
        [StringLength(25)]
        public string SITEID { get; set; }

        [Required]
        [StringLength(25)]
        public string ORGID { get; set; }

        [Required]
        [StringLength(500)]
        public string SITEDESC { get; set; }

        public bool ACTIVE { get; set; }

        [StringLength(25)]
        public string SITEID_PARENT { get; set; }

        public int? SITEORD { get; set; }

        [StringLength(250)]
        public string LST_DEPTID { get; set; }

        public int? JobId { get; set; }

        public bool? IsSync { get; set; }
    }
}
