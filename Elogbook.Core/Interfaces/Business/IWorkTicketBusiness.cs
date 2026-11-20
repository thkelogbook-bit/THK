using Elogbook.Core.Enums;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Request.RegistrationTicket;
using Elogbook.Core.Models.Request.WorkTicket;
using Elogbook.Core.Models.Response;
using System.Collections.Generic;
using System.Net.Http;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Quản lý cấp phiếu công tác
    /// </summary>
    public interface IWorkTicketBusiness
    {
        /// <summary>
        /// Tìm kiếm phiếu công tác theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel<WorkTicketDetailModel> GetWorkTicketById(int id);

        /// <summary>
        /// Tìm kiếm phiếu công tác theo ID (mobile)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isWaitingTicket"></param>
        /// <returns></returns>
        ResponseModel<WorkTicketDetailMobileModel> GetWorkTicketByIdMobile(int id, bool isWaitingTicket);

        /// <summary>
        /// Từ chốicấp phiếu đăng ký công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> RejectTicket(AddTicketStatusHistoryModel requestModel);

        /// <summary>
        /// Đồng ý cấp phiếu đăng ký công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<int> ApproveTicket(AddNewWorkTicketModel requestModel);

        /// <summary>
        /// Tìm kiếm tất cả phiếu công tác theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetWorkTickets(SearchWorkTicketModel requestModel);

        /// <summary>
        /// Tìm kiếm tất cả phiếu công tác theo điều kiện (mobile)
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetWorkTicketsMobile(SearchWorkTicketMobileModel requestModel);
        
        /// <summary>
        /// Tìm kiếm tất cả số lượng phiếu công tác theo điều kiện (mobile)
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        /// 
        ResponseModel<DataCountModel> GetWorkTicketsCountMobile(SearchWorkTicketMobileModel requestModel);

        /// <summary>
        /// Thêm mới phiếu công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<int> AddWorkTicketNew(AddNewWorkTicketModel requestModel);

        /// <summary>
        /// Thêm mới phiếu công tác bằng cách nhập liệu dữ liệu cũ
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<int> AddWorkTicketOld(AddOldWorkTicketModel requestModel);

        /// <summary>
        /// Tìm kiếm những phiếu đăng ký công tác đang chờ cấp
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetRegistrationTickets(SearchRegistrationTicketModel requestModel);

        /// <summary>
        /// Lấy thông tin cho phép
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <param name="screenType"></param>
        /// <returns></returns>
        ResponseModel<AllowedInfoModel> GetAcceptanceInfoTickets(int workTicketID, int? screenType);

        /// <summary>
        /// Tạo thông tin tiếp nhận nơi làm việc
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel CreateAdmissionWorkplace(AddReceiveWorkplaceMobileModel requestModel);

        /// <summary>
        /// Tạo thông tin tiếp nhận nơi làm việc
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel CreateDailyWorkInfo(AddDetailTicketMobileModel requestModel);

        /// <summary>
        /// Tạo chi tiết kết thúc công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel CreateInfoEndTicket(AddEditWorkFinishMobileModel requestModel);

        /// <summary>
        /// Trả về danh sách thông tin nhân viên đơn vị công tác
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="isMobile"></param>
        /// <returns></returns>
        ResponseBaseModel GetWorkerInformations(int workTicketID, int pageNumber, int pageSize, bool isMobile);

        /// <summary>
        /// Trả về danh sách thông tin địa điểm công tác hằng ngày
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        ResponseBaseModel GetWorkPlacements(int workTicketID, int pageNumber, int pageSize);

        /// <summary>
        /// Trả về danh sách thông tin địa điểm công tác hằng ngày ở mobile
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <returns></returns>
        ResponseBaseModel GetWorkPlacements(int workTicketID);

        /// <summary>
        /// Tạo Thông tin nhân viên đơn vị công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel AddEditWorkerInformation(AddEditWorkerInformationModel requestModel);

        /// <summary>
        /// Xóa thông tin công tác hàng ngày
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseBaseModel DeleteWorkerInformation(int ID, int userID);

        /// <summary>
        /// Tạo Thông tin địa điểm công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel AddEditWorkPlacement(AddEditWorkPlacementModel requestModel);

        /// <summary>
        /// Xóa  địa điểm làm việc
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseBaseModel DeleteWorkPlacement(int ID, int userID);

        /// <summary>
        /// Thêm thông tin PCT cho phép công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> AddAllowedInfo(AddAllowedInfoWithoutAcceptorModel requestModel);

        /// <summary>
        /// In file PDF
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        HttpResponseMessage ExportPdfFile(int id);

        /// <summary>
        /// Hoàn thành phiếu công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateWorkFinish(UpdateWorkFinishModel requestModel);

        /// <summary>
        /// Tạo Thông tin nhân viên đơn vị công tác ở ngày cụ thể
        /// </summary>
        /// <param name="workTicketId"></param>
        /// <param name="currentUserID"></param>
        /// <returns></returns>
        ResponseBaseModel AddWorkerInformationForDate(int workTicketId, int currentUserID);

        /// <summary>
        /// Thêm thông tin biện pháp cho phép công tác ở phiếu thủy cơ nhiệt hóa
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel AddSubAllowedInfo(AddSubAllowedInfoModel requestModel);

        /// <summary>
        /// Xóa biện pháp công tác
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseBaseModel DeleteSubAllowedInfo(int ID, int userID);

        /// <summary>
        /// Thêm công tác hàng ngày. Phiếu Thủy cơ nhiệt
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel AddWorkDailyModel(AddWorkDailyMobileModel requestModel);

        /// <summary>
        /// Cập nhật flag note cho PCT
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <param name="isFlagNote"></param>
        /// <param name="currentUserID"></param>
        /// <returns></returns>
        ResponseBaseModel UpdateFlagNote(int workTicketID, bool isFlagNote, int currentUserID);

        /// <summary>
        /// Trả về danh sách thông tin nhân viên đơn vị công tác (881)
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="isMobile"></param>
        /// <returns></returns>
        ResponseBaseModel GetWorkDailys(int workTicketID, int? pageNumber, int? pageSize, bool isMobile);

        /// <summary>
        /// Danh sách những người được đề cử để thực hiện công việc
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <returns></returns>
        ResponseBaseModel GetWorker(int workTicketID);

        /// <summary>
        /// Tổ chức thực hiện (Thủy cơ nhiệt hóa)
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel AddWorkPlanInfo(AddWorkPlanInfoModel requestModel);

        /// <summary>
        /// Trả về thông tin Tổ chức thực hiện (881)
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <returns></returns>
        ResponseModel<WorkPlanInfoModel> GetWorkPlanInfo(int workTicketID);

        /// <summary>
        /// Trả về danh sách vị trí làm việc và danh sách nhân viên (881)
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <returns></returns>
        ResponseModel<List<ContentPositionModel>> GetWorkLocations(int workTicketID);

        /// <summary>
        /// Số phiếu Chờ cấp phiếu công tác
        /// </summary>
        /// <param name="productionPlantID"></param>
        /// <returns></returns>
        ResponseModel<WaitingNumberTicketModel> GetNumberWaitingRegistrationTicket(int productionPlantID);

        /// <summary>
        /// Trả về chi tiết kết thúc công tác (881)
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <returns></returns>
        ResponseBaseModel GetWorkFinish(int workTicketID);

        /// <summary>
        /// Xóa nhân viên công tác 881
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseBaseModel DeleteStaffInformation(int id, int userID);

        /// <summary>
        /// Lấy danh sách nhân viên trong và ngoài công ty
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetListStaff(SearchStaffModel requestModel);

        /// <summary>
        /// Cập nhật chi tiết thông tin cho phép
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateAllowedInfoDetail(AddAllowedInfoDetailModel requestModel);

        /// <summary>
        /// Cập nhật thông tin cho phép
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateAllowedInfo(UpdateAllowedInfoModel requestModel);

        /// <summary>
        /// Cập nhật chi tiết tổ chức thực hiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateSafetyMeasure(AddSafetyMeasureModel requestModel);

        /// <summary>
        /// Cập nhật tổ chức thực hiện (Cập nhật số lượng người và đơn vị công tác)
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateReceiveWorkplace(UpdateReceiveWorkplaceModel requestModel);

        /// <summary>
        /// Xóa nhóm nhân viên công tác 881
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseBaseModel DeleteStaffInformationGroup(int id, int userID);

        /// <summary>
        /// Kiểm tra tồn tại phiếu công tác
        /// </summary>
        /// <param name="workTicketNumber"></param>
        /// <param name="screenType"></param>
        /// <returns></returns>
        ResponseModel<bool> CheckExistTicketNumber(string workTicketNumber, int screenType);

        /// <summary>
        /// Reset số phiếu không sử dụng
        /// </summary>
        void ResetTicketNumber();

        /// <summary>
        /// Xóa phiếu công tác
        /// </summary>
        /// <param name="workTicketId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        ResponseModel<bool> DeleteWorkTicket(int workTicketId, int userId);

        /// <summary>
        /// Updaet status
        /// </summary>
        /// <param name="workTicketId"></param>
        /// <param name="status"></param>
        /// <param name="description"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateStatus(int workTicketId, EnumTicketStatus status, string description, int currentUserId);

        /// <summary>
        /// Gửi email, thông báo để nhắc nhở, cảnh báo phiếu công tác chưa hoàn thành theo kế hoạch
        /// </summary>
        void SendEmailWorkTicketCompletionReminder();
    }
}
