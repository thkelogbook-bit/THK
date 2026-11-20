namespace Elogbook.Data
{
    using Elogbook.Data.DataModels.Base;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("PMIS_A_ASSET")]
#pragma warning disable S101 // Types should be named in PascalCase
    public partial class PMIS_A_ASSET : PmisSyncEntityBase
#pragma warning restore S101 // Types should be named in PascalCase
    {
        [Required]
        [StringLength(25)]
        public string ASSETID { get; set; }

        [StringLength(25)]
        public string ASSETID_PARENT { get; set; }

        [StringLength(100)]
        public string ASSETID_ORG { get; set; }

        [StringLength(500)]
        public string ASSETDESC { get; set; }

        public int? ASSETORD { get; set; }

        [Required]
        [StringLength(25)]
        public string SITEID { get; set; }

        [StringLength(25)]
        public string CATEGORYID { get; set; }

        [StringLength(25)]
        public string TYPEID { get; set; }

        [StringLength(1000)]
        public string MANDEPTIDLIST { get; set; }

        [StringLength(25)]
        public string USESTATUS_LAST_ID { get; set; }

        public DateTime? USESTATUS_LAST_DTIME { get; set; }

        [StringLength(255)]
        public string SERIALNUM { get; set; }

        [StringLength(25)]
        public string P_MANUFACTURERID { get; set; }

        [StringLength(25)]
        public string P_VENDORID { get; set; }

        public DateTime? P_INSTALLDATE { get; set; }

        public decimal? P_PRICE { get; set; }

        [StringLength(25)]
        public string P_PRICEUOM { get; set; }

        public bool? P_PRICE_INC_ALL_CHILD { get; set; }

        public int? PRIORITY { get; set; }

        [StringLength(25)]
        public string USER_CR_ID { get; set; }

        public DateTime? USER_CR_DTIME { get; set; }

        [StringLength(25)]
        public string USER_MDF_ID { get; set; }

        public DateTime? USER_MDF_DTIME { get; set; }

        [StringLength(25)]
        public string USESTATUS_UPDATE_ID { get; set; }

        [StringLength(255)]
        public string ASSETIDSORT { get; set; }

        public int? CHILDCOUNT { get; set; }

        public int? COUNTRY_ID { get; set; }

        public bool? ISFIX { get; set; }

        public DateTime? P_OPERATIONDATE { get; set; }

        public bool? NONE_ATTACH { get; set; }

        public bool? NONE_MATERIAL { get; set; }

        public bool? NONE_METER_DYN { get; set; }

        public bool? NONE_SAFE { get; set; }

        public int? P_MAN_YEAR { get; set; }

        public bool? ISDEL { get; set; }

        [StringLength(25)]
        public string USER_DEL_ID { get; set; }

        public DateTime? USER_DEL_DTIME { get; set; }

        public bool? CHILD_METER_DYN { get; set; }

        public bool? CHILD_MATERIAL { get; set; }

        public int? JobId { get; set; }

        public string KKS { get; set; }

        public string KHVH { get; set; }

        public string ASSET_TYPE { get; set; }

        public string METER { get; set; }

        public string DRAWING { get; set; }

        public string NOTE { get; set; }
    }
}
