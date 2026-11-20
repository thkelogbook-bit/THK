using Elogbook.Core.Identity;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Elogbook.Core.Models.Response.OperationShift;
using Elogbook.Core.Models.Response.User;
using System.Collections.Generic;
using System.Net.Http;

namespace Elogbook.Core.Interfaces.Business
{
    /// <summary>
    /// Khai báo các phương thức dùng chung
    /// </summary>
    public interface ICommonBusiness
    {
        /// <summary>
        /// Lấy danh sách dữ liệu theo từng loại khác nhau
        /// </summary>
        /// <param name="dataTypes"></param>
        /// <param name="companyID"></param>
        /// <param name="productionPlantID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ResponseModel<SystemDataModel> GetSystemData(string[] dataTypes, int companyID, int productionPlantID, int userID);

        /// <summary>
        /// Tìm kiếm dữ liệu danh mục theo điều kiện
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="filterDescription"></param>
        /// <param name="companyID"></param>
        /// <param name="productionPlantID"></param>
        /// <returns></returns>
        ResponseModel<List<ValueLabelModel>> GetSystemDataByFilter(string dataType, string filterDescription, int companyID, int productionPlantID);

        /// <summary>
        /// Danh sách Phân xưởng theo Nhà máy
        /// </summary>
        /// <param name="factoryID"></param>
        /// <returns></returns>
        List<ValueLabelModel> GetProductionPlantByFactoryID(int factoryID);

        /// <summary>
        /// Danh sách Phân xưởng theo Công ty
        /// </summary>
        /// <param name="companyID"></param>
        /// <returns></returns>
        List<ProductionPlantInfoModel> GetProductionPlantByCompanyID(int companyID);

        /// <summary>
        /// Lấy danh sách tổ chức theo từng level
        /// </summary>
        /// <param name="organizationID"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        List<ValueLabelModel> GetOrganizations(int organizationID, string level);

        /// <summary>
        /// Danh sách nhà máy theo phân xưởng
        /// </summary>
        /// <param name="productionPlantID"></param>
        /// <returns></returns>
        List<ValueLabelModel> GetFactories(int productionPlantID);

        /// <summary>
        /// Danh sách dữ liệu dùng offline trên mobile
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="factoryID"></param>
        /// <param name="operationPositionID"></param>
        /// <param name="force">Trả về tất cả dữ liệu nếu force = true</param>
        /// <returns></returns>
        ResponseModel<MasterDataModel> GetMasterData(string deviceID, int factoryID, int operationPositionID, bool force);

        /// <summary>
        /// Lấy danh sách người dùng cho mobile
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        ResponseModel<List<UserInfo>> GetUsersInfo(string fullName);

        /// <summary>
        /// Lấy danh sách cương vị trong ca cho mobile
        /// </summary>
        /// <param name="operationPositionID"></param>
        /// <returns></returns>
        ResponseModel<List<PositionInShiftInfo>> GetPositionShiftInfo(int operationPositionID);

        /// <summary>
        /// Lấy danh sách cương vị trong ca và nhân sự phân bổ
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<List<PositionInShiftInfo>> GetPositionShiftAndEmployeeInfo(PositionInShiftAndEmployeeModel requestModel);

        /// <summary>
        /// Lấy danh sách phiếu công tác
        /// </summary>
        /// <param name="factoryID"></param>
        /// <param name="ticketNumber"></param>
        /// <returns></returns>
        ResponseModel<List<TicketInforModel>> GetTickets(int factoryID, string ticketNumber);

        /// <summary>
        /// Lấy danh sách phiếu công tác điện, thủy cơ nhiệt hóa cho mobile
        /// </summary>
        /// <param name="factoryID"></param>
        /// <param name="ticketNumber"></param>
        /// <returns></returns>
        ResponseModel<List<TicketNumberInfo>> GetMobileTickets(int factoryID, string ticketNumber);

        /// <summary>
        /// Lấy chữ ký người dùng theo yêu cầu
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        ResponseModel<UserInfo> GetUserSignature(string username, string password, CustomIdentity currentUser);

