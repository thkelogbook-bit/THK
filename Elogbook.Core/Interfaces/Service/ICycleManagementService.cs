using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Định nghĩa các phương thức của Chu kỳ
    /// </summary>
    public interface ICycleManagementService
    {
        /// <summary>
        /// Thêm mới Chu kỳ
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        int AddCycleManagement(AddEditCycleManagerModel requestModel);

        /// <summary>
        /// Kiểu tra Chu kỳ đã tồn tại
        /// </summary>
        /// <param name="cycleID"></param>
        /// <param name="cycleName"></param>
        /// <param name="isAddMode"></param>
        /// <returns></returns>
        bool CheckDuplicateData(int cycleID, string cycleName, bool isAddMode);

        /// <summary>
        /// Tìm kiếm Chu kỳ theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        CycleManagementModel GetCycleManagementById(int id);

        /// <summary>
        /// Cập nhật Chu kỳ
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        bool UpdateCycleManagement(AddEditCycleManagerModel requestModel);

        /// <summary>
        /// Xóa Chu kỳ
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        bool DeleteCycleManagement(int id, int userID);

        /// <summary>
        /// Tìm kiếm tất cả Chu kỳ theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetCycleManagements(SearchCycleManagementModel requestModel);

        /// <summary>
        /// Kiểm tra đã sử dụng Chu kỳ để nhập nhật ký vận hành nào chưa
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool CheckInUseCycleManagement(int id);
    }
}
