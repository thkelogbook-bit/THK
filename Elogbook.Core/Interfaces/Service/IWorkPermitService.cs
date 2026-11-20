using Elogbook.Core.Enums;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request.WorkPermit;
using Elogbook.Core.Models.Response;
using Elogbook.Core.Models.Response.WorkPermit;
using System.Collections.Generic;
using System.Net.Http;

namespace Elogbook.Core.Interfaces.Service
{
    /// <summary>
    /// Định nghĩa các phương thức của lệnh công tác
    /// </summary>
    public interface IWorkPermitService
    {
        /// <summary>
        /// Tìm kiếm lệnh công tác theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        LctWorkTicketDetailModel GetWorkTicketById(int id);

        /// <summary>
        /// Tìm kiếm lệnh công tác theo id (mobile)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isWaitingTicket"></param>
        /// <returns></returns>
        LctWorkTicketDetailMobileModel GetWorkTicketByIdMobile(int id, bool isWaitingTicket);

        /// <summary>
        /// Tìm kiếm tất cả lệnh công tác theo điều kiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetWorkTickets(LctSearchWorkTicketModel requestModel);

        /// <summary>
        /// Tìm kiếm tất cả lệnh công tác theo điều kiện (mobile)
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetWorkTicketsMobile(LctSearchWorkTicketMobileModel requestModel);
        /// <summary>
        /// Số lượng Tìm kiếm tất cả lệnh công tác theo điều kiện (mobile)
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        DataCountModel GetWorkTicketsCountMobile(LctSearchWorkTicketMobileModel requestModel);
        /// <summary>
        /// Tìm kiếm tất cả lệnh đăng ký công tác đăng chờ cấp
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetRegistrationTickets(LctSearchRegistrationTicketModel requestModel);

        /// <summary>
        /// Lấy thông tin cho phép
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <param name="screenType"></param>
        /// <returns></returns>
        LctAllowedInfoModel GetAcceptanceInfoTickets(int workTicketID, int? screenType);

        /// <summary>
        /// Tạo thông tin tiếp nhận nơi làm việc
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel CreateAdmissionWorkplace(LctAddReceiveWorkplaceMobileModel requestModel);

        /// <summary>
        /// Tạo thông tin công tác hàng ngày
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel CreateDailyWorkInfo(LctAddDetailTicketMobileModel requestModel);

        /// <summary>
        /// Tạo chi tiết kết thúc công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel CreateInfoEndTicket(LctAddEditWorkFinishMobileModel requestModel);

        /// <summary>
        /// Kiểm tra lệnh đăng ký công tác tồn tại
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool CheckExistRegistrationTicketData(int id);

        /// <summary>
        /// Thêm mới lệnh công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="isApprove"></param>
        /// <returns></returns>
        ResponseModel<int> AddWorkTicketNew(LctAddNewWorkTicketModel requestModel, bool isApprove);

        /// <summary>
        /// Thêm mới lệnh công tác (Nhập dữ liệu cũ)
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<int> AddWorkTicketOld(LctAddOldWorkTicketModel requestModel);

        /// <summary>
        /// Kiểu tra tồn tại Nhóm thông số trong Nhà máy
        /// </summary>
        /// <param name="numberWorkTicket"></param>
        /// <returns></returns>
        bool CheckDuplicateData(string numberWorkTicket);

        /// <summary>
        /// kiểm tra user + pass
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="passWord"></param>
        bool IsExistUser(string userName, string passWord);

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
        ResponseBaseModel AddEditWorkerInformation(LctAddEditWorkerInformationModel requestModel);

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
        ResponseBaseModel AddEditWorkPlacement(LctAddEditWorkPlacementModel requestModel);

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
        ResponseModel<bool> AddAllowedInfo(LctAddAllowedInfoWithoutAcceptorModel requestModel);

        /// <summary>
        /// In file PDF
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        HttpResponseMessage ExportPdfFile(int id);

        /// <summary>
        /// Hoàn thành lệnh công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateWorkFinish(LctUpdateWorkFinishModel requestModel);

        /// <summary>
        /// Tạo Thông tin nhân viên đơn vị công tác ở ngày cụ thể
        /// </summary>
        /// <param name="workTicketId"></param>
        /// <param name="currentUserID"></param>
        /// <returns></returns>
        ResponseBaseModel AddWorkerInformationForDate(int workTicketId, int currentUserID);

