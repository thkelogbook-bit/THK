
using Autofac;
using Core.System.Runtime.Managers;
using Elogbook.Business.RegisterModule;
using Elogbook.Core.Interfaces.Service;
using Elogbook.Data;
using Elogbook.Data.RegisterModule;
using Elogbook.Domain.RegisterModule;
using Elogbook.Domain.Services;
using Hangfire;
using Hangfire.SqlServer;
using Owin;
using System;

namespace Elogbook.Api
{
    /// <summary>
    /// Startup config
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Configuration
        /// </summary>
        /// <param name="app"></param>
        public void Configuration(IAppBuilder app)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<IEmailService>();
            builder.RegisterType<ElogbookUnitOfWork>().InstancePerLifetimeScope();
            builder.RegisterType<WorkTicketService>().As<IWorkTicketService>().InstancePerLifetimeScope();
            builder.RegisterType<AutonomousMaintenanceService>().As<IAutonomousMaintenanceService>().InstancePerLifetimeScope();
            builder.RegisterType<AmReportService>().As<IAmReportService>().InstancePerLifetimeScope();
            builder.RegisterType<EmailConfigurationService>().As<IEmailConfigurationService>().InstancePerLifetimeScope();
            builder.RegisterType<SystemConfigurationService>().As<ISystemConfigurationService>().InstancePerLifetimeScope();
            builder.RegisterType<EmailService>().As<IEmailService>().InstancePerLifetimeScope();
            builder.RegisterType<WorkTicketService>().As<IWorkTicketService>().InstancePerLifetimeScope();
            builder.RegisterType<ElogbookDataContext>().InstancePerLifetimeScope();
            builder.RegisterType<PdfSigningService>().As<IPdfSigningService>().InstancePerLifetimeScope();
            builder.RegisterType<VnptClientService>().As<IVnptClientService>().InstancePerLifetimeScope();
            builder.RegisterType<ViettelMySignService>().As<IViettelMySignService>().InstancePerLifetimeScope();

            builder.RegisterModule<DataModule>();
            builder.RegisterModule<ServiceModule>();
            builder.RegisterModule<BusinessModule>();

            var container = builder.Build();
            var connectionString = RuntimeManager.GetConnectionString("HangfireConnection");
            GlobalConfiguration.Configuration
                .UseSqlServerStorage(
                    connectionString,
                    new SqlServerStorageOptions { QueuePollInterval = TimeSpan.FromSeconds(1) })
                .UseAutofacActivator(container);

            app.UseHangfireDashboard();
            app.UseHangfireServer();
        }
    }
}