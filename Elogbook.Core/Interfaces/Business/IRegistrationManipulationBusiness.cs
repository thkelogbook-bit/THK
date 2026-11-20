using Elogbook.Core.Models;
using Elogbook.Core.Models.Request.Manipulation;
using Elogbook.Core.Models.Response.Manipulation;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// IRegistrationManipulationBusiness
    /// </summary>
    public interface IRegistrationManipulationBusiness
    {
        /// <summary>
        /// Tìm kiếm phiếu thao tác theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel Find(SearchRegistrationManipulationTicketModel requestModel);

        /// <summary>
        /// Thêm mới phiếu thao tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<int> Add(AddEditTicketModel requestModel);

        /// <summary>
        /// cập nhật phiếu thao tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> Update(AddEditTicketModel requestModel);

        /// <summary>
        /// Tìm kiếm phiếu thao tác theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel<ManipulationTicketDetailModel> GetManipulationTicketDetailById(int id);

        /// <summary>
        /// Cập nhật trạng thái phiếu thao tác
        /// </summary>
        /// <param name="id"></param>
        /// <param name="statusValue"></param>
        /// <param name="description"></param>
        /// <param name="attackments"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateStatus(int id, int statusValue, string description, string attackments, int userID);

        /// <summary>
        /// Xoá phiếu thao tác
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseModel<bool> DeleteTicket(int id, int userID);

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
        ResponseModel<int> AddApprovalTicket(AddManipulationTicketInfoModel requestModel);

        /// <summary>
        /// Tìm kiếm phiếu mẫu theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel FindTemplate(Models.Request.Manipulation.SearchTemplateTicketModel requestModel);

        /// <summary>
        /// tạo phiếu thao tác mẫu
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<int> AddTemplateTicket(AddEditTemplateTicketModel requestModel);

        /// <summary>
        /// Xoá phiếu thao tác mẫu
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        ResponseModel<bool> DeleteTemplateTicket(int id, int userId);

        /// <summary>
        /// cập nhật phiếu thao tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateTemplateTicket(AddEditTemplateTicketModel requestModel);

        /// <summary>
        /// Tìm kiếm phiếu thao tác mẫu theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel<DetailTemplateTicketModel> FindTemplateId(int id);

        /// <summary>
        /// Tìm kiếm phiếu mẫu đang active
        /// </summary>
        /// <returns></returns>
        ResponseBaseModel FindTemplateActive();
    }
}