        /// <summary>
        /// Thêm thông tin biện pháp cho phép công tác ở lệnh thủy cơ nhiệt hóa
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel AddSubAllowedInfo(LctAddSubAllowedInfoModel requestModel);

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
        ResponseBaseModel AddWorkDailyModel(LctAddWorkDailyMobileModel requestModel);

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
        /// Tổ chức thực hiện (Thủy cơ nhiệt hóa)
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel AddWorkPlanInfo(LctAddWorkPlanInfoModel requestModel);

        /// <summary>
        /// Trả về thông tin Tổ chức thực hiện (881)
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <returns></returns>
        LctWorkPlanInfoModel GetWorkPlanInfo(int workTicketID);

        /// <summary>
        /// Trả về danh sách vị trí làm việc và danh sách nhân viên (881)
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <returns></returns>
        List<LctContentPositionModel> GetWorkLocations(int workTicketID);

        /// <summary>
        /// Số lệnh Chờ cấp lệnh công tác
        /// </summary>
        /// <param name="productionPlantID"></param>
        /// <returns></returns>
        LctWaitingNumberTicketModel GetNumberWaitingRegistrationTicket(int productionPlantID);

        /// <summary>
        /// Trả về chi tiết kết thúc công tác (881)
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <returns></returns>
        ResponseBaseModel GetWorkFinishDetail(int workTicketID);

        /// <summary>
        /// Xóa thông tin công tác hàng ngày
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
        ResponseBaseModel GetListStaff(LctSearchStaffModel requestModel);

        /// <summary>
        /// Lấy danh sách nhân viên được đăng ký
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <param name="commander"></param>
        /// <returns></returns>
        ResponseBaseModel GetStakeholders(int workTicketID, RolesStakeholder commander);

        /// <summary>
        /// Cập nhật chi tiết thông tin cho phép
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateAllowedInfoDetail(LctAddAllowedInfoDetailModel requestModel);

        /// <summary>
        /// Cập nhật thông tin cho phép
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateAllowedInfo(LctUpdateAllowedInfoModel requestModel);

        /// <summary>
        /// Cập nhật chi tiết tổ chức thực hiện
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateSafetyMeasure(LctAddSafetyMeasureModel requestModel);

        /// <summary>
        /// Cập nhật tổ chức thực hiện (Cập nhật số lượng người và đơn vị công tác)
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateReceiveWorkplace(LctUpdateReceiveWorkplaceModel requestModel);

        /// <summary>
        /// Xóa nhóm thông tin công tác hàng ngày
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseBaseModel DeleteStaffInformationGroup(int id, int userID);

        /// <summary>
        /// Kiểm tra tồn tại lệnh công tác
        /// </summary>
        /// <param name="workTicketNumber"></param>
        /// <param name="screenType"></param>
        /// <returns></returns>
        bool CheckExistTicketNumber(string workTicketNumber, int screenType);

        /// <summary>
        /// Reset số lệnh không sử dụng
        /// </summary>
        void ResetTicketNumber();

        /// <summary>
        /// Trả về thông tin thi hành lệnh
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <returns></returns>
        LctOrderExecutionMobileModel GetOrderExecutionMobile(int workTicketID);

        /// <summary>
        /// Tạo thông tin thi hành lệnh
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel AddEditOrderExecutionMobile(AddLctOrderExecutionMobileModel requestModel);

        /// <summary>
        /// Trả về thông tin kết thúc công tác
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <returns></returns>
        LctdWorkFinishMobileModel GetWorkFinishMobile(int workTicketID);

        /// <summary>
        /// Tạo thông tin kết thúc công tác
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel AddEditWorkFinishMobile(LctdAddEditWorkFinishMobileModel requestModel);

        /// <summary>
        /// Lấy danh sách người tham gia công việc lct Điện
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <returns></returns>
        List<LctStakeholdersResponseModel> GetLCTDTaskParticipants(int workTicketID);

        /// <summary>
        /// Lấy danh sách người tham gia công việc lct TCNH
        /// </summary>
        /// <param name="workTicketID"></param>
        /// <returns></returns>
        List<LctContentPositionModel> GetLCTTCNHTaskParticipants(int workTicketID);

        /// <summary>
        /// Tạo thông tin tổ chức thực hiện mobile
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel CreateExecutingOrgLCTDMobile(LctAddExecutingOrgMobileModel requestModel);

        /// <summary>
        /// Tạo thông tin Trình tự công việc và điều kiện an toàn 
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel AddEditWorkProcessSafetyMobile(AddEditWorkProcessSafetyMobileModel requestModel);
    }
}
