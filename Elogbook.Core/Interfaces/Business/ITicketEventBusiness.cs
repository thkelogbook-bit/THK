using Elogbook.Core.Models;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Quản lý sự kiện
    /// </summary>
    public interface ITicketEventBusiness
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
        ResponseModel<TicketEventModel> GetTicketEventById(int id);

        /// <summary>
        /// Thêm mới sự kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> CreateTicketEvent(AddEditTicketEventModel requestModel);

        /// <summary>
        /// Cập nhật sự kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateTicketEvent(AddEditTicketEventModel requestModel);

        /// <summary>
        /// Xóa sự kiện
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseModel<bool> DeleteTicketEvent(int id, int userID);
    }
}
