using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Phương thức xử lý dịch vụ quản lý lịch làm việc
    /// </summary>
    public interface IWorkScheduleService
    {
        /// <summary>
        /// Thêm lịch làm việc mới
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        bool AddEditWorkSchedule(WorkScheduleRequestModel requestModel);

        /// <summary>
        /// Lấy dữ liệu ban đầu cho lịch làm việc
        /// </summary>
        /// <returns></returns>
        WorkScheduleResponseModel GetInitialData();

        /// <summary>
        /// Lấy danh sách lịch nghỉ lễ
        /// </summary>
        /// <returns></returns>
        List<Elogbook.Core.Models.Response.HolidayScheduleResponseModel> GetAllHolidaySchedule();

        /// <summary>
        /// Lấy thông tin chi tiết của lịch nghỉ lễ theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Elogbook.Core.Models.Response.HolidayScheduleResponseModel GetHolidayScheduleById(int id);

        /// <summary>
        /// Thêm hoặc chỉnh sửa lịch nghỉ lễ
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        bool AddEditHolidaySchedule(HolidayScheduleRequestModel requestModel, int currentUserId);

        /// <summary>
        /// Xóa lịch nghỉ lễ theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        bool DeleteHolidaySchedule(int id, int currentUserId);
    }
}
