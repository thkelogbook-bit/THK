using Elogbook.Core.Models.Response.MasterData.AMs;
using Elogbook.Core.Models.Response.SystemConfiguration;
using Elogbook.Data;
using System.Collections.Generic;

namespace Elogbook.Core.Models.Response
{
    /// <summary>
    /// Danh sách dữ liệu offile cho mobile
    /// </summary>
    public class MasterDataModel
    {
        /// <summary>
        /// Loại Phiếu
        /// </summary>
        public List<MobileSignatureInfo> Signatures { get; set; }

        /// <summary>
        /// Loại Phiếu
        /// </summary>
        public List<WorkTicketFeaturesModel> WorkTicketFeatures { get; set; }

        /// <summary>
        /// Nhân viên
        /// </summary>
        public List<MobileUserInfo> Users { get; set; }

        /// <summary>
        /// Vai trò của Nhân viên
        /// </summary>
        public List<MobileRoleInUsersInfo> RoleInUsers { get; set; }

        /// <summary>
        /// Vai trò
        /// </summary>
        public List<MobileUserRoleInfo> UserRole { get; set; }

        /// <summary>
        /// Modules phân quyền
        /// </summary>
        public List<MobileModulesInfo> Modules { get; set; }

        /// <summary>
        /// Các quyền của hệ thống
        /// </summary>
        public List<MobileUserPermissionInfo> UserPermission { get; set; }

        /// <summary>
        /// Nhà máy
        /// </summary>
        public List<MobileFactoryInfo> Factory { get; set; }

        /// <summary>
        /// Tổ chức
        /// </summary>
        public List<MobileOrganizationInfo> Organization { get; set; }

        /// <summary>
        /// Ca vận hành
        /// </summary>
        public List<MobileOperationShiftInfo> PositionShift { get; set; }

        /// <summary>
        /// Thời gian sử dụng ca vận hành
        /// </summary>
        public List<MobileUsedTimesInfo> UsedTimes { get; set; }

        /// <summary>
        /// Chu kỳ
        /// </summary>
        public List<MobileCycleManagementInfo> CycleManagement { get; set; }

        /// <summary>
        /// Nhà máy sử dụng Chu kỳ
        /// </summary>
        public List<MobileCycleFactoryInfo> CycleFactory { get; set; }

        /// <summary>
        /// Các mốc thời gian của Chu kỳ
        /// </summary>
        public List<MobileTimeLineCycleInfo> TimeLineCycle { get; set; }

        /// <summary>
        /// Tổ máy
        /// </summary>
        public List<MobilePowerUnitInfo> PowerUnit { get; set; }

        /// <summary>
        /// Thông số
        /// </summary>
        public List<MobileParameterInfo> Parameter { get; set; }

        /// <summary>
        /// Thông tin setting cho thông số
        /// </summary>
        public List<MobileAlertDetailInfo> AlertDetail { get; set; }

        /// <summary>
        /// Dữ liệu cấu hình setting cho thông số
        /// </summary>
        public List<MobileAlarmSettingsInfo> AlarmSettings { get; set; }

        /// <summary>
        /// Nhóm thông số
        /// </summary>
        public List<MobileParameterGroupInfo> ParameterGroup { get; set; }

        /// <summary>
        /// Kíp vận hành
        /// </summary>
        public List<MobileOperationCrewInfo> OperationCrew { get; set; }

        /// <summary>
        /// Cương vị
        /// </summary>
        public List<MobileOperationPositionInfo> OperationPosition { get; set; }

        /// <summary>
        /// Cương vị trong ca
        /// </summary>
        public List<MobilePositionInShiftInfo> PositionInShift { get; set; }

        /// <summary>
        /// Vị trí phân công nhân sự
        /// </summary>
        public List<MobileAssignmentSlotInfo> AssignmentSlot { get; set; }

        /// <summary>
        /// Thông tin nhân sự được phân công
        /// </summary>
        public List<MobileAssignmentEmployeeInfo> AssignmentEmployee { get; set; }

