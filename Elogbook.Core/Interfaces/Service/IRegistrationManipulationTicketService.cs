using Elogbook.Core.Models;
using Elogbook.Core.Models.Request.Manipulation;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Định nghĩa các phương thức cho chức năng đăng ký phiếu thao tác
    /// </summary>
    public interface IRegistrationManipulationTicketService
    {
        /// <summary>
        /// Tìm kiếm phiếu thao tác theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel Find(SearchRegistrationManipulationTicketModel requestModel);

        /// <summary>
        /// kiểm tra dữ liệu có trùng lặp
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ticketName"></param>
        /// <param name="isAddMode"></param>
        /// <returns></returns>
        bool CheckDuplicateData(int id, string ticketName, bool isAddMode);

        /// <summary>
        /// Thêm mới phiếu thao tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        int Add(AddEditTicketModel requestModel);
    }
}
