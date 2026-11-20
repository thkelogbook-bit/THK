namespace Elogbook.Data
{
    using Elogbook.Data.DataModels.Base;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("A_LST_CATEGORY")]
    public partial class A_LST_CATEGORY : PmisEntityBase
    {
        [Key]
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
    }
}