        /// <summary>
        /// Nhật ký vận hành
        /// </summary>
        public List<MobileOperationDiaryInfo> OperationDiary { get; set; }

        /// <summary>
        /// Cương vị trong ca của Nhật ký vận hành
        /// </summary>
        public List<MobileOperationDiaryPositionShiftInfo> OperationDiaryPositionShift { get; set; }

        /// <summary>
        /// Tình hình vận hành
        /// </summary>
        public List<MobileOperationSituationInfo> OperationSituation { get; set; }

        /// <summary>
        /// Công tác đang thực hiện
        /// </summary>
        public List<MobileActiveWorkInfo> ActiveWork { get; set; }

        /// <summary>
        /// Bất thường sự cố
        /// </summary>
        public List<MobileAbnormalIncidentInfo> AbnormalIncident { get; set; }

        /// <summary>
        /// Phương thức giao ca
        /// </summary>
        public List<MobileDeliveryShiftMethodInfo> DeliveryShiftMethod { get; set; }

        /// <summary>
        /// Tồn tại trong ca
        /// </summary>
        public List<MobileExistInShiftInfo> ExistInShift { get; set; }

        /// <summary>
        /// Giao/nhận ca
        /// </summary>
        public List<MobileDeliveryReceiveShiftInfo> DeliveryReceiveShift { get; set; }

        /// <summary>
        /// Thông số vận hành
        /// </summary>
        public List<MobileOperationParametersInfo> OperationParameters { get; set; }

        /// <summary>
        /// Các mốc thời gian ghi thông số vận hành
        /// </summary>
        public List<MobileOperationTimeLineInfo> OperationTimeLine { get; set; }

        /// <summary>
        /// Phiếu thao tác
        /// </summary>
        public List<MobileManipulationTicketInfo> ManipulationTicket { get; set; }

        /// <summary>
        /// Table lưu dữ liệu thông tin hủy phiếu thao tác
        /// </summary>
        public List<MobileCancelTicketInfo> CancelTicketInfo { get; set; }

        /// <summary>
        /// Table lưu dữ liệu giao nhận, nghiệm thu đường dây, thiết bị điện trước và sau khi thao tác
        /// </summary>
        public List<MobileDeliveryAccepDeviceInfo> DeliveryAccepDevice { get; set; }

        /// <summary>
        /// Table lưu dữ liệu Người giám sát, Người thao tác
        /// </summary>
        public List<MobileParticipantInfo> Participant { get; set; }

        /// <summary>
        /// Table lưu dữ liệu trình tự hạng mục thao tác
        /// </summary>
        public List<MobileSequenceOperationInfo> SequenceOperation { get; set; }

        /// <summary>
        /// Table tồn tại trong ca
        /// </summary>
        public List<MobileIssueOfShiftDetailInfo> IssueOfShiftDetail { get; set; }

        /// <summary>
        /// Table lưu dữ liệu 1/Điều kiện cần có thể thực hiện, 2/Lưu ý
        /// </summary>
        public List<MobileOtherRequestInfo> OtherRequest { get; set; }

        /// <summary>
        /// Table lưu dữ liệu người tham gia phiếu thao tác
        /// </summary>
        public List<MobileSequenceOperationParticipantInfo> SequenceOperationParticipant { get; set; }

        /// <summary>
        /// Phiếu công tác
        /// </summary>
        public List<MobileWorkTicketInfo> WorkTicket { get; set; }

        /// <summary>
        /// Danh sách tệp đính kèm
        /// </summary>
        public List<MobileAttachmentInfo> Attachments { get; set; }

        /// <summary>
        /// Thông tin cấu hình
        /// </summary>
        public MobileConfigurationInfo ConfigurationInfo { get; set; }

        /// <summary>
        /// Thông tin version app
        /// </summary>
        public AppVersionModel AppVersionInfo { get; set; }

        #region AM data
        /// <summary>
        /// Danh sách Danh mục - Thiết bị AM
        /// </summary>
        public List<MobileAmDevice> MobileAmDevices { get; set; }

