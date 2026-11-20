namespace Elogbook.Data
{
    using Elogbook.Core.Mapper;
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Factory")]
    public partial class Factory : EntityBase
    {
        [Required]
        [StringLength(255)]
        public string FactoryName { get; set; }

        [Required]
        public int CompanyID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(CompanyID))]
        public virtual Organization Company { get; set; }

        [Required]
        public int ProductionPlantID { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(ProductionPlantID))]
        public virtual Organization ProductionPlant { get; set; }

        public int Status { get; set; }

        public bool IsUseOperationParameterGroup { get; set; }

        /// <summary>
        /// Xác định user lúc đồng bộ TSVH sang PMIS
        /// </summary>
        [StringLength(25)]
        public string PMIS_USERID { get; set; }

        /// <summary>
        /// Cấu hình filter dữ liệu theo nhà máy
        /// </summary>
        public bool IsFilterFactory { get; set; }

        /// <summary>
        /// Tên viết tắt của nhà máy
        /// </summary>
        [StringLength(100)]
        public string ShortName { get; set; }

        /// <summary>
        /// Cho phép sử dụng tên viết tắt của nhà máy trong số phiếu
        /// </summary>
        [Required]
        public bool IsUseShortNameToTicket { get; set; }

        /// <summary>
        /// Tên phân xưởng hiển trị trên PCT/PTT
        /// </summary>
        [StringLength(255)]
        public string TicketDescription { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(CreatedUserID))]
        public virtual User CreatedUser { get; set; }

        [MapperIgnore]
        [ForeignKey(nameof(LastUpdatedUserID))]
        public virtual User LastUpdatedUser { get; set; }

        /// <summary>
        /// Format phiếu thao tác
        /// </summary>
        [StringLength(50)]
        public string PTTFormat { get; set; }

        /// <summary>
        /// Format phiếu công tác điện
        /// </summary>
        [StringLength(50)]
        public string PCTDFormat { get; set; }

        /// <summary>
        /// Format phiếu công tác thủy cơ nhiệt hóa
        /// </summary>
        [StringLength(50)]
        public string PCTTCNHFormat { get; set; }

        [StringLength(25)]
        public string PMIS_ID { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [StringLength(255)]
        public string Email { get; set; }
    }
}
