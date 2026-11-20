using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Định nghĩa các phương thức
    /// </summary>
    public interface IAmMaintenanceScheduleBusiness
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
        ResponseModel<AmMaintenanceScheduleModel> GetMaintenanceScheduleById(int id);

        /// <summary>
        /// Thêm mới
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<int> CreateMaintenanceSchedule(AmMaintenanceScheduleAddEditModel requestModel);

        /// <summary>
        /// Cập nhật
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateMaintenanceSchedule(AmMaintenanceScheduleAddEditModel requestModel);

        /// <summary>
        /// Xóa
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseBaseModel DeleteMaintenanceSchedule(int id, int userID);
        
        /// <summary>
        /// Tạo bảo trì tự quản
        /// </summary>
        /// <param name="maintenanceScheduleId"></param>
        /// <returns></returns>
        int CreateAM(int maintenanceScheduleId);
    }
}
