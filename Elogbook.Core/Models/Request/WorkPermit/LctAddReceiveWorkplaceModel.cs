using System.Collections.Generic;

namespace Elogbook.Core.Models.Request.WorkPermit
{
    /// <summary>
    /// Các tham số để thêm mới tiếp nhân nơi làm việc
    /// </summary>
    public class LctAddReceiveWorkplaceModel
    {
        /// <summary>
        /// Danh sách các biện pháp
        /// </summary>
        public List<LctAddSafetyMeasureModel> SafetyMeasures { get; set; }

        /// <summary>
        /// Tên đơn vị công tác (Cấp tổ)
        /// </summary>
        public string WorkUnitName { get; set; }

        /// <summary>
        /// ID đơn vị công tác (Cấp tổ)
        /// </summary>
        public int? WorkUnitID { get; set; }

        /// <summary>
        /// Số người tham gia
        /// </summary>
        public string AmountPersons { get; set; }

        /// <summary>
        /// Thời gian bắt đầu
        /// </summary>
        public string StartWorkingDate { get; set; }

        /// <summary>
        /// Người giám sát
        /// </summary>
        public string Supervisor { get; set; }

        /// <summary>
        /// Chức vụ người lãnh đạo công việc
        /// </summary>
        public string LeaderPositionName { get; set; }

        /// <summary>
        /// Chức vụ người chỉ huy trực tiếp
        /// </summary>
        public string CommanderPositionName { get; set; }

        /// <summary>
        /// Chức vụ người giám sát
        /// </summary>
        public string SupervisorPositionName { get; set; }
    }
}
