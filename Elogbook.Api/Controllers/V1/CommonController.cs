using Elogbook.Core;
using Elogbook.Core.Constants;
using Elogbook.Core.Enums;
using Elogbook.Core.Interfaces.Business;
using Elogbook.Core.Log;
using Elogbook.Core.Models;
using Elogbook.Core.Models.Request;
using Elogbook.Core.Models.Response;
using Elogbook.Core.Models.Response.OperationShift;
using Microsoft.Web.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Elogbook.Api.Controllers.V1
{
    /// <summary>
    /// Định nghĩa những api chung cho toàn ứng dụng
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/common")]
    public class CommonController : BaseApiController
    {
        private readonly ICommonBusiness _commonBusiness;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="commonBusiness"></param>
        public CommonController(ICommonBusiness commonBusiness)
        {
            _commonBusiness = commonBusiness;
        }

        /// <summary>
        /// Danh sách dữ liệu dùng chung:
        /// Nhà máy, Phân xưởng, Trạng thái truy cập, Loại tài khoản, Vai trò, Nguồn tài khoản, Trạng thái sử dụng
        /// </summary>
        /// <param name="dataTypes"></param>
        /// <returns></returns>
        [Route("getSystemData")]
        [HttpGet]
        public ResponseModel<SystemDataModel> GetSystemData([FromUri] string[] dataTypes)
        {
            return _commonBusiness.GetSystemData(dataTypes, CurrentUser.CompanyID, CurrentUser.ProductionPlantID, CurrentUser.ID);
        }

        /// <summary>
        /// Tìm kiếm dữ liệu danh mục theo điều kiện
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="filterDescription"></param>
        /// <returns></returns>
        [Route("getSystemDataByFilter")]
        [HttpGet]
        public ResponseModel<List<ValueLabelModel>> GetSystemDataByFilter(string dataType, string filterDescription = "")
        {
            return _commonBusiness.GetSystemDataByFilter(dataType, filterDescription, CurrentUser.CompanyID, CurrentUser.ProductionPlantID);
        }

        /// <summary>
        ///  Danh sách Phân xưởng theo Nhà máy
        /// </summary>
        /// <param name="companyID"></param>
        /// <returns></returns>
        [Route("getProductionPlantByFactoryID")]
        [HttpGet]
        public List<ValueLabelModel> GetProductionPlantByFactoryID(int companyID)
        {
            return _commonBusiness.GetProductionPlantByFactoryID(companyID);
        }

        /// <summary>
        ///  Danh sách Phân xưởng theo Công ty
        /// </summary>
        /// <param name="companyID"></param>
        /// <returns></returns>
        [Route("getProductionPlantByCompanyID")]
        [HttpGet]
        public List<ProductionPlantInfoModel> GetProductionPlantByCompanyID(int companyID)
        {
            return _commonBusiness.GetProductionPlantByCompanyID(companyID);
        }

        /// <summary>
        /// Danh sách tổ chức theo từng level
        /// </summary>
        /// <param name="organizationID"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        [Route("getOrganizations")]
        [HttpGet]
        public List<ValueLabelModel> GetOrganizations(int organizationID, string level)
        {
            return _commonBusiness.GetOrganizations(organizationID, level);
        }

        /// <summary>
        /// Danh sách nhà máy theo phân xưởng
        /// </summary>
        /// <param name="productionPlantID"></param>
        /// <returns></returns>
        [Route("getFactories/{productionPlantID}")]
        [HttpGet]
        public List<ValueLabelModel> GetFactories(int productionPlantID)
        {
            return _commonBusiness.GetFactories(productionPlantID);
        }

        /// <summary>
        /// Danh sách dữ liệu Quản trị/Danh mục dùng offline cho mobile
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="factoryID"></param>
        /// <param name="operationPositionID"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        [Route("getMasterData")]
        [HttpGet]
        [AllowAnonymous]
        public ResponseModel<MasterDataModel> GetMasterData(string deviceID, int factoryID, int operationPositionID, bool force = false)
        {
            LogCustom.LogMessage(TrackingLogLevel.INFO, $"{GetType()}, GetMasterData(), deviceID: {deviceID}, factoryID: {factoryID}, operationPositionID: {operationPositionID}, force: {force}");
            return _commonBusiness.GetMasterData(deviceID, factoryID, operationPositionID, force);
        }

        /// <summary>
        /// Danh sách người dùng cho mobile
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        [Route("getUsersInfo")]
        [HttpGet]
        public ResponseModel<List<UserInfo>> GetUsersInfo(string fullName = "")
        {
            return _commonBusiness.GetUsersInfo(fullName);
        }

        /// <summary>
        /// Danh sách cương vị trong ca cho mobile
        /// </summary>
        /// <param name="operationPositionID"></param>
        /// <returns></returns>
        [Route("getPositionShiftInfo")]
        [HttpGet]
        public ResponseModel<List<PositionInShiftInfo>> GetPositionShiftInfo(int operationPositionID)
        {
            return _commonBusiness.GetPositionShiftInfo(operationPositionID);
        }

        /// <summary>
        /// Lấy danh sách cương vị trong ca và nhân sự phân bổ
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("getPositionShiftAndEmployeeInfo")]
        [HttpPost]
        public ResponseModel<List<PositionInShiftInfo>> GetPositionShiftAndEmployeeInfo([FromBody] PositionInShiftAndEmployeeModel requestModel)
        {
            return _commonBusiness.GetPositionShiftAndEmployeeInfo(requestModel);
        }

        /// <summary>
        /// Danh sách phiếu công tác(Đã cho phép và Đang thực hiện)
        /// </summary>
        /// <param name="factoryID"></param>
        /// <param name="ticketNumber"></param>
        /// <returns></returns>
        [Route("getTickets")]
        [HttpGet]
        public ResponseModel<List<TicketInforModel>> GetTickets(int factoryID, string ticketNumber = "")
        {
            return _commonBusiness.GetTickets(factoryID, ticketNumber);
        }

        /// <summary>
        /// Lấy danh sách phiếu công tác điện, thủy cơ nhiệt hóa cho mobile
        /// </summary>
        /// <param name="factoryID"></param>
        /// <param name="ticketNumber"></param>
        /// <returns></returns>
        [Route("getMobileTickets")]
        [HttpGet]
        public ResponseModel<List<TicketNumberInfo>> GetMobileTickets(int factoryID, string ticketNumber = "")
        {
            return _commonBusiness.GetMobileTickets(factoryID, ticketNumber);
        }

        /// <summary>
        /// Chữ ký người dùng theo yêu cầu
        /// </summary>
        /// <returns></returns>
        [Route("getUserSignature")]
        [HttpGet]
        public ResponseModel<UserInfo> GetUserSignature(string username, string password)
        {
            return _commonBusiness.GetUserSignature(username, password, CurrentUser);
        }

        /// <summary>
        /// Danh sách các tổ máy trong nhà máy
        /// </summary>
        /// <param name="factoryID"></param>
        /// <returns></returns>
        [Route("getPowerUnits")]
        [HttpGet]
        public ResponseModel<List<ValueLabelModel>> GetPowerUnits(int factoryID)
        {
            return _commonBusiness.GetPowerUnits(factoryID);
        }

        /// <summary>
        /// Danh sách chu kỳ trong nhà máy
        /// </summary>
        /// <param name="factoryID"></param>
        /// <returns></returns>
        [Route("getCycles")]
        [HttpGet]
        public ResponseModel<List<ValueLabelModel>> GetCycles(int factoryID)
        {
            return _commonBusiness.GetCycles(factoryID);
        }

        /// <summary>
        /// Danh sách chu kỳ ghi thông số
        /// </summary>
        /// <param name="factoryID">ID Nhà máy</param>
        /// <param name="operationPositionID">ID Cương vị</param>
        /// <returns></returns>
        [Route("getOperationParameterCycles")]
        [HttpGet]
        public ResponseModel<List<CyclesModel>> GetOperationParameterCycles(int factoryID, int operationPositionID)
        {
            return _commonBusiness.GetOperationParameterCycles(factoryID, operationPositionID);
        }

        /// <summary>
        /// Danh sách cương vị trong nhà máy
        /// </summary>
        /// <param name="factoryID"></param>
        /// <returns></returns>
        [Route("getOperationPositionsByFactoryID/{factoryID}")]
        [HttpGet]
        public ResponseModel<List<ComboboxOperationPositionModel>> GetOperationPositionsByFactoryID(int factoryID)
        {
            return _commonBusiness.GetOperationPositionsByFactoryID(factoryID);
        }

        /// <summary>
        /// Danh sách cương vị trong nhà máy ngoại trừ cương vị hiện tại
        /// </summary>
        /// <param name="factoryID">mã nhà máy</param>
        /// <param name="currentPositionID">mã cương vị hiện tại</param>
        /// <returns></returns>
        [Route("GetPositionsWithoutCurrentPosition")]
        [HttpGet]
        public ResponseModel<List<ComboboxOperationPositionModel>> GetPositionsWithoutCurrentPosition(int factoryID, int currentPositionID)
        {
            return _commonBusiness.GetPositionsWithoutCurrentPosition(factoryID, currentPositionID);
        }

        /// <summary>
        /// Danh sách ca vận hành trong nhà máy
        /// </summary>
        /// <param name="factoryID"></param>
        /// <param name="hour"></param>
        /// <returns></returns>
        [Route("getOperationShiftsByFactoryID")]
        [HttpGet]
        public ResponseModel<List<OperationShiftBaseModel>> GetOperationShiftsByFactoryID(int factoryID, string hour = "")
        {
            return _commonBusiness.GetOperationShiftsByFactoryID(factoryID, hour);
        }

        /// <summary>
        /// Danh sách kíp vận hành trong nhà máy
        /// </summary>
        /// <param name="factoryID"></param>
        /// <returns></returns>
        [Route("getOperationCrewsByFactoryID/{factoryID}")]
        [HttpGet]
        public ResponseModel<List<ValueLabelModel>> GetOperationCrewsByFactoryID(int factoryID)
        {
            return _commonBusiness.GetOperationCrewsByFactoryID(factoryID);
        }

        /// <summary>
        /// Danh sách nhóm thông số theo cương vị
        /// </summary>
        /// <param name="operationPositionID"></param>
        /// <returns></returns>
        [Route("getParameterGroupByOperationPositionID/{operationPositionID}")]
        [HttpGet]
        public ResponseModel<List<ValueLabelModel>> GetParameterGroupByOperationPositionID(int operationPositionID)
        {
            return _commonBusiness.GetParameterGroupByOperationPositionID(operationPositionID);
        }

        /// <summary>
        /// Danh sách nhân viên trong nhà máy
        /// </summary>
        /// <param name="factoryID"></param>
        /// <returns></returns>
        [Route("GetCategoryByFactoryID/{factoryID}")]
        [HttpGet]
        public ResponseModel<CategoryModel> GetCategoryByFactoryID(int factoryID)
        {
            return _commonBusiness.GetCategoryByFactoryID(factoryID);
        }

        /// <summary>
        /// Lấy danh sách setting cảnh báo
        /// </summary>
        /// <returns></returns>
        [Route("GetAlarmSetting")]
        [HttpGet]
        public ResponseModel<AlarmSettingModel> GetAlarmSettings()
        {
            return _commonBusiness.GetAlarmSettings();
        }

        /// <summary>
        /// Lấy thông tin chữ ký
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [Route("getSignatureById")]
        [HttpGet]
        public ResponseModel<SignatureInfo> GetSignatureById(int ID)
        {
            return _commonBusiness.GetSignatureById(ID);
        }

        /// <summary>
        /// Tìm đơn vị đăng ký và đơn vị cấp phiếu
        /// </summary>
        /// <param name="isUnitRegistration"></param>
        /// <returns></returns>
        [Route("GetUnitRegistrationOrUnitApprove")]
        [HttpGet]
        public List<ValueLabelModel> GetUnitRegistrationOrUnitApprove(bool isUnitRegistration)
        {
            return _commonBusiness.GetUnitRegistrationOrUnitApprove(isUnitRegistration);
        }

        /// <summary>
        /// Tìm đơn vị đơn vị cấp phiếu (chỉ lấy phân xưởng vận hành)
        /// </summary>
        /// <returns></returns>
        [Route("GetUnitApprove")]
        [HttpGet]
        public List<ValueLabelModel> GetUnitApprove()
        {
            return _commonBusiness.GetUnitApprove();
        }

        /// <summary>
        /// Danh sách người dùng nội bộ/Bên ngoài đơn vị
        /// </summary>
        /// <param name="factoryID"></param>
        /// <param name="searchText"></param>
        /// <param name="electricRank"></param>
        /// <param name="userType"></param>
        /// <param name="ticketType"></param>
        /// <returns></returns>
        [Route("getInternalExternalUser")]
        [HttpGet]
        public List<InternalExternalUserModel> GetInternalExternalUser(int factoryID = CommonConstant.ALL_KEY, string searchText = "", string electricRank = "",
            int userType = CommonConstant.ALL_KEY, string ticketType = "PCTD")
        {
            return _commonBusiness.GetInternalExternalUser(factoryID, searchText, electricRank, userType, ticketType);
        }

        /// <summary>
        /// Danh sách người dùng nội bộ/Bên ngoài đơn vị
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("getInternalExternalUserMobile")]
        [HttpPost]
        public ResponseBaseModel GetInternalExternalUserMobile([FromBody] GetInternalExternalUserModel request)
        {
            request.ProductionPlantID = CurrentUser.ProductionPlantID;
            return _commonBusiness.GetInternalExternalUserMobile(request);
        }

        /// <summary>
        /// lấy danh sách chữ ký theo id
        /// </summary>
        /// <returns></returns>
        [Route("getSignatures")]
        [HttpGet]
        public List<SignatureInfo> GetSignatures(string signatureIds)
        {
            return _commonBusiness.GetSignatures(signatureIds);
        }

        /// <summary>
        /// Lấy trạng thái phiếu công tác
        /// </summary>
        /// <param name="isRegistrationTicket"></param>
        /// <param name="ticketType"></param>
        /// <returns></returns>
        [Route("getTicketStatus")]
        [HttpGet]
        public List<ValueLabelModel> GetTicketStatus(bool isRegistrationTicket, string ticketType = "PCT")
        {
            return _commonBusiness.GetTicketStatus(isRegistrationTicket, ticketType);
        }

        /// <summary>
        /// Lấy danh sách trạng thái phiếu thao tác
        /// </summary>
        /// <param name="isRegistrationTicket">true: đăng ký phiếu thao tác - false: quản lý phiếu</param>
        /// <returns></returns>
        [Route("GetTicketManipulationStatus")]
        [HttpGet]
        public List<ValueLabelModel> GetTicketManipulationStatus(bool isRegistrationTicket)
        {
            return _commonBusiness.GetTicketManipulationStatus(isRegistrationTicket);
        }

        /// <summary>
        /// Tìm mã số đơn vị đăng ký
        /// </summary>
        /// <param name="ticketType">DKPCTD: Phiếu công tác điện, DKPTCNH: Phiếu thủy cơ nhiệt hóa, DKPTT: Phiếu thao tác</param>
        /// <returns></returns>
        [Route("getRegistrationTicketNumber")]
        [HttpGet]
        public string GetRegistrationTicketNumber(string ticketType)
        {
            return _commonBusiness.GetRegistrationTicketNumber(CurrentUser.ID, ticketType);
        }

        /// <summary>
        /// Tìm tên hiển thị đơn vị cấp phiếu của phiếu thao tác
        /// </summary>
        /// <returns></returns>
        [Route("GetManipulationUnitApprover")]
        [HttpGet]
        public string GetManipulationUnitApprover()
        {
            return _commonBusiness.GetManipulationUnitApprover(CurrentUser.ID);
        }

        /// <summary>
        /// Tìm số phiếu thao tác
        /// </summary>
        /// <param name="idManipulationTicket">Mã số phiếu thao tác đã đăng ký cần duyệt</param>
        /// <returns></returns>
        [Route("GetTicketNumberManipulation")]
        [HttpGet]
        public string GetTicketNumberManipulation(int idManipulationTicket)
        {
            return _commonBusiness.GetTicketNumberManipulation(idManipulationTicket);
        }

        /// <summary>
        /// Tìm tên đơn vị đăng ký
        /// </summary>
        /// <param name="userId">mã người dùng dùng để lấy tên đơn vị đăng ký</param>
        /// <returns></returns>
        [Route("GetRegistrationUnitView")]
        [HttpGet]
        public string GetRegistrationUnitView([FromUri] int userId)
        {
            return _commonBusiness.GetRegistrationUnitView(userId);
        }

        /// <summary>
        /// Danh sách nhà máy theo người dùng
        /// </summary>
        /// <param name="filterDescription"></param>
        /// <param name="isActive">True: Trả về nhà máy đang hoạt động, False: Trả về tất cả</param>
        /// <returns></returns>
        [Route("getFactoriesByUserRight")]
        [HttpGet]
        public ResponseModel<List<ValueLabelModel>> GetFactoriesByUserRight(string filterDescription = "", bool isActive = false)
        {
            return _commonBusiness.GetFactoriesByUserRight(CurrentUser.ID, filterDescription, isActive);
        }

        /// <summary>
        /// Danh sách nhóm thông số theo nhà máy, cương vị
        /// </summary>
        /// <param name="factoryID"></param>
        /// <param name="operationPositionID"></param>
        /// <returns></returns>
        [Route("getParameterGroupByFactoryIDAndPositionID")]
        [HttpGet]
        public List<ValueLabelModel> GetParameterGroupByFactoryIDAndPositionID(int factoryID, int operationPositionID)
        {
            return _commonBusiness.GetParameterGroupByFactoryIDAndPositionID(factoryID, operationPositionID);
        }

        /// <summary>
        /// Danh sách nhà máy đang hoạt động
        /// </summary>
        /// <returns></returns>
        [Route("getFactoriesActive")]
        [HttpGet]
        public List<ValueLabelModel> GetFactoriesActive()
        {
            return _commonBusiness.GetFactoriesActive();
        }

        /// <summary>
        /// Danh sách nhà máy đang hoạt động dành cho Mobile
        /// </summary>
        /// <returns></returns>
        [Route("getFactoriesActiveMobile")]
        [HttpGet]
        [AllowAnonymous]
        public List<ValueLabelModel> GetFactoriesActiveMobile()
        {
            return _commonBusiness.GetFactoriesActive();
        }

        /// <summary>
        /// Danh sách cương vị trong nhà máy cho mobile app
        /// </summary>
        /// <param name="factoryID"></param>
        /// <returns></returns>
        [Route("getOperationPositionsByFactoryIDMobile/{factoryID}")]
        [HttpGet]
        [AllowAnonymous]
        public ResponseModel<List<MobileOperationPositionInfo>> GetOperationPositionsByFactoryIDMobile(int factoryID)
        {
            return _commonBusiness.GetOperationPositionsByFactoryIDMobile(factoryID);
        }

        /// <summary>
        /// Lấy danh sách đơn vị công tác theo công ty
        /// </summary>
        /// <param name="companyID"></param>
        /// <returns></returns>
        [Route("getWorkUnitByCompany")]
        [HttpGet]
        public List<ValueLabelModel> GetWorkUnitByCompany(int companyID)
        {
            return _commonBusiness.GetWorkUnitByCompany(companyID);
        }

        /// <summary>
        /// Lọc user
        /// </summary>
        /// <param name="filterUserModel"></param>
        /// <returns></returns>
        [Route("filterUser")]
        [HttpPost]
        public List<InternalExternalUserModel> FilterUser(FilterUserModel filterUserModel)
        {
            return _commonBusiness.FilterUser(filterUserModel);
        }

        /// <summary>
        /// Phương thức ký cấp/khóa phiếu công tác
        /// </summary>
        /// <returns></returns>
        [Route("getSigningMethods")]
        [HttpGet]
        public List<ValueLabelModel> GetSigningMethods()
        {
            return _commonBusiness.GetSigningMethods();
        }

        /// <summary>
        /// Danh sách email template
        /// </summary>
        /// <returns></returns>
        [Route("getEmailTemplates")]
        [HttpGet]
        public List<ValueLabelModel> GetEmailTemplates()
        {
            return _commonBusiness.GetEmailTemplates();
        }

        /// <summary>
        /// Chức năng hoạt động trên mobile
        /// </summary>
        /// <returns></returns>
        [Route("getMobileActivities")]
        [HttpGet]
        [AllowAnonymous]
        public List<ValueLabelModel> GetMobileActivities()
        {
            return _commonBusiness.GetMobileActivities();
        }

        /// <summary>
        /// Lấy các sự kiện theo loại phiếu
        /// </summary>
        /// <param name="dataType">Loại phiếu PCT-Đ, PCT-TCNH</param>
        /// <returns></returns>
        [Route("getTicketEventTypes")]
        [HttpGet]
        public List<ValueLabelModel> GetTicketEventTypes(int dataType)
        {
            return _commonBusiness.GetTicketEventTypes(dataType);
        }

        /// <summary>
        /// Danh sách định dạng số phiếu
        /// </summary>
        /// <param name="type">PTT, PCTTCNH, PCTD</param>
        /// <returns></returns>
        [Route("getTicketFormatDefault/{type}")]
        [HttpGet]
        public ResponseModel<TicketFormatDefaultModel> GetTicketFormatDefault(string type)
        {
            return _commonBusiness.GetTicketFormatDefault(type);
        }

        /// <summary>
        /// Am- Danh sách dữ liệu dùng chung: Thiết bị, Loại thiết bị, Khu vực,Trạng thái,...
        /// </summary>
        /// <param name="categoryNames">"AmUsageStatus,AmStatus,AmDevice,..."</param>
        /// <param name="factoryID">1</param>
        /// <returns></returns>
        [Route("getAmCommonData")]
        [HttpGet]
        public AmSystemDataModel GetAmCommonData([FromUri] string categoryNames, int factoryID = 0)
        {
            return _commonBusiness.GetAmCommonData(categoryNames, factoryID, CurrentUser.CompanyID);
        }

        /// <summary>
        /// Tìm mã số phiếu yêu cầu
        /// </summary>
        /// <returns></returns>
        [Route("getRequestTicketNumber")]
        [HttpGet]
        public string GetRequestTicketNumber()
        {
            return _commonBusiness.GetRequestTicketNumber(CurrentUser.ID);
        }

        /// <summary>
        /// Tìm mã số đơn vị đăng ký theo tháng năm
        /// </summary>
        /// <param name="ticketType">QLPCTD: Phiếu công tác điện, QLPTCNH: Phiếu thủy cơ nhiệt hóa, QLPTT: Phiếu thao tác</param>
        /// <param name="registrationTicketID"></param>
        /// <returns></returns>
        [Route("GetRegistrationTicketNumberByMonthYear")]
        [HttpGet]
        public string GetWorkTicketNumber(string ticketType, int registrationTicketID)
        {
            return _commonBusiness.GetWorkTicketNumber(CurrentUser.ID, ticketType, registrationTicketID);
        }

        /// <summary>
        /// Tìm mã số phiếu yêu cầu theo công ty
        /// </summary>
        /// <param name="factoryID"></param>
        /// <returns></returns>
        [Route("getRequestTicketNumberByFactory/{factoryID}")]
        [HttpGet]
        public string GetRequestTicketNumberByFactory([FromUri] int factoryID = 0)
        {
            return _commonBusiness.GetRequestTicketNumberByFactory(factoryID);
        }

        /// <summary>
        /// Đồng bộ dữ liệu tracking đăng nhập cương vị
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("syncData")]
        [HttpPost]
        public ResponseModel<bool> SyncData([FromBody] DataSyncModel requestModel)
        {
            return _commonBusiness.SyncData(requestModel);
        }

        /// <summary>
        /// Tìm kiếm nhanh thông tin dữ liệu
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("quickAutoComplete")]
        [HttpPost]
        public ResponseBaseModel QuickAutoComplete([FromBody] QuickAutoCompleteModel requestModel)
        {
            return _commonBusiness.QuickAutoComplete(requestModel);
        }

        /// <summary>
        /// Trả về tất cả thông báo theo người dùng đăng nhập
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("getListAlertNotice")]
        [HttpPost]
        public ResponseBaseModel GetAlertNotifications([FromBody] AlertNotificationSearchModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _commonBusiness.GetAlertNotifications(requestModel);
        }

        /// <summary>
        /// Get số thông báo chưa đọc
        /// </summary>
        /// <returns></returns>
        [Route("getCountAlertNotice")]
        [HttpGet]
        public int GetCountAlertNotice()
        {
            return _commonBusiness.GetCountAlertNotice(CurrentUser.ID);
        }

        /// <summary>
        /// Trả về tất cả thông báo theo người dùng đăng nhập
        /// </summary>
        /// <param name="alertId"></param>
        /// <returns></returns>
        [Route("getAlertNoticeById")]
        [HttpGet]
        public AlertNotificationModel GetAlertNoticeById([FromUri] int alertId)
        {
            return _commonBusiness.GetAlertNoticeById(alertId);
        }

        /// <summary>
        /// Cập nhật trạng thái thông báo
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Route("updateAlertNotice")]
        [HttpPut]
        public ResponseModel<bool> UpdateAlertNotice([FromBody] UpdateAlertNoticeModel requestModel)
        {
            requestModel.CurrentUserID = CurrentUser.ID;
            return _commonBusiness.UpdateAlertNotice(requestModel);
        }

        /// <summary>
        /// Get số thông báo chưa đọc cho Mobile
        /// </summary>
        /// <returns></returns>
        [Route("getCountAlertNoticeForMobile")]
        [HttpGet]
        public ResponseModel<int> GetCountAlertNoticeForMobile()
        {
            return _commonBusiness.GetCountAlertNoticeForMobile(CurrentUser.ID);
        }

        /// <summary>
        /// Tải file mẫu
        /// </summary>
        /// <param name="categoryType">1: Loại công việc, 2 Công việc, 3:Mức độ khẩn cấp, 4: Khu vực,... </param>
        /// <returns>File</returns>
        [Route("exportFileTemplate")]
        [HttpGet]
        public HttpResponseMessage ExportFileTemplate(int categoryType)
        {
            return _commonBusiness.ExportFileTemplate(categoryType, CurrentUser.CompanyID);
        }

        /// <summary>
        /// Import data to Database
        /// </summary>
        /// <param name="categoryType">1: Loại công việc, 2 Công việc, 3:Mức độ khẩn cấp, 4: Khu vực,... </param>
        /// <returns></returns>
        [Route("importData")]
        [HttpPost]
        public async Task<ResponseModel<bool>> ImportDataFromExcelFile(int categoryType)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();

            try
            {
                var excelPath = Path.Combine(AppSettings.FILE_STORAGE_SERVER_DIR, CommonConstant.MAJOR_REPAIR);
                var provider = new MultipartFormDataStreamProvider(Path.Combine(excelPath));
                string timeLable = DateTime.Now.ToString("yyyyMMddHHmmss");

                if (!Directory.Exists(excelPath))
                {
                    Directory.CreateDirectory(excelPath);
                }

                await Request.Content.ReadAsMultipartAsync(provider);
                if (provider.FileData.Count > 0)
                {
                    var file = provider.FileData[0];
                    var fileName = file.Headers.ContentDisposition.FileName;
                    string[] arrFileName = file.Headers.ContentDisposition.FileName.Trim().Split('.');
                    var fileExtension = arrFileName[arrFileName.Length - 1].Trim('"').ToLower();

                    if (!fileExtension.Equals("xlsx"))
                    {
                        responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                        responseModel.Data = false;
                        responseModel.ErrorMessages.Add(CommonConstant.XLSX_EXTENTION_REQUIRED);
                        return responseModel;
                    }
                    fileName = fileName.Remove(fileName.LastIndexOf('.'), fileName.Length - fileName.LastIndexOf('.'));
                    fileName = System.Text.RegularExpressions.Regex.Replace(fileName.Trim('"'), @"[\W]+", "_") + "_" + timeLable + "." + fileExtension;
                    var localFileName = file.LocalFileName;

                    FileUploadInfoModel requestModel = new FileUploadInfoModel()
                    {
                        FileName = fileName,
                        LocalFileName = localFileName,
                        CurrentUserID = CurrentUser.ID,
                        FilePath = Path.Combine(excelPath, fileName),
                        CurrentCompanyID = CurrentUser.CompanyID
                    };
                    return _commonBusiness.ImportDataFromExcelFile(requestModel, categoryType);
                }
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Data = false;
                responseModel.ErrorMessages.Add(ex.Message);
                return responseModel;
            }

            return responseModel;
        }

        /// <summary>
        /// Lấy trạng thái lệnh công tác
        /// </summary>
        /// <param name="ticketType"></param>
        /// <returns></returns>
        [Route("getLctTicketStatus")]
        [HttpGet]
        public List<ValueLabelModel> GetLctTicketStatus(string ticketType = null)
        {
            return _commonBusiness.GetLctTicketStatus(ticketType);
        }
    }
}
