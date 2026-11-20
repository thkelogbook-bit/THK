namespace Elogbook.Data
{
    using Elogbook.Data.DataModels.Base;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("PMIS_A_LST_CATEGORY")]
#pragma warning disable S101 // Types should be named in PascalCase
    public partial class PMIS_A_LST_CATEGORY : PmisSyncEntityBase
#pragma warning restore S101 // Types should be named in PascalCase
    {
        [Required]
        [StringLength(25)]
        public string CATEGORYID { get; set; }

        [Required]
        [StringLength(255)]
        public string CATEGORYDESC { get; set; }

        [StringLength(25)]
        public string CATEGORYID_PARENT { get; set; }

        [StringLength(25)]
        public string ORGID { get; set; }

        public int? CATEGORYORD { get; set; }

        [StringLength(25)]
        public string ASSETIDPREFIX { get; set; }

        [StringLength(25)]
        public string USER_CR_ID { get; set; }

        public DateTime? USER_CR_DTIME { get; set; }

        [StringLength(25)]
        public string USER_MDF_ID { get; set; }

        public DateTime? USER_MDF_DTIME { get; set; }

        public int? JobId { get; set; }

        public bool? IsSync { get; set; }
    }
}
