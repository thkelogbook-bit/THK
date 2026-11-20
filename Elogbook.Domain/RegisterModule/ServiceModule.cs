using Autofac;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Domain.Services;

namespace Elogbook.Domain.RegisterModule
{
    public class ServiceModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Register services
            // Register for all services in Elogbook.Domain.Services
            builder.RegisterAssemblyTypes(System.AppDomain.CurrentDomain.GetAssemblies())
               .Where(a =>
                   a.FullName.StartsWith("Elogbook.Domain.Services")
               )
               .AsImplementedInterfaces()
               .InstancePerRequest();

            builder.RegisterType(typeof(OperationParametersService))
                .As(typeof(IOperationParametersService))
                .InstancePerLifetimeScope();

            builder.RegisterType(typeof(WorkTicketService))
                .As(typeof(IWorkTicketService))
                .InstancePerLifetimeScope();

            builder.RegisterType(typeof(CommonService))
                .As(typeof(ICommonService))
                .InstancePerLifetimeScope();

            builder.RegisterType(typeof(RegistrationTicketService))
                .As(typeof(IRegistrationTicketService))
                .InstancePerLifetimeScope();

            builder.RegisterType(typeof(PmisService))
                .As(typeof(IPmisService))
                .InstancePerLifetimeScope();

            builder.RegisterType(typeof(EmailService))
                .As(typeof(IEmailService))
                .InstancePerLifetimeScope();

            builder.RegisterType(typeof(EmailConfigurationService))
                .As(typeof(IEmailConfigurationService))
                .InstancePerLifetimeScope();

            builder.RegisterType(typeof(UserService))
                .As(typeof(IUserService))
                .InstancePerLifetimeScope();

            builder.RegisterType(typeof(PasswordResetService))
                .As(typeof(IPasswordResetService))
                .InstancePerLifetimeScope();

            builder.RegisterType(typeof(MajorRepairService))
               .As(typeof(IMajorRepairService))
               .InstancePerLifetimeScope();

            builder.RegisterType(typeof(BatchEtlService))
               .As(typeof(IBatchEtlService))
               .InstancePerLifetimeScope();

            builder.RegisterType(typeof(AutonomousMaintenanceService))
               .As(typeof(IAutonomousMaintenanceService))
               .InstancePerLifetimeScope();

            builder.RegisterType(typeof(JobHistoryService))
               .As(typeof(IJobHistoryService))
               .InstancePerLifetimeScope();

            builder.RegisterType(typeof(DashboardIncidentService))
               .As(typeof(IDashboardIncidentService))
               .InstancePerLifetimeScope();

            builder.RegisterType(typeof(SystemConfigurationService))
               .As(typeof(ISystemConfigurationService))
               .InstancePerLifetimeScope();

            builder.RegisterType(typeof(AmRequestTicketService))
               .As(typeof(IAmRequestTicketService))
               .InstancePerLifetimeScope();

            builder.RegisterType(typeof(EmailTemplateService))
               .As(typeof(IEmailTemplateService))
               .InstancePerLifetimeScope();

            builder.RegisterType(typeof(OrganizationService))
               .As(typeof(IOrganizationService))
               .InstancePerLifetimeScope();

            builder.RegisterType(typeof(AmReportService))
               .As(typeof(IAmReportService))
               .InstancePerLifetimeScope();

            builder.RegisterType(typeof(DcsImportDataService))
                .As(typeof(IDcsImportDataService))
                .InstancePerLifetimeScope();

            builder.RegisterType(typeof(WidgetService))
                .As(typeof(IWidgetService))
                .InstancePerLifetimeScope();
            builder.RegisterType(typeof(DashboardService))
                .As(typeof(IDashboardService))
                .InstancePerLifetimeScope();
            builder.RegisterType(typeof(ReportService))
                .As(typeof(IReportService))
                .InstancePerLifetimeScope();
            builder.RegisterType(typeof(BackupDataService))
                .As(typeof(IBackupDataService))
                .InstancePerLifetimeScope();
            builder.RegisterType(typeof(FileManagementService))
                .As(typeof(IFileManagementService))
                .InstancePerLifetimeScope();
            builder.RegisterType(typeof(PdfSigningService))
               .As(typeof(IPdfSigningService))
               .InstancePerLifetimeScope();
            builder.RegisterType(typeof(VnptClientService))
               .As(typeof(IVnptClientService))
               .InstancePerLifetimeScope();
            builder.RegisterType(typeof(ViettelMySignService))
               .As(typeof(IViettelMySignService))
               .InstancePerLifetimeScope();

            builder.RegisterType(typeof(LuceneSearchService))
               .As(typeof(ILuceneSearchService))
               .InstancePerLifetimeScope();

            builder.RegisterType(typeof(LuceneSearchService))
               .As(typeof(ILuceneSearchService))
               .SingleInstance();

            builder.RegisterType(typeof(KnowledgeBaseAiService))
               .As(typeof(IKnowledgeBaseAiService))
               .InstancePerLifetimeScope();

            builder.RegisterType(typeof(DashboardModuleService))
               .As(typeof(IDashboardModuleService))
               .InstancePerLifetimeScope();

            builder.RegisterType(typeof(HistoryCallService))
               .As(typeof(IHistoryCallService))
               .InstancePerLifetimeScope();

            builder.RegisterType(typeof(WorkScheduleService))
               .As(typeof(IWorkScheduleService))
               .InstancePerLifetimeScope();

            builder.RegisterType(typeof(UserGroupService))
               .As(typeof(IUserGroupService))
               .InstancePerLifetimeScope();

            builder.RegisterType(typeof(SlaConfigService))
               .As(typeof(ISlaConfigService))
               .InstancePerLifetimeScope();

            builder.RegisterType(typeof(OpenNotebookService))
               .As(typeof(IOpenNotebookService))
               .InstancePerLifetimeScope();

            builder.RegisterType(typeof(BatchWorkRegService))
               .As(typeof(IBatchWorkRegService))
               .InstancePerLifetimeScope();
        }
    }
}
