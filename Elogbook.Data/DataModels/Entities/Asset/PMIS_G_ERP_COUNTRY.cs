namespace Elogbook.Data
{
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("PMIS_G_ERP_COUNTRY")]
#pragma warning disable S101 // Types should be named in PascalCase
    public partial class PMIS_G_ERP_COUNTRY : PmisSyncEntityBase
#pragma warning restore S101 // Types should be named in PascalCase
    {
        [Required]
        [StringLength(5)]
        public string COUNTRY_CODE { get; set; }

        [StringLength(100)]
        public string COUNTRY_NAME { get; set; }

        public int? JobId { get; set; }

        public int? Status { get; set; }

        public bool? IsSync { get; set; }
    }
}
