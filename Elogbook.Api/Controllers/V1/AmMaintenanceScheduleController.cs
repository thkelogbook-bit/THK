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
    /// Lịch bảo trì AM
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/amMaintenanceSchedule")]
    public class AmMaintenanceScheduleController : BaseApiController
    {
        private readonly IAmMaintenanceScheduleBusiness _maintenanceScheduleBusiness;
        private readonly ICommonBusiness _commonBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="maintenanceScheduleBusiness"></param>
        /// <param name="commonBusiness"></param>
        public AmMaintenanceScheduleController(IAmMaintenanceScheduleBusiness maintenanceScheduleBusiness,
            ICommonBusiness commonBusiness)
        {
            _maintenanceScheduleBusiness = maintenanceScheduleBusiness;
            _commonBusiness = commonBusiness;
        }

        /// <summary>
        /// Tìm kiếm lịch bảo trì theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_QLLTHAM
        [Route("find")]
        [HttpPost]
        public ResponseBaseModel GetMaintenanceSchedules([FromBody] AmMaintenanceScheduleSearchModel requestModel)
        {
            return _maintenanceScheduleBusiness.GetMaintenanceSchedules(requestModel);
        }

        /// <summary>
        /// Tìm kiếm lịch bảo trì theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_QLLTHAM
        [Route("{id}")]
        [HttpGet]
        public ResponseModel<AmMaintenanceScheduleModel> GetMaintenanceScheduleById([FromUri] int id)
        {
            return _maintenanceScheduleBusiness.GetMaintenanceScheduleById(id);
        }

        /// <summary>
        /// Thêm mới lịch bảo trì
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        public ResponseModel<int> CreateMaintenanceSchedule([FromBody] AmMaintenanceScheduleAddEditModel requestModel)
        {
            ResponseModel<int> responseModel = new ResponseModel<int>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.ADD_QLLTHAM)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _maintenanceScheduleBusiness.CreateMaintenanceSchedule(requestModel);
        }

        /// <summary>
        /// Cập nhập lịch bảo trì
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPut]
        public ResponseModel<bool> UpdateMaintenanceSchedule([FromBody] AmMaintenanceScheduleAddEditModel requestModel)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_QLLTHAM)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _maintenanceScheduleBusiness.UpdateMaintenanceSchedule(requestModel);
        }

        /// <summary>
        /// Xóa lịch bảo trì
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        public ResponseBaseModel DeleteMaintenanceSchedule([FromUri] int id)
        {
            ResponseBaseModel responseModel = new ResponseBaseModel();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.DELETE_QLLTHAM)).Data)
            {
                responseModel.Message = CommonConstant.USER_RIGHT;
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            return _maintenanceScheduleBusiness.DeleteMaintenanceSchedule(id, CurrentUser.ID);
        }

        /// <summary>
        /// Tạo bảo trì tự quản
        /// </summary>
        /// <param name="maintenanceScheduleId"></param>
        /// <returns></returns>
        [Route("createAm")]
        [HttpPost]
        public int CreateAM([FromUri] int maintenanceScheduleId)
        {
            return _maintenanceScheduleBusiness.CreateAM(maintenanceScheduleId);
        }
    }
}
