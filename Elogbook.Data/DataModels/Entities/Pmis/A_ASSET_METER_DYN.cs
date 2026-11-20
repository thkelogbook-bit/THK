namespace Elogbook.Data
{
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("A_ASSET_METER_DYN")]
    public partial class A_ASSET_METER_DYN : PmisEntityBase
    {
        [Key]
        [Required]
        [StringLength(25)]
        public string ADMETERID { get; set; }

        [Required]
        [StringLength(25)]
        public string ASSETID { get; set; }

        [Required]
        [StringLength(25)]
        public string METERID { get; set; }

        [StringLength(51)]
        public string ADMETERID_PARENT { get; set; }

        [StringLength(25)]
        public string VALTRENDID { get; set; }

        [StringLength(25)]
        public string READMETHODID { get; set; }

        public int? METERORD { get; set; }

        [StringLength(50)]
        public string NOTE { get; set; }

        [StringLength(255)]
        public string TAGID { get; set; }

        [StringLength(25)]
        public string INPUTTYPEID { get; set; }

        [StringLength(50)]
        public string MA_DIEM_DO_HES { get; set; }

        [StringLength(50)]
        public string MA_THONG_SO_EVNHES { get; set; }

        [StringLength(25)]
        public string DESIGN_PARAM { get; set; }

        [StringLength(25)]
        public string OPERATING_PARAM { get; set; }

        [StringLength(25)]
        public string DESIRED_PARAM { get; set; }

        [StringLength(25)]
        public string DESIGN_METHOD { get; set; }

        [StringLength(25)]
        public string OPERATING_METHOD { get; set; }

        [StringLength(25)]
        public string DESIRED_METHOD { get; set; }

        public decimal? ALARM_PARAM { get; set; }
    }
}
