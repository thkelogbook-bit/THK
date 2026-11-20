using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Định nghĩa các phương thức cho system schedule
    /// </summary>
    public interface IScheduleBusiness
    {
        /// <summary>
        /// Tìm kiếm schedule theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetSchedules(SearchScheduleRequestModel requestModel);

        /// <summary>
        /// Thông tin của schedule
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel<ScheduleResponseModel> GetScheduleById(int id);

        /// <summary>
        /// Thêm mới schedule
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<ScheduleResponseModel> AddSchedule(AddEditScheduleRequestModel requestModel);

        /// <summary>
        /// Cập nhật schedule
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<ScheduleResponseModel> UpdateSchedule(AddEditScheduleRequestModel requestModel);

        /// <summary>
        /// Xóa schedule
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseModel<bool> DeleteSchedule(int id, int userID);

        /// <summary>
        /// Kiểm tra schedule đang sử dụng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel<bool> CheckInUseSchedule(int id);

        /// <summary>
        /// Cron job description
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<string> GetCronJobDescription(CronJobDescriptionRequestModel requestModel);

    }
}
