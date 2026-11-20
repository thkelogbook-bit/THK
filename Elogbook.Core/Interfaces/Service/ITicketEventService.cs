using Elogbook.Core.Models;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Quản lý sự kiện
    /// </summary>
    public interface ITicketEventService
    {
        /// <summary>
        /// Tìm kiếm sự kiện theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetTicketEvents(SearchTicketEventModel requestModel);

        /// <summary>
        /// Tìm kiếm sự kiện theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TicketEventModel GetTicketEventById(int id);

        /// <summary>
        /// Thêm mới sự kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        bool CreateTicketEvent(AddEditTicketEventModel requestModel);

        /// <summary>
        /// Cập nhật sự kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        bool UpdateTicketEvent(AddEditTicketEventModel requestModel);

        /// <summary>
        /// Xóa sự kiện
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        bool DeleteTicketEvent(int id, int userID);

        /// <summary>
        /// Kiểm tra tồn tại loại sự kiện
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dataType"></param>
        /// <param name="eventTypeID"></param>
        /// <param name="factoryID"></param>
        /// <param name="isAddMode"></param>
        /// <returns></returns>
        bool CheckDuplicateData(int id, int dataType, int eventTypeID, int factoryID, bool isAddMode);
    }
}
