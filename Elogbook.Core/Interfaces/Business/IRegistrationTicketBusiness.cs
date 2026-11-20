using Elogbook.Core.Models;
using Elogbook.Core.Models.Request.RegistrationTicket;
using Elogbook.Core.Models.Response;
using System.Net.Http;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Định nghĩa các phương thức đăng ký Phiếu công tác
    /// </summary>
    public interface IRegistrationTicketBusiness
    {
        /// <summary>
        /// Tạo mới phiếu đăng ký công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<int> AddRegistrationTicket(AddEditRegistrationTicketModel requestModel);

        /// <summary>
        /// Tìm kiếm những phiếu đăng ký công tác theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetRegistrationTickets(SearchRegistrationTicketModel requestModel);

        /// <summary>
        /// Tìm kiếm Phiếu công tác theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel<RegistrationTicketModel> GetRegistrationTicketById(int id);

        /// <summary>
        /// Cập nhật trạng thái phiếu đăng ký công tác
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isSendTicket"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateStatusRegistrationTicket(int id, bool isSendTicket, int userID);

        /// <summary>
        /// Cập nhật phiếu đăng ký công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateRegistrationTicket(AddEditRegistrationTicketModel requestModel);

        /// <summary>
        /// Xóa phiếu đăng ký công tác
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentUserID"></param>
        /// <returns></returns>
        ResponseModel<bool> DeleteRegistrationTicket(int id, int currentUserID);

        /// <summary>
        /// Tìm kiếm phiếu đăng ký công tác theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseBaseModel GetStatusRegistrationTicketById(int id);

        /// <summary>
        /// In file PDF phiếu đăng ký công tác
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        HttpResponseMessage ExportPdfFile(int id);

        /// <summary>
        /// Download file mẫu phiếu đăng ký công tác
        /// </summary>
        HttpResponseMessage DownloadFileTemplate();
    }
}
