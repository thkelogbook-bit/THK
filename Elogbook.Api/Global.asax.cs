using Autofac;
using Autofac.Extras.Quartz;
using Autofac.Integration.WebApi;
using Core.System.Runtime.Managers;
using Elogbook.Api.Filters;
using Elogbook.Api.Jobs;
using Elogbook.Business.RegisterModule;
using Elogbook.Core;
using Elogbook.Core.Log;
using Elogbook.Data;
using Elogbook.Data.RegisterModule;
using Elogbook.Domain.RegisterModule;
using Quartz;
using System.Data.Entity;
using System.Reflection;
using System.Web.Http;

namespace Elogbook.Api
{
    /// <summary>
    /// Global configuration
    /// </summary>
    public class WebApiApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// Application start event
        /// </summary>
        protected void Application_Start()
        {
            RuntimeManager.Initialize();

            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configure(SwaggerConfig.Register);

            // Register module
            var builder = new ContainerBuilder();
            var config = GlobalConfiguration.Configuration;
            // How to return JSON with camelCased property names, on objects and their sub-objects
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCaseIgnoreNullForValueTypesResolver();
            config.Formatters.JsonFormatter.UseDataContractJsonSerializer = false;
            // Format date
            config.Formatters.JsonFormatter.SerializerSettings.Culture = new System.Globalization.CultureInfo("vi-VI");
            // Trimming data
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new TrimmingConverter());
            builder.RegisterModule<DataModule>();
            builder.RegisterModule<ServiceModule>();
            builder.RegisterModule<BusinessModule>();
            // Init database changed
            // Database.SetInitializer(new CreateDatabaseIfNotExists<ElogbookDataContext>());
            // Disbale option auto update database changed
            // Database.SetInitializer(new MigrateDatabaseToLatestVersion<ElogbookDataContext, Configuration>());
            // Giải quyết lỗi khi thay đổi model của EF nhưng không muốn auto generate database
            // The model backing the 'ElogbookDataContext' context has changed since the database was created.
            // Consider using Code First Migrations to update the database (http://go.microsoft.com/fwlink/?LinkId=238269
            Database.SetInitializer<ElogbookDataContext>(null);
            Database.SetInitializer<PmisDataContext>(null);
            Database.SetInitializer<PmisSyncDataContext>(null);
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            //Register Quartz
            // 1) Register IScheduler
            builder.RegisterModule(new QuartzAutofacFactoryModule());
            // 2) Register jobs
            builder.RegisterModule(new QuartzAutofacJobsModule(typeof(PmisJob).Assembly));
            // 3) Register jobs
            builder.RegisterModule(new QuartzAutofacJobsModule(typeof(WorkTicketJob).Assembly));
            // 4) Register Pmis jobs
            builder.RegisterModule(new QuartzAutofacJobsModule(typeof(PmisToElogbookJob).Assembly));
            // 5) Register Am Create jobs
            builder.RegisterModule(new QuartzAutofacJobsModule(typeof(AmCreateJob).Assembly));
            // 6) Register Am Send Mail Notification jobs
            builder.RegisterModule(new QuartzAutofacJobsModule(typeof(AmNotificationsJob).Assembly));
            // 7) Register Am Auto Complete jobs
            builder.RegisterModule(new QuartzAutofacJobsModule(typeof(AmAutoCompleteJob).Assembly));
            // 8) Register Send Mail Report jobs
            builder.RegisterModule(new QuartzAutofacJobsModule(typeof(SendMailReportJob).Assembly));

            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            //Register scheduler
            var scheduler = container.Resolve<IScheduler>();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            // Init Mapping
            MapperInstaller.Init();
            // Init Log4net
            LogCustom.Initialize(Server.MapPath("~"));

            _ = JobInitial.InitJobAsync(scheduler);
        }
    }
}
