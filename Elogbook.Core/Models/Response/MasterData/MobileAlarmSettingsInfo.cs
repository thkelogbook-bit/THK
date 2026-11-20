namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin cấu hình setting
    /// </summary>
    public class MobileAlarmSettingsInfo : MobileEntityBaseModel
    {
        /// <summary>
        /// Tên setting
        /// </summary>
        public string SettingName { get; set; }

        /// <summary>
        /// Loại setting
        /// </summary>
        public int SettingType { get; set; }

        /// <summary>
        /// Màu sắc
        /// </summary>
        public string ColorCode { get; set; }
    }
}
