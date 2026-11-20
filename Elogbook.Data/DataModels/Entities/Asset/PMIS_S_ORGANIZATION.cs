namespace Elogbook.Data
{
    using Elogbook.Data.DataModels.Base;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("PMIS_S_ORGANIZATION")]
#pragma warning disable S101 // Types should be named in PascalCase
    public partial class PMIS_S_ORGANIZATION : PmisSyncEntityBase
#pragma warning restore S101 // Types should be named in PascalCase
    {
        [Required]
        [StringLength(25)]
        public string ORGID { get; set; }

        [Required]
        [StringLength(500)]
        public string ORGDESC { get; set; }

        [StringLength(50)]
        public string ORGCODE { get; set; }

        public bool ACTIVE { get; set; }

        [StringLength(25)]
        public string ORGID_PARENT { get; set; }

        public int? ORGORD { get; set; }

        [StringLength(25)]
        public string PARRELTYPEID { get; set; }

        [StringLength(25)]
        public string ORGLEVEL { get; set; }

        [StringLength(500)]
        public string APPURL { get; set; }

        public bool? APPURLACTIVE { get; set; }

        public DateTime ACTIVEDATE { get; set; }

        public bool? ISRUNNING { get; set; }

        [StringLength(500)]
        public string AUTO_LOGIN_CODE { get; set; }

        public DateTime? UPD_LOGIN_CODE_TIME { get; set; }

        [StringLength(50)]
        public string PARENT_RPT_LEVEL { get; set; }

        [StringLength(50)]
        public string VERSION { get; set; }

        [StringLength(50)]
        public string FUELTYPEID { get; set; }

        [StringLength(250)]
        public string SHORT_NAME { get; set; }

        public int? JobId { get; set; }

        public bool? IsSync { get; set; }
    }
}
