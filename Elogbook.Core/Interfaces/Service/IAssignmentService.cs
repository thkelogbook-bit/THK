using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System.Collections.Generic;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// IAssignmentPlanService
    /// </summary>
    public interface IAssignmentService
    {
        /// <summary>
        /// Kiểm tra trùng lặp phân công
        /// </summary>
        /// <param name="inputRequest"></param>
        /// <returns></returns>
        bool CheckDuplicate(CheckAssignmentDuplicateRequestModel inputRequest);

        /// <summary>
        /// Kiểm tra khoảng thời gian phân công đã tồn tại
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> CheckRangeExists(CheckRangeAssignmentRequestModel requestModel);

        /// <summary>
        /// Tạo phân công theo bộ lọc
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> CreateAssignment(AssignmentRequestModel requestModel);

        /// <summary>
        /// Lấy bảng phân công theo bộ lọc (View 1 - Quản lý phân công)
        /// </summary>
        /// <param name="requestModel">Bộ lọc: nhà máy, khoảng ngày, cương vị, ca</param>
        /// <returns>Danh sách phân công nhóm theo cương vị → ca → ngày</returns>
        List<AssignmentScheduleResponseModel> GetAssignmentSchedule(GetAssignmentScheduleRequestModel requestModel);

        /// <summary>
        /// Lấy thông tin vị trí nhân sự
        /// </summary>
        /// <param name="assignmentSlotID"></param>
        /// <returns></returns>
        AssignmentSlotResponseModel GetAssignmentSlotInfo(int assignmentSlotID);

        /// <summary>
        /// Cập nhật thông tin nhân sự cho vị trí
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateAssignmentSlot(UpdateAssignmentSlotModel requestModel);

    }
}
