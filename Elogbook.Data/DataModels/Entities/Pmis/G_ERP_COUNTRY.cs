namespace Elogbook.Data
{
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("G_ERP_COUNTRY")]
    public partial class G_ERP_COUNTRY : PmisEntityBase
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(5)]
        public string COUNTRY_CODE { get; set; }

        [StringLength(100)]
        public string COUNTRY_NAME { get; set; }
    }
}
