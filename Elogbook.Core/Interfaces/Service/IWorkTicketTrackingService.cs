using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using System.Collections.Generic;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Interface WorkTicketTracking Service
    /// </summary>
    public interface IWorkTicketTrackingService
    {
        /// <summary>
        /// Tìm kiếm WorkTicketTracking theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        WorkTicketTrackingModel GetWorkTicketTrackingById(int id);

        /// <summary>
        /// Xóa WorkTicketTracking
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        bool ApproveWorkTicketTracking(int id, int userID);

        /// <summary>
        /// Thêm mới WorkTicketTracking
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        bool CreateWorkTicketTracking(AddWorkTicketTrackingModel requestModel);

        /// <summary>
        /// Xóa WorkTicketTracking
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        bool DeleteWorkTicketTracking(int id, int userID);

        /// <summary>
        /// Từ chối WorkTicketTracking
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        bool RejectWorkTicketTracking(int id, int userID, string reason);

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
        WorkTicketDetailModel GetDetailWorkTicket(int id);

        /// <summary>
        /// Danh sách trạng thái thay đổi người cho phép
        /// </summary>
        /// <returns></returns>
        List<ValueLabelModel> GetTrackingStatus();
    }
}
