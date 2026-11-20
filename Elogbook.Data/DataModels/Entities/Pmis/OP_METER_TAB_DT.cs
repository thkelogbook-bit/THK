namespace Elogbook.Data
{
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("OP_METER_TAB_DT")]
    public partial class OP_METER_TAB_DT : PmisEntityBase
    {
        [Key, Column(Order = 0)]
        [Required]
        [StringLength(25)]
        public string OPTABID { get; set; }

        [Key, Column(Order = 1)]
        [Required]
        [StringLength(25)]
        public string ADMETERID { get; set; }

        public int? ROWORD { get; set; }

        [StringLength(255)]
        public string SHORTNAME { get; set; }

        public string TIME_ROW_CELL { get; set; }
    }
}
