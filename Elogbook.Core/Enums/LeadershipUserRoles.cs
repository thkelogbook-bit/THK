using System.ComponentModel;
namespace Elogbook.Core.Enums
{
    /// <summary>
    /// chức vụ người tham gia công việc
    /// </summary>
    public enum LeadershipUserRoles
    {
        /// <summary>
        /// Người đăng ký phiếu công tác
        /// </summary>
        [Description("Người đăng ký phiếu công tác")]
        Subscribers = 1,

        /// <summary>
        /// Người cấp phiếu công tác
        /// </summary>
        [Description("Người cấp phiếu công tác")]
        Approver,

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
        /// Người đại diện đơn vị quản lý
        /// </summary>
        [Description("Người đại diện đơn vị quản lý")]
        RepresentativeUnit,

        /// <summary>
        /// Người thao tác
        /// </summary>
        [Description("Người thao tác")]
        ManipulationWorker,

        /// <summary>
        /// Người xác nhận hoàn thành
        /// </summary>
        [Description("Người xác nhận hoàn thành")]
        CompletedConfirm,

        /// <summary>
        /// Người chỉ huy công việc hàng ngày
        /// </summary>
        [Description("Người chỉ huy công việc hàng ngày")]
        CommanderWorkDaily,

        /// <summary>
        /// Người giám sát công việc hàng ngày
        /// </summary>
        [Description("Người giám sát công việc công việc hàng ngày")]
        SupervisorDaily,

        /// <summary>
        /// Người chỉ huy  kết thúc
        /// </summary>
        [Description("Người chỉ huy kết thúc công tác")]
        CommanderWorkFinish,

        /// <summary>
        /// Người chỉ huy công thi hành lệnh
        /// </summary>
        [Description("Người chỉ huy công thi hành lệnh")]
        CommanderExecution,

        /// <summary>
        /// Người giám sát thi hành lệnh
        /// </summary>
        [Description("Người giám sát thi hành lệnh")]
        SupervisorExecution,

        /// <summary>
        /// Người cấp phiếu tổ chức thực hiện
        /// </summary>
        [Description("Người cấp phiếu tổ chức thực hiện")]
        ExecutingOrgApprover

    }
}