        /// <summary>
        /// Lấy các tổ máy của nhà máy
        /// </summary>
        /// <param name="factoryID"></param>
        /// <returns></returns>
        ResponseModel<List<ValueLabelModel>> GetPowerUnits(int factoryID);

        /// <summary>
        /// Lấy các chu kỳ trong nhà máy
        /// </summary>
        /// <param name="factoryID"></param>
        /// <returns></returns>
        ResponseModel<List<ValueLabelModel>> GetCycles(int factoryID);

        /// <summary>
        /// Lấy danh sách chu kỳ ghi thông số
        /// </summary>
        /// <param name="factoryID"></param>
        /// <param name="operationPositionID"></param>
        /// <returns></returns>
        ResponseModel<List<CyclesModel>> GetOperationParameterCycles(int factoryID, int operationPositionID);

        /// <summary>
        /// Lấy các cương vị mobile app
        /// </summary>
        /// <param name="factoryID"></param>
        /// <returns></returns>
        ResponseModel<List<MobileOperationPositionInfo>> GetOperationPositionsByFactoryIDMobile(int factoryID);

        /// <summary>
        /// Danh sách cương vị trong nhà máy
        /// </summary>
        /// <param name="factoryID"></param>
        /// <returns></returns>
        ResponseModel<List<ComboboxOperationPositionModel>> GetOperationPositionsByFactoryID(int factoryID);

        /// <summary>
        /// Danh sách cương vị trong nhà máy ngoại trừ cương vị hiện tại
        /// </summary>
        /// <param name="factoryID"></param>
        /// <param name="currentPositionID"></param>
        /// <returns></returns>
        ResponseModel<List<ComboboxOperationPositionModel>> GetPositionsWithoutCurrentPosition(int factoryID, int currentPositionID);

        /// <summary>
        /// Danh sách ca trong nhà máy
        /// </summary>
        /// <param name="factoryID"></param>
        /// <param name="hour"></param>
        /// <returns></returns>
        ResponseModel<List<OperationShiftBaseModel>> GetOperationShiftsByFactoryID(int factoryID, string hour);

        /// <summary>
        /// Danh sách kíp vận hành trong nhà máy
        /// </summary>
        /// <param name="factoryID"></param>
        /// <returns></returns>
        ResponseModel<List<ValueLabelModel>> GetOperationCrewsByFactoryID(int factoryID);

        /// <summary>
        /// Danh sách nhóm thông số theo cương vị
        /// </summary>
        /// <param name="operationPositionID"></param>
        /// <returns></returns>
        ResponseModel<List<ValueLabelModel>> GetParameterGroupByOperationPositionID(int operationPositionID);

        /// <summary>
        /// Danh sách nhân viên trong nhà máy
        /// </summary>
        /// <param name="factoryID"></param>
        /// <returns></returns>
        ResponseModel<CategoryModel> GetCategoryByFactoryID(int factoryID);

        /// <summary>
        /// Lấy danh sách setting cảnh báo
        /// </summary>
        ResponseModel<AlarmSettingModel> GetAlarmSettings();

        /// <summary>
        /// Lấy thông tin chữ ký
        /// </summary>
        /// <param name="ID"> Id chữ ký </param>
        /// <returns></returns>
        ResponseModel<SignatureInfo> GetSignatureById(int ID);

        /// <summary>
        /// Lấy thông tin người dùng
        /// </summary>
        /// <param name="userName"> userName </param>
        /// <param name="isExternal"> Người ngoại hệ thống hay không </param>
        /// <returns></returns>
        UserInfo GetUserByUserName(string userName, bool isExternal);

        /// <summary>
        /// Lấy thông tin người dùng ngoài hệ thống
        /// </summary>
        /// <param name="ID"> ID </param>
        /// <returns></returns>
        ResponseModel<UserModel> GetExternalUserById(int ID);

        /// <summary>
        /// Tìm đơn vị đăng ký và đơn vị cấp phiếu
        /// </summary>
        /// <param name="isUnitRegistration"></param>
        /// <returns></returns>
        List<ValueLabelModel> GetUnitRegistrationOrUnitApprove(bool isUnitRegistration);