        /// <summary>
        /// Danh sách Danh mục - Thiết bị AM
        /// </summary>
        public List<MobileAmStatus> MobileAmStatus { get; set; }

        /// <summary>
        /// Danh sách Danh mục - Trạng thái bất thường thiết bị AM
        /// </summary>
        public List<MobileAmDeviceAbnormalStatus> MobileAmDeviceAbnormalStatus { get; set; }

        /// <summary>
        /// Danh sách Danh mục - Loại thiết bị
        /// </summary>
        public List<MobileAmDeviceType> MobileAmDeviceTypes { get; set; }

        /// <summary>
        /// Danh sách MobileAmEmergency model 
        /// </summary>
        public List<MobileAmEmergency> MobileAmEmergencys { get; set; }

        /// <summary>
        /// Danh sách Danh mục - Địa điểm/Khu vực 
        /// </summary>
        public List<MobileAmLocation> MobileAmLocations { get; set; }

        /// <summary>
        /// Danh sách Quản lý lịch thực hiện AM 
        /// </summary>
        public List<MobileAmMaintenanceSchedule> MobileAmMaintenanceSchedules { get; set; }

        /// <summary>
        /// Danh sách Quản lý Công việc AM của một lịch trình 
        /// </summary>
        public List<MobileAmMaintenanceWork> MobileAmMaintenanceWorks { get; set; }

        /// <summary>
        /// Danh sách Danh mục - Nguyên nhân tạm dừng thực hiện 
        /// </summary>
        public List<MobileAmReasonForPending> MobileAmReasonForPendings { get; set; }

        /// <summary>
        /// Danh sách - Nguyên nhân không thực hiện (Dùng cho Không thực hiện Bảo trì tự quản và Dừng 1 thực hiện AM cụ thể ) 
        /// </summary>
        public List<MobileAmReasonForStop> MobileAmReasonForStops { get; set; }

        /// <summary>
        /// Danh sách Không thực hiện bảo trì tự quản 
        /// </summary>
        public List<MobileAmStopMaintenance> MobileAmStopMaintenances { get; set; }

        /// <summary>
        /// Danh sách Danh mục - Công việc 
        /// </summary>
        public List<MobileAmWork> MobileAmWorks { get; set; }

        /// <summary>
        /// Danh sách Thông tin chi tiết thực hiện công việc AM 
        /// </summary>
        public List<MobileAmWorkDetail> MobileAmWorkDetails { get; set; }

        /// <summary>
        /// Danh sách Tiến trình thực hiện AM 
        /// </summary>
        public List<MobileAmWorkingProgress> MobileAmWorkingProgresss { get; set; }

        /// <summary>
        /// Danh sách Tạm dừng hoặc Dừng thực hiện công việc 
        /// </summary>
        public List<MobileAmWorkPending> MobileAmWorkPendings { get; set; }

        /// <summary>
        /// Danh sách Danh muc - Loại công việc 
        /// </summary>
        public List<MobileAmWorkType> MobileAmWorkTypes { get; set; }

        /// <summary>
        /// Danh sách Bảo trì tự quản - AutonomousMaintenance 
        /// </summary>
        public List<MobileAutonomousMaintenance> MobileAutonomousMaintenances { get; set; }

        /// <summary>
        /// Danh sách phân loại AM
        /// </summary>
        public List<MobileAmCategory> MobileAmCategories { get; set; }

        /// <summary>
        /// Danh sách Danh mục Nước sản xuất
        /// </summary>
        public List<MobileAmCountryManufacture> MobileAmCountryManufactures { get; set; }

        /// <summary>
        /// Danh sách status của thiết bị
        /// </summary>
        public List<MobileAmDeviceStatus> MobileAmDeviceStatus { get; set; }

        /// <summary>
        /// Danh mục Nhà chế tạo AM
        /// </summary>
        public List<MobileAmManufacturer> MobileAmManufacturers { get; set; }

        /// <summary>
        /// Danh mục Nhà cung cấp
        /// </summary>
        public List<MobileAmSupplier> MobileAmSuppliers { get; set; }

        #endregion

    }

}
