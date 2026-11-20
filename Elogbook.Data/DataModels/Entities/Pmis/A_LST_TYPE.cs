namespace Elogbook.Data
{
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("A_LST_TYPE")]
    public partial class A_LST_TYPE : PmisEntityBase
    {
        [Key]
        [Required]
        [StringLength(25)]
        public string TYPEID { get; set; }

        [StringLength(255)]
        public string TYPEDESC { get; set; }

        public int? TYPEORD { get; set; }
    }
}
