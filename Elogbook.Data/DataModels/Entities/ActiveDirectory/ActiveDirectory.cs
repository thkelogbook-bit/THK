namespace Elogbook.Data
{
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ActiveDirectory")]
    public partial class ActiveDirectory : EntityBase
    {
        /// <summary>
        /// Cho phép sử dụng AD
        /// </summary>
        [Required]
        public bool IsEnableActiveDirectory { get; set; }

        /// <summary>
        /// IP Address
        /// </summary>
        [Required]
        [StringLength(255)]
        public string IPAddress { get; set; }

        /// <summary>
        /// Domain Controller
        /// </summary>
        [Required]
        [StringLength(255)]
        public string DomainController { get; set; }

        /// <summary>
        /// Organizational Unit
        /// </summary>
        [Required]
        [StringLength(255)]
        public string OrganizationalUnit { get; set; }
    }
}
