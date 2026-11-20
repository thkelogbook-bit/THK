using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Request.Manipulation;
using Elogbook.Core.Models.Response;
using Elogbook.Core.Models.Response.Manipulation;
using Microsoft.Web.Http;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Quản lý phiếu thao tác
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/manipulation")]
    public class ManipulationMobileController : BaseApiController
    {
        private readonly IManipulationTicketBusiness _manipulationTicketBusiness;
        private readonly ICommonBusiness _commonBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="manipulationTicketBusiness"></param>
        /// <param name="commonBusiness"></param>
        public ManipulationMobileController(IManipulationTicketBusiness manipulationTicketBusiness,
            ICommonBusiness commonBusiness)
        {
            _manipulationTicketBusiness = manipulationTicketBusiness;
            _commonBusiness = commonBusiness;
        }

        /// <summary>
        /// Trả về tất cả phiếu thao tác theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// EnumUserPermission.VIEW_PTT_MOBILE
        /// <returns></returns>
        [Route("find")]
        [HttpPost]
        public ResponseBaseModel<TicketSearchMobileModel> Find([FromBody] SearchTicketMobileModel requestModel)
        {
            return _manipulationTicketBusiness.Find(requestModel);
        }

        /// <summary>
        /// Trả về tất cả số lượng phiếu thao tác theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// EnumUserPermission.VIEW_PTT_MOBILE
        /// <returns></returns>
        [Route("findMobileCount")]
        [HttpPost]
        public ResponseModel<DataCountModel> FindMobileCount([FromBody] SearchTicketMobileModel requestModel)
        {
            return _manipulationTicketBusiness.FindMobileCount(requestModel);
        }

        /// <summary>
        /// Trả về một phiếu thao tác
        /// </summary>
        /// <param name="id"></param>
        /// EnumUserPermission.VIEW_PTT_MOBILE
        /// <returns></returns>
        [Route("detail")]
        [HttpGet]
        public ResponseModel<TicketDetailModel> GetWorkTicketById(int id)
        {
            return _manipulationTicketBusiness.GetWorkTicketById(id);
        }

        /// <summary>
        /// Cập nhật thao tác hàng ngày
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("sequenceOperation/update")]
        [HttpPost]
        public ResponseModel<bool> UpdatesequenceOperation([FromBody] UpdateSequenceOperationModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_PTT_MOBILE)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _manipulationTicketBusiness.UpdateWork(requestModel);
        }

        /// <summary>
        /// hủy phiếu
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("cancel")]
        [HttpPut]
        public ResponseModel<bool> Cancel([FromBody] CancelTicketModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_PTT_MOBILE)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _manipulationTicketBusiness.Cancel(requestModel);
        }

        /// <summary>
        /// Kết thúc công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("close")]
        [HttpPut]
        public ResponseModel<bool> CloseTicket([FromBody] CloseTicketModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_PTT_MOBILE)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _manipulationTicketBusiness.CloseTicket(requestModel);
        }

        /// <summary>
        /// Lấy Trình tự thao tác
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [Route("sequenceOperation/get")]
        [HttpGet]
        public ResponseModel<List<SequenceOperationModel>> GetSequenceOperation(int ID)
        {
            return _manipulationTicketBusiness.GetSequenceOperation(ID);
        }

        /// <summary>
        /// Lấy danh sách users
        /// </summary>
        /// <param name="TicketID"></param>
        /// <param name="Role"></param>
        /// <returns></returns>
        [Route("users")]
        [HttpGet]
        public ResponseModel<List<UserInTicketModel>> GetUsersInTicket(int TicketID, int Role)
        {
            return _manipulationTicketBusiness.GetUsersInTicket(TicketID, Role);
        }

        /// <summary>
        /// Đồng bộ dữ liệu với mobile app
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("syncData")]
        [HttpPost]
        public ResponseBaseModel SyncData([FromBody] ManipulationTicketSyncDataModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _manipulationTicketBusiness.SyncData(requestModel);
        }
    }
}
