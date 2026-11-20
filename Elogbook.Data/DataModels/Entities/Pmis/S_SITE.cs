using Elogbook.Data.DataModels.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elogbook.Data
{
    [Table("S_SITE")]
    public partial class S_SITE : PmisEntityBase
    {
        [Key]
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
    }
}
