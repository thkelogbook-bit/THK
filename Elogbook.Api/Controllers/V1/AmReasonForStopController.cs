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
    /// Nguyên nhân dừng thực hiện AM
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/amReasonForStop")]
    public class AmReasonForStopController : BaseApiController
    {
        private readonly IAmReasonForStopBusiness _amReasonForStopBusiness;
        private readonly ICommonBusiness _commonBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="amReasonForStopBusiness"></param>
        /// <param name="commonBusiness"></param>
        public AmReasonForStopController(IAmReasonForStopBusiness amReasonForStopBusiness,
            ICommonBusiness commonBusiness)
        {
            _amReasonForStopBusiness = amReasonForStopBusiness;
            _commonBusiness = commonBusiness;
        }

        /// <summary>
        /// Tìm kiếm Nguyên nhân dừng thực hiện theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_QLNNKTH
        [Route("find")]
        [HttpPost]
        public ResponseBaseModel GetReasonForStops([FromBody] AmReasonForStopSearchModel requestModel)
        {
            return _amReasonForStopBusiness.GetReasonForStops(requestModel);
        }

        /// <summary>
        /// Tìm kiếm Nguyên nhân dừng thực hiện theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_QLNNKTH
        [Route("{id}")]
        [HttpGet]
        public ResponseModel<AmReasonForStopModel> GetReasonForStopById([FromUri] int id)
        {
            return _amReasonForStopBusiness.GetReasonForStopById(id);
        }

        /// <summary>
        /// Thêm mới Nguyên nhân dừng thực hiện 
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        public ResponseModel<bool> CreateReasonForStop([FromBody] AmReasonForStopAddEditModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.ADD_QLNNKTH)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _amReasonForStopBusiness.CreateReasonForStop(requestModel);
        }

        /// <summary>
        /// Cập nhập Nguyên nhân dừng thực hiện 
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPut]
        public ResponseModel<bool> UpdateReasonForStop([FromBody] AmReasonForStopAddEditModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_QLNNKTH)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _amReasonForStopBusiness.UpdateReasonForStop(requestModel);
        }

        /// <summary>
        /// Xóa Nguyên nhân dừng thực hiện 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        public ResponseBaseModel DeleteReasonForStop([FromUri] int id)
        {
            ResponseBaseModel responseModel = new ResponseBaseModel();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.DELETE_QLNNKTH)).Data)
            {
                responseModel.Message = CommonConstant.USER_RIGHT;
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            return _amReasonForStopBusiness.DeleteReasonForStop(id, CurrentUser.ID);
        }
    }
}
