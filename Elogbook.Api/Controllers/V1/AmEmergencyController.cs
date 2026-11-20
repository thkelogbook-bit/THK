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
    /// Quản lý mức độ khẩn cấp
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/amEmergency")]
    public class AmEmergencyController : BaseApiController
    {
        private readonly IAmEmergencyBusiness _emergencyBusiness;
        private readonly ICommonBusiness _commonBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="emergencyBusiness"></param>
        /// <param name="commonBusiness"></param>
        public AmEmergencyController(IAmEmergencyBusiness emergencyBusiness,
            ICommonBusiness commonBusiness)
        {
            _emergencyBusiness = emergencyBusiness;
            _commonBusiness = commonBusiness;
        }

        /// <summary>
        /// Tìm kiếm theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_QLMDKC
        [Route("find")]
        [HttpPost]
        public ResponseBaseModel GetEmergencys([FromBody] AmSearchEmergencyModel requestModel)
        {
            return _emergencyBusiness.Find(requestModel);
        }

        /// <summary>
        /// Tìm kiếm theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_QLMDKC
        [Route("{id}")]
        [HttpGet]
        public ResponseModel<AmEmergencyModel> GetEmergencyById([FromUri] int id)
        {
            return _emergencyBusiness.GetById(id);
        }

        /// <summary>
        /// Thêm mới
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        public ResponseModel<AmEmergencyModel> CreateEmergency([FromBody] AmAddEditEmergencyModel requestModel)
        {
            ResponseModel<AmEmergencyModel> responseModel = new ResponseModel<AmEmergencyModel>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.ADD_QLMDKC)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _emergencyBusiness.Add(requestModel);
        }

        /// <summary>
        /// Cập nhập
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPut]
        public ResponseModel<AmEmergencyModel> UpdateEmergency([FromBody] AmAddEditEmergencyModel requestModel)
        {
            ResponseModel<AmEmergencyModel> responseModel = new ResponseModel<AmEmergencyModel>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_QLMDKC)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _emergencyBusiness.Update(requestModel);
        }

        /// <summary>
        /// Xóa
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        public ResponseModel<bool> DeleteEmergency([FromUri] int id)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.DELETE_QLMDKC)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            return _emergencyBusiness.Delete(id, CurrentUser.ID);
        }
    }
}