        /// <summary>
        /// Danh sách người dùng nội bộ/Bên ngoài đơn vị
        /// </summary>
        /// <param name="factoryID"></param>
        /// <param name="searchText"></param>
        /// <param name="electricRank"></param>
        /// <param name="userType"></param>
        /// <param name="ticketType"></param>
        /// <returns></returns>
        List<InternalExternalUserModel> GetInternalExternalUser(int factoryID, string searchText, string electricRank, int userType, string ticketType);

        /// <summary>
        /// Danh sách người dùng nội bộ/Bên ngoài đơn vị
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        ResponseBaseModel GetInternalExternalUserMobile(GetInternalExternalUserModel request);

        /// <summary>
        /// Lấy danh sách chữ ký theo id
        /// </summary>
        /// <param name="signatureIds"></param>
        /// <returns></returns>
        List<SignatureInfo> GetSignatures(string signatureIds);

        /// <summary>
        /// Lấy trạng thai phiếu công tác
        /// </summary>
        /// <param name="isRegistrationTicket"></param>
        /// <param name="ticketType"></param>
        /// <returns></returns>
        List<ValueLabelModel> GetTicketStatus(bool isRegistrationTicket, string ticketType);

        /// <summary>
        /// Lấy trạng thai phiếu thao tác
        /// </summary>
        /// <param name="isRegistrationTicket"></param>
        /// <returns></returns>
        List<ValueLabelModel> GetTicketManipulationStatus(bool isRegistrationTicket);

        /// <summary>
        /// Tìm mã số đơn vị đăng ký
        /// </summary>
        /// <param name="iD"></param>
        /// <param name="ticketType"></param>
        /// <returns></returns>
        string GetRegistrationTicketNumber(int iD, string ticketType);

        /// <summary>
        /// Tìm tên đơn vị đăng ký
        /// </summary>
        /// <returns></returns>
        string GetRegistrationUnitView(int Id);

        /// <summary>
        /// Danh sách nhà máy theo người dùng
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="filterDescription"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>
        ResponseModel<List<ValueLabelModel>> GetFactoriesByUserRight(int userID, string filterDescription, bool isActive);

        /// <summary>
        /// Danh sách quyền người dùng
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        UserRolesInfo GetUserRight(int userID);

        /// <summary>
        /// Danh sách nhóm thông số theo nhà máy, cương vị
        /// </summary>
        /// <param name="factoryID"></param>
        /// <param name="operationPositionID"></param>
        /// <returns></returns>
        List<ValueLabelModel> GetParameterGroupByFactoryIDAndPositionID(int factoryID, int operationPositionID);

        /// <summary>
        /// Tìm mã số đơn vị cấp phiếu của phiếu thao tác
        /// </summary>
        /// <param name="iD"></param>
        /// <returns></returns>
        string GetManipulationUnitApprover(int iD);

        /// <summary>
        /// Tìm đơn vị đơn vị cấp phiếu (chỉ lấy phân xưởng vận hành)
        /// </summary>
        /// <returns></returns>
        List<ValueLabelModel> GetUnitApprove();

        /// <summary>
        /// Tìm số phiếu thao tác
        /// </summary>
        /// <param name="idManipulationTicket"></param>
        /// <returns></returns>
        string GetTicketNumberManipulation(int idManipulationTicket);

        /// <summary>
        /// Danh sách nhà máy đang hoạt động
        /// </summary>
        /// <returns></returns>
        List<ValueLabelModel> GetFactoriesActive();

        /// <summary>
        /// Lấy danh sách đơn vị công tác theo công ty
        /// </summary>
        /// <param name="companyID"></param>
        /// <returns></returns>
        List<ValueLabelModel> GetWorkUnitByCompany(int companyID);

        /// <summary>
        /// Lọc user
        /// </summary>
        /// <param name="filterUserModel"></param>
        /// <returns></returns>
        List<InternalExternalUserModel> FilterUser(FilterUserModel filterUserModel);

        /// <summary>
        /// Phương thức ký cấp/khóa phiếu công tác
        /// </summary>
        /// <returns></returns>
        List<ValueLabelModel> GetSigningMethods();

