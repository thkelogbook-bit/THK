using Elogbook.Core.Enums;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request.RegistrationTicket;
using Elogbook.Core.Models.Response;
using System.Collections.Generic;
using System.Net.Http;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Định nghĩa các phương thức đăng ký Phiếu công tác
    /// </summary>
    public interface IRegistrationTicketService
    {
        /// <summary>
        /// Kiểm tra tồn tại Số phiếu đăng ký công tác/Căn cứ biên bản số
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ticketNumber"></param>
        /// <param name="isAddMode"></param>
        /// <param name="isTicketNumber"></param>
        /// <returns></returns>
        bool CheckDuplicateData(int id, string ticketNumber, bool isAddMode, bool isTicketNumber);

        /// <summary>
        /// kiểm tra trùng dữ liệu theo số phiếu đăng ký công tác
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ticketNumber"></param>
        /// <param name="isAddMode"></param>
        /// <returns></returns>. 
        bool CheckDuplicateData(int id, string ticketNumber, bool isAddMode);

        /// <summary>
        /// Tạo mới phiếu đăng ký công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        int AddRegistrationTicket(AddEditRegistrationTicketModel requestModel);

        /// <summary>
        /// Tìm kiếm Phiếu công tác theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        RegistrationTicketModel GetRegistrationTicketById(int id);

        /// <summary>
        /// Tìm kiếm những phiếu đăng ký công tác theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetRegistrationTickets(SearchRegistrationTicketModel requestModel);

        /// <summary>
        /// Thay đổi trạng thái phiếu công tác
        /// </summary>
        /// <param name="id"></param>
        /// <param name="statusValue"></param>
        /// <param name="description"></param>
        /// <param name="userID"></param>
        /// <param name="lstAttachment"></param>
        /// <returns></returns>
        bool UpdateStatusRegistrationTicket(int id, int statusValue, string description, int userID, string lstAttachment);

        /// <summary>
        /// Cập nhật phiếu đăng ký công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        bool UpdateRegistrationTicket(AddEditRegistrationTicketModel requestModel);

        /// <summary>
        /// Xóa phiếu đăng ký công tác
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentUserID"></param>
        /// <returns></returns>
        bool DeleteRegistrationTicket(int id, int currentUserID);

        /// <summary>
        /// Tìm kiếm trạng thái phiếu đăng ký công tác theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseBaseModel GetListTicketStatusHistoryById(int id);

        /// <summary>
        /// kiểm tra user có đang được sử dụng ở phiếu công tác khác
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
        List<StakeholdersResponseModel> GetStakeholders(int registrationTicketID, RolesStakeholder rolesStakeholder);

        /// <summary>
        /// In file PDF phiếu đăng ký công tác
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        HttpResponseMessage ExportPdfFile(int id);

        /// <summary>
        /// Download file mẫu thông tin phiếu đăng ký công tác
        /// </summary>
        /// <returns></returns>
        HttpResponseMessage DownloadFileTemplate();
    }
}
