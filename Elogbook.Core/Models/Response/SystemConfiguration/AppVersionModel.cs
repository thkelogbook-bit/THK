using Newtonsoft.Json;

namespace Elogbook.Core.Models.Response.SystemConfiguration
{
    /// <summary>
    /// Thông tin version mobile app
    /// </summary>
    public class AppVersionModel : EntityBaseModel
    {
        /// <summary>
        /// Version Application
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Cho phép auto upgrade app
        /// </summary>
        public bool ForceUpdate { get; set; }

        /// <summary>
        /// LogoImage
        /// </summary>
        public string LogoImage { get; set; }

        /// <summary>
        /// Tiêu đề của app
        /// </summary>
        public string AppTitle { get; set; }

        /// <summary>
        /// LogoAttachment
        /// </summary>
        public AttachmentModel LogoAttachment { get; set; }

        /// <summary>
        /// LogoImage
        /// </summary>
        public int? LogoImageID { get; set; }

        /// <summary>
        /// BackgroundImageBeforeLogin
        /// </summary>
        public int? BackgroundImageBeforeLogin { get; set; }

        /// <summary>
        /// BackgroundImageBeforeLoginAttachment
        /// </summary>
        public AttachmentModel BackgroundImageBeforeLoginAttachment { get; set; }

        /// <summary>
        /// BackgroundImageBeforeLogin
        /// </summary>
        public int? BackgroundImageAfterLogin { get; set; }

        /// <summary>
        /// BackgroundImageAfterLoginAttachment
        /// </summary>
        public AttachmentModel BackgroundImageAfterLoginAttachment { get; set; }

        /// <summary>
        /// LogoImage
        /// </summary>
        public int? LogoImageMobile { get; set; }

        /// <summary>
        /// LogoImageMobileAttachment
        /// </summary>
        public AttachmentModel LogoImageMobileAttachment { get; set; }

        /// <summary>
        /// BackgroundImageMobile
        /// </summary>
        public int? BackgroundImageMobile { get; set; }

        /// <summary>
        /// BackgroundImageMobileAttachment
        /// </summary>
        public AttachmentModel BackgroundImageMobileAttachment { get; set; }

        /// <summary>
        /// BackgroundLoginImage
        /// </summary>
        public int? BackgroundLoginImage { get; set; }

        /// <summary>
        /// BackgroundLoginImageAttachment
        /// </summary>
        public AttachmentModel BackgroundLoginImageAttachment { get; set; }

        /// <summary>
        /// Copyright
        /// </summary>
        public string Copyright { get; set; }

        /// <summary>
        /// LogoImageIOSMobile
        /// </summary>
        public int? LogoImageIOSMobile { get; set; }

        /// <summary>
        /// LogoImageIOSMobileAttachment
        /// </summary>
        public AttachmentModel LogoImageIOSMobileAttachment { get; set; }

        /// <summary>
        /// BackgroundImageIOSMobile
        /// </summary>
        public int? BackgroundImageIOSMobile { get; set; }

        /// <summary>
        /// BackgroundImageIOSMobileAttachment
        /// </summary>
        public AttachmentModel BackgroundImageIOSMobileAttachment { get; set; }

        /// <summary>
        /// WebVersion
        /// </summary>
        public string WebVersion { get; set; }

        /// <summary>
        /// Logo icon ở góc trái của menu
        /// </summary>
        public int? LeftMenuLogoIcon { get; set; }

        /// <summary>
        /// LeftMenuLogoIconAttachment
        /// </summary>
        public AttachmentModel LeftMenuLogoIconAttachment { get; set; }

        /// <summary>
        /// Show thông tin Tình hình lúc nhận ca cho nhật ký vận hành hay không
        /// </summary>
        public bool IsShowShiftReceiving { get; set; } = false;

        /// <summary>
        /// Show thông báo hay không
        /// </summary>
        public bool IsShowNotification { get; set; } = false;

        /// <summary>
        /// Time refresh interval dashboard (Seconds)
        /// </summary>
        public int RefreshIntervalDashboard { get; set; }

        /// <summary>
        /// Max file size upload (Bytes)
        /// </summary>
        public int MaxFileSizeUpload { get; set; }

        /// <summary>
        /// Khoảng thời gian tối đa cho phép mỗi lần tạo bảng phân công ca, giá trị tính bằng ngày
        /// </summary>
        public int AssignmentMaxCreateDays { get; set; }

        /// <summary>
        /// Thời gian cho phép được cập nhật nhân sự trong một slot phân công,
        /// tính bằng số giờ trước giờ bắt đầu ca
        /// </summary>
        public int AssignmentUpdateDeadlineHours { get; set; }
    }
}