        /// <summary>
        /// Danh sách email template
        /// </summary>
        /// <returns></returns>
        List<ValueLabelModel> GetEmailTemplates();

        /// <summary>
        /// Chức năng hoạt động trên mobile
        /// </summary>
        /// <returns></returns>
        List<ValueLabelModel> GetMobileActivities();

        /// <summary>
        /// Lấy các sự kiện theo loại phiếu
        /// </summary>
        /// <param name="dataType">Loại phiếu PCT-Đ, PCT-TCNH</param>
        /// <returns></returns>
        List<ValueLabelModel> GetTicketEventTypes(int dataType);

        /// <summary>
        /// Lấy danh sách định dạng số phiếu
        /// </summary>
        /// <param name="type">PTT, PCTTCNH, PCTĐ</param>
        /// <returns></returns>
        ResponseModel<TicketFormatDefaultModel> GetTicketFormatDefault(string type);

        /// <summary>
        /// AM - Danh sách dữ liệu dùng chung:
        /// </summary>
        /// <param name="categoryNames"></param>
        /// <param name="factoryID"></param>
        /// <param name="companyID"></param>
        /// <returns></returns>
        AmSystemDataModel GetAmCommonData(string categoryNames, int factoryID, int companyID);

        /// <summary>
        /// tìm mã số phiếu yêu cầu
        /// </summary>
        /// <param name="iD"></param>
        /// <returns></returns>
        string GetRequestTicketNumber(int iD);

        /// <summary>
        /// Tìm mã số đơn vị đăng ký theo tháng năm
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="ticketType"></param>
        /// <param name="registrationTicketID"></param>
        /// <returns></returns>
        string GetWorkTicketNumber(int userID, string ticketType, int registrationTicketID);

        /// <summary>
        /// Tìm mã số phiếu yêu cầu theo công ty
        /// </summary>
        /// <param name="factoryID"></param>
        /// <returns></returns>
        string GetRequestTicketNumberByFactory(int factoryID);

        /// <summary>
        /// Đồng bộ dữ liệu
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> SyncData(DataSyncModel requestModel);

        /// <summary>
        /// Kiểm tra quyền của người dùng
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="permissionKey"></param>
        /// <returns></returns>
        ResponseModel<bool> CheckUserHasRight(int userID, string permissionKey);

        /// <summary>
        /// Tìm kiếm nhanh thông tin dữ liệu
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel QuickAutoComplete(QuickAutoCompleteModel requestModel);

        /// <summary>
        /// Trả về tất cả thông báo theo người dùng đăng nhập
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseBaseModel GetAlertNotifications(AlertNotificationSearchModel requestModel);

        /// <summary>
        /// get số thông báo chưa đọc
        /// </summary>
        /// <param name="iD"></param>
        /// <returns></returns>
        int GetCountAlertNotice(int iD);

        /// <summary>
        /// Get thông báo theo id
        /// </summary>
        /// <param name="alertId"></param>
        /// <returns></returns>
        AlertNotificationModel GetAlertNoticeById(int alertId);

        /// <summary>
        /// Update thông báo
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        ResponseModel<bool> UpdateAlertNotice(UpdateAlertNoticeModel requestModel);

        /// <summary>
        /// Get số thông báo chưa đọc cho Mobile
        /// </summary>
        /// <returns></returns>
        ResponseModel<int> GetCountAlertNoticeForMobile(int userID);

        /// <summary>
        /// Tải file mẫu
        /// </summary>
        /// <param name="categoryType"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        HttpResponseMessage ExportFileTemplate(int categoryType, int companyId);

        /// <summary>
        /// Import data
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="categoryType"></param>
        /// <returns></returns>
        ResponseModel<bool> ImportDataFromExcelFile(FileUploadInfoModel requestModel, int categoryType);

        /// <summary>
        /// Lấy trạng thái lệnh công tác
        /// </summary>
        /// <param name="ticketType"></param>
        /// <returns></returns>
        List<ValueLabelModel> GetLctTicketStatus(string ticketType);
    }
}
