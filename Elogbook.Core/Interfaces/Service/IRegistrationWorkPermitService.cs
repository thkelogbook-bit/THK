using Elogbook.Core.Enums;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request.WorkPermit;
using Elogbook.Core.Models.Response.WorkPermit;
using System.Collections.Generic;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Định nghĩa các phương thức đăng ký lệnh công tác
    /// </summary>
    public interface IRegistrationWorkPermitService
    {
        /// <summary>
        /// Kiểm tra tồn tại Số lệnh đăng ký công tác/Căn cứ biên bản số
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ticketNumber"></param>
        /// <param name="isAddMode"></param>
        /// <param name="isTicketNumber"></param>
        /// <returns></returns>
        bool CheckDuplicateData(int id, string ticketNumber, bool isAddMode, bool isTicketNumber);

        /// <summary>
        /// kiểm tra trùng dữ liệu theo số lệnh đăng ký công tác
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ticketNumber"></param>
        /// <param name="isAddMode"></param>
        /// <returns></returns>. 
        bool CheckDuplicateData(int id, string ticketNumber, bool isAddMode);

        /// <summary>
        /// Tạo mới lệnh đăng ký công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        int AddRegistrationTicket(LctAddEditRegistrationTicketModel requestModel);

        /// <summary>
        /// Tìm kiếm lệnh công tác theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        LctRegistrationTicketModel GetRegistrationTicketById(int id);

        /// <summary>
        /// Tìm kiếm những lệnh đăng ký công tác theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetRegistrationTickets(LctSearchRegistrationTicketModel requestModel);

        /// <summary>
        /// Thay đổi trạng thái lệnh công tác
        /// </summary>
        /// <param name="id"></param>
        /// <param name="statusValue"></param>
        /// <param name="description"></param>
        /// <param name="userID"></param>
        /// <param name="lstAttachment"></param>
        /// <returns></returns>
        bool UpdateStatusRegistrationTicket(int id, int statusValue, string description, int userID, string lstAttachment);

        /// <summary>
        /// Cập nhật lệnh đăng ký công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        bool UpdateRegistrationTicket(LctAddEditRegistrationTicketModel requestModel);

        /// <summary>
        /// Xóa lệnh đăng ký công tác
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentUserID"></param>
        /// <returns></returns>
        bool DeleteRegistrationTicket(int id, int currentUserID);

        /// <summary>
        /// Tìm kiếm trạng thái lệnh đăng ký công tác theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseBaseModel GetListTicketStatusHistoryById(int id);

        /// <summary>
        /// kiểm tra user có đang được sử dụng ở lệnh công tác khác
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        bool CheckUserIsUse(int userID);

        /// <summary>
        /// Lấy danh sách người trong ticket
        /// </summary>
        /// <param name="registrationTicketID"></param>
        /// <param name="rolesStakeholder"></param>
        /// <returns></returns>
        List<LctStakeholdersResponseModel> GetStakeholders(int registrationTicketID, RolesStakeholder rolesStakeholder);
    }
}
