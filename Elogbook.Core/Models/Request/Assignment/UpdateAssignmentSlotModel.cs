using Newtonsoft.Json;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Thông tin nhân sự cho mỗi vị trí theo kế hoạch
    /// </summary>
    public class UpdateAssignmentSlotModel
    {
        /// <summary>
        /// ID nhà máy
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// ID Cương vị
        /// </summary>
        public int OperationPositionID { get; set; }

        /// <summary>
        /// ID Ca vận hành
        /// </summary>
        public int OperationShiftID { get; set; }

        /// <summary>
        /// Ngày làm việc
        /// </summary>
        public string WorkDate { get; set; }

        /// <summary>
        /// Mã vị trí phân công nhân sự
        /// </summary>
        public int AssignmentSlotID { get; set; }

        /// <summary>
        /// Nội dung thay đổi
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Danh sách nhân sự bố trí
        /// </summary>
        public List<UpdateAssignmentEmployeeModel> AssignmentEmployees { get; set; }

        /// <summary>
        /// Người dùng hiện tại
        /// </summary>
        [JsonIgnore]
        public int CurrentUserID { get; set; }
    }
}
