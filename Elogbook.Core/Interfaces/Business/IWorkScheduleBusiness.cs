using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Phương thức xử lý quản lý lịch làm việc
    /// </summary>
    public interface IWorkScheduleBusiness
    {
        /// <summary>
        /// Thêm lịch làm việc
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> AddEditWorkSchedule(WorkScheduleRequestModel requestModel);

        /// <summary>
        /// Lấy dữ liệu khởi tạo cho màn hình quản lý lịch làm việc
        /// </summary>
        /// <returns></returns>
        ResponseModel<WorkScheduleResponseModel> GetInitialData();

        /// <summary>
        /// Lấy danh sách lịch làm việc
        /// </summary>
        /// <returns></returns>
        ResponseBaseModel GetAllHolidaySchedule();

        /// <summary>
        /// Lấy chi tiết lịch làm việc theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel<Core.Models.Response.HolidayScheduleResponseModel> GetHolidayScheduleById(int id);

        /// <summary>
        /// Thêm hoặc chỉnh sửa lịch làm việc
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        ResponseModel<bool> AddEditHolidaySchedule(HolidayScheduleRequestModel requestModel, int currentUserId);

        /// <summary>
        /// Xóa lịch làm việc
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        ResponseModel<bool> DeleteHolidaySchedule(int id, int currentUserId);
    }
}
