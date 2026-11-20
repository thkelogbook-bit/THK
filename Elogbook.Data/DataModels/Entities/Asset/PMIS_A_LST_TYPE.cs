namespace Elogbook.Data
{
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("PMIS_A_LST_TYPE")]
#pragma warning disable S101 // Types should be named in PascalCase
    public partial class PMIS_A_LST_TYPE : PmisSyncEntityBase
#pragma warning restore S101 // Types should be named in PascalCase
    {
        [Required]
        [StringLength(25)]
        public string TYPEID { get; set; }

        [StringLength(255)]
        public string TYPEDESC { get; set; }

        public int? TYPEORD { get; set; }

        public int? JobId { get; set; }

        public bool? IsSync { get; set; }
    }
}
