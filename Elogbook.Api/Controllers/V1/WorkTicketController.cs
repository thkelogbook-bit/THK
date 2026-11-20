using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Request.RegistrationTicket;
using Elogbook.Core.Models.Request.WorkTicket;
using Elogbook.Core.Models.Response;
using Microsoft.Web.Http;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Quản lý phiếu công tác
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/workTicket")]
    public class WorkTicketController : BaseApiController
    {
        private readonly IWorkTicketBusiness _workTicketBusiness;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="workTicketBusiness"></param>
        public WorkTicketController(IWorkTicketBusiness workTicketBusiness)
        {
            _workTicketBusiness = workTicketBusiness;
        }

        /// <summary>
        /// Từ chối cấp phiếu đăng ký công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("rejectTicket")]
        [HttpPut]
        public ResponseModel<bool> RejectTicket([FromBody] AddTicketStatusHistoryModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _workTicketBusiness.RejectTicket(requestModel);
        }

        /// <summary>
        /// Đồng ý cấp phiếu đăng ký công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("approveTicket")]
        [HttpPut]
        public ResponseModel<int> ApproveTicket([FromBody] AddNewWorkTicketModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _workTicketBusiness.ApproveTicket(requestModel);
        }

        /// <summary>
        /// Trả về một phiếu công tác đã được cấp
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpGet]
        public ResponseModel<WorkTicketDetailModel> GetWorkTicketById([FromUri] int id)
        {
            return _workTicketBusiness.GetWorkTicketById(id);
        }

        /// <summary>
        /// Trả về một phiếu công tác đã được cấp (mobile)
        /// </summary>
        /// <param name="id">mã phiếu công tác</param>
        /// <param name="isWaitingTicket">True: phiếu đang chờ cho phép / False: phiêu đã cho phép</param>
        /// <returns></returns>
        [Route("getByIdMobile")]
        [HttpGet]
        public ResponseModel<WorkTicketDetailMobileModel> GetWorkTicketByIdMobile([FromUri] int id, bool isWaitingTicket)
        {
            return _workTicketBusiness.GetWorkTicketByIdMobile(id, isWaitingTicket);
        }

        /// <summary>
        /// Trả về tất cả phiếu công tác theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("findWorkTickets")]
        [HttpPost]
        public ResponseBaseModel GetWorkTickets([FromBody] SearchWorkTicketModel requestModel)
        {
            return _workTicketBusiness.GetWorkTickets(requestModel);
        }

        /// <summary>
        /// Trả về tất cả phiếu công tác theo điều kiện (mobile)
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("findMobile")]
        [HttpPost]
        public ResponseBaseModel GetWorkTicketsMobile([FromBody] SearchWorkTicketMobileModel requestModel)
        {
            return _workTicketBusiness.GetWorkTicketsMobile(requestModel);
        }

        /// <summary>
        /// Trả về tất cả số lượng phiếu công tác theo điều kiện (mobile)
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("findMobileCount")]
        [HttpPost]
        public ResponseModel<DataCountModel> GetWorkTicketsCountMobile([FromBody] SearchWorkTicketMobileModel requestModel)
        {
            return _workTicketBusiness.GetWorkTicketsCountMobile(requestModel);
        }

        /// <summary>
        /// Tìm kiếm phiếu đăng ký công tác đang chờ cấp
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("findRegistrationTickets")]
        [HttpPost]
        public ResponseBaseModel GetRegistrationTickets([FromBody] SearchRegistrationTicketModel requestModel)
        {
            return _workTicketBusiness.GetRegistrationTickets(requestModel);
        }

        /// <summary>
        /// Thêm mới phiếu công tác mà không cần phải có phiếu đăng ký công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("addWorkTicketNew")]
        [HttpPost]
        public ResponseModel<int> AddWorkTicketNew([FromBody] AddNewWorkTicketModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _workTicketBusiness.AddWorkTicketNew(requestModel);
        }

        /// <summary>
        /// Thêm thông tin PCT cho phép công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("addAllowedInfo")]
        [HttpPost]
        public ResponseModel<bool> AddAllowedInfo([FromBody] AddAllowedInfoWithoutAcceptorModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _workTicketBusiness.AddAllowedInfo(requestModel);
        }

        /// <summary>
        /// Thêm mới phiếu công tác bằng cách nhập liệu dữ liệu cũ
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("addWorkTicketOld")]
        [HttpPost]
        public ResponseModel<int> AddWorkTicketOld([FromBody] AddOldWorkTicketModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _workTicketBusiness.AddWorkTicketOld(requestModel);
        }

        /// <summary>
        /// Trả về danh sách thông tin nhân viên đơn vị công tác
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [Route("getWorkerInformations")]
        [HttpGet]
        public ResponseBaseModel GetWorkerInformations([FromUri] int workTicketID, int pageNumber, int pageSize)
        {
            return _workTicketBusiness.GetWorkerInformations(workTicketID, pageNumber, pageSize, false);
        }

        /// <summary>
        /// Trả về danh sách thông tin nhân viên đơn vị công tác (mobile)
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [Route("getWorkerInformationsMobile")]
        [HttpGet]
        public ResponseBaseModel GetWorkerInformationsMobile([FromUri] int workTicketID, int pageNumber, int pageSize)
        {
            return _workTicketBusiness.GetWorkerInformations(workTicketID, pageNumber, pageSize, true);
        }

        /// <summary>
        /// Trả về danh sách thông tin địa điểm công tác hằng ngày
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [Route("getWorkPlacements")]
        [HttpGet]
        public ResponseBaseModel GetWorkPlacements([FromUri] int workTicketID, int pageNumber, int pageSize)
        {
            return _workTicketBusiness.GetWorkPlacements(workTicketID, pageNumber, pageSize);
        }

        /// <summary>
        /// Trả về danh sách thông tin địa điểm công tác hằng ngày ở mobile
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <returns></returns>
        [Route("getWorkPlacementsMobile")]
        [HttpGet]
        public ResponseBaseModel GetWorkPlacements([FromUri] int workTicketID)
        {
            return _workTicketBusiness.GetWorkPlacements(workTicketID);
        }

        /// <summary>
        /// Trả về thông tin phiếu công tác "Đã cho phép"
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <param name="screenType"></param>
        /// <returns></returns>
        [Route("getAcceptanceInfoTickets/{workTicketID}")]
        [HttpGet]
        public ResponseModel<AllowedInfoModel> GetAcceptanceInfoTickets([FromUri] int workTicketID, int? screenType)
        {
            return _workTicketBusiness.GetAcceptanceInfoTickets(workTicketID, screenType);
        }

        /// <summary>
        /// Tạo thông tin tiếp nhận nơi làm việc
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("createAdmissionWorkplace")]
        [HttpPost]
        public ResponseBaseModel CreateAdmissionWorkplace([FromBody] AddReceiveWorkplaceMobileModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _workTicketBusiness.CreateAdmissionWorkplace(requestModel);
        }

        /// <summary>
        /// Tạo thông tin công tác hàng ngày (thêm hình ảnh công tác và kết quả công tác)
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("CreateDailyWorkInfo")]
        [HttpPost]
        public ResponseBaseModel CreateDailyWorkInfo([FromBody] AddDetailTicketMobileModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _workTicketBusiness.CreateDailyWorkInfo(requestModel);
        }

        /// <summary>
        /// Tạo chi tiết kết thúc công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("createInfoEndTicket")]
        [HttpPost]
        public ResponseBaseModel CreateInfoEndTicket([FromBody] AddEditWorkFinishMobileModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _workTicketBusiness.CreateInfoEndTicket(requestModel);
        }

        /// <summary>
        /// Tạo Thông tin nhân viên đơn vị công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("addEditWorkerInformation")]
        [HttpPost]
        public ResponseBaseModel AddEditWorkerInformation([FromBody] AddEditWorkerInformationModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _workTicketBusiness.AddEditWorkerInformation(requestModel);
        }

        /// <summary>
        /// Tạo Thông tin địa điểm công tác (PCT Điện)
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("addEditWorkPlacement")]
        [HttpPost]
        public ResponseBaseModel AddEditWorkPlacement([FromBody] AddEditWorkPlacementModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _workTicketBusiness.AddEditWorkPlacement(requestModel);
        }

        /// <summary>
        /// Xóa thông tin công tác hàng ngày
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [Route("deleteWorkerInformation")]
        [HttpDelete]
        public ResponseBaseModel DeleteWorkerInformation(int ID)
        {
            return _workTicketBusiness.DeleteWorkerInformation(ID, CurrentUser.ID);
        }

        /// <summary>
        /// Xóa  địa điểm làm việc
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [Route("deleteWorkPlacement")]
        [HttpDelete]
        public ResponseBaseModel DeleteWorkPlacement(int ID)
        {
            return _workTicketBusiness.DeleteWorkPlacement(ID, CurrentUser.ID);
        }

        /// <summary>
        /// In tài liệu phiếu công tác ra file PDF
        /// </summary>
        /// <param name="id">ID phiếu công tác</param>
        /// <returns></returns>
        [Route("exportPdfFile")]
        [HttpGet]
        public HttpResponseMessage ExportPdfFile([FromUri] int id)
        {
            return _workTicketBusiness.ExportPdfFile(id);
        }

        /// <summary>
        /// Hoàn thành phiếu công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("workTicketComplete")]
        [HttpPost]
        public ResponseModel<bool> UpdateWorkFinish([FromBody] UpdateWorkFinishModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _workTicketBusiness.UpdateWorkFinish(requestModel);
        }

        /// <summary>
        /// Hoàn thành phiếu công tác
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <returns></returns>
        [Route("addWorkerInformationForDate")]
        [HttpPost]
        public ResponseBaseModel AddWorkerInformationForDate([FromUri] int workTicketID)
        {
            return _workTicketBusiness.AddWorkerInformationForDate(workTicketID, CurrentUser.ID);
        }

        /// <summary>
        /// Thêm biện pháp cho phép công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("updateSubAllowedInfo")]
        [HttpPost]
        public ResponseBaseModel SaveSubAllowedInfo([FromBody] AddSubAllowedInfoModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _workTicketBusiness.AddSubAllowedInfo(requestModel);
        }

        /// <summary>
        /// Xóa biện pháp cho phép công tác
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [Route("deleteSubAllowedInfo")]
        [HttpPost]
        public ResponseBaseModel DeleteSubAllowedInfo(int ID)
        {
            return _workTicketBusiness.DeleteSubAllowedInfo(ID, CurrentUser.ID);
        }

        /// <summary>
        /// Công tác hàng ngày. Phiếu thủy cơ nhiệt
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("addWorkDailyModel")]
        [HttpPost]
        public ResponseBaseModel AddWorkDailyModel([FromBody] AddWorkDailyMobileModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _workTicketBusiness.AddWorkDailyModel(requestModel);
        }

        /// <summary>
        /// Cập nhật flag note cho PCT
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <param name="isFlagNote"></param>
        /// <returns></returns>
        [Route("updateFlagNote")]
        [HttpGet]
        public ResponseBaseModel UpdateFlagNote([FromUri] int workTicketID, [FromUri] bool isFlagNote)
        {
            return _workTicketBusiness.UpdateFlagNote(workTicketID, isFlagNote, CurrentUser.ID);
        }

        /// <summary>
        /// Trả về danh sách thông tin nhân viên hằng ngày (881)
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="isMobile"></param>
        /// <returns></returns>
        [Route("getWorkDailys")]
        [HttpGet]
        public ResponseBaseModel GetWorkDailys([FromUri] int workTicketID, int? pageNumber, int? pageSize, bool isMobile)
        {
            return _workTicketBusiness.GetWorkDailys(workTicketID, pageNumber, pageSize, isMobile);
        }

        /// <summary>
        /// Danh sách những người được đề cử để thực hiện công việc
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <returns></returns>
        [Route("getWorker")]
        [HttpGet]
        public ResponseBaseModel GetWorker(int workTicketID)
        {
            return _workTicketBusiness.GetWorker(workTicketID);
        }

        /// <summary>
        /// Thêm thông tin Tổ chức thực hiện (881)
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("addWorkPlanInfo")]
        [HttpPost]
        public ResponseBaseModel AddWorkPlanInfo([FromBody] AddWorkPlanInfoModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _workTicketBusiness.AddWorkPlanInfo(requestModel);
        }

        /// <summary>
        /// Trả về thông tin Tổ chức thực hiện (881)
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <returns></returns>
        [Route("getWorkPlanInfo")]
        [HttpGet]
        public ResponseModel<WorkPlanInfoModel> GetWorkPlanInfo(int workTicketID)
        {
            return _workTicketBusiness.GetWorkPlanInfo(workTicketID);
        }

        /// <summary>
        /// Trả về danh sách vị trí làm việc và danh sách nhân viên (881)
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <returns></returns>
        [Route("getWorkLocations")]
        [HttpGet]
        public ResponseModel<List<ContentPositionModel>> GetWorkLocations(int workTicketID)
        {
            return _workTicketBusiness.GetWorkLocations(workTicketID);
        }

        /// <summary>
        /// Số phiếu Chờ cấp phiếu công tác
        /// </summary>
        /// <returns></returns>
        [Route("getNumberWaitingRegistrationTicket")]
        [HttpGet]
        public ResponseModel<WaitingNumberTicketModel> GetNumberWaitingRegistrationTicket()
        {
            return _workTicketBusiness.GetNumberWaitingRegistrationTicket(CurrentUser.ProductionPlantID);
        }

        /// <summary>
        /// Trả về chi tiết kết thúc công tác (881)
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <returns></returns>
        [Route("getWorkFinish")]
        [HttpGet]
        public ResponseBaseModel GetWorkFinish(int workTicketID)
        {
            return _workTicketBusiness.GetWorkFinish(workTicketID);
        }

        /// <summary>
        /// Xóa nhân viên công tác hằng ngày (881)
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [Route("deleteStaffInformation")]
        [HttpDelete]
        public ResponseBaseModel DeleteStaffInformation(int ID)
        {
            return _workTicketBusiness.DeleteStaffInformation(ID, CurrentUser.ID);
        }

        /// <summary>
        /// Lấy danh sách nhân viên trong và ngoài công ty
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("getListStaff")]
        [HttpPost]
        public ResponseBaseModel GetListStaff([FromBody] SearchStaffModel requestModel)
        {
            return _workTicketBusiness.GetListStaff(requestModel);
        }

        /// <summary>
        /// Cập nhật chi tiết thông tin cho phép
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("updateAllowedInfoDetail")]
        [HttpPut]
        public ResponseModel<bool> UpdateAllowedInfoDetail([FromBody] AddAllowedInfoDetailModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _workTicketBusiness.UpdateAllowedInfoDetail(requestModel);
        }

        /// <summary>
        /// Cập nhật thông tin cho phép
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("updateAllowedInfo")]
        [HttpPut]
        public ResponseModel<bool> UpdateAllowedInfo([FromBody] UpdateAllowedInfoModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _workTicketBusiness.UpdateAllowedInfo(requestModel);
        }

        /// <summary>
        /// Cập nhật chi tiết tổ chức thực hiện (Cập nhật các biện pháp)
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("updateSafetyMeasure")]
        [HttpPut]
        public ResponseModel<bool> UpdateSafetyMeasure([FromBody] AddSafetyMeasureModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _workTicketBusiness.UpdateSafetyMeasure(requestModel);
        }

        /// <summary>
        /// Cập nhật tổ chức thực hiện (Cập nhật số lượng người và đơn vị công tác)
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("updateReceiveWorkplace")]
        [HttpPut]
        public ResponseModel<bool> UpdateReceiveWorkplace([FromBody] UpdateReceiveWorkplaceModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _workTicketBusiness.UpdateReceiveWorkplace(requestModel);
        }

        /// <summary>
        /// Xóa nhóm nhân viên công tác hằng ngày (881)
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [Route("deleteStaffInformationGroup")]
        [HttpDelete]
        public ResponseBaseModel DeleteStaffInformationGroup(int ID)
        {
            return _workTicketBusiness.DeleteStaffInformationGroup(ID, CurrentUser.ID);
        }

        /// <summary>
        /// Kiểm tra tồn tại phiếu công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("checkExistTicketNumber")]
        [HttpPost]
        public ResponseModel<bool> CheckExistTicketNumber([FromBody] ExistTicketModel requestModel)
        {
            return _workTicketBusiness.CheckExistTicketNumber(requestModel.WorkTicketNumber, requestModel.ScreenType);
        }

        /// <summary>
        /// Xóa phiếu công tác
        /// </summary>
        /// <param name="workTicketId"></param>
        /// <returns></returns>
        [Route("deleteWorkTicktet")]
        [HttpDelete]
        public ResponseModel<bool> DeleteWorkTicket(int workTicketId)
        {
            return _workTicketBusiness.DeleteWorkTicket(workTicketId, CurrentUser.ID);
        }

        /// <summary>
        /// Tự động gửi mail thông báo phiếu công tác chưa hoàn thành theo kế hoạch
        /// </summary>
        [Route("SendEmailWorkTicketCompletionReminder")]
        [HttpPost]
        public ResponseModel<bool> SendEmailWorkTicketCompletionReminder()
        {
            _workTicketBusiness.SendEmailWorkTicketCompletionReminder();
            return new ResponseModel<bool> { Data = true };
        }

        /// <summary>
        /// Hủy phiếu công tác
        /// </summary>
        /// <returns></returns>
        [Route("cancelTicket")]
        [HttpPut]
        [Permission(EnumUserPermission.ANNULLED_QLDS_PCTD_CDK, EnumUserPermission.ANNULLED_QLDS_PCTTCNH_CDK)]
        public ResponseModel<bool> CancelTicket([FromBody] NotAllowedModel requestModel)
        {
            return _workTicketBusiness.UpdateStatus(requestModel.WorkTicketID, EnumTicketStatus.Annulled, requestModel.Reason, CurrentUser.ID);
        }

        /// <summary>
        /// Chờ đăng ký công tác
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("waitForRegist/{id}")]
        [HttpPut]
        [Permission(EnumUserPermission.VIEW_QLDS_PCTD_CDK, EnumUserPermission.VIEW_QLDS_PCTTCNH_CDK)]
        public ResponseModel<bool> WaitForRegistTicket([FromUri] int id)
        {
            return _workTicketBusiness.UpdateStatus(id, EnumTicketStatus.WaitForRegist, null, CurrentUser.ID);
        }
    }
}
