namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin các modules phân quyền
    /// </summary>
    public class MobileModulesInfo : MobileEntityBaseModel
    {
        /// <summary>
        /// Mã định danh module
        /// </summary>
        public string ModuleValue { get; set; }

        /// <summary>
        /// Tên module
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// Module thuộc Admin Portal hay Mobile App
        /// </summary>
        public string SystemType { get; set; }
    }
}
