using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System.Collections.Generic;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// IAssignmentPlanBusiness
    /// </summary>
    public interface IAssignmentBusiness
    {
        /// <summary>
        /// Kiểm tra khoảng ngày đã tồn tại trong bảng phân công hay chưa
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> CheckRangeExists(CheckRangeAssignmentRequestModel requestModel);

        /// <summary>
        /// Tạo kế hoạch phân công
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> CreateAssignment(AssignmentRequestModel requestModel);

        /// <summary>
        /// Lấy bảng phân công theo bộ lọc (View 1 - Quản lý phân công)
        /// </summary>
        /// <param name="requestModel">Bộ lọc: nhà máy, khoảng ngày, cương vị, ca</param>
        /// <returns>Danh sách phân công nhóm theo cương vị → ca → ngày</returns>
        ResponseModel<List<AssignmentScheduleResponseModel>> GetAssignmentSchedule(GetAssignmentScheduleRequestModel requestModel);

        /// <summary>
        /// Lấy thông tin vị trí nhân sự
        /// </summary>
        /// <param name="assignmentSlotID"></param>
        /// <returns></returns>
        ResponseModel<AssignmentSlotResponseModel> GetAssignmentSlotInfo(int assignmentSlotID);

        /// <summary>
        /// Cập nhật thông tin nhân sự cho vị trí
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateAssignmentSlot(UpdateAssignmentSlotModel requestModel);

    }
}
