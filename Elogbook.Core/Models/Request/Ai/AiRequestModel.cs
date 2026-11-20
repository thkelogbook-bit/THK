using Elogbook.Core.Constants;
using Elogbook.Core.Models.BaseFilter;
using Elogbook.Core.Models.Response;
using Newtonsoft.Json;

namespace Elogbook.Core.Models.Request
{
    /// <summary>
    /// Thêm mới/Cập nhật - Mức độ cảnh báo
    /// </summary>
    public class AddEditConfigWarningRuleRequestModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Cấu hình chung ID
        /// </summary>
        public int ConfigurationWarningID { get; set; }

        /// <summary>
        /// Quy tắc
        /// </summary>
        public int RuleType { get; set; }

        /// <summary>
        /// Mức vượt ngưỡng (%) (thấp)
        /// Ví dụ: 5.5%, 10.25%, 20%
        /// </summary>
        public decimal ThresholdPercent1 { get; set; }

        /// <summary>
        /// Mức vượt ngưỡng (%) (trung bình)
        /// Ví dụ: 5.5%, 10.25%, 20%
        /// </summary>
        public decimal ThresholdPercent2 { get; set; }

        /// <summary>
        /// Mức vượt ngưỡng (%) (cao)
        /// </summary>
        public decimal ThresholdPercent3 { get; set; }

        /// <summary>
        /// Số lần vượt ngưỡng liên tiếp (thấp)
        /// </summary>
        public int ConsecutiveExceedCount1 { get; set; }

        /// <summary>
        /// Số lần vượt ngưỡng liên tiếp (trung bình)
        /// </summary>
        public int ConsecutiveExceedCount2 { get; set; }

        /// <summary>
        /// Số lần vượt ngưỡng liên tiếp (cao)
        /// </summary>
        public int ConsecutiveExceedCount3 { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }

    /// <summary>
    /// Thêm mới/ Cập nhật cấu hình chung
    /// </summary>
    public class AddEditConfigWarningRequestModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Số ngày
        /// </summary>
        public int Days { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
