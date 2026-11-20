using Elogbook.Api.Jobs;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Microsoft.Web.Http;
using Quartz;
using System.Net;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Quản lý schedule
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/schedule")]
    public class ScheduleController : BaseApiController
    {
        private readonly IScheduleBusiness _scheduleBusiness;
        private readonly IScheduler _scheduler;
        private readonly ICommonBusiness _commonBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="scheduleBusiness"></param>
        /// <param name="scheduler"></param>
        /// <param name="commonBusiness"></param>
        public ScheduleController(IScheduleBusiness scheduleBusiness,
            IScheduler scheduler,
            ICommonBusiness commonBusiness)
        {
            _scheduleBusiness = scheduleBusiness;
            _scheduler = scheduler;
            _commonBusiness = commonBusiness;
        }

        /// <summary>
        /// Tìm kiếm schedule theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_SCHEDULE
        [Route("find")]
        [HttpPost]
        public ResponseBaseModel GetSchedules([FromBody] SearchScheduleRequestModel requestModel)
        {
            return _scheduleBusiness.GetSchedules(requestModel);
        }

        /// <summary>
        /// Thông tin của mỗi schedule
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_SCHEDULE
        [Route("{id}")]
        [HttpGet]
        public ResponseModel<ScheduleResponseModel> GetScheduleById([FromUri] int id)
        {
            return _scheduleBusiness.GetScheduleById(id);
        }

        /// <summary>
        /// Thêm mới schedule
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        public ResponseModel<ScheduleResponseModel> AddSchedule([FromBody] AddEditScheduleRequestModel requestModel)
        {
            ResponseModel<ScheduleResponseModel> responseModel = new ResponseModel<ScheduleResponseModel>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.ADD_SCHEDULE)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            return _scheduleBusiness.AddSchedule(requestModel);
        }

        /// <summary>
        /// Cập nhật schedule
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPut]
        public ResponseModel<ScheduleResponseModel> UpdateSchedule([FromBody] AddEditScheduleRequestModel requestModel)
        {
            ResponseModel<ScheduleResponseModel> responseModel = new ResponseModel<ScheduleResponseModel>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.EDIT_SCHEDULE)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            requestModel.CurrentUserID = CurrentUser.ID;
            var res = _scheduleBusiness.UpdateSchedule(requestModel);
            _ = JobInitial.InitJobAsync(_scheduler);
            return res;
        }

        /// <summary>
        /// Xóa schedule
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        public ResponseModel<bool> DeleteSchedule([FromUri] int id)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.DELETE_SCHEDULE)).Data)
            {
                responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
                responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                return responseModel;
            }

            return _scheduleBusiness.DeleteSchedule(id, CurrentUser.ID);
        }

        /// <summary>
        /// Kiểm tra schedule đang sử dụng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("checkInUse/{id}")]
        [HttpGet]
        public ResponseModel<bool> CheckInUseSchedule([FromUri] int id)
        {
            return _scheduleBusiness.CheckInUseSchedule(id);
        }

        /// <summary>
        /// Thông tin cấu hình lập lịch
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// EnumUserPermission.VIEW_SCHEDULE
        [Route("getCronJobDescription")]
        [HttpPost]
        public ResponseModel<string> GetCronJobDescription([FromBody] CronJobDescriptionRequestModel requestModel)
        {
            return _scheduleBusiness.GetCronJobDescription(requestModel);
        }
    }
}