namespace Elogbook.Data
{
    using Elogbook.Data.DataModels.Base;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("OP_METER_TAB")]
    public partial class OP_METER_TAB : PmisEntityBase
    {
        [Key]
        [Required]
        [StringLength(25)]
        public string OPTABID { get; set; }

        [Required]
        [StringLength(255)]
        public string OPTABDESC { get; set; }

        [Required]
        [StringLength(25)]
        public string OPTABGROUPID { get; set; }

        [StringLength(1000)]
        public string COLLISTDTIME { get; set; }

        [StringLength(1000)]
        public string COLLISTDISP { get; set; }

        [StringLength(25)]
        public string VIEWTYPE { get; set; }

        public bool? ENABLE { get; set; }

        [StringLength(25)]
        public string USER_CR_ID { get; set; }

        public DateTime? USER_CR_DTIME { get; set; }

        [StringLength(25)]
        public string USER_MDF_ID { get; set; }

        public DateTime? USER_MDF_DTIME { get; set; }

        [StringLength(100)]
        public string INPUT_DAY { get; set; }

        [StringLength(25)]
        public string INPUTTYPEID { get; set; }

        public bool? GROUPMETERBYASSET { get; set; }

        public bool? HIDE_HEADER_COLUMN_IF_SAME_ASSET { get; set; }

        public bool? SHOW_DON_VI_TINH { get; set; }

        public bool? SHOW_SHORT_NAME_METER { get; set; }

        public int? INDEX_SHEET { get; set; }

        public int? NUM_DECIMAL_ROUND { get; set; }
    }
}
