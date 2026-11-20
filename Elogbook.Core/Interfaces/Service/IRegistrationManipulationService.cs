using Elogbook.Core.Models;
using Elogbook.Core.Models.Request.Manipulation;
using Elogbook.Core.Models.Response.Manipulation;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// IRegistrationManipulationService
    /// </summary>
    public interface IRegistrationManipulationService
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
        /// <param name="isTicketNumber"></param>
        /// <returns></returns>
        bool CheckDuplicateData(int id, string ticketName, bool isAddMode, bool isTicketNumber);

        /// <summary>
        /// Thêm mới phiếu thao tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        int Add(AddEditTicketModel requestModel);

        /// <summary>
        /// cập nhật phiếu thao tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        bool Update(AddEditTicketModel requestModel);

        /// <summary>
        /// Tìm kiếm phiếu thao tác theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ManipulationTicketDetailModel GetManipulationTicketDetailById(int id);

        /// <summary>
        /// Cập nhật trạng thái phiếu thao tác
        /// </summary>
        /// <param name="id"></param>
        /// <param name="statusValue"></param>
        /// <param name="description"></param>
        /// <param name="userID"></param>
        /// <param name="lstAttachment"></param>
        /// <returns></returns>
        bool UpdateStatus(int id, int statusValue, string description, int userID, string lstAttachment);

        /// <summary>
        /// Xoá phiếu thao tác
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        bool DeleteTicket(int id, int userID);

        /// <summary>
        /// Tìm kiếm trạng thái phiếu thao tác theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseBaseModel GetStatusById(int id);

        /// <summary>
        /// Duyệt phiếu thao tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        int AddApprovalTicket(AddManipulationTicketInfoModel requestModel);

        /// <summary>
        /// Tìm kiếm phiếu mẫu theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel FindTemplate(Models.Request.Manipulation.SearchTemplateTicketModel requestModel);

        /// <summary>
        /// kiểm tra dữ liệu phiếu mẫu
        /// </summary>
        /// <param name="id"></param>
        /// <param name="requestModel"></param>
        /// <param name="isAddMode"></param>
        /// <returns></returns>
        bool CheckDuplicateDataTemplate(int id, AddEditTemplateTicketModel requestModel, bool isAddMode);

        /// <summary>
        /// tạo phiếu thao tác mẫu
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        int AddTemplateTicket(AddEditTemplateTicketModel requestModel);

        /// <summary>
        /// Xoá phiếu thao tác mẫu
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        bool DeleteTemplateTicket(int id, int userID);

        /// <summary>
        /// cập nhật phiếu thao tác mẫu
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        bool UpdateTemplateTicket(AddEditTemplateTicketModel requestModel);

        /// <summary>
        /// Tìm kiếm phiếu thao tác mẫu theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DetailTemplateTicketModel FindTemplateId(int id);

        /// <summary>
        /// Tìm kiếm phiếu mẫu đang active
        /// </summary>
        /// <returns></returns>
        ResponseBaseModel FindTemplateActive();
    }
}
