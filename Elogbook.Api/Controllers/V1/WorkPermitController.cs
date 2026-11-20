using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Request.WorkPermit;
using Elogbook.Core.Models.Response;
using Elogbook.Core.Models.Response.WorkPermit;
using Microsoft.Web.Http;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Quản lý lệnh công tác
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/workPermit")]
    public class WorkPermitController : BaseApiController
    {
        private readonly IWorkPermitBusiness _workPermitBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="workPermitBusiness"></param>
        public WorkPermitController(IWorkPermitBusiness workPermitBusiness)
        {
            _workPermitBusiness = workPermitBusiness;
        }

        /// <summary>
        /// Từ chối cấp lệnh đăng ký công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("rejectTicket")]
        [HttpPut]
        public ResponseModel<bool> RejectTicket([FromBody] LctAddTicketStatusHistoryModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _workPermitBusiness.RejectTicket(requestModel);
        }

        /// <summary>
        /// Đồng ý cấp lệnh đăng ký công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("approveTicket")]
        [HttpPut]
        public ResponseModel<int> ApproveTicket([FromBody] LctAddNewWorkTicketModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _workPermitBusiness.ApproveTicket(requestModel);
        }

        /// <summary>
        /// Trả về một lệnh công tác đã được cấp
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpGet]
        public ResponseModel<LctWorkTicketDetailModel> GetWorkTicketById([FromUri] int id)
        {
            return _workPermitBusiness.GetWorkTicketById(id);
        }

        /// <summary>
        /// Trả về một lệnh công tác đã được cấp (mobile)
        /// </summary>
        /// <param name="id">mã lệnh công tác</param>
        /// <param name="isWaitingTicket">True: lệnh đang chờ cho phép / False: phiêu đã cho phép</param>
        /// <returns></returns>
        [Route("getByIdMobile")]
        [HttpGet]
        public ResponseModel<LctWorkTicketDetailMobileModel> GetWorkTicketByIdMobile([FromUri] int id, bool isWaitingTicket)
        {
            return _workPermitBusiness.GetWorkTicketByIdMobile(id, isWaitingTicket);
        }

        /// <summary>
        /// Trả về tất cả lệnh công tác theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("findWorkTickets")]
        [HttpPost]
        public ResponseBaseModel GetWorkTickets([FromBody] LctSearchWorkTicketModel requestModel)
        {
            return _workPermitBusiness.GetWorkTickets(requestModel);
        }

        /// <summary>
        /// Trả về tất cả lệnh công tác theo điều kiện (mobile)
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("findMobile")]
        [HttpPost]
        public ResponseBaseModel GetWorkTicketsMobile([FromBody] LctSearchWorkTicketMobileModel requestModel)
        {
            return _workPermitBusiness.GetWorkTicketsMobile(requestModel);
        }

        /// <summary>
        /// Trả về tất cả số lượng lệnh công tác theo điều kiện (mobile)
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("findMobileCount")]
        [HttpPost]
        public ResponseModel<DataCountModel> GetWorkTicketsCountMobile([FromBody] LctSearchWorkTicketMobileModel requestModel)
        {
            return _workPermitBusiness.GetWorkTicketsCountMobile(requestModel);
        }

        /// <summary>
        /// Tìm kiếm lệnh đăng ký công tác đang chờ cấp
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("findRegistrationTickets")]
        [HttpPost]
        public ResponseBaseModel GetRegistrationTickets([FromBody] LctSearchRegistrationTicketModel requestModel)
        {
            return _workPermitBusiness.GetRegistrationTickets(requestModel);
        }

        /// <summary>
        /// Thêm mới lệnh công tác mà không cần phải có lệnh đăng ký công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("addWorkTicketNew")]
        [HttpPost]
        public ResponseModel<int> AddWorkTicketNew([FromBody] LctAddNewWorkTicketModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _workPermitBusiness.AddWorkTicketNew(requestModel);
        }

        /// <summary>
        /// Thêm thông tin PCT cho phép công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("addAllowedInfo")]
        [HttpPost]
        public ResponseModel<bool> AddAllowedInfo([FromBody] LctAddAllowedInfoWithoutAcceptorModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _workPermitBusiness.AddAllowedInfo(requestModel);
        }

        /// <summary>
        /// Thêm mới lệnh công tác bằng cách nhập liệu dữ liệu cũ
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("addWorkTicketOld")]
        [HttpPost]
        public ResponseModel<int> AddWorkTicketOld([FromBody] LctAddOldWorkTicketModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _workPermitBusiness.AddWorkTicketOld(requestModel);
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
            return _workPermitBusiness.GetWorkerInformations(workTicketID, pageNumber, pageSize, false);
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
            return _workPermitBusiness.GetWorkerInformations(workTicketID, pageNumber, pageSize, true);
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
            return _workPermitBusiness.GetWorkPlacements(workTicketID, pageNumber, pageSize);
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
            return _workPermitBusiness.GetWorkPlacements(workTicketID);
        }

        /// <summary>
        /// Trả về thông tin lệnh công tác "Đã cho phép"
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <param name="screenType"></param>
        /// <returns></returns>
        [Route("getAcceptanceInfoTickets/{workTicketID}")]
        [HttpGet]
        public ResponseModel<LctAllowedInfoModel> GetAcceptanceInfoTickets([FromUri] int workTicketID, int? screenType)
        {
            return _workPermitBusiness.GetAcceptanceInfoTickets(workTicketID, screenType);
        }

        /// <summary>
        /// Tạo thông tin tiếp nhận nơi làm việc
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("createAdmissionWorkplace")]
        [HttpPost]
        public ResponseBaseModel CreateAdmissionWorkplace([FromBody] LctAddReceiveWorkplaceMobileModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _workPermitBusiness.CreateAdmissionWorkplace(requestModel);
        }

        /// <summary>
        /// Tạo thông tin công tác hàng ngày (thêm hình ảnh công tác và kết quả công tác)
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("createDailyWorkInfo")]
        [HttpPost]
        public ResponseBaseModel CreateDailyWorkInfo([FromBody] LctAddDetailTicketMobileModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _workPermitBusiness.CreateDailyWorkInfo(requestModel);
        }

        /// <summary>
        /// Tạo chi tiết kết thúc công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("createInfoEndTicket")]
        [HttpPost]
        public ResponseBaseModel CreateInfoEndTicket([FromBody] LctAddEditWorkFinishMobileModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _workPermitBusiness.CreateInfoEndTicket(requestModel);
        }

        /// <summary>
        /// Tạo Thông tin nhân viên đơn vị công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("addEditWorkerInformation")]
        [HttpPost]
        public ResponseBaseModel AddEditWorkerInformation([FromBody] LctAddEditWorkerInformationModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _workPermitBusiness.AddEditWorkerInformation(requestModel);
        }

        /// <summary>
        /// Tạo Thông tin địa điểm công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("addEditWorkPlacement")]
        [HttpPost]
        public ResponseBaseModel AddEditWorkPlacement([FromBody] LctAddEditWorkPlacementModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _workPermitBusiness.AddEditWorkPlacement(requestModel);
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
            return _workPermitBusiness.DeleteWorkerInformation(ID, CurrentUser.ID);
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
            return _workPermitBusiness.DeleteWorkPlacement(ID, CurrentUser.ID);
        }

        /// <summary>
        /// In tài liệu lệnh công tác ra file PDF
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("exportPdfFile")]
        [HttpGet]
        public HttpResponseMessage ExportPdfFile([FromUri] int id)
        {
            return _workPermitBusiness.ExportPdfFile(id);
        }

        /// <summary>
        /// Hoàn thành lệnh công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("workTicketComplete")]
        [HttpPost]
        public ResponseModel<bool> UpdateWorkFinish([FromBody] LctUpdateWorkFinishModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _workPermitBusiness.UpdateWorkFinish(requestModel);
        }

        /// <summary>
        /// Hoàn thành lệnh công tác
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <returns></returns>
        [Route("addWorkerInformationForDate")]
        [HttpPost]
        public ResponseBaseModel AddWorkerInformationForDate([FromUri] int workTicketID)
        {
            return _workPermitBusiness.AddWorkerInformationForDate(workTicketID, CurrentUser.ID);
        }

        /// <summary>
        /// Thêm biện pháp cho phép công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("updateSubAllowedInfo")]
        [HttpPost]
        public ResponseBaseModel SaveSubAllowedInfo([FromBody] LctAddSubAllowedInfoModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _workPermitBusiness.AddSubAllowedInfo(requestModel);
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
            return _workPermitBusiness.DeleteSubAllowedInfo(ID, CurrentUser.ID);
        }

        /// <summary>
        /// Công tác hàng ngày. lệnh thủy cơ nhiệt
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("addWorkDailyModel")]
        [HttpPost]
        public ResponseBaseModel AddWorkDailyModel([FromBody] LctAddWorkDailyMobileModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _workPermitBusiness.AddWorkDailyModel(requestModel);
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
            return _workPermitBusiness.UpdateFlagNote(workTicketID, isFlagNote, CurrentUser.ID);
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
            return _workPermitBusiness.GetWorkDailys(workTicketID, pageNumber, pageSize, isMobile);
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
            return _workPermitBusiness.GetWorker(workTicketID);
        }

        /// <summary>
        /// Thêm thông tin Tổ chức thực hiện (881)
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("addWorkPlanInfo")]
        [HttpPost]
        public ResponseBaseModel AddWorkPlanInfo([FromBody] LctAddWorkPlanInfoModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _workPermitBusiness.AddWorkPlanInfo(requestModel);
        }

        /// <summary>
        /// Trả về thông tin Tổ chức thực hiện (881)
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <returns></returns>
        [Route("getWorkPlanInfo")]
        [HttpGet]
        public ResponseModel<LctWorkPlanInfoModel> GetWorkPlanInfo(int workTicketID)
        {
            return _workPermitBusiness.GetWorkPlanInfo(workTicketID);
        }

        /// <summary>
        /// Trả về danh sách vị trí làm việc và danh sách nhân viên (881)
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <returns></returns>
        [Route("getWorkLocations")]
        [HttpGet]
        public ResponseModel<List<LctContentPositionModel>> GetWorkLocations(int workTicketID)
        {
            return _workPermitBusiness.GetWorkLocations(workTicketID);
        }

        /// <summary>
        /// Số lệnh Chờ cấp lệnh công tác
        /// </summary>
        /// <returns></returns>
        [Route("getNumberWaitingRegistrationTicket")]
        [HttpGet]
        public ResponseModel<LctWaitingNumberTicketModel> GetNumberWaitingRegistrationTicket()
        {
            return _workPermitBusiness.GetNumberWaitingRegistrationTicket(CurrentUser.ProductionPlantID);
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
            return _workPermitBusiness.GetWorkFinish(workTicketID);
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
            return _workPermitBusiness.DeleteStaffInformation(ID, CurrentUser.ID);
        }

        /// <summary>
        /// Lấy danh sách nhân viên trong và ngoài công ty
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("getListStaff")]
        [HttpPost]
        public ResponseBaseModel GetListStaff([FromBody] LctSearchStaffModel requestModel)
        {
            return _workPermitBusiness.GetListStaff(requestModel);
        }

        /// <summary>
        /// Cập nhật chi tiết thông tin cho phép
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("updateAllowedInfoDetail")]
        [HttpPut]
        public ResponseModel<bool> UpdateAllowedInfoDetail([FromBody] LctAddAllowedInfoDetailModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _workPermitBusiness.UpdateAllowedInfoDetail(requestModel);
        }

        /// <summary>
        /// Cập nhật thông tin cho phép
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("updateAllowedInfo")]
        [HttpPut]
        public ResponseModel<bool> UpdateAllowedInfo([FromBody] LctUpdateAllowedInfoModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _workPermitBusiness.UpdateAllowedInfo(requestModel);
        }

        /// <summary>
        /// Cập nhật chi tiết tổ chức thực hiện (Cập nhật các biện pháp)
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("updateSafetyMeasure")]
        [HttpPut]
        public ResponseModel<bool> UpdateSafetyMeasure([FromBody] LctAddSafetyMeasureModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _workPermitBusiness.UpdateSafetyMeasure(requestModel);
        }

        /// <summary>
        /// Cập nhật tổ chức thực hiện (Cập nhật số lượng người và đơn vị công tác)
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("updateReceiveWorkplace")]
        [HttpPut]
        public ResponseModel<bool> UpdateReceiveWorkplace([FromBody] LctUpdateReceiveWorkplaceModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _workPermitBusiness.UpdateReceiveWorkplace(requestModel);
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
            return _workPermitBusiness.DeleteStaffInformationGroup(ID, CurrentUser.ID);
        }

        /// <summary>
        /// Kiểm tra tồn tại lệnh công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("checkExistTicketNumber")]
        [HttpPost]
        public ResponseModel<bool> CheckExistTicketNumber([FromBody] ExistTicketModel requestModel)
        {
            return _workPermitBusiness.CheckExistTicketNumber(requestModel.WorkTicketNumber, requestModel.ScreenType);
        }

        /// <summary>
        /// Trả về thông tin thi hành lệnh
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <returns></returns>
        [Route("getOrderExecutionMobile")]
        [HttpGet]
        public ResponseModel<LctOrderExecutionMobileModel> GetOrderExecutionMobile([FromUri] int workTicketID)
        {
            return _workPermitBusiness.GetOrderExecutionMobile(workTicketID);
        }

        /// <summary>
        /// Tạo thông tin thi hành lệnh
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("addEditOrderExecutionMobile")]
        [HttpPost]
        public ResponseBaseModel AddEditOrderExecutionMobile([FromBody] AddLctOrderExecutionMobileModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _workPermitBusiness.AddEditOrderExecutionMobile(requestModel);
        }

        /// <summary>
        /// Trả về thông tin kết thúc công tác
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <returns></returns>
        [Route("getWorkFinishMobile")]
        [HttpGet]
        public ResponseModel<LctdWorkFinishMobileModel> GetWorkFinishMobile([FromUri] int workTicketID)
        {
            return _workPermitBusiness.GetWorkFinishMobile(workTicketID);
        }

        /// <summary>
        /// Tạo thông tin kết thúc công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("addEditWorkFinishMobile")]
        [HttpPost]
        public ResponseBaseModel AddEditWorkFinishMobile([FromBody] LctdAddEditWorkFinishMobileModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _workPermitBusiness.AddEditWorkFinishMobile(requestModel);
        }

        /// <summary>
        /// Lấy danh sách người tham gia công việc lct Điện
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <returns></returns>
        [Route("getLCTDTaskParticipants")]
        [HttpGet]
        public ResponseModel<List<LctStakeholdersResponseModel>> GetLCTDTaskParticipants(int workTicketID)
        {
            return _workPermitBusiness.GetLCTDTaskParticipants(workTicketID);
        }


        /// <summary>
        /// Lấy danh sách người tham gia công việc lct TCNH
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <returns></returns>
        [Route("getLCTTCNHTaskParticipants")]
        [HttpGet]
        public ResponseModel<List<LctContentPositionModel>> GetLCTTCNHTaskParticipants(int workTicketID)
        {
            return _workPermitBusiness.GetLCTTCNHTaskParticipants(workTicketID);
        }

        /// <summary>
        /// Tạo thông tin tổ chức thực hiện mobile LCT Điện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("createExecutingOrgLCTDMobile")]
        [HttpPost]
        public ResponseBaseModel CreateExecutingOrgLCTDMobile([FromBody] LctAddExecutingOrgMobileModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _workPermitBusiness.CreateExecutingOrgLCTDMobile(requestModel);
        }

        /// <summary>
        /// Tạo thông tin Tạo thông tin Trình tự công việc và điều kiện an toàn 
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("addEditWorkProcessSafetyMobile")]
        [HttpPost]
        public ResponseBaseModel AddEditWorkProcessSafetyMobile([FromBody] AddEditWorkProcessSafetyMobileModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _workPermitBusiness.AddEditWorkProcessSafetyMobile(requestModel);
        }
    }
}
