using System.ComponentModel;
namespace Elogbook.Core.Enums
{
    /// <summary>
    /// chức vụ người tham gia công việc
    /// </summary>
    public enum UserInTicketRoles
    {
        /// <summary>
        /// Người tạo phiếu thao tác
        /// </summary>
        [Description("Người tạo phiếu thao tác")]
        Creator = 1,

        /// <summary>
        /// Người cấp phiếu 
        /// </summary>
        [Description("Người cấp phiếu ")]
        Approval,

        /// <summary>
        /// Người cho phép thực hiện công việc
        /// </summary>
        [Description("Người cho phép thực hiện công việc")]
        Acceptor,

        /// <summary>
        /// Người lãnh đạo công việc
        /// </summary>
        [Description("Người lãnh đạo công việc")]
        Leader,

        /// <summary>
        /// Người chỉ huy công việc
        /// </summary>
        [Description("Người chỉ huy công việc")]
        Commander,

        /// <summary>
        /// Người giám sát công việc
        /// </summary>
        [Description("Người giám sát công việc")]
        Supervisor,

        /// <summary>
        /// Người cảnh giới
        /// </summary>
        [Description("Người cảnh giới")]
        Watchman,

        /// <summary>
        /// Người thao tác
        /// </summary>
        [Description("Người thao tác")]
        Worker,

        /// <summary>
        /// Người đại diện đơn vị quản lý
        /// </summary>
        [Description("Người đại diện đơn vị quản lý")]
        RepresentativeUnit,
    }
}
