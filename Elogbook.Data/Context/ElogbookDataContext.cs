using Core.System.Runtime.Managers;
using Elogbook.Data.DataModels.Entities.BackupData;
using Elogbook.Data.DataModels.Entities.Dashboard;
using Elogbook.Data.DataModels.Entities.Report;
using Elogbook.Data.DataModels.Entities.SystemConfiguration;
using Elogbook.Data.DataModels.Entities.Widget;
using Elogbook.Data.WorkPermit;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace Elogbook.Data
{
    public partial class ElogbookDataContext : DbContext
    {
        public ElogbookDataContext()
            : base(RuntimeManager.GetConnectionString("ElogbookConnection"))
        {
            int.TryParse(ConfigurationManager.AppSettings["EFCommandTimeout"], out int commandTimeout);
            if (commandTimeout == 0) commandTimeout = 180;
            ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = commandTimeout;
        }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<GuestUser> GuestUsers { get; set; }

        public virtual DbSet<RoleInUser> RoleInUsers { get; set; }

        public virtual DbSet<UserRole> UserRoles { get; set; }

        public virtual DbSet<KnowledgeBase> KnowledgeBases { get; set; }

        public virtual DbSet<Modules> Modules { get; set; }

        public virtual DbSet<Factory> Factory { get; set; }

        public virtual DbSet<PowerUnit> PowerUnit { get; set; }

        public virtual DbSet<ParameterGroup> ParameterGroup { get; set; }

        public virtual DbSet<Parameter> Parameters { get; set; }

        public virtual DbSet<OperationShift> OperationShift { get; set; }

        public virtual DbSet<UsedTimes> UsedTimes { get; set; }

        public virtual DbSet<CycleManagement> CycleManagement { get; set; }

        public virtual DbSet<CycleFactory> CycleFactory { get; set; }

        public virtual DbSet<TimeLineCycle> TimeLineCycle { get; set; }

        public virtual DbSet<UserPermission> UserPermission { get; set; }

        public virtual DbSet<OperationPosition> OperationPosition { get; set; }

        public virtual DbSet<PositionInShift> PositionInShift { get; set; }

        public virtual DbSet<OperationCrew> OperationCrew { get; set; }

        public virtual DbSet<OperationDiary> OperationDiary { get; set; }

        public virtual DbSet<OperationSituation> OperationSituation { get; set; }

        public virtual DbSet<ActiveWork> ActiveWork { get; set; }

        public virtual DbSet<OperationDiaryPositionShift> OperationDiaryPositionShift { get; set; }

        public virtual DbSet<ProbationaryStaff> ProbationaryStaff { get; set; }

        public virtual DbSet<DeliveryReceiveShift> DeliveryReceiveShift { get; set; }

        public virtual DbSet<ExistInShift> ExistInShift { get; set; }

        public virtual DbSet<TrackingSyncMobileData> TrackingSyncMobileData { get; set; }

        public virtual DbSet<RegistrationTicket> RegistrationTicket { get; set; }

        public virtual DbSet<Stakeholders> Stakeholders { get; set; }

        public virtual DbSet<OtherRequirements> OtherRequirements { get; set; }

        public virtual DbSet<WorkTicket> WorkTicket { get; set; }

        public virtual DbSet<AllowedInfo> AllowedInfo { get; set; }

        public virtual DbSet<AllowedInfoDetail> AllowedInfoDetail { get; set; }

        public virtual DbSet<ReceiveWorkplace> ReceiveWorkplace { get; set; }

        public virtual DbSet<SafetyMeasure> SafetyMeasure { get; set; }

        public virtual DbSet<WorkerInformation> WorkerInformation { get; set; }

        public virtual DbSet<WorkPlacement> WorkPlacement { get; set; }

        public virtual DbSet<WorkFinish> WorkFinish { get; set; }

        public virtual DbSet<TicketStatus> TicketStatus { get; set; }

        public virtual DbSet<TicketManipulationStatus> TicketManipulationStatus { get; set; }

        public virtual DbSet<TicketStatusHistory> TicketStatusHistory { get; set; }

        public virtual DbSet<WorkFinishPicture> WorkFinishPicture { get; set; }

        public virtual DbSet<Attachment> Attachment { get; set; }

        public virtual DbSet<Organization> Organization { get; set; }

        public virtual DbSet<OperationParameters> OperationParameters { get; set; }

        public virtual DbSet<OperationTimeLine> OperationTimeLine { get; set; }

        public virtual DbSet<IdentificationAndSafetyMeasures> IdentificationAndSafetyMeasures { get; set; }

        public virtual DbSet<AbnormalIncident> AbnormalIncident { get; set; }

        public virtual DbSet<DeliveryShiftMethod> DeliveryShiftMethod { get; set; }

        public virtual DbSet<IssueOfShiftDetail> IssueOfShiftDetail { get; set; }

        public virtual DbSet<AlarmSetting> AlarmSettings { get; set; }

        public virtual DbSet<AlertDetail> AlertDetails { get; set; }

        public virtual DbSet<Signature> Signature { get; set; }

        public virtual DbSet<WorkTicketFeatures> WorkTicketFeatures { get; set; }

        public virtual DbSet<WarnAndSafe> WarnAndSafe { get; set; }

        public virtual DbSet<WorkDaily> WorkDaily { get; set; }

        public virtual DbSet<StaffInformation> StaffInformation { get; set; }

        public virtual DbSet<ContentPosition> ContentPosition { get; set; }

        public virtual DbSet<Participant> Participant { get; set; }

        public virtual DbSet<OtherRequest> OtherRequest { get; set; }

        public virtual DbSet<ManipulationTicket> ManipulationTicket { get; set; }

        public virtual DbSet<SequenceOperationParticipant> SequenceOperationParticipant { get; set; }

        public virtual DbSet<SequenceOperation> SequenceOperation { get; set; }

        public virtual DbSet<CancelTicketInfo> CancelTicketInfo { get; set; }

        public virtual DbSet<RegistrationManipulationTicket> RegistrationManipulationTicket { get; set; }

        public virtual DbSet<DeliveryAccepDevice> DeliveryAccepDevice { get; set; }

        public virtual DbSet<ManipulationTemplateTicket> ManipulationTemplateTicket { get; set; }

        public virtual DbSet<ExclusiveData> ExclusiveData { get; set; }

        public virtual DbSet<AppVersion> AppVersion { get; set; }

        public virtual DbSet<EmailConfiguration> EmailConfiguration { get; set; }

        public virtual DbSet<EmailTemplate> EmailTemplate { get; set; }

        public virtual DbSet<Schedule> Schedule { get; set; }

        public virtual DbSet<ActiveDirectory> ActiveDirectory { get; set; }

        public virtual DbSet<SystemData> SystemData { get; set; }

        public virtual DbSet<StatusCategory> StatusCategory { get; set; }

        public virtual DbSet<EmailTransmissions> EmailTransmissions { get; set; }

        public virtual DbSet<EmailTransmissionRecipients> EmailTransmissionRecipients { get; set; }

        public virtual DbSet<WorkTicketTracking> WorkTicketTrackings { get; set; }

        public virtual DbSet<TrackingStatus> TrackingStatus { get; set; }

        public virtual DbSet<TicketEvent> TicketEvents { get; set; }

        public virtual DbSet<MajorRepair> MajorRepairs { get; set; }

        public virtual DbSet<MajorRepairExcelDataTemp> MajorRepairExcelDataTemps { get; set; }

        public virtual DbSet<ScheduleHistory> ScheduleHistories { get; set; }

        public virtual DbSet<MajorRepairStatus> MajorRepairStatus { get; set; }

        public virtual DbSet<PMIS_A_ASSET> Assets { get; set; }

        public virtual DbSet<PMIS_A_LST_CATEGORY> AssetCategories { get; set; }

        public virtual DbSet<PMIS_A_LST_TYPE> AssetTypes { get; set; }

        public virtual DbSet<PMIS_S_ORGANIZATION> AssetOrg { get; set; }

        public virtual DbSet<AmLocation> AmLocation { get; set; }

        public virtual DbSet<AmDeviceType> AmDeviceType { get; set; }

        public virtual DbSet<AmDevice> AmDevice { get; set; }

        public virtual DbSet<AmStatus> AmStatus { get; set; }

        public virtual DbSet<AmWorkType> AmWorkType { get; set; }

        public virtual DbSet<AmWork> AmWork { get; set; }

        public virtual DbSet<AmMaintenanceWork> AmMaintenanceWork { get; set; }

        public virtual DbSet<AmMaintenanceSchedule> AmMaintenanceSchedule { get; set; }

        public virtual DbSet<AmReasonForStop> AmReasonForStop { get; set; }

        public virtual DbSet<AmReasonForPending> AmReasonForPending { get; set; }

        public virtual DbSet<AmDeviceAbnormalStatus> AmDeviceAbnormalStatus { get; set; }

        public virtual DbSet<AutonomousMaintenance> AutonomousMaintenance { get; set; }

        public virtual DbSet<AmWorkingProgress> AmWorkingProgress { get; set; }

        public virtual DbSet<AmWorkDetail> AmWorkDetail { get; set; }

        public virtual DbSet<AmWorkPending> AmWorkPending { get; set; }

        public virtual DbSet<AmEmergency> AmEmergency { get; set; }

        public virtual DbSet<AmStopMaintenance> AmStopMaintenance { get; set; }

        public virtual DbSet<AmRequestTicket> AmRequestTicket { get; set; }

        public virtual DbSet<AmRequestTicketHistory> AmRequestTicketHistory { get; set; }

        public virtual DbSet<AmRequestTicketStatus> AmRequestTicketStatus { get; set; }

        public virtual DbSet<PMIS_ASSET_MAINTENANCE> AssetMaintenances { get; set; }

        public virtual DbSet<PmisJobHistory> JobHistory { get; set; }

        public virtual DbSet<PMIS_G_ERP_COUNTRY> Countries { get; set; }

        public virtual DbSet<PMIS_S_COMPANY> Companies { get; set; }

        public virtual DbSet<PMIS_S_SITE> Sites { get; set; }

        public virtual DbSet<AmDeviceCategory> AmDeviceCategory { get; set; }

        public virtual DbSet<AmCountryManufacture> AmCountryManufacture { get; set; }

        public virtual DbSet<AmDeviceStatus> AmDeviceStatus { get; set; }

        public virtual DbSet<AmManufacturer> AmManufacturer { get; set; }

        public virtual DbSet<AmSupplier> AmSupplier { get; set; }

        public virtual DbSet<AmReportStatus> AmReportStatus { get; set; }

        public virtual DbSet<TrackingLoginOperation> TrackingLoginOperation { get; set; }

        public virtual DbSet<MajorRepairHistory> MajorRepairHistories { get; set; }

        public virtual DbSet<AlertNotification> AlertNotification { get; set; }

        public virtual DbSet<TokenExpire> TokenExpire { get; set; }

        public virtual DbSet<HazardsAndSafetyMeasures> HazardsAndSafetyMeasures { get; set; }

        public virtual DbSet<InspectionReport> InspectionReport { get; set; }

        public virtual DbSet<RelatedUser> RelatedUser { get; set; }

        public virtual DbSet<LctReceiveWorkplace> LctReceiveWorkplace { get; set; }

        public virtual DbSet<LctRegistrationTicket> LctRegistrationTicket { get; set; }

        public virtual DbSet<LctSafetyMeasure> LctSafetyMeasure { get; set; }

        public virtual DbSet<LctStaffInformation> LctStaffInformation { get; set; }

        public virtual DbSet<LctStakeholders> LctStakeholders { get; set; }

        public virtual DbSet<LctTicketStatus> LctTicketStatus { get; set; }

        public virtual DbSet<LctTicketStatusHistory> LctTicketStatusHistory { get; set; }

        public virtual DbSet<LctWarnAndSafe> LctWarnAndSafe { get; set; }

        public virtual DbSet<LctWorkDaily> LctWorkDaily { get; set; }

        public virtual DbSet<LctWorkerInformation> LctWorkerInformation { get; set; }

        public virtual DbSet<LctWorkFinish> LctWorkFinish { get; set; }

        public virtual DbSet<LctWorkFinishPicture> LctWorkFinishPicture { get; set; }

        public virtual DbSet<LctWorkPlacement> LctWorkPlacement { get; set; }

        public virtual DbSet<LctWorkTicket> LctWorkTicket { get; set; }

        public virtual DbSet<LctWorkTicketFeatureLink> LctWorkTicketFeatureLink { get; set; }

        public virtual DbSet<LctWorkTicketFeatures> LctWorkTicketFeatures { get; set; }

        public virtual DbSet<LctAllowedInfo> LctAllowedInfo { get; set; }

        public virtual DbSet<LctAllowedInfoDetail> LctAllowedInfoDetail { get; set; }

        public virtual DbSet<LctContentPosition> LctContentPosition { get; set; }

        public virtual DbSet<LctIdentificationAndSafetyMeasures> LctIdentificationAndSafetyMeasures { get; set; }

        public virtual DbSet<LctLeadershipUser> LctLeadershipUser { get; set; }

        public virtual DbSet<LctOtherRequirements> LctOtherRequirements { get; set; }

        public virtual DbSet<LctWorkDailyInfo> LctWorkDailyInfo { get; set; }

        public virtual DbSet<Report> Report { get; set; }

        public virtual DbSet<ReportWidgetType> ReportWidgetType { get; set; }

        public virtual DbSet<Widget> Widget { get; set; }

        public virtual DbSet<DashboardModule> DashboardModule { get; set; }

        public virtual DbSet<BackupData> BackupData { get; set; }

        public virtual DbSet<SystemConfiguration> SystemConfiguration { get; set; }

        public virtual DbSet<PasswordResetOtp> PasswordResetOtps { get; set; }

        public virtual DbSet<ResetSessionToken> ResetSessionTokens { get; set; }

        public virtual DbSet<ConfigurationWarning> ConfigurationWarnings { get; set; }

        public virtual DbSet<ConfigurationWarningRule> ConfigurationWarningRules { get; set; }

        public virtual DbSet<ExportOperationDiary> ExportOperationDiary { get; set; }

        public virtual DbSet<AssignmentEmployee> AssignmentEmployee { get; set; }

        public virtual DbSet<AssignmentEmployeeHistory> AssignmentEmployeeHistory { get; set; }

        public virtual DbSet<AssignmentSlot> AssignmentSlot { get; set; }

        public virtual DbSet<OperationParametersArchive> OperationParametersArchive { get; set; }

        public virtual DbSet<SlaConfig> SlaConfigs { get; set; }

        public virtual DbSet<SlaLevel> SlaLevels { get; set; }

        public virtual DbSet<SlaLevelRole> SlaLevelRoles { get; set; }

        public virtual DbSet<SlaCategory> SlaCategories { get; set; }

        public virtual DbSet<SlaCondition> SlaConditions { get; set; }

        public virtual DbSet<SlaRole> SlaRoles { get; set; }

        public virtual DbSet<UserGroup> UserGroups { get; set; }

        public virtual DbSet<UserInGroup> UserInGroup { get; set; }

        public virtual DbSet<BatchWorkReg> BatchWorkRegs { get; set; }

        public virtual DbSet<BatchWorkRegHistory> BatchWorkRegHistories { get; set; }

        public virtual DbSet<BatchWorkRegStatus> BatchWorkRegStatus { get; set; }

        public virtual DbSet<BatchWorkRegTemp> BatchWorkRegTemps { get; set; }

        public virtual DbSet<SlaLevelUserGroup> SlaLevelUserGroups { get; set; }

        public virtual DbSet<WorkSchedule> WorkSchedule { get; set; }

        public virtual DbSet<WorkScheduleBreak> WorkScheduleBreak { get; set; }

        public virtual DbSet<WorkScheduleDay> WorkScheduleDay { get; set; }

        public virtual DbSet<HolidaySchedule> HolidaySchedule { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Method intentionally left empty.
            modelBuilder.Entity<CycleFactory>()
            .HasRequired(c => c.Cycle)
            .WithMany()
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<CycleManagement>()
            .HasMany(o => o.CycleFactories)
            .WithOptional()
            .HasForeignKey(c => c.CycleID);

            modelBuilder.Entity<CycleFactory>()
            .HasRequired(c => c.Factory)
            .WithMany()
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
            .HasRequired(c => c.Company)
            .WithMany()
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<RegistrationTicket>()
            .HasRequired(c => c.Factory)
            .WithMany()
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<RegistrationTicket>()
            .HasRequired(c => c.ReceiveUnit)
            .WithMany()
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<Factory>()
            .HasRequired(c => c.Company)
            .WithMany()
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<Factory>()
            .HasRequired(c => c.ProductionPlant)
            .WithMany()
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<ConfigurationWarningRule>()
                .Property(p => p.ThresholdPercent1)
                .HasPrecision(5, 2);

            modelBuilder.Entity<ConfigurationWarningRule>()
                .Property(p => p.ThresholdPercent2)
                .HasPrecision(5, 2);

            modelBuilder.Entity<ConfigurationWarningRule>()
                .Property(p => p.ThresholdPercent3)
                .HasPrecision(5, 2);

            // OperationParametersArchive: composite PK (InsertedAt, ID)
            // InsertedAt là partition key — phải đứng đầu trong clustered index
            modelBuilder.Entity<OperationParametersArchive>()
                .HasKey(e => new { e.InsertedAt, e.ID });
        }
    }
}
