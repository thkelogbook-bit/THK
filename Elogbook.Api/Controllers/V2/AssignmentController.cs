using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Microsoft.Web.Http;
using System.Collections.Generic;
using System.Web.Http;
namespace Elogbook.Api.Controllers.V2
{
    /// <summary>
    /// Quản lý phân công đổi ca
    /// </summary>
    [ApiVersion("2.0")]
    [RoutePrefix("v{version:apiVersion}/assignment")]
    public class AssignmentController : BaseApiController
    {
        private readonly IAssignmentBusiness _assignmentBusiness;
        private readonly ICommonBusiness _commonBusiness;

        /// <summary>
        /// Contruction
        /// </summary>
        /// <param name="assignmentBusiness"></param>
        /// <param name="commonBusiness"></param>
        public AssignmentController(IAssignmentBusiness assignmentBusiness, ICommonBusiness commonBusiness)
        {
            _assignmentBusiness = assignmentBusiness;
            _commonBusiness = commonBusiness;
        }

        /// <summary>
        /// Tạo bảng phân công ca
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        public ResponseModel<bool> CreateAssignmentPlan([FromBody] AssignmentRequestModel requestModel)
        {
            //ResponseModel<bool> responseModel = new ResponseModel<bool>();
            //if (!_commonBusiness.CheckUserHasRight(CurrentUser.ID, nameof(EnumUserPermission.ADD_PC_CVH)).Data)
            //{
            //    responseModel.ErrorMessages.Add(CommonConstant.USER_RIGHT);
            //    responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
            //    return responseModel;
            //}

            requestModel.CurrentUserID = CurrentUser.ID;
            return _assignmentBusiness.CreateAssignment(requestModel);
        }

        /// <summary>
        /// Lấy bảng phân công theo bộ lọc (View 1 - Quản lý phân công)
        /// </summary>
        /// <param name="requestModel">Bộ lọc: factoryID (bắt buộc), fromDate, toDate, operationPositionID, operationShiftID</param>
        /// <returns>Danh sách phân công nhóm theo cương vị → ca → ngày</returns>
        [Route("getAssignmentSchedule")]
        [HttpPost]
        public ResponseModel<List<AssignmentScheduleResponseModel>> GetAssignmentSchedule([FromBody] GetAssignmentScheduleRequestModel requestModel)
        {
            return _assignmentBusiness.GetAssignmentSchedule(requestModel);
        }

        /// <summary>
        /// Lấy thông tin vị trí nhân sự
        /// </summary>
        /// <param name="assignmentSlotID"></param>
        /// <returns></returns>
        [Route("{assignmentSlotID}")]
        [HttpGet]
        public ResponseModel<AssignmentSlotResponseModel> GetAssignmentSlotInfo([FromUri] int assignmentSlotID)
        {
            return _assignmentBusiness.GetAssignmentSlotInfo(assignmentSlotID);
        }

        /// <summary>
        /// Cập nhật thông tin nhân sự cho vị trí
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("updateAssignmentSlot")]
        [HttpPut]
        public ResponseModel<bool> UpdateAssignmentSlot([FromBody] UpdateAssignmentSlotModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _assignmentBusiness.UpdateAssignmentSlot(requestModel);
        }

        /// <summary>
        /// Kiểm tra khoảng ngày đã tồn tại trong bảng phân công hay chưa
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("checkRangeExists")]
        [HttpPost]
        public ResponseModel<bool> CheckRangeExists([FromBody] CheckRangeAssignmentRequestModel requestModel)
        {
            return _assignmentBusiness.CheckRangeExists(requestModel);
        }

    }
}