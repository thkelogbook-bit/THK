using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Microsoft.Web.Http;
using System.Collections.Generic;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Quản lý đơn yêu cầu thay đổi người cho phép
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/workTicketTracking")]
    public class WorkTicketTrackingController : BaseApiController
    {
        private readonly IWorkTicketTrackingBusiness _workTicketTrackingBusiness;

        /// <summary>
        /// Constuctor
        /// </summary>
        /// <param name="workTicketTrackingBusiness"></param>
        public WorkTicketTrackingController(IWorkTicketTrackingBusiness workTicketTrackingBusiness)
        {
            _workTicketTrackingBusiness = workTicketTrackingBusiness;
        }

        /// <summary>
        /// Tìm kiếm theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpGet]
        public ResponseModel<WorkTicketTrackingModel> GetWorkTicketTrackingById([FromUri] int id)
        {
            return _workTicketTrackingBusiness.GetWorkTicketTrackingById(id);
        }

        /// <summary>
        /// Vận hành viên tạo đơn yêu cầu thay đổi người cho phép
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        public ResponseModel<bool> CreateWorkTicketTracking([FromBody] AddWorkTicketTrackingModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _workTicketTrackingBusiness.CreateWorkTicketTracking(requestModel);
        }

        /// <summary>
        /// Người cấp phiếu xét duyệt đơn yêu cầu thay đổi người cho phép
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPut]
        public ResponseModel<bool> ApproveWorkTicketTracking([FromUri] int id)
        {
            return _workTicketTrackingBusiness.ApproveWorkTicketTracking(id, CurrentUser.ID);
        }

        /// <summary>
        /// Vận hành viên hủy đơn yêu cầu thay đổi người cho phép
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        public ResponseModel<bool> DeleteWorkTicketTracking([FromUri] int id)
        {
            return _workTicketTrackingBusiness.DeleteWorkTicketTracking(id, CurrentUser.ID);
        }

        /// <summary>
        /// Người cấp phiếu từ chối đơn yêu cầu thay đổi người cho phép
        /// </summary>
        /// <param name="id"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        [Route("reject")]
        [HttpPut]
        public ResponseModel<bool> RejectWorkTicketTracking([FromUri] int id, string reason)
        {
            return _workTicketTrackingBusiness.RejectWorkTicketTracking(id, CurrentUser.ID, reason);
        }

        /// <summary>
        /// Trả về tất cả phiếu công tác theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("getAllWorkTickets")]
        [HttpPost]
        public ResponseBaseModel GetAllWorkTickets([FromBody] SearchWorkTicketModel requestModel)
        {
            return _workTicketTrackingBusiness.GetAllWorkTickets(requestModel);
        }

        /// <summary>
        /// Trả về một phiếu công tác
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("getDetailWorkTicket/{id}")]
        [HttpGet]
        public ResponseModel<WorkTicketDetailModel> GetDetailWorkTicket([FromUri] int id)
        {
            return _workTicketTrackingBusiness.GetDetailWorkTicket(id);
        }

        /// <summary>
        /// Lấy trạng thái thay đổi người cho phép
        /// </summary>
        /// <returns></returns>
        [Route("getTrackingStatus")]
        [HttpGet]
        public List<ValueLabelModel> GetTrackingStatus()
        {
            return _workTicketTrackingBusiness.GetTrackingStatus();
        }
    }
}
