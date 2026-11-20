using Elogbook.Core.Models;
using Elogbook.Core.Models.Request.WorkPermit;
using Elogbook.Core.Models.Response.WorkPermit;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Định nghĩa các phương thức đăng ký lệnh công tác
    /// </summary>
    public interface IRegistrationWorkPermitBusiness
    {
        /// <summary>
        /// Tạo mới lệnh đăng ký công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<int> AddRegistrationTicket(LctAddEditRegistrationTicketModel requestModel);

        /// <summary>
        /// Tìm kiếm những lệnh đăng ký công tác theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetRegistrationTickets(LctSearchRegistrationTicketModel requestModel);

        /// <summary>
        /// Tìm kiếm lệnh công tác theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel<LctRegistrationTicketModel> GetRegistrationTicketById(int id);

        /// <summary>
        /// Cập nhật trạng thái lệnh đăng ký công tác
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isSendTicket"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateStatusRegistrationTicket(int id, bool isSendTicket, int userID);

        /// <summary>
        /// Cập nhật lệnh đăng ký công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateRegistrationTicket(LctAddEditRegistrationTicketModel requestModel);

        /// <summary>
        /// Xóa lệnh đăng ký công tác
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentUserID"></param>
        /// <returns></returns>
        ResponseModel<bool> DeleteRegistrationTicket(int id, int currentUserID);

        /// <summary>
        /// Tìm kiếm lệnh đăng ký công tác theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseBaseModel GetStatusRegistrationTicketById(int id);
    }
}
