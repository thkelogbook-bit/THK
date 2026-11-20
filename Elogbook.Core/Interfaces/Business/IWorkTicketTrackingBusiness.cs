using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System.Collections.Generic;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Interface WorkTicketTrackingBusiness
    /// </summary>
    public interface IWorkTicketTrackingBusiness
    {
        /// <summary>
        /// Tìm kiếm WorkTicketTracking theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel<WorkTicketTrackingModel> GetWorkTicketTrackingById(int id);

        /// <summary>
        /// Người cấp phiếu xét duyệt đơn yêu cầu thay đổi người cho phép
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseModel<bool> ApproveWorkTicketTracking(int id, int userID);

        /// <summary>
        /// Vận hành viên tạo đơn yêu cầu thay đổi người cho phép
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> CreateWorkTicketTracking(AddWorkTicketTrackingModel requestModel);

        /// <summary>
        /// Vận hành viên hủy đơn yêu cầu thay đổi người cho phép
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseModel<bool> DeleteWorkTicketTracking(int id, int userID);

        /// <summary>
        /// Người cấp phiếu từ chối đơn yêu cầu thay đổi người cho phép
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        ResponseModel<bool> RejectWorkTicketTracking(int id, int userID, string reason);

        /// <summary>
        /// Tìm kiếm tất cả phiếu công tác theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetAllWorkTickets(SearchWorkTicketModel requestModel);

        /// <summary>
        /// Tìm kiếm phiếu công tác theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel<WorkTicketDetailModel> GetDetailWorkTicket(int id);

        /// <summary>
        /// Lấy trạng thái thay đổi người cho phép
        /// </summary>
        /// <returns></returns>
        List<ValueLabelModel> GetTrackingStatus();
    }
}
