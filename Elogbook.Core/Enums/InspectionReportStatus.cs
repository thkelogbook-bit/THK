using System.ComponentModel;

namespace Elogbook.Core.Enums
{
    /// <summary>
    /// Trạng thái biên bản khảo sát thị trường
    /// </summary>
    public enum InspectionReportStatus
    {

        /// <summary>
        /// Mở
        /// </summary>
        [Description("Mở")]
        Open = 1,

        /// <summary>
        /// Đang thực hiện
        /// </summary>
        [Description("Đang thực hiện")]
        Inprogress = 2,

        /// <summary>
        /// Hoàn thành
        /// </summary>
        [Description("Hoàn thành")]
        Complete =3
    }
}
