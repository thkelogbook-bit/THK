using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Microsoft.Web.Http;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Loại công việc AM
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/amWorkType")]
    public class AmWorkTypeController : BaseApiController
    {
        private readonly IAmWorkTypeBusiness _workTypeBusiness;
        private readonly ICommonBusiness _commonBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="workTypeBusiness"></param>
        /// <param name="commonBusiness"></param>
        public AmWorkTypeController(IAmWorkTypeBusiness workTypeBusiness,
            ICommonBusiness commonBusiness)
        {
            _workTypeBusiness = workTypeBusiness;
            _commonBusiness = commonBusiness;
        }

        /// <summary>
        /// Tìm kiếm loại công việc theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_QLLCVAM
        [Route("find")]
        [HttpPost]
        public ResponseBaseModel GetWorkTypes([FromBody] AmWorkTypeSearchModel requestModel)
        {
            return _workTypeBusiness.GetWorkTypes(requestModel);
        }

        /// <summary>
        /// Tìm kiếm loại công việc theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_QLLCVAM
        [Route("{id}")]
        [HttpGet]
        public ResponseModel<AmWorkTypeModel> GetWorkTypeById([FromUri] int id)
        {
            return _workTypeBusiness.GetWorkTypeById(id);
        }

        /// <summary>
        /// Thêm mới loại công việc 
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        public ResponseModel<bool> CreateWorkType([FromBody] AmWorkTypeAddEditModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.ADD_QLLCVAM)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _workTypeBusiness.CreateWorkType(requestModel);
        }

        /// <summary>
        /// Cập nhập loại công việc 
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPut]
        public ResponseModel<bool> UpdateWorkType([FromBody] AmWorkTypeAddEditModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_QLLCVAM)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _workTypeBusiness.UpdateWorkType(requestModel);
        }

        /// <summary>
        /// Xóa loại công việc 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        public ResponseBaseModel DeleteWorkType([FromUri] int id)
        {
            ResponseBaseModel responseModel = new ResponseBaseModel();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.DELETE_QLLCVAM)).Data)
            {
                responseModel.Message = CommonConstant.USER_RIGHT;
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            return _workTypeBusiness.DeleteWorkType(id, CurrentUser.ID);
        }

        /// <summary>
        ///  Danh sách loại công việc theo nhà máy
        /// </summary>
        /// <param name="factoryID"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_QLLCVAM
        [Route("getWorkTypeByFactoryID")]
        [HttpGet]
        public List<ComboboxDataModel> GetWorkTypeByFactoryID(int factoryID)
        {
            return _workTypeBusiness.GetWorkTypeByFactoryID(factoryID);
        }
    }
}
