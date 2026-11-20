using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Định nghĩa các phương thức
    /// </summary>
    public interface IAmMaintenanceScheduleService
    {
        /// <summary>
        /// Tìm kiếm theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetMaintenanceSchedules(AmMaintenanceScheduleSearchModel requestModel);

        /// <summary>
        /// Tìm kiếm theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        AmMaintenanceScheduleModel GetMaintenanceScheduleById(int id);

        /// <summary>
        /// Thêm mới
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<int> CreateMaintenanceSchedule(AmMaintenanceScheduleAddEditModel requestModel);
        
        /// <summary>
        /// Tạo bảo trì tự quản
        /// </summary>
        /// <param name="maintenanceScheduleId"></param>
        /// <returns></returns>
        int CreateAM(int maintenanceScheduleId);

        /// <summary>
        /// Cập nhật
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        bool UpdateMaintenanceSchedule(AmMaintenanceScheduleAddEditModel requestModel);

        /// <summary>
        /// Xóa
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseBaseModel DeleteMaintenanceSchedule(int id, int userID);

        /// <summary>
        /// Kiểm tra tồn tại
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="isCreate"></param>
        /// <param name="factoryID"></param>
        /// <returns></returns>
        bool CheckDuplicateMaintenanceSchedule(int id, string name, bool isCreate, int factoryID);
    }
}
