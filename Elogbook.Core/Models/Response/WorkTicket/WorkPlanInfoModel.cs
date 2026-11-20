using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin tổ chức thực hiện
    /// </summary>
    public class WorkPlanInfoModel
    {
        /// <summary>
        /// Các biện pháp
        /// </summary>
        public List<SafetyMeasureModel> SafetyMeasures { get; set; }

        /// <summary>
        /// Đơn vị công tác (cấp phân xưởng)
        /// </summary>
        public ValueLabelModel WorkUnitLv1 { get; set; }

        /// <summary>
        /// Đơn vị công tác (cấp tổ)
        /// </summary>
        public ValueLabelModel WorkUnitLv2 { get; set; }

        /// <summary>
        /// Tên đơn vị công tác
        /// </summary>
        public string WorkUnitName { get; set; }

        /// <summary>
        /// Số lượng người
        /// </summary>
        public string AmountPersons { get; set; }
    }
}
