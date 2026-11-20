using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request.WorkPermit;
using Elogbook.Core.Models.Response.WorkPermit;
using Microsoft.Web.Http;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Quản lý lệnh đăng ký công tác
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/registrationWorkPermit")]
    public class RegistrationWorkPermitController : BaseApiController
    {
        private readonly IRegistrationWorkPermitBusiness _registrationWorkPermitBusiness;

        public RegistrationWorkPermitController(IRegistrationWorkPermitBusiness registrationWorkPermitBusiness)
        {
            _registrationWorkPermitBusiness = registrationWorkPermitBusiness;
        }

        /// <summary>
        /// Thêm mới lệnh đăng ký công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        public ResponseModel<int> AddRegistrationTicket([FromBody] LctAddEditRegistrationTicketModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _registrationWorkPermitBusiness.AddRegistrationTicket(requestModel);
        }

        /// <summary>
        /// Tìm kiếm lệnh đăng ký công tác theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("find")]
        [HttpPost]
        public ResponseBaseModel GetRegistrationTickets([FromBody] LctSearchRegistrationTicketModel requestModel)
        {
            return _registrationWorkPermitBusiness.GetRegistrationTickets(requestModel);
        }

        /// <summary>
        /// Tìm kiếm lệnh đăng ký công tác theo id
        /// </summary>
        /// <param name="id">mã lệnh công tác</param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpGet]
        public ResponseModel<LctRegistrationTicketModel> GetRegistrationTicketById([FromUri] int id)
        {
            return _registrationWorkPermitBusiness.GetRegistrationTicketById(id);
        }

        /// <summary>
        /// Cập nhật trạng thái lệnh đăng ký công tác (Chờ cấp lệnh công tác/Rút lệnh đăng ký)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isSendTicket"></param>
        /// <returns></returns>
        [Route("updateStatus")]
        [HttpPut]
        public ResponseModel<bool> UpdateStatusRegistrationTicket([FromUri] int id, bool isSendTicket)
        {
            return _registrationWorkPermitBusiness.UpdateStatusRegistrationTicket(id, isSendTicket, CurrentUser.ID);
        }

        /// <summary>
        /// Cập nhật lệnh đăng ký công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPut]
        public ResponseModel<bool> UpdateRegistrationTicket([FromBody] LctAddEditRegistrationTicketModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _registrationWorkPermitBusiness.UpdateRegistrationTicket(requestModel);
        }

        /// <summary>
        /// Xoá lệnh đăng ký công tác
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        public ResponseModel<bool> DeleteRegistrationTicket([FromUri] int id)
        {
            return _registrationWorkPermitBusiness.DeleteRegistrationTicket(id, CurrentUser.ID);
        }

        /// <summary>
        /// Tìm kiếm trạng thái lệnh đăng ký công tác theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("getStatusHistory/{id}")]
        [HttpGet]
        public ResponseBaseModel GetStatusRegistrationTicketById([FromUri] int id)
        {
            return _registrationWorkPermitBusiness.GetStatusRegistrationTicketById(id);
        }
    }
}