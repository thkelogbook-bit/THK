using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Microsoft.Web.Http;
using System.Net;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V2
{
    /// <summary>
    /// Quản lý phiếu yêu cầu
    /// </summary>
    [ApiVersion("2.0")]
    [RoutePrefix("v{version:apiVersion}/amRequestTicket")]
    public class AmRequestTicketController : BaseApiController
    {
        private readonly IAmRequestTicketBusiness _amRequestTicketBusiness;
        private readonly ICommonBusiness _commonBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="amRequestTicketBusiness"></param>
        /// <param name="commonBusiness"></param>
        public AmRequestTicketController(IAmRequestTicketBusiness amRequestTicketBusiness,
            ICommonBusiness commonBusiness)
        {
            _amRequestTicketBusiness = amRequestTicketBusiness;
            _commonBusiness = commonBusiness;
        }

        /// <summary>
        /// Thêm mới phiếu yêu cầu
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        public ResponseModel<AmRequestTicketModel> Add([FromBody] AmAddEditRequestTicketModel requestModel)
        {
            ResponseModel<AmRequestTicketModel> responseModel = new ResponseModel<AmRequestTicketModel>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.ADD_QLPYCVHV)).Data
                && !_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.ADD_QLPYCTC)).Data
                && !_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.ADD_QLPYCDVSC)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _amRequestTicketBusiness.Add(requestModel);
        }

        /// <summary>
        /// Trả về phiếu yêu cầu theo mã số
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpGet]
        public ResponseModel<AmRequestTicketModel> GetById([FromUri] int id)
        {
            return _amRequestTicketBusiness.GetById(id);
        }

        /// <summary>
        /// Trả về tất cả phiếu yêu cầu theo điều kiện tìm kiếm
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("find")]
        [HttpPost]
        public ResponseBaseModel Find([FromBody] AmSearchRequestTicketModel requestModel)
        {
            return _amRequestTicketBusiness.Find(requestModel);
        }

        /// <summary>
        /// Trả về tất cả phiếu yêu cầu theo điều kiện tìm kiếm (mobile)
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("findMobile")]
        [HttpPost]
        public ResponseBaseModel FindMobile([FromBody] AmSearchRequestTicketMobileModel requestModel)
        {
            return _amRequestTicketBusiness.FindMobile(requestModel);
        }

        /// <summary>
        /// Trả về tất cả số lượng phiếu yêu cầu theo điều kiện của mỗi tab (mobile)
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("findMobileCount")]
        [HttpPost]
        public ResponseModel<CountStatusRequestTicketMobileModel> FindMobileCount([FromBody] AmSearchRequestTicketMobileModel requestModel)
        {
            return _amRequestTicketBusiness.FindMobileCount(requestModel);
        }

        /// <summary>
        /// Cập nhập phiếu yêu cầu
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPut]
        public ResponseModel<AmRequestTicketModel> Update([FromBody] AmAddEditRequestTicketModel requestModel)
        {
            ResponseModel<AmRequestTicketModel> responseModel = new ResponseModel<AmRequestTicketModel>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_QLPYCVHV)).Data
                && !_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_QLPYCTC)).Data
                && !_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_QLPYCDVSC)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }
            requestModel.CurrentUserID = CurrentUser.ID;
            return _amRequestTicketBusiness.Update(requestModel);
        }

        /// <summary>
        /// Cập nhật trạng thái phiếu
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("changeStatus")]
        [HttpPut]
        public ResponseModel<bool> UpdateStatus([FromBody] ChangeStatusRequestModel request)
        {
            request.CurrentUserID = CurrentUser.ID;
            return _amRequestTicketBusiness.UpdateStatus(request);
        }

        /// <summary>
        /// Xóa phiếu yêu cầu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        public ResponseModel<bool> Delete([FromUri] int id)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.DELETE_QLPYCVHV)).Data
                && !_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.DELETE_QLPYCTC)).Data
                && !_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.DELETE_QLPYCDVSC)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            return _amRequestTicketBusiness.Delete(id, CurrentUser.ID);
        }

        /// <summary>
        /// Đếm số phiếu yêu cầu
        /// trạng thái đang xử lý với PYC trưởng ca
        /// trạng thái đã duyệt với PYC đơn vị sửa chữa
        /// </summary>
        /// <returns></returns>
        [Route("getCountNumberTicket")]
        [HttpGet]
        public ResponseModel<CountNumberTicketStatusModel> GetCountNumberTicket()
        {
            return _amRequestTicketBusiness.GetCountNumberTicket();
        }

        /// <summary>
        /// lấy data tạo phiếu đăng ký Phiếu Điện/TCNH
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="isCTD"></param>
        /// <returns></returns>
        [Route("getDataRegistrationTicket/{id}/{isCTD}")]
        [HttpGet]
        public ResponseModel<DataRegistrationTicketModel> GetDataRegistrationTicket(int Id, bool isCTD)
        {
            return _amRequestTicketBusiness.GetDataRegistrationTicket(Id, isCTD);
        }

        /// <summary>
        /// lấy data lịch sử phiếu yêu cầu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("getHistory/{id}")]
        [HttpGet]
        public ResponseBaseModel GetHistory(int id)
        {
            return _amRequestTicketBusiness.GetHistory(id);
        }

        /// <summary>
        /// lấy data thông tin liên quan phiếu yêu cầu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("getRelateInfo/{id}")]
        [HttpGet]
        public ResponseBaseModel GetRelateInfo(int id)
        {
            return _amRequestTicketBusiness.GetRelateInfo(id);
        }
    }
}