using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Microsoft.Web.Http;
using System.Net;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Nguyên nhân tạm dừng công việc AM
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/amReasonForPending")]
    public class AmReasonForPendingController : BaseApiController
    {
        private readonly IAmReasonForPendingBusiness _amReasonForPendingBusiness;
        private readonly ICommonBusiness _commonBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="amReasonForPendingBusiness"></param>
        /// <param name="commonBusiness"></param>
        public AmReasonForPendingController(IAmReasonForPendingBusiness amReasonForPendingBusiness,
            ICommonBusiness commonBusiness)
        {
            _amReasonForPendingBusiness = amReasonForPendingBusiness;
            _commonBusiness = commonBusiness;
        }

        /// <summary>
        /// Tìm kiếm Nguyên nhân tạm dừng công việc theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_QLNNTDCV
        [Route("find")]
        [HttpPost]
        public ResponseBaseModel GetReasonForPendings([FromBody] AmReasonForPendingSearchModel requestModel)
        {
            return _amReasonForPendingBusiness.GetReasonForPendings(requestModel);
        }

        /// <summary>
        /// Tìm kiếm Nguyên nhân tạm dừng công việc theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_QLNNTDCV
        [Route("{id}")]
        [HttpGet]
        public ResponseModel<AmReasonForPendingModel> GetReasonForPendingById([FromUri] int id)
        {
            return _amReasonForPendingBusiness.GetReasonForPendingById(id);
        }

        /// <summary>
        /// Thêm mới Nguyên nhân tạm dừng công việc 
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        public ResponseModel<bool> CreateReasonForPending([FromBody] AmReasonForPendingAddEditModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.ADD_QLNNTDCV)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _amReasonForPendingBusiness.CreateReasonForPending(requestModel);
        }

        /// <summary>
        /// Cập nhập Nguyên nhân tạm dừng công việc 
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPut]
        public ResponseModel<bool> UpdateReasonForPending([FromBody] AmReasonForPendingAddEditModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_QLNNTDCV)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _amReasonForPendingBusiness.UpdateReasonForPending(requestModel);
        }

        /// <summary>
        /// Xóa Nguyên nhân tạm dừng công việc 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        public ResponseBaseModel DeleteReasonForPending([FromUri] int id)
        {
            ResponseBaseModel responseModel = new ResponseBaseModel();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.DELETE_QLNNTDCV)).Data)
            {
                responseModel.Message = CommonConstant.USER_RIGHT;
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            return _amReasonForPendingBusiness.DeleteReasonForPending(id, CurrentUser.ID);
        }
    }
}
