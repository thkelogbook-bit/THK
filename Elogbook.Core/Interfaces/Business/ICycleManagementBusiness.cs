using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Định nghĩa các phương thức cho mỗi chu kỳ
    /// </summary>
    public interface ICycleManagementBusiness
    {
        /// <summary>
        /// Thêm mới Chu kỳ
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<int> AddCycleManagement(AddEditCycleManagerModel requestModel);

        /// <summary>
        /// Tìm kiếm tất cả Chu kỳ theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetCycleManagements(SearchCycleManagementModel requestModel);

        /// <summary>
        /// Tìm kiếm tất cả Chu kỳ theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel<CycleManagementModel> GetCycleManagementById(int id);

        /// <summary>
        /// Cập nhật Chu kỳ
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateCycleManagement(AddEditCycleManagerModel requestModel);

        /// <summary>
        /// Xóa Chu kỳ
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseModel<bool> DeleteCycleManagement(int id, int userID);

        /// <summary>
        /// Kiểm tra đã sử dụng Chu kỳ để nhập nhật ký vận hành nào chưa
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel<bool> CheckInUseCycleManagement(int id);
    }
}
