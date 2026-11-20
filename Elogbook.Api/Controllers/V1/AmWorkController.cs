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
    /// Công việc AM
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/amWork")]
    public class AmWorkController : BaseApiController
    {
        private readonly IAmWorkBusiness _workBusiness;
        private readonly ICommonBusiness _commonBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="workBusiness"></param>
        /// <param name="commonBusiness"></param>
        public AmWorkController(IAmWorkBusiness workBusiness,
            ICommonBusiness commonBusiness)
        {
            _workBusiness = workBusiness;
            _commonBusiness = commonBusiness;
        }

        /// <summary>
        /// Tìm kiếm công việc theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_QLCVAM
        [Route("find")]
        [HttpPost]
        public ResponseBaseModel GetWorks([FromBody] AmWorkSearchModel requestModel)
        {
            return _workBusiness.GetWorks(requestModel);
        }

        /// <summary>
        /// Tìm kiếm công việc theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_QLCVAM
        [Route("{id}")]
        [HttpGet]
        public ResponseModel<AmWorkModel> GetWorkById([FromUri] int id)
        {
            return _workBusiness.GetWorkById(id);
        }

        /// <summary>
        /// Thêm mới công việc 
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        public ResponseModel<bool> CreateWork([FromBody] AmWorkAddEditModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.ADD_QLCVAM)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _workBusiness.CreateWork(requestModel);
        }

        /// <summary>
        /// Cập nhập công việc 
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPut]
        public ResponseModel<bool> UpdateWork([FromBody] AmWorkAddEditModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_QLCVAM)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _workBusiness.UpdateWork(requestModel);
        }

        /// <summary>
        /// Xóa công việc 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        public ResponseBaseModel DeleteWork([FromUri] int id)
        {
            ResponseBaseModel responseModel = new ResponseBaseModel();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.DELETE_QLCVAM)).Data)
            {
                responseModel.Message = CommonConstant.USER_RIGHT;
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            return _workBusiness.DeleteWork(id, CurrentUser.ID);
        }

        /// <summary>
        /// Lấy danh sách công việc theo loại công việc và nhà máy
        /// </summary>
        /// <param name="workTypeId">Id loại công việc</param>
        /// <param name="factoryID"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_QLCVAM
        [Route("getWorkByType")]
        [HttpGet]
        public ResponseBaseModel GetWorkByType([FromUri] int workTypeId, int factoryID)
        {
            return _workBusiness.GetWorkByType(workTypeId, factoryID);
        }
    }
}
