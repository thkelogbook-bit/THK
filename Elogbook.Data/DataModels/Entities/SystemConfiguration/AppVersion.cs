namespace Elogbook.Data
{
    using Elogbook.Data.DataModels.Base;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("AppVersion")]
    public partial class AppVersion : EntityBase
    {
        /// <summary>
        /// Version Application
        /// </summary>
        [StringLength(50)]
        [Required]
        public string Version { get; set; }

        /// <summary>
        /// Cho phép auto upgrade app
        /// </summary>
        public bool ForceUpdate { get; set; }

        /// <summary>
        /// logo
        /// </summary>
        public int? LogoImage { get; set; }

        /// <summary>
        /// Tiêu đề của app
        /// </summary>
        public string AppTitle { get; set; }

        /// <summary>
        /// BackgroundImageBeforeLogin
        /// </summary>
        public int? BackgroundImageBeforeLogin { get; set; }

        /// <summary>
        /// BackgroundImageBeforeLogin
        /// </summary>
        public int? BackgroundImageAfterLogin { get; set; }

        /// <summary>
        /// LogoImage
        /// </summary>
        public int? LogoImageMobile { get; set; }

        /// <summary>
        /// BackgroundImageMobile
        /// </summary>
        public int? BackgroundImageMobile { get; set; }

        /// <summary>
        /// BackgroundLoginImage
        /// </summary>
        public int? BackgroundLoginImage { get; set; }

        /// <summary>
        /// Copyright
        /// </summary>
        public string Copyright { get; set; }

        /// <summary>
        /// LogoImageIOSMobile
        /// </summary>
        public int? LogoImageIOSMobile { get; set; }

        /// <summary>
        /// BackgroundImageIOSMobile
        /// </summary>
        public int? BackgroundImageIOSMobile { get; set; }

        /// <summary>
        /// WebVersion
        /// </summary>
        [StringLength(100)]
        public string WebVersion { get; set; }

        /// <summary>
        /// Logo icon ở góc trái của menu
        /// </summary>
        public int? LeftMenuLogoIcon { get; set; }

        /// <summary>
        /// Cho phép đồng bộ dữ liệu giữa Phiếu công tác và tồn tại ca của NKVH
        /// </summary>
        public bool IsSyncData_PCT_TDTTC { get; set; }
    }
}
