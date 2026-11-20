namespace Elogbook.Core.Log
{
    /// <summary>
    /// Định nghĩa các category log
    /// </summary>
    public enum Category
    {
        /// <summary>
        /// Phân xưởng
        /// </summary>
        ProductionPlantBusiness = 1,
        /// <summary>
        /// Tổ máy
        /// </summary>
        PowerUnitBusiness = 2,
        /// <summary>
        /// Nhóm thông số
        /// </summary>
        ParameterGroupBusiness = 3,
        /// <summary>
        /// Cương vị
        /// </summary>
        OperationPositionBusiness = 4,
        /// <summary>
        /// Cương vị trong ca
        /// </summary>
        PositionInShiftBusiness = 5,
        /// <summary>
        /// Kíp vận hành
        /// </summary>
        OperationCrewBusiness = 6,
        /// <summary>
        /// Người dùng
        /// </summary>
        UserBusiness,
        /// <summary>
        /// Common
        /// </summary>
        Common,
        /// <summary>
        /// Chu Kỳ
        /// </summary>
        CycleManagerBusiness,
        /// <summary>
        /// Chu Kỳ
        /// </summary>
        UserPermission,
        /// <summary>
        /// Vai trò
        /// </summary>
        UserRoleBusiness,
        /// <summary>
        /// Ca vận hành
        /// </summary>
        OperationShiftBusiness,
        /// <summary>
        /// Nhật ký vận hành
        /// </summary>
        OperationDiaryBusiness,
        /// <summary>
        /// Phiếu công tác
        /// </summary>
        WorkTicketBusiness,
        /// <summary>
        /// Phiếu đăng ký công tác
        /// </summary>
        RegistrationTicketBusiness,
        /// <summary>
        /// Download và Upload file
        /// </summary>
        DownloadAndUpload,
        /// <summary>
        /// Tổ chức
        /// </summary>
        OrganizationBusiness,
        /// <summary>
        /// Nhà máy
        /// </summary>
        FactoryBusiness,
        /// <summary>
        /// Thông số
        /// </summary>
        ParameterBusiness,
        /// <summary>
        /// Thông số vận hành
        /// </summary>
        OperationParameterBusiness,
        /// <summary>
        /// Phiếu đăng ký công tác cơ thuỷ nhiệt hoá
        /// </summary>
        RegistrationWorkTicketBusiness,
        /// <summary>
        /// Quản lý download/upload file
        /// </summary>
        FileManagement,
        /// <summary>
        /// Quản lý tồn tại
        /// </summary>
        IssueOfShift,
        /// <summary>
        /// Người ngoài hệ thống
        /// </summary>
        GuestUserBusiness,
        /// <summary>
        /// Phiếu thao tác
        /// </summary>
        RegistrationManipulationTicketBusiness,
        /// <summary>
        /// Quản lý Phiếu thao tác
        /// </summary>
        ManipulationTicketBusiness,
        /// <summary>
        /// Danh mục trạng thái
        /// </summary>
        StatusCategoryBusiness,
        /// <summary>
        /// Bất thường sự cố
        /// </summary>
        AbnormalIncidentBusiness,
        /// <summary>
        /// Thông tin thay đổi người cho phép
        /// </summary>
        WorkTicketTrackingBusiness,
        /// <summary>
        /// Thông tin sửa chữa lớn
        /// </summary>
        MajorRepairBusiness,
        /// <summary>
        /// TicketEvent
        /// </summary>
        TicketEventBusiness,
        /// <summary>
        /// PMISParameterTask
        /// </summary>
        PMISParameterTask,
        /// <summary>
        /// (AM) Danh mục thiết bị
        /// </summary>
        AmDeviceBusiness,
        /// <summary>
        /// (AM) Danh mục loại thiết bị
        /// </summary>
        AmDeviceTypeBusiness,
        /// <summary>
        /// (AM) Danh mục địa điểm
        /// </summary>
        AmLocationBusiness,
        /// <summary>
        /// (AM) Danh mục loại công viêc
        /// </summary>
        AmWorkTypeBusiness,
        /// <summary>
        /// (AM) Danh mục công việc
        /// </summary>
        AmWorkBusiness,
        /// <summary>
        /// (AM) Lịch bảo trì
        /// </summary>
        AmMaintenanceScheduleBusiness,
        /// <summary>
        /// (AM) Nguyên nhân tạm dừng công việc
        /// </summary>
        AmReasonForPendingBusiness,
        /// <summary>
        /// (AM) Nguyên nhân dừng thực hiện
        /// </summary>
        AmReasonForStopBusiness,
        /// <summary>
        /// (AM) Bảo trì tự quản
        /// </summary>
        AutonomousMaintenanceBusiness,
        /// <summary>
        /// mức độ khẩn cấp
        /// </summary>
        EmergencyBusiness,
        /// <summary>
        /// phiếu yêu cầu
        /// </summary>
        RequestTicketBusiness,
        /// <summary>
        /// lịch sử đồng bộ
        /// </summary>
        JobHistoryBusiness,
        /// <summary>
        /// phân loại thiết bị
        /// </summary>
        DeviceCategoryBusiness,
        /// <summary>
        /// lịch sử đồng bộ
        /// </summary>
        DashboardIncidentBusiness,
        /// <summary>
        /// am report
        /// </summary>
        AmReportBusiness,
        /// <summary>
        /// Active Directory
        /// </summary>
        ActiveDirectoryBusiness,
        /// <summary>
        /// Email Template
        /// </summary>
        EmailTemplateBusiness,
        /// <summary>
        /// Traking login cương vị
        /// </summary>
        TrackingLoginOperationBusiness,
        /// <summary>
        /// AmSupplierBusiness
        /// </summary>
        AmSupplierBusiness,
        /// <summary>
        /// CountryManufactureBusiness
        /// </summary>
        AmCountryManufactureBusiness,
        /// <summary>
        /// ManufacturerBusiness
        /// </summary>
        AmManufacturerBusiness,
        /// <summary>
        /// HazardsAndSafetyMeasuresBusiness
        /// </summary>
        HazardsAndSafetyMeasuresBusiness,
        /// <summary>
        /// Thông tin phiếu đăng ký công tác số lượng lớn
        /// </summary>
        BatchWorkRegBusiness,
        /// <summary>
        /// DcsImportDataService
        /// </summary>
        DcsImportDataService,
        /// <summary>
        /// BBKSHT
        /// </summary>
        InspectionReportBusiness,
        /// <summary>
        /// InspectionReportService
        /// </summary>
        InspectionReportService,
        /// <summary>
        /// Lệnh đăng ký công tác
        /// </summary>
        RegistrationWorkPermitBusiness,
        /// <summary>
        /// Widget
        /// </summary>
        WidgetBusiness,
        /// <summary>
        /// Dashboard
        /// </summary>
        DashboardBusiness,
        /// <summary>
        /// Report
        /// </summary>
        ReportBusiness,
        /// <summary>
        /// Backup data
        /// </summary>
        BackupDataBusiness,
        /// <summary>
        /// Tài liệu
        /// </summary>
        KnowledgeBaseBusiness,
        /// <summary>
        /// AiBusiness
        /// </summary>
        AiBusiness,
        /// <summary>
        /// Quản lý phân công nhân sự ca vận hành
        /// </summary>
        AssignmentBusiness,
        /// <summary>
        /// KnowledgeBaseAi — tích hợp Gemini AI cho tra cứu tài liệu
        /// </summary>
        KnowledgeBaseAiBusiness,
        /// <summary>
        /// Cấu hình SLA
        /// </summary>
        SlaConfigBusiness,
        /// <summary>
        /// Nhóm người dùng
        /// </summary>
        UserGroupBusiness,
        /// <summary>
        /// Thời gian làm việc
        /// </summary>
        WorkScheduleBusiness,
        /// <summary>
        /// Open Notebook integration — upload tài liệu sang Open Notebook
        /// </summary>
        OpenNotebook,
    }
}
