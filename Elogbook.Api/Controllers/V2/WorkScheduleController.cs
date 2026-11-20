using Elogbook.Business;
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

namespace Elogbook.Api.Controllers.V2
{
    /// <summary>
    /// Quản lý lịch làm việc
    /// </summary>
    [ApiVersion("2.0")]
    [RoutePrefix("v{version:apiVersion}/workSchedule")]
    public class WorkScheduleController : BaseApiController
    {
        private readonly IWorkScheduleBusiness _workScheduleBusiness;
        private readonly ICommonBusiness _commonBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="workScheduleBusiness"></param>
        /// <param name="commonBusiness"></param>
        public WorkScheduleController(IWorkScheduleBusiness workScheduleBusiness,
            ICommonBusiness commonBusiness)
        {
            _workScheduleBusiness = workScheduleBusiness;
            _commonBusiness = commonBusiness;
        }

        /// <summary>
        /// Thêm mới lịch làm việc tiêu chuẩn hoặc chỉnh sửa lịch làm việc tiêu chuẩn
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("addEditWorkSchedule")]
        [HttpPost]
        public ResponseModel<bool> AddEditWorkSchedule([FromBody] WorkScheduleRequestModel requestModel)
        {
            //ResponseModel<bool> responseModel = new ResponseModel<bool>();
            //if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.ADD_WORK_SCHEDULE)).Data)
            //{
            //    responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
            //    responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
            //    return responseModel;
            //}

            requestModel.CurrentUserID = CurrentUser.ID;
            return _workScheduleBusiness.AddEditWorkSchedule(requestModel);
        }

        /// <summary>
        /// Trả về dữ liệu ban đầu cho lịch làm việc
        /// </summary>
        /// <returns></returns>
        [Route("getInitialData")]
        [HttpGet]
        public ResponseModel<WorkScheduleResponseModel> GetInitialData()
        {
            return _workScheduleBusiness.GetInitialData();
        }

        /// <summary>
        /// Thêm mới lịch làm việc ngày lễ hoặc chỉnh sửa lịch làm việc ngày lễ
        /// </summary>
        /// <returns></returns>
        [Route("getAllHolidaySchedule")]
        [HttpGet]
        public ResponseBaseModel GetAllHolidaySchedule()
        {
            //ResponseModel<bool> responseModel = new ResponseModel<bool>();
            //if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.ADD_WORK_SCHEDULE)).Data)
            //{
            //    responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
            //    responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
            //    return responseModel;
            //}
            return _workScheduleBusiness.GetAllHolidaySchedule();
        }

        /// <summary>
        /// Tìm kiếm lịch làm việc ngày lễ theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("getHolidayScheduleById")]
        [HttpGet]
        public ResponseModel<HolidayScheduleResponseModel> GetHolidayScheduleById([FromUri] int id)
        {
            return _workScheduleBusiness.GetHolidayScheduleById(id);
        }

        /// <summary>
        /// Thêm mới lịch làm việc ngày lễ hoặc chỉnh sửa lịch làm việc ngày lễ
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("addEditHolidaySchedule")]
        [HttpPost]
        public ResponseModel<bool> AddEditHolidaySchedule([FromBody] HolidayScheduleRequestModel requestModel)
        {
            //ResponseModel<bool> responseModel = new ResponseModel<bool>();
            //if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.ADD_WORK_SCHEDULE)).Data)
            //{
            //    responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
            //    responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
            //    return responseModel;
            //}

            return _workScheduleBusiness.AddEditHolidaySchedule(requestModel, CurrentUser.ID);
        }

        /// <summary>
        /// Thêm mới lịch làm việc ngày lễ hoặc chỉnh sửa lịch làm việc ngày lễ
        /// </summary>
        /// <returns></returns>
        [Route("deleteHolidaySchedule")]
        [HttpDelete]
        public ResponseModel<bool> DeleteHolidaySchedule([FromUri] int id)
        {
            //ResponseModel<bool> responseModel = new ResponseModel<bool>();
            //if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.ADD_WORK_SCHEDULE)).Data)
            //{
            //    responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
            //    responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
            //    return responseModel;
            //}
            return _workScheduleBusiness.DeleteHolidaySchedule(id, CurrentUser.ID);
        }
    }
}