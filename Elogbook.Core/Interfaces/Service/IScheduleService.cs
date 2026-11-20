using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System.Threading.Tasks;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Định nghĩa các phương thức cho system schedule
    /// </summary>
    public interface IScheduleService
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
        ScheduleResponseModel GetScheduleById(int id);

        /// <summary>
        /// Thêm mới schedule
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ScheduleResponseModel AddSchedule(AddEditScheduleRequestModel requestModel);

        /// <summary>
        /// Cập nhật schedule
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ScheduleResponseModel UpdateSchedule(AddEditScheduleRequestModel requestModel);

        /// <summary>
        /// Xóa schedule
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        bool DeleteSchedule(int id, int userID);

        /// <summary>
        /// Kiểm tra schedule đang sử dụng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool CheckInUseSchedule(int id);

        /// <summary>
        /// Kiểm tra schedule đã tồn tại trong nhà máy
        /// </summary>
        /// <param name="scheduleID"></param>
        /// <param name="scheduleName"></param>
        /// <param name="isAddMode"></param>
        /// <returns></returns>
        bool CheckDuplicateData(int scheduleID, string scheduleName, bool isAddMode);

        /// <summary>
        /// Cron job description
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        string GetCronJobDescription(CronJobDescriptionRequestModel requestModel);

        /// <summary>
        /// Kiểm tra SlaConfig tồn tại
        /// </summary>
        /// <param name="slaConfigId"></param>
        /// <param name="slaCategoryId"></param>
        /// <returns></returns>
        bool SlaConfigExists(int slaConfigId, int slaCategoryId);

        /// <summary>
        /// Xóa ScheduleAM
        /// </summary>
        /// <param name="idAm"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        Task DeleteScheduleAM(string idAm, int userID);
    }
}
