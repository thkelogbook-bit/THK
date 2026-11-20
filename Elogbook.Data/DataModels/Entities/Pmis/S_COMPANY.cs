namespace Elogbook.Data
{
    using Elogbook.Data.DataModels.Base;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("S_COMPANY")]
    public partial class S_COMPANY : PmisEntityBase
    {
        [Key]
        [Required]
        [StringLength(25)]
        public string COMPID { get; set; }

        [Required]
        [StringLength(255)]
        public string COMPDESC { get; set; }

        [StringLength(255)]
        public string ADDRESS { get; set; }

        [StringLength(50)]
        public string EMAIL { get; set; }

        [StringLength(50)]
        public string TEL { get; set; }

        [StringLength(50)]
        public string FAX { get; set; }

        [StringLength(2000)]
        public string NOTE { get; set; }

        [StringLength(25)]
        public string COMPID_PARENT { get; set; }

        [StringLength(25)]
        public string ORGID { get; set; }

        [StringLength(1000)]
        public string TYPEIDLIST { get; set; }

        [StringLength(255)]
        public string HOMEPAGE { get; set; }

        [StringLength(25)]
        public string USER_CREATE_ID { get; set; }

        public DateTime? USER_CREATE_DTIME { get; set; }

        [StringLength(25)]
        public string USER_MODIFY_ID { get; set; }

        public DateTime? USER_MODIFY_DTIME { get; set; }

        public bool? ACTIVE { get; set; }

        public string ATTACH_FILE { get; set; }

        public string MUCDICH_SD_TROXI { get; set; }
    }
}
