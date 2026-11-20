namespace Elogbook.Data
{
    using Elogbook.Data.DataModels.Base;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("A_METER")]
    public partial class A_METER : PmisEntityBase
    {
        [Key]
        [Required]
        [StringLength(25)]
        public string METERID { get; set; }

        [Required]
        [StringLength(25)]
        public string METERGRPID { get; set; }

        [StringLength(25)]
        public string ORGID { get; set; }

        [Required]
        [StringLength(255)]
        public string METERDESC { get; set; }

        [StringLength(25)]
        public string UOMID { get; set; }

        [StringLength(25)]
        public string VALTYPEID { get; set; }

        [StringLength(25)]
        public string VALLISTID { get; set; }

        [StringLength(100)]
        public string SIGN { get; set; }

        [StringLength(25)]
        public string USER_CR_ID { get; set; }

        public DateTime? USER_CR_DTIME { get; set; }

        [StringLength(25)]
        public string USER_MDF_ID { get; set; }

        public DateTime? USER_MDF_DTIME { get; set; }

        public int? METER_ORD { get; set; }

        [StringLength(50)]
        public string NOTE { get; set; }
    }
}
